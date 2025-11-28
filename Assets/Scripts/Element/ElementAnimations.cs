using System.Threading;

using UnityEngine;

using Cysharp.Threading.Tasks;
using DG.Tweening;

namespace MatchThree.Element
{
    public class ElementAnimations : MonoBehaviour
    {
        [SerializeField]
        private Transform _target;

        [SerializeField]
        private Vector3 _defaultScale;

        [Header("Move/Tap animation")]
        [SerializeField]
        private Vector2 _punchScale;

        [SerializeField]
        private float _punchDuration;

        //for avoid double animations
        private AsyncLazy _punchAnimation;
        private AsyncLazy _matchAnimation;

        public void ResetToDefault()
        {
            _target.localScale = _defaultScale;
        }

        public async UniTask PlayPunchAnimationAsync(CancellationToken token)
        {
            _punchAnimation ??= PlayPunchAsync(token).ToAsyncLazy();

            await _punchAnimation;

            _punchAnimation = null;
        }

        public async UniTask PlayMatchAnimationAsync(CancellationToken token)
        {
            _matchAnimation ??= PlayMatchAsync(token).ToAsyncLazy();

            await _matchAnimation;

            _matchAnimation = null;
        }

        private async UniTask PlayPunchAsync(CancellationToken token)
        {
            await _target.DOPunchScale(_punchScale, _punchDuration).AwaitForComplete(
                tweenCancelBehaviour: TweenCancelBehaviour.KillAndCancelAwait, cancellationToken: token);
        }

        private async UniTask PlayMatchAsync(CancellationToken token)
        {
            await PlayPunchAsync(token);
            await _target.DOScale(Vector2.zero, _punchDuration).AwaitForComplete(
                tweenCancelBehaviour: TweenCancelBehaviour.KillAndCancelAwait, cancellationToken: token);
        }
    }
}