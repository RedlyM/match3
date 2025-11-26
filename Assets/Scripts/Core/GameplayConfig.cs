using Match;
using UnityEngine;

namespace MatchThree
{
    [CreateAssetMenu(fileName = "GameplayConfig", menuName = "MatchThree/GameplayConfig")]
    public class GameplayConfig : ScriptableObject
    {
        [SerializeField] private Vector2Int _boardSize;
        [SerializeField] private Vector2 _elementSize;
        [SerializeField] private MatchElement[] _fruits;

        public Vector2Int BoardSize => _boardSize;

        public Vector2 ElementSize => _elementSize;

        public MatchElement[] Elements => _fruits;
    }
}