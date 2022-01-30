using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace GGJ
{
    [RequireComponent(typeof(TurnSystem))]
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

        EffectsManager m_EffectsManager;

        [SerializeField]
        internal BoardMode StartingBoardMode;

        public Board ActiveBoard { get; private set; }
        public BoardMode CurrentBoardMode { get; private set; }
        public Character PlayerCharacter { get; private set; }

        public UnityEvent OnRealmSwitchStart;
        public UnityEvent OnRealmSwitchFinish;

        void Awake()
        {
            EnsureInitialized();
            m_EffectsManager = FindObjectOfType<EffectsManager>();
        }

        void ToggleBoardMode()
        {
            CurrentBoardMode = CurrentBoardMode == BoardMode.Physical
                ? BoardMode.Spiritual
                : BoardMode.Physical;
            Debug.Log($"{nameof(CurrentBoardMode)} switched to {CurrentBoardMode}.");
        }

        internal static bool IsTangible(BoardPiece piece, BoardMode mode)
        {
            var tangibility = piece.PieceTangibility;
            Assert.AreNotEqual(tangibility, BoardPiece.Tangibility.Undefined,
                $"{piece.name} was not initialized correctly, its {nameof(BoardPiece.Tangibility)} is not set.");
            return tangibility == BoardPiece.Tangibility.Both
                || tangibility == BoardPiece.Tangibility.Physical && mode == BoardMode.Physical
                || tangibility == BoardPiece.Tangibility.Spritual && mode == BoardMode.Spiritual;
        }

        public bool IsTangible(BoardPiece piece)
        {
            return IsTangible(piece, CurrentBoardMode);
        }

        public bool DoTangibilitiesMatch(BoardPiece a, BoardPiece b)
        {
            return IsTangible(a) ? IsTangible(b) : !IsTangible(b);
        }

        internal static bool SpaceSupportsHoldingPiece(
            BoardSpace space, BoardPiece piece, BoardMode mode)
        {
            return IsTangible(piece, mode) ? space.CanHoldTangible : space.CanHoldEthereal;
        }

        public bool SpaceSupportsHoldingPiece(BoardSpace space, BoardPiece piece)
        {
            return SpaceSupportsHoldingPiece(space, piece, CurrentBoardMode);
        }

        public IEnumerator InitiateRealmSwitch()
        {
            OnRealmSwitchStart?.Invoke();
            ToggleBoardMode();
            yield return m_EffectsManager.SetRealmEffects();
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
                s_Instance.CurrentBoardMode = s_Instance.StartingBoardMode;
                s_Instance.PlayerCharacter = FindObjectOfType<PlayerTurnReceiver>()?.PlayerCharacter;
            }
        }
    }
}
