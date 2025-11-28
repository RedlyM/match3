using UnityEngine;

namespace MatchThree.Match
{
    public readonly struct MatchData
    {
        public readonly Vector2Int Begin;
        public readonly Vector2Int Direction;
        public readonly int Length;

        public MatchData(Vector2Int begin, Vector2Int direction, int length)
        {
            Begin = begin;
            Direction = direction;
            Length = length;
        }
    }
}