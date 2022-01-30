using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GGJ.GlobalConsts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using UnityEngine.Assertions;

using GGJ.Utility;
using GGJ.Utility.Runtime.Utility;
using GGJ.Utility.Utility;

namespace GGJ
{
    [RequireComponent(typeof(Grid))]
    public class Board : MonoBehaviour
    {
        Grid m_Grid;
        BoardSpace[] m_BoardSpaces;
        BoundsInt m_BoardBounds;
        Tilemap m_EtherealTilemap;

        [SerializeField]
        TileBase m_EarthlyMapTile;
        [SerializeField]
        TileBase m_EtherealMapTile;
        [SerializeField]
        TileBase m_EarthlyBlockingTile;
        [SerializeField]
        TileBase m_EtherealBlockingTile;

        // Each piece can only occupy one space (although one space can hold many pieces)
        // Keep track of which space each piece is on so we don't have to do a bunch of math every time
        // we need to figure out this information
        internal Dictionary<BoardPiece, BoardSpace> PieceToSpaceMap { get; private set; }

        // Helpful events for debugging and implementing other features against
        [SerializeField]
        public UnityEvent<Board> OnNewBoard;
        [SerializeField]
        public UnityEvent<Board> OnBoardConstructed;
        [SerializeField]
        public UnityEvent<Board, BoardSpace> OnBoardSpaceConstructed;
        [SerializeField]
        public UnityEvent<Board, BoardPiece, BoardSpace> OnPiecePlaced;

        public Grid Grid => m_Grid;
        public int Width => m_BoardBounds.size.x;
        public int Height => m_BoardBounds.size.y;
        public IEnumerable<BoardSpace> AllSpaces => m_BoardSpaces;
        public bool IsValid => m_BoardSpaces != null && m_Grid != null;
        public bool IsConstructing { get; private set; }
        public bool NeedsInitialization => !IsValid && !IsConstructing;
        public bool Contains(BoardPiece piece) => PieceToSpaceMap.ContainsKey(piece);

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
                    if (m_EarthlyBlockingTile == tile)
                    {
                        isBlocked = true;
                    }
                    else if (m_EarthlyMapTile == tile)
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
            if (m_EarthlyMapTile == null || m_EarthlyBlockingTile == null ||
                m_EtherealMapTile == null || m_EtherealBlockingTile == null)
            {
                Debug.LogError($"Can't construct map because you haven't defined all of the tiles yet.");
                yield break;
            }

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
            if (Application.isPlaying)
            {
                var etherealGo = new GameObject(Names.EtherealTilemap);
                etherealGo.transform.parent = transform;
                m_EtherealTilemap = etherealGo.AddComponent<Tilemap>();
                // Because we're adding the Tilemap from code, this doesn't get added automatically...
                var renderer = etherealGo.AddComponent<TilemapRenderer>();
                renderer.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
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
                    if (!Application.isPlaying)
                    {
                        continue;
                    }

                    switch (flavor)
                    {
                        case BoardSpace.Flavor.Normal:
                            m_EtherealTilemap.SetTile(new Vector3Int(x, y), m_EtherealMapTile);
                            break;
                        case BoardSpace.Flavor.Wall:
                            m_EtherealTilemap.SetTile(new Vector3Int(x, y), m_EtherealBlockingTile);
                            break;
                        case BoardSpace.Flavor.Unknown:
                        case BoardSpace.Flavor.Null:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(
                                $"No handling defined for {flavor}");
                    }
                }
            }

