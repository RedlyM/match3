using UnityEngine;
using UnityEngine.U2D;

namespace MatchThree.Board
{
    public class BoardView : MonoBehaviour
    {
        public SpriteShapeController SpriteShape => _spriteShape;

        [SerializeField]
        private SpriteShapeController _spriteShape;
    }
}