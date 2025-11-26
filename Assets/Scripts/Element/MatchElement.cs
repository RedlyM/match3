using Element;
using MatchThree;
using MatchThree.Core;
using UnityEngine;

namespace Match
{
    public class MatchElement : MonoBehaviour
    {
        public IObjectIdentifier Identifier => _identifier;

        public Movable Movable => _movable;

        public ElementAnimations Animations => _animations;

        [SerializeField]
        private Identifier _identifier;

        [SerializeField]
        private Movable _movable;

        [SerializeField]
        private ElementAnimations _animations;

        private void OnValidate()
        {
            if (_identifier == null || string.IsNullOrEmpty(_identifier.Id))
            {
                _identifier = new Identifier(name);
            }
        }
    }
}