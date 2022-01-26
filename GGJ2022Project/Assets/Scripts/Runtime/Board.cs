using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

using GGJ.Runtime.Utility;
using GGJ.Utility;

namespace GGJ
{
    [RequireComponent(typeof(Grid))]
    public class Board : MonoBehaviour
    {
        Grid m_Grid;
        BoardSpace[] m_BoardSpaces;
        BoundsInt m_BoardBounds;

        [SerializeField]
        List<TileBase> m_MapTiles;
        [SerializeField]
        List<TileBase> m_BlockingTiles;

        // Helpful events for debugging and implementing other features against
        [SerializeField]
        public UnityEvent<Board> OnNewBoard;
        [SerializeField]
        public UnityEvent<Board, BoardSpace> OnBoardSpaceConstructed;

        public Grid Grid => m_Grid;
        public int Width => m_BoardBounds.size.x;
        public int Height => m_BoardBounds.size.y;
        public IEnumerable<BoardSpace> AllSpaces => m_BoardSpaces;
        public bool IsValid => m_BoardSpaces != null && m_Grid != null;
        public bool IsConstructing { get; private set; }
        public bool NeedsInitialization => !IsValid && !IsConstructing;

        // Determine the initial state of a space on the board, based on what kinds of tiles
        // are present on the Grid for that space
        BoardSpace.Flavor DetermineBoardSpaceType(IEnumerable<TileBase> tiles)
        {
            var hasATile = false;
            var isBlocked = false;
            var hasMapTile = false;
            foreach (var tile in tiles)
            {
                if (tile != null)
                {
                    hasATile = true;
                    if (m_BlockingTiles.Contains(tile))
                    {
                        isBlocked = true;
                    }
                    else if (m_MapTiles.Contains(tile))
                    {
                        hasMapTile = true;
                    }
                }
            }

            if (!hasATile)
            {
                return BoardSpace.Flavor.Null;
            }
            if (isBlocked)
            {
                return BoardSpace.Flavor.Wall;
            }

            if (hasMapTile)
            {
                return BoardSpace.Flavor.Normal;
            }

            Debug.LogError($"{name} found a tile stack with no recognizable tiles:\n" +
                $"{tiles.First()}");
            return BoardSpace.Flavor.Unknown;
        }

        // Pulls all of the tilemaps out of the Board's associated Grid and converts them
        // into a collection of BoardSpaces
        public IEnumerator ConstructBoardSpaces(object coroutineYield = null)
        {
            IsConstructing = true;
            OnNewBoard?.Invoke(this);
            m_Grid = GetComponent<Grid>();
            var allTilemaps = GetComponentsInChildren<Tilemap>().ToList();
            var minX = int.MaxValue;
            var maxX = int.MinValue;
            var minY = int.MaxValue;
            var maxY = int.MinValue;

            var allTiles = new List<TilemapHelpers.TileMatrix>();

            foreach (var tilemap in allTilemaps)
            {
                var tiles = TilemapHelpers.GetAllTilesInTilemap(tilemap);
                if (tiles.Width == 0)
                {
                    continue;
                }

                minX = Math.Min(minX, tiles.Bounds.xMin);
                maxX = Math.Max(maxX, tiles.Bounds.xMax);
                minY = Math.Min(minY, tiles.Bounds.yMin);
                maxY = Math.Max(maxY, tiles.Bounds.yMax);
                allTiles.Add(tiles);
            }

            var numColumns = maxX - minX;
            var numRows = maxY - minY;
            var tileStack = new List<TileBase>();
            m_BoardSpaces = new BoardSpace[numRows * numColumns];
            m_BoardBounds = new BoundsInt(minX, minY, 0, numColumns, numRows, 0);
            for (var i = 0; i < numColumns; ++i)
            {
                for (var j = 0; j < numRows; ++j)
                {
                    tileStack.Clear();
                    var x = i + minX;
                    var y = j + minY;
                    var currentCoordinates = new Vector3Int(x, y, 0);
                    foreach (var tileLayer in allTiles)
                    {
                        if (tileLayer.Bounds.Contains(currentCoordinates))
                        {
                            tileStack.Add(tileLayer.GetAt(x, y));
                        }

                        //tilesTemp.Add(allTiles[i][x][y]);
                    }

                    var flavor = DetermineBoardSpaceType(tileStack);
                    var space = new BoardSpace(this, x, y, flavor);
                    var index = QuickMaths.IJToIndex(numColumns, i, j);
                    m_BoardSpaces[index] = space;

                    OnBoardSpaceConstructed?.Invoke(this, space);
                    yield return coroutineYield;
                }
            }

            Debug.Log("Board populated.");
            IsConstructing = false;
        }

        void Start()
        {
            StartCoroutine(ConstructBoardSpaces());
        }

        // Update is called once per frame
        void Update()
        {

        }

        int ToIndex(Vector3Int coordinates)
        {
            var i = coordinates.x - m_BoardBounds.xMin;
            var j = coordinates.y - m_BoardBounds.yMin;
            if (i < 0 || i >= Width || j < 0 || j >= Height)
            {
                Debug.LogWarning($"Attempted to compute an out-of-bounds index ({coordinates})");
                return -1;
            }
            return QuickMaths.IJToIndex(Width,
                coordinates.x - m_BoardBounds.xMin, coordinates.y - m_BoardBounds.yMin);
        }

        public Vector3 GetWorldCoordinates(BoardSpace space)
        {
            if (m_Grid == null)
            {
                Debug.LogError($"Can't get coordinates if {nameof(m_Grid)} isn't set.");
                return Vector3.positiveInfinity;
            }

            if (space == null)
            {
                Debug.LogError($"Passed in null {nameof(BoardSpace)}");
                return Vector3.negativeInfinity;
            }

            var position = new Vector3Int(space.Coordinates.x, space.Coordinates.y, 0);
            return m_Grid.GetCellCenterWorld(position);
        }

        public bool TryGetSpace(Vector3 positionWorld, out BoardSpace space)
        {
            var cellPosition = m_Grid.WorldToCell(positionWorld);
            var index = ToIndex(cellPosition);
            if (index == -1)
            {
                space = null;
                return false;
            }

            space = m_BoardSpaces[index];
            return true;
        }

        public bool TryPlacePiece(BoardPiece piece, BoardSpace space)
        {
            // TODO: Check the space is owned by this board (or throw exception)
            // TODO: Check that space is available for a piece
            space.PlacePiece(piece);
            return true;
        }
    }
}
