using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

using MatchThree.Core;
using MatchThree.Element;
using MatchThree.Utils;

namespace MatchThree.Match
{
    public class MatchModel
    {
        private MatchElement[,] _elements;
        private IReadOnlyList<IObjectIdentifier> _identifiers;

        public void Init(Vector2Int gridSize, IReadOnlyList<IObjectIdentifier> identifiers)
        {
            _identifiers = identifiers;
            _elements = new MatchElement[gridSize.x, gridSize.y];
        }

        public IObjectIdentifier[,] GetItemsForSpawn()
        {
            int width = _elements.GetLength(0);
            int height = _elements.GetLength(1);
            IObjectIdentifier[,] grid = new IObjectIdentifier[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (_elements[i, j] == null)
                    {
                        grid[i, j] = _identifiers.RandomElement();
                    }
                }
            }

            return grid;
        }

        public List<MatchData> FindMatches()
        {
            List<MatchData> matches = new List<MatchData>();

            int width = _elements.GetLength(0);
            int height = _elements.GetLength(1);

            void Scan(int outerLimit, int innerLimit,
                Func<int, int, IObjectIdentifier> getId,
                Func<int, int, Vector2Int> getBeginPos,
                Vector2Int direction)
            {
                for (int outer = 0; outer < outerLimit; outer++)
                {
                    int runStartInner = 0;
                    int runLen = 0;
                    string runId = null;

                    for (int inner = 0; inner <= innerLimit; inner++)
                    {
                        bool inBounds = inner < innerLimit;
                        IObjectIdentifier cur = inBounds ? getId(outer, inner) : null;
                        string curId = cur != null && cur.Id != null ? cur.Id : null;

                        if (inBounds && runLen > 0 && string.Equals(curId, runId, StringComparison.Ordinal))
                        {
                            runLen++;
                            continue;
                        }

                        if (inBounds && cur != null)
                        {
                            if (runLen >= 3)
                            {
                                matches.Add(new MatchData(getBeginPos(outer, runStartInner), direction, runLen));
                            }

                            runStartInner = inner;
                            runLen = 1;
                            runId = curId;
                        }
                        else
                        {
                            if (runLen >= 3)
                            {
                                matches.Add(new MatchData(getBeginPos(outer, runStartInner), direction, runLen));
                            }

                            runLen = 0;
                            runId = null;
                        }
                    }
                }
            }

            //horizontal
            Scan(height, width,
                (y, x) => _elements[x, y],
                (y, runStartX) => new Vector2Int(runStartX, y),
                Vector2Int.right);

            //vertical
            Scan(width, height,
                (x, y) => _elements[x, y],
                (x, runStartY) => new Vector2Int(x, runStartY),
                Vector2Int.up);

            return matches;
        }

        public void SetElementAtCoord(Vector2Int coord, MatchElement element)
        {
            _elements[coord.x, coord.y] = element;
        }

        public MatchElement GetElementAtCoord(Vector2Int coord)
        {
            return _elements[coord.x, coord.y];
        }

        public void RemoveElementAtCoord(Vector2Int coord)
        {
            _elements[coord.x, coord.y] = null;
        }

        public void SwapElements(Vector2Int first, Vector2Int second)
        {
            (_elements[first.x, first.y], _elements[second.x, second.y]) =
                (_elements[second.x, second.y], _elements[first.x, first.y]);
        }

        public Vector2Int GetElementCoord(MatchElement element)
        {
            for (int i = 0; i < _elements.GetLength(0); i++)
            {
                for (int j = 0; j < _elements.GetLength(1); j++)
                {
                    if (_elements[i, j] == element)
                    {
                        return new Vector2Int(i, j);
                    }
                }
            }

            throw new Exception("Element not found");
        }

        /// <summary>
        /// Inserts the element above it into an empty cell
        /// </summary>
        /// <returns>List of changed coords</returns>
        public List<Vector2Int> ActualizeElements()
        {
            int width = _elements.GetLength(0);
            int height = _elements.GetLength(1);
            var changedElements = new List<Vector2Int>();

            for (int x = 0; x < width; x++)
            {
                for (int y = height - 1; y >= 0; y--)
                {
                    var pos = new Vector2Int(x, y);
                    var elem = GetElementAtCoord(pos);

                    if (elem == null)
                    {
                        for (int i = y - 1; i >= 0; i--)
                        {
                            var pos2 = new Vector2Int(x, i);
                            var elem2 = GetElementAtCoord(pos2);

                            if (elem2 != null)
                            {
                                changedElements.Add(pos);
                                SwapElements(pos, pos2);
                                break;
                            }
                        }
                    }
                }
            }

            return changedElements;
        }
    }
}