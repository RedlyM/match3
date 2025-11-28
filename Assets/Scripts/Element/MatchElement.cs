using UnityEngine;

using MatchThree.Core;
using MatchThree.Spawning;

namespace MatchThree.Element
{
    public class MatchElement : MonoBehaviour, IObjectIdentifier, IPoolable
    {
        public string Id => _identifier.Id;

        public UserInput UserInput => _userInput;

        public ElementMovement Movement => _movement;

        public ElementAnimations Animations => _animations;

        [SerializeField]
        private Identifier _identifier;

        [SerializeField]
        private UserInput _userInput;

        [SerializeField]
        private ElementMovement _movement;

        [SerializeField]
        private ElementAnimations _animations;

        private void OnValidate()
        {
            if (_identifier == null || string.IsNullOrEmpty(_identifier.Id))
            {
                _identifier = new Identifier(name);
            }
        }

        public void Prepare()
        {
            _animations.ResetToDefault();
            gameObject.SetActive(true);
        }

        public void Release()
        {
            gameObject.SetActive(false);
            transform.position = Vector3.up * 10f;
        }
    }
}