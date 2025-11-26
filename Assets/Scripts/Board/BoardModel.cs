using System.Collections.Generic;
using UnityEngine;

namespace MatchThree.Board
{
    public class BoardModel
    {
        private readonly GameplayConfig _config;

        public Bounds Bounds { get; private set; }

        public IReadOnlyDictionary<Vector2Int, Vector2> CoordPositions => _coordPositions;

        private Dictionary<Vector2Int, Vector2> _coordPositions;

        public BoardModel(GameplayConfig config)
        {
            _config = config;
        }

        public void Init()
        {
            InitBounds();
            InitCoords();
        }

        public Vector2 GetPosition(Vector2Int coord)
        {
            return _coordPositions[coord];
        }

        private void InitCoords()
        {
            _coordPositions = new Dictionary<Vector2Int, Vector2>();

            for (int i = 0; i < _config.BoardSize.x; i++)
            {
                for (int j = 0; j < _config.BoardSize.y; j++)
                {
                    var posX = Bounds.min.x + _config.ElementSize.x * i + _config.ElementSize.x * 0.5f;
                    var posY = Bounds.max.y - _config.ElementSize.y * j - _config.ElementSize.y * 0.5f;
                    _coordPositions[new Vector2Int(i, j)] = new Vector2(posX, posY);
                }
            }
        }

        private void InitBounds()
        {
            var fullSize = new Vector2(_config.BoardSize.x * _config.ElementSize.x, _config.BoardSize.y * _config.ElementSize.y);

            Bounds = new Bounds(Vector3.zero, fullSize);
        }
    }
}