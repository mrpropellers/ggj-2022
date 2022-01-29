using System;
using UnityEngine;

namespace GGJ
{
    [RequireComponent(typeof(BoardPiece))]
    public class Obstacle : MonoBehaviour, IBoardPiece
    {
        BoardPiece m_Piece;

        public BoardPiece Piece => m_Piece;
        public bool IsTangible => StageState.Instance.IsTangible(m_Piece);

        public static implicit operator BoardPiece(Obstacle self) => self?.Piece;

        void Awake()
        {
            m_Piece = GetComponent<BoardPiece>();
        }
    }
}
