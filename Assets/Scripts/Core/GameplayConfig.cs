using UnityEngine;

using MatchThree.Element;

namespace MatchThree.Core
{
    [CreateAssetMenu(fileName = "GameplayConfig", menuName = "MatchThree/GameplayConfig")]
    public class GameplayConfig : ScriptableObject
    {
        [SerializeField]
        private Vector2 _elementSize;

        [SerializeField]
        private MatchElement[] _fruits;

        public Vector2 ElementSize => _elementSize;

        public MatchElement[] Elements => _fruits;
    }
}