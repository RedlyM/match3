using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Element
{
    public class ElementAnimations : MonoBehaviour
    {
        [SerializeField] private Transform _target;

        [Header("Move/Tap animation")] [SerializeField]
        private Vector2 _punchScale;

        [SerializeField] private float _punchDuration;

        public async UniTask PlayPunchAnimationAsync(CancellationToken token)
        {
            await _target.DOPunchScale(_punchScale, _punchDuration).AwaitForComplete(
                tweenCancelBehaviour: TweenCancelBehaviour.KillAndCancelAwait, cancellationToken: token);
        }
    }
}