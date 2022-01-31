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

        public enum LevelEndResult
        {
            Success,
            Failure
        }

        EffectsManager m_EffectsManager;

        [SerializeField]
        internal BoardMode StartingBoardMode;

        public Board ActiveBoard { get; private set; }
        public BoardMode CurrentBoardMode { get; private set; }
        public Character PlayerCharacter { get; private set; }

        public UnityEvent OnRealmSwitchStart;
        public UnityEvent OnRealmSwitchFinish;
        public UnityEvent OnRealmSwitchToPhysical;
        public UnityEvent OnRealmSwitchToSpiritual;
        // TODO: These will be invoked by a gameplay monitor that gets implemented once the oven is in
        public UnityEvent OnLevelFailure;
        // When a player tries to interact with an oven without all their ingredients
        public UnityEvent OnLevelIncomplete;
        public UnityEvent OnLevelSuccess;

        public UnityEvent<LevelEndResult> OnLevelEndResolved;

        void Awake()
        {
            EnsureInitialized();
            m_EffectsManager = FindObjectOfType<EffectsManager>();
            OnLevelFailure.AddListener(() => ResolveLevelEnd(LevelEndResult.Failure));
            OnLevelSuccess.AddListener(() => ResolveLevelEnd(LevelEndResult.Success));
        }

        void ToggleBoardMode()
        {
            switch (CurrentBoardMode)
            {
                case BoardMode.Physical:
                    CurrentBoardMode = BoardMode.Spiritual;
                    OnRealmSwitchToSpiritual?.Invoke();
                    break;
                case BoardMode.Spiritual:
                    CurrentBoardMode = BoardMode.Physical;
                    OnRealmSwitchToPhysical?.Invoke();
                    break;
            }
            Debug.Log($"{nameof(CurrentBoardMode)} switched to {CurrentBoardMode}.");
        }

        void ResolveLevelEnd(LevelEndResult result)
        {
            // TODO: Wait for end of level stuff here like sound effects or animations
            OnLevelEndResolved?.Invoke(result);
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