            Debug.Log("Board populated.");
            IsConstructing = false;
            OnBoardConstructed?.Invoke(this);
            PopulatePieceToBoardMap();
        }

        void PopulatePieceToBoardMap()
        {
            if (PieceToSpaceMap != null)
            {
                Debug.LogWarning(
                    $"{nameof(PieceToSpaceMap)} has already been constructed once for {name} - overwriting...");
            }

            PieceToSpaceMap = new Dictionary<BoardPiece, BoardSpace>();
            var pieces = FindObjectsOfType<BoardPiece>();
            foreach (var piece in pieces)
            {
                if (TryGetSpace(piece.transform.position, out var space))
                {
                    PlacePiece(piece, space);
                }
            }
        }

        void Awake()
        {
            UnityHelpers.DoCoroutineImmediately(ConstructBoardSpaces());
        }

        // Update is called once per frame
        void Update()
        {

        }

        int ToIndex(Vector2Int coordinates)
        {
            var i = coordinates.x - m_BoardBounds.xMin;
            var j = coordinates.y - m_BoardBounds.yMin;
            if (i < 0 || i >= Width || j < 0 || j >= Height)
            {
                if (Application.isPlaying)
                {
                    Debug.LogWarning($"Attempted to compute an out-of-bounds index ({coordinates}).");
                }

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

            var position = new Vector3Int(space.CoordinatesGrid.x, space.CoordinatesGrid.y, 0);
            return m_Grid.GetCellCenterWorld(position);
        }

        public Vector3 GetWorldCoordinates(BoardPiece piece)
        {
            return GetWorldCoordinates(GetSpace(piece));
        }

        public Vector2Int GetCellCoordinates(BoardPiece piece)
        {
            return GetSpace(piece).CoordinatesGrid;
        }

        public bool TryGetSpace(Vector2Int positionGrid, out BoardSpace space)
        {
            var index = ToIndex(positionGrid);
            if (index == -1)
            {
                space = null;
                return false;
            }

            space = m_BoardSpaces[index];
            return true;
        }

        public bool TryGetSpace(Vector3 positionWorld, out BoardSpace space)
        {
            var cellPosition = (Vector2Int) m_Grid.WorldToCell(positionWorld);
            return TryGetSpace(cellPosition, out space);
        }

        public bool TryGetSpace(BoardPiece piece, out BoardSpace space)
        {
            if (PieceToSpaceMap.TryGetValue(piece, out space))
            {
                if(space.Contains(piece))
                {
                    return true;
                }

                Debug.LogError($"{name} thinks {piece.name} is at {space.CoordinatesGrid}, but the " +
                    $"{nameof(BoardSpace)} there doesn't agree...");
                return false;
            }

            return false;
        }

        public BoardSpace GetSpace(BoardPiece piece)
        {
            var success = TryGetSpace(piece, out var space);
            Assert.IsTrue(success, $"Failed to {nameof(GetSpace)} for {piece.name} -" +
                $"Should you use {nameof(TryGetSpace)} here instead?");
            return space;
        }

        public BoardSpace GetSpace(Vector2Int cellCoordinates)
        {
            var success = TryGetSpace(cellCoordinates, out var space);
            Assert.IsTrue(success, $"Failed to {nameof(GetSpace)} at {cellCoordinates} -" +
                $"Should you use {nameof(TryGetSpace)} here instead?");
            return space;
        }

        public void RemovePiece(BoardPiece piece)
        {
            // TODO? Add some asserts here
            GetSpace(piece).Remove(piece);
        }

        // Probably should keep this the only public means to put a piece in a space to avoid branching logic
        // inside the Board class, which is meant to be primarily a data container
        // Put decision trees and other state checking in the MovementHandler or wherever else makes most sense
        public void PlacePiece(BoardPiece piece, BoardSpace targetSpace)
        {
            Assert.IsTrue(m_BoardSpaces.Contains(targetSpace) && targetSpace.ParentBoard == this,
                $"{name} is not keeping track of the space at {targetSpace.CoordinatesGrid} - " +
                $"{nameof(targetSpace)} says its parent is {targetSpace.ParentBoard}.");

            if (TryGetSpace(piece, out var spaceCurrent))
            {
                spaceCurrent.Remove(piece);
                LogHelpers.LogIfPlaying($"Moving {piece.name} from {spaceCurrent.CoordinatesGrid} to {targetSpace.CoordinatesGrid}");
            }
            else
            {
                LogHelpers.LogIfPlaying($"Placing {piece.name} on {nameof(BoardSpace)} at {targetSpace.CoordinatesGrid}.");
            }

            targetSpace.Add(piece);
            PieceToSpaceMap[piece] = targetSpace;
            OnPiecePlaced?.Invoke(this, piece, targetSpace);
        }
    }
}
