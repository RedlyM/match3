using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MatchThree
{
    public class Movable : MonoBehaviour
    {
        public event Action<Vector2Int> SwipeDetected;
        public bool Blocked { get; set; }

        [SerializeField] private InputAction _press;
        [SerializeField] private InputAction _position;

        [SerializeField] private float _swipePixelsPerStep = 50f;

        private Camera _camera;

        private bool _moving;
        private Vector2 _curScreenPos;
        private bool _isPointerDown;
        private bool _pressedOnThis;

        private Vector2 _lastSwipeOrigin;

        private bool IsClickedOn
        {
            get
            {
                var origin = _camera.ScreenToWorldPoint(_curScreenPos);
                var hit = Physics2D.Raycast(origin, Vector2.zero);
                return hit.collider != null && hit.collider.gameObject == gameObject;
            }
        }

        private void Awake()
        {
            _camera = Camera.main;

            _position.Enable();
            _press.Enable();

            _position.performed += OnPointerMove;

            _press.performed += OnPress;
            _press.canceled += OnRelease;
        }

        private void OnDestroy()
        {
            _position.performed -= OnPointerMove;

            _press.performed -= OnPress;
            _press.canceled -= OnRelease;
        }

        private void OnPress(InputAction.CallbackContext context)
        {
            if (_moving || Blocked)
            {
                return;
            }

            _curScreenPos = _position.ReadValue<Vector2>();
            _isPointerDown = true;

            _pressedOnThis = IsClickedOn;

            if (_pressedOnThis)
            {
                _lastSwipeOrigin = _curScreenPos;
            }
        }

        private void OnRelease(InputAction.CallbackContext context)
        {
            _isPointerDown = false;
            _pressedOnThis = false;
            _moving = false;
            Blocked = false;
        }

        private void OnPointerMove(InputAction.CallbackContext context)
        {
            if (Blocked)
            {
                return;
            }

            _curScreenPos = _position.ReadValue<Vector2>();

            if (!_isPointerDown || !_pressedOnThis || _moving)
                return;

            ProcessSwipeSteps();
        }

        private void ProcessSwipeSteps()
        {
            if (Blocked)
            {
                return;
            }

            Vector2 delta = _curScreenPos - _lastSwipeOrigin;
            if (delta == Vector2.zero)
            {
                return;
            }

            Vector2Int direction;
            bool horizontal = Mathf.Abs(delta.x) >= Mathf.Abs(delta.y);
            float distance = horizontal ? delta.x : delta.y;
            int steps = (int)(distance / _swipePixelsPerStep);
            int sign = Math.Sign(steps);

            if (steps == 0)
            {
                return;
            }

            if (horizontal)
            {
                direction = sign > 0 ? Vector2Int.right : Vector2Int.left;
            }
            else
            {
                direction = sign > 0 ? Vector2Int.up : Vector2Int.down;
            }

            SwipeDetected?.Invoke(direction);

            _lastSwipeOrigin += new Vector2(_swipePixelsPerStep * sign, 0f);
        }

        public async UniTask MoveAsync(Vector2 position, CancellationToken token)
        {
            _moving = true;

            await transform.DOMove(position, 0.2f)
                .AwaitForComplete(
                    tweenCancelBehaviour: TweenCancelBehaviour.KillAndCancelAwait, token);
        }
    }
}