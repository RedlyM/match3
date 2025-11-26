using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MatchThree;
using MatchThree.Board;
using MatchThree.Core;
using UnityEngine;
using VContainer.Unity;

namespace Match
{
    public class MatchController : IInitializable
    {
        private readonly MatchView _view;
        private readonly MatchModel _model;

        private readonly BoardModel _boardModel;
        private readonly PoolWithFactory<MatchElement> _pool;

        private Dictionary<Movable, Action<Vector2Int>> _swipeHandlers;

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
            _swipeHandlers = new Dictionary<Movable, Action<Vector2Int>>();
            var firstSpawnIdentifiers = _model.GetFirstSpawnItems();
            SpawnCycleAsync(firstSpawnIdentifiers, CancellationToken.None).Forget();
        }

        private async UniTask SpawnCycleAsync(IReadOnlyDictionary<Vector2Int, IObjectIdentifier> elements,
            CancellationToken token)
        {
            foreach (var pair in elements)
            {
                var item = await _pool.GetAsync(pair.Value, token);
                var itemPosition = _boardModel.GetPosition(pair.Key);

                item.transform.SetParent(_view.SpawnParent);
                item.transform.position = itemPosition;

                var movable = item.Movable;

                Action<Vector2Int> handler = v => MoveElementAsync(item, v, token).Forget();
                movable.SwipeDetected += handler;

                _swipeHandlers[movable] = handler;
                _model.SetElementAtCoord(pair.Key, item);
            }
        }

        private async UniTask MoveElementAsync(MatchElement element, Vector2Int direction, CancellationToken token)
        {
            //нужно, чтобы Y координата совпадала с координатами сетки
            direction.y = -direction.y;
            element.Movable.Blocked = true;
            var current = _model.GetElementCoord(element);
            var targetCoord = current + direction;

            element.Animations.PlayPunchAnimationAsync(token).Forget();

            if (_boardModel.IsMovableCoord(targetCoord))
            {
                var targetElement = _model.GetElementAtCoord(targetCoord);
                targetElement.Animations.PlayPunchAnimationAsync(token).Forget();

                Vector2 targetPos = _boardModel.GetPosition(targetCoord);
                Vector2 oldPos = _boardModel.GetPosition(current);
                var moveCurrentOp = element.Movable.MoveAsync(targetPos, token);
                var moveTargetOp = targetElement.Movable.MoveAsync(oldPos, token);

                await UniTask.WhenAll(moveCurrentOp, moveTargetOp);

                var moveCurrentBackOp = element.Movable.MoveAsync(oldPos, token);
                var moveTargetBackOp = targetElement.Movable.MoveAsync(targetPos, token);

                await UniTask.WhenAll(moveCurrentBackOp, moveTargetBackOp);
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