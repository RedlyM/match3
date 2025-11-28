using System;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;

using VContainer.Unity;
using Cysharp.Threading.Tasks;

using MatchThree.Board;
using MatchThree.Core;
using MatchThree.Element;
using MatchThree.Spawning;

namespace MatchThree.Match
{
    public class MatchController : IInitializable, IDisposable
    {
        private readonly MatchView _view;
        private readonly MatchModel _model;

        private readonly BoardModel _boardModel;
        private readonly PoolWithFactory<MatchElement> _pool;

        private CancellationTokenSource _lifetimeCts;
        private Dictionary<UserInput, Action<Vector2Int>> _swipeHandlers;
        private bool _inputsBlocked;

        public MatchController(MatchView view, MatchModel model, BoardModel boardModel,
            PoolWithFactory<MatchElement> pool)
        {
            _view = view;
            _model = model;
            _boardModel = boardModel;
            _pool = pool;
        }

        void IInitializable.Initialize()
        {
            _lifetimeCts = new CancellationTokenSource();
            _model.Init();
            _swipeHandlers = new Dictionary<UserInput, Action<Vector2Int>>();
            var firstSpawnIdentifiers = _model.GetItemsForSpawn();
            SpawnCycleAsync(firstSpawnIdentifiers, _lifetimeCts.Token).Forget();
        }

        void IDisposable.Dispose()
        {
            _lifetimeCts.Cancel();
            _lifetimeCts.Dispose();
        }

        private async UniTask SpawnCycleAsync(IObjectIdentifier[,] identifiers, CancellationToken token)
        {
            _inputsBlocked = true;

            await SpawnAsync(identifiers, token);

            while (await CollectAllMatchesAsync(token))
            {
                await MoveActualPositionsAsync(token);
                await SpawnAsync(_model.GetItemsForSpawn(), token);
            }

            _inputsBlocked = false;
        }

        private async UniTask SpawnAsync(IObjectIdentifier[,] identifiers, CancellationToken token)
        {
            int width = identifiers.GetLength(0);
            int height = identifiers.GetLength(1);
            List<UniTask> moveOps = new List<UniTask>();

            for (int x = 0; x < width; x++)
            {
                int stackSize = 0;
                int currentStack = 0;

                //HACK for calculate position out of board
                for (int y = 0; y < height; y++)
                {
                    if (identifiers[x, y] != null)
                    {
                        stackSize++;
                    }
                }

                for (int y = 0; y < height; y++)
                {
                    if (identifiers[x, y] == null)
                    {
                        continue;
                    }

                    Vector2Int coord = new Vector2Int(x, y);
                    MatchElement item = await _pool.GetAsync(identifiers[x, y], token);
                    Vector2 outerPosition = _boardModel.GetOuterPosition(coord, stackSize - currentStack);
                    Vector2 boardPosition = _boardModel.GetPosition(coord);

                    currentStack++;

                    item.transform.SetParent(_view.SpawnParent);
                    item.transform.position = outerPosition;
                    await UniTask.Yield();
                    moveOps.Add(item.Movement.MoveAsync(boardPosition, token));

                    _model.SetElementAtCoord(coord, item);

                    Action<Vector2Int> move = targetCoord =>
                    {
                        if (!_inputsBlocked)
                        {
                            MoveElementAsync(item, targetCoord, token).Forget();
                        }
                    };
  
                    UserInput userInput = item.UserInput;
                    userInput.SwipeDetected += move;

                    _swipeHandlers[userInput] = move;
                }
            }

            await UniTask.WhenAll(moveOps);
        }

        private async UniTask MoveElementAsync(MatchElement selectedElement, Vector2Int direction, CancellationToken token)
        {
            if (_inputsBlocked)
            {
                return;
            }

            _inputsBlocked = true;

            //нужно, чтобы Y координата совпадала с координатами сетки
            direction.y = -direction.y;

            Vector2Int current = _model.GetElementCoord(selectedElement);
            Vector2Int targetCoord = current + direction;

            selectedElement.Animations.PlayPunchAnimationAsync(token).Forget();

            if (_boardModel.IsMovableCoord(targetCoord))
            {
                MatchElement targetElement = _model.GetElementAtCoord(targetCoord);
                Vector2 targetPos = _boardModel.GetPosition(targetCoord);
                Vector2 oldPos = _boardModel.GetPosition(current);
                UniTask moveCurrentOp = selectedElement.Movement.MoveAsync(targetPos, token);
                UniTask moveTargetOp = targetElement.Movement.MoveAsync(oldPos, token);

                targetElement.Animations.PlayPunchAnimationAsync(token).Forget();

                await UniTask.WhenAll(moveCurrentOp, moveTargetOp);

                _model.SwapElements(current, targetCoord);
                bool foundMatches = false;

                while (await CollectAllMatchesAsync(token))
                {
                    foundMatches = true;
                    await MoveActualPositionsAsync(token);
                    await SpawnAsync(_model.GetItemsForSpawn(), token);
                }

                if (!foundMatches)
                {
                    UniTask moveCurrentBackOp = selectedElement.Movement.MoveAsync(oldPos, token);
                    UniTask moveTargetBackOp = targetElement.Movement.MoveAsync(targetPos, token);

                    await UniTask.WhenAll(moveCurrentBackOp, moveTargetBackOp);
                    _model.SwapElements(current, targetCoord);
                }
            }

            _inputsBlocked = false;
        }

        private async UniTask<bool> CollectAllMatchesAsync(CancellationToken token)
        {
            var matches = _model.FindMatches();

            if (matches.Count == 0)
            {
                return false;
            }

            List<(MatchElement, Vector2Int)> matchedElements = new List<(MatchElement, Vector2Int)>();
            List<UniTask> ops = new List<UniTask>();

            foreach (var match in matches)
            {
                Vector2Int coord = match.Begin;

                for (int i = 0; i < match.Length; i++)
                {
                    MatchElement element = _model.GetElementAtCoord(coord);
                    ops.Add(element.Animations.PlayMatchAnimationAsync(token));
                    matchedElements.Add((element, coord));
                    coord += match.Direction;
                }
            }

            await UniTask.WhenAll(ops);

            foreach (var tuple in matchedElements)
            {
                _model.RemoveElementAtCoord(tuple.Item2);
                Unsubscribe(tuple.Item1.UserInput);
                _pool.ReturnToPool(tuple.Item1);
            }

            return matches.Count > 0;
        }

        private async UniTask MoveActualPositionsAsync(CancellationToken token)
        {
            List<Vector2Int> changed = _model.ActualizeElements();
            List<UniTask> moveOps = new List<UniTask>();

            foreach (var newPos in changed)
            {
                MatchElement el = _model.GetElementAtCoord(newPos);
                UniTask op = el.Movement.MoveAsync(_boardModel.GetPosition(newPos), token);
                moveOps.Add(op);
            }

            await UniTask.WhenAll(moveOps);
        }

        private void Unsubscribe(UserInput userInput)
        {
            if (_swipeHandlers.TryGetValue(userInput, out Action<Vector2Int> sub))
            {
                userInput.SwipeDetected -= sub;
                _swipeHandlers.Remove(userInput);
            }
        }

        private void UnsubscribeAllSwipes()
        {
            foreach (var kvp in _swipeHandlers)
            {
                kvp.Key.SwipeDetected -= kvp.Value;
            }

            _swipeHandlers.Clear();
        }
    }
}