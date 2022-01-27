using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace GGJ
{
    public class StageState : MonoBehaviour
    {

        static StageState s_Instance;

        public static StageState Instance
        {
            get
            {
                EnsureInitialized();
                return s_Instance;
            }
        }

        Dictionary<BoardPiece, Board> m_PieceToBoardMap;
        Board[] m_AllBoards;

        public Board ActiveBoard { get; private set; }

        void Awake()
        {
            EnsureInitialized();
        }

        static void EnsureInitialized()
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType<StageState>();
                if (s_Instance == null)
                {
                    Debug.LogWarning($"No {nameof(StageState)} instance found in scene, creating one.");
                    var go = new GameObject(nameof(StageState));
                    s_Instance = go.AddComponent<StageState>();
                    s_Instance.m_AllBoards = FindObjectsOfType<Board>();
                    s_Instance.ActiveBoard = s_Instance.m_AllBoards[0];
                    Debug.LogWarning($"Pointing at the first {nameof(Board)} we found " +
                        $"({s_Instance.m_AllBoards[0].name}) could lead to undefined behavior " +
                        $"(avoid this by manually creating a {nameof(StageState)} object)");
                }
                s_Instance.EnsureAllBoardsAreTracked();
            }
        }

        void EnsureAllBoardsAreTracked()
        {
            m_PieceToBoardMap = new Dictionary<BoardPiece, Board>();
            foreach (var board in FindObjectsOfType<Board>())
            {
                board.OnPiecePlaced.AddListener(UpdatePieceToBoardMap);
            }
        }

        public Board GetBoard(BoardPiece piece)
        {
            if (!m_PieceToBoardMap.ContainsKey(piece))
            {
                foreach (var board in m_AllBoards)
                {
                    if (board.Contains(piece))
                    {
                        Debug.Log(
                            $"{piece.name} on {board.name} wasn't tracked yet - adding it to {nameof(m_PieceToBoardMap)}");
                        m_PieceToBoardMap.Add(piece, board);
                        return board;
                    }
                }
            }

            return m_PieceToBoardMap[piece];
        }

        void UpdatePieceToBoardMap(Board board, BoardPiece piece, BoardSpace _)
        {
            // TODO? Make this more robust? Might want to compare to ActiveBoard or something
            if (m_PieceToBoardMap.ContainsKey(piece))
            {
                m_PieceToBoardMap[piece] = board;
            }
            else
            {
                m_PieceToBoardMap.Add(piece, board);
            }
        }
    }
}
