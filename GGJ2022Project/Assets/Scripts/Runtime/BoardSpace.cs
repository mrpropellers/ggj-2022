using System.Collections.Generic;
using UnityEngine;

namespace GGJ
{

    public class BoardSpace
    {
        internal enum Flavor
        {
            Unknown,
            Null,
            Normal,
            Wall,
        }

        Flavor m_Flavor;
        HashSet<BoardPiece> m_PiecesHere;

        internal Flavor BoardSpaceType => m_Flavor;
        // Only get this for debugging purposes - write clearly-named interfaces into this class when you need
        // to interpret the state for gameplay reasons
        public Board ParentBoard { get; }
        public Vector2Int Coordinates { get; }

        // Indicates whether this space could be moved into
        // TODO: Check status of pieces on space
        public bool IsAvailableForMove => m_Flavor == Flavor.Normal;


        internal BoardSpace(Board parent, int x, int y, Flavor flavor)
        {
            ParentBoard = parent;
            Coordinates = new Vector2Int(x, y);
            m_Flavor = flavor;
            m_PiecesHere = new HashSet<BoardPiece>();
        }

        internal void PlacePiece(BoardPiece piece)
        {
            m_PiecesHere.Add(piece);
        }
    }
}
