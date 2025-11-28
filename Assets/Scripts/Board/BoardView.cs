using UnityEngine;
using UnityEngine.U2D;

namespace MatchThree.Board
{
    public class BoardView : MonoBehaviour
    {
        public SpriteShapeController SpriteShape => _spriteShape;

        public Transform Mask => _mask;

        [SerializeField]
        private SpriteShapeController _spriteShape;

        [SerializeField]
        private Transform _mask;
    }
}