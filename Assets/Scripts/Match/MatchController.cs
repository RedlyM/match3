using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using MatchThree;
using MatchThree.Board;
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

        private async UniTask SpawnCycleAsync(IReadOnlyDictionary<Vector2Int, IObjectIdentifier> elements, CancellationToken token)
        {
            foreach (var pair in elements)
            {
                var item = await _pool.GetAsync(pair.Value, token);
                var itemPosition = _boardModel.GetPosition(pair.Key) + (Vector2)_view.SpawnParent.position;

                item.transform.SetParent(_view.SpawnParent);
                item.transform.position = itemPosition;

                var movable = item.Movable;

                Action<Vector2Int> handler = v => MoveElement(item, v);
                movable.SwipeDetected += handler;

                _swipeHandlers[movable] = handler;
                _model.SetElementAtCoord(pair.Key, item);
            }
        }

        private void MoveElement(MatchElement element, Vector2Int direction)
        {
            direction.y = -direction.y;
            element.Movable.Blocked = true;
            var current = _model.GetElementCoord(element);

            if (_model.CanSwipeToDirection(current, direction))
            {
                Vector2 targetPos = _boardModel.GetPosition(current + direction) + (Vector2)_view.SpawnParent.position;
                element.Movable.MoveAsync(targetPos, CancellationToken.None).Forget();
                Debug.Log($"Move {element.Identifier.Id} from {current} to {targetPos}. Direction: {direction}");
            }
            else
            {
                Debug.Log($"Can't move {element.Identifier.Id} from {current}. Direction: {direction}");
            }
        }

        public void UnsubscribeAllSwipes()
        {
            foreach (var kvp in _swipeHandlers)
            {
                kvp.Key.SwipeDetected -= kvp.Value;
            }

            _swipeHandlers.Clear();
        }

    }
}