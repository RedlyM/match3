using DefaultNamespace;
using MatchThree;
using UnityEngine;

namespace Match
{
    public class MatchElement : MonoBehaviour
    {
        public IObjectIdentifier Identifier => _identifier;

        public Movable Movable => _movable;

        [SerializeField]
        private Identifier _identifier;

        [SerializeField]
        private Movable _movable;

        private void OnValidate()
        {
            if (_identifier == null || string.IsNullOrEmpty(_identifier.Id))
            {
                _identifier = new Identifier(name);
            }
        }
    }
}