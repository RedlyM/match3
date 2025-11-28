using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace MatchThree.Element
{
    public class ElementMovement : MonoBehaviour
    {
        [SerializeField]
        private Transform _target;

        [SerializeField]
        private float _timePerStep;

        public async UniTask MoveAsync(Vector2 position, CancellationToken token)
        {
            float distance = Vector2.Distance(_target.position, position);

            await transform.DOMove(position, _timePerStep * Mathf.RoundToInt(distance))
                .AwaitForComplete(
                    tweenCancelBehaviour: TweenCancelBehaviour.KillAndCancelAwait, token);
        }
    }
}