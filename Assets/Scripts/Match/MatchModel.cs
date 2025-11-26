using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using MatchThree;
using UnityEngine;
using Utils;

namespace Match
{
    public class MatchModel
    {
        private readonly GameplayConfig _config;

        private readonly Dictionary<Vector2Int, MatchElement> _elements;

        public MatchModel(GameplayConfig config)
        {
            _config = config;
            _elements = new Dictionary<Vector2Int, MatchElement>();
        }

        public IReadOnlyDictionary<Vector2Int, IObjectIdentifier> GetFirstSpawnItems()
        {
            var result = new Dictionary<Vector2Int, IObjectIdentifier>();

            var identifiers = _config.Elements.Select(element => new Identifier(element.Identifier.Id) as IObjectIdentifier)
                .ToList();

            for (int i = 0; i < _config.BoardSize.x; i++)
            {
                for (int j = 0; j < _config.BoardSize.y; j++)
                {
                    result[new Vector2Int(i, j)] = identifiers.RandomElement();
                }
            }

            return result;
        }

        public void SetElementAtCoord(Vector2Int coord, MatchElement element)
        {
            _elements[coord] = element;
        }

        public Vector2Int GetElementCoord(MatchElement element)
        {
            foreach (var pair in _elements)
            {
                if (pair.Value == element)
                {
                    return pair.Key;
                }
            }

            throw new Exception("Element not found");
        }

        public bool CanSwipeToDirection(Vector2Int currentCoord, Vector2Int direction)
        {
            var target = currentCoord + direction;
            var boardSize = _config.BoardSize;

            return target.x >= 0 && target.x < boardSize.x && target.y >= 0 && target.y < boardSize.y;
        }
    }
}