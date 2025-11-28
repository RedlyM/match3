using UnityEngine;

using MatchThree.Core;

namespace MatchThree.Board
{
    public class BoardModel
    {
        public Bounds Bounds { get; private set; }

        private Vector2[,] _coordPositions;
        private Vector2Int _gridSize;
        private Vector2 _elementSize;

        public void Init(Vector2Int gridSize, Vector2 elementSize, Vector2 boardOffset)
        {
            _gridSize = gridSize;
            _elementSize = elementSize;

            InitBounds(boardOffset);
            InitCoords();
        }

        public Vector2 GetPosition(Vector2Int coord)
        {
            return _coordPositions[coord.x, coord.y];
        }

        public Vector2 GetOuterPosition(Vector2Int coord, int step)
        {
            Vector2 original = GetPosition(coord);
            Vector2 higherElementPosition = _coordPositions[coord.x, 0];
            original.y = higherElementPosition.y + _elementSize.y * step;
            return original;
        }

        public bool IsMovableCoord(Vector2Int coord)
        {
            return coord.x >= 0 &&
                   coord.y >= 0 &&
                   coord.x < _coordPositions.GetLength(0) &&
                   coord.y < _coordPositions.GetLength(1);
        }

        private void InitCoords()
        {
            int width = _gridSize.x;
            int height = _gridSize.y;
            _coordPositions = new Vector2[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    float posX = Bounds.min.x + _elementSize.x * x + _elementSize.x * 0.5f;
                    float posY = Bounds.max.y - _elementSize.y * y - _elementSize.y * 0.5f;
                    _coordPositions[x, y] = new Vector2(posX, posY);
                }
            }
        }

        private void InitBounds(Vector2 center)
        {
            Vector2 fullSize = new Vector2(_gridSize.x * _elementSize.x, _gridSize.y * _elementSize.y);

            Bounds = new Bounds(center, fullSize);
        }
    }
}