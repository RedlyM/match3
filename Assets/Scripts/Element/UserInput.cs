using System;

using UnityEngine;
using UnityEngine.InputSystem;

namespace MatchThree.Element
{
    public class UserInput : MonoBehaviour
    {
        public event Action<Vector2Int> SwipeDetected;

        [SerializeField]
        private InputAction _press;

        [SerializeField]
        private InputAction _position;

        [SerializeField]
        private float _swipePixelsPerStep = 50f;

        private Camera _camera;

        private bool _detected;
        private bool _isPointerDown;
        private bool _pressedOnThis;
        private Vector2 _curScreenPos;
        private Vector2 _lastSwipeOrigin;

        private bool isClickedOn
        {
            get
            {
                Vector3 origin = _camera.ScreenToWorldPoint(_curScreenPos);
                RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.zero);
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
            if (_detected)
            {
                return;
            }

            _curScreenPos = _position.ReadValue<Vector2>();
            _isPointerDown = true;

            _pressedOnThis = isClickedOn;

            if (_pressedOnThis)
            {
                _lastSwipeOrigin = _curScreenPos;
            }
        }

        private void OnRelease(InputAction.CallbackContext context)
        {
            _isPointerDown = false;
            _pressedOnThis = false;
            _detected = false;
        }

        private void OnPointerMove(InputAction.CallbackContext context)
        {
            _curScreenPos = _position.ReadValue<Vector2>();

            if (!_isPointerDown || !_pressedOnThis || _detected)
            {
                return;
            }

            ProcessSwipeSteps();
        }

        private void ProcessSwipeSteps()
        {
            Vector2 delta = _curScreenPos - _lastSwipeOrigin;

            if (delta == Vector2.zero || _detected)
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

            _detected = true;
            SwipeDetected?.Invoke(direction);

            _lastSwipeOrigin += new Vector2(_swipePixelsPerStep * sign, 0f);
        }
    }
}