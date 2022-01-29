using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

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

        // XXX: No addy third mode-y >:I
        public enum BoardMode
        {
            Physical,
            Spiritual
        }

        public Board ActiveBoard { get; private set; }
        public BoardMode CurrentBoardMode { get; private set; }
        public Character PlayerCharacter { get; private set; }

        public UnityEvent OnRealmSwitchStart;
        public UnityEvent OnRealmSwitchFinish;

        void Awake()
        {
            EnsureInitialized();
        }

        void ToggleBoardMode()
        {
            CurrentBoardMode = CurrentBoardMode == BoardMode.Physical
                ? BoardMode.Spiritual
                : BoardMode.Physical;
            Debug.Log($"{nameof(CurrentBoardMode)} switched to {CurrentBoardMode}.");
        }

        public bool IsTangible(BoardPiece piece)
        {
            var tangibility = piece.PieceTangibility;
            Assert.AreNotEqual(tangibility, BoardPiece.Tangibility.Undefined,
                $"{piece.name} was not initialized correctly, its {nameof(BoardPiece.Tangibility)} is not set.");
            return tangibility == BoardPiece.Tangibility.Both
                || tangibility == BoardPiece.Tangibility.Physical && CurrentBoardMode == BoardMode.Physical
                || tangibility == BoardPiece.Tangibility.Spritual && CurrentBoardMode == BoardMode.Spiritual;
        }

        public IEnumerator InitiateRealmSwitch()
        {
            OnRealmSwitchStart?.Invoke();
            ToggleBoardMode();
            OnRealmSwitchFinish?.Invoke();
            yield break;
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
                }
                s_Instance.ActiveBoard = FindObjectOfType<Board>();
                s_Instance.PlayerCharacter = FindObjectOfType<PlayerTurnReceiver>().PlayerCharacter;
            }
        }
    }
}
