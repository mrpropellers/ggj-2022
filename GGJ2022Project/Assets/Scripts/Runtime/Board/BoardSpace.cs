using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
        public Vector2Int CoordinatesGrid { get; }
        public Vector3 CoordinatesWorld => ParentBoard.GetWorldCoordinates(this);

        public bool CanHoldTangible => m_Flavor == Flavor.Normal;
        public bool CanHoldEthereal => m_Flavor is Flavor.Normal or Flavor.Wall;

        // TODO: These LINQ expressions probably do some unnecessary allocating - could be optimized
        public bool HasAny<T>() where T : MonoBehaviour
            => m_PiecesHere.Any(piece => piece.GetComponent<T>() != null);

        public IEnumerable<T> GetAll<T>() where T : MonoBehaviour
            => m_PiecesHere
                .Where(piece => piece.GetComponent<T>() != null)
                .Select(piece => piece.GetComponent<T>());

        // Indicates whether this space could be moved into by the specified piece
        // TODO: Check status of pieces on space

        internal bool Contains(BoardPiece piece) => m_PiecesHere.Contains(piece);
        internal void Add(BoardPiece piece) => m_PiecesHere.Add(piece);
        internal void Remove(BoardPiece piece) => m_PiecesHere.Remove(piece);

        internal BoardSpace(Board parent, int x, int y, Flavor flavor)
        {
            ParentBoard = parent;
            CoordinatesGrid = new Vector2Int(x, y);
            m_Flavor = flavor;
            m_PiecesHere = new HashSet<BoardPiece>();
        }

        public static Vector2Int operator +(BoardSpace space, Vector2Int delta) =>
            space.CoordinatesGrid + delta;
    }
}
