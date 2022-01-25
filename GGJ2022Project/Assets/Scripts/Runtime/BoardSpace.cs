using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GGJ
{
    public interface IBoardSpaceState
    {
    }

    public struct Unknown : IBoardSpaceState
    {
    }

    public struct Null : IBoardSpaceState { }

    public struct Empty : IBoardSpaceState { }

    public struct Blocked : IBoardSpaceState { }

    public class BoardSpace
    {
        public Vector2Int Coordinates { get; }
        public IBoardSpaceState State { get; }

        public BoardSpace(int x, int y, IEnumerable<TileBase> tiles, IBoardSpaceState state)
        {
            Coordinates = new Vector2Int(x, y);
            State = state;
        }
    }
}
