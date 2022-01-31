using System;
using System.Collections;
using System.Linq;
using GGJ.Utility;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace GGJ
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerTurnReceiver : TurnReceiver
    {
        [SerializeField]
        Character m_SelectedCharacter;

        public Character PlayerCharacter => m_SelectedCharacter;

        public override int StartingTurnNumber => -2;

        bool CanRealmSwitch => !Application.isPlaying || IsWaitingForInput;

        bool IsWaitingForInput => CurrentPhase == TurnPhase.WaitingForIntent;

        #region Engine Messages
        // TODO? If the player had an arbitrary number of characters, we'd want to subscribe to some other event
        void Awake()
        {
            Assert.IsNotNull(m_SelectedCharacter, $"{name} has no {nameof(Character)} assigned. " +
                $"Game is broken.");
            m_SelectedCharacter.OnMovementFinished.AddListener(MarkMovementPhaseComplete);
        }

        void Update()
        {
            if (PlayerCharacter.HasIntentionsQueued && CurrentPhase == TurnPhase.WaitingForIntent)
            {
                ResolveIntention();
            }
        }

        protected new void OnEnable()
        {
            base.OnEnable();
            OnPlayerTurnReceiverDiscovered?.Invoke(this);
        }

        protected new void OnDisable()
        {
            OnPlayerTurnReceiverLost?.Invoke(this);
            base.OnDisable();
        }
        #endregion

        void ResolveIntention()
        {
            Assert.AreEqual(CurrentPhase, TurnPhase.WaitingForIntent);
            PhaseComplete(TurnPhase.WaitingForIntent);
            m_SelectedCharacter.ProcessIntents();
        }

        // Need a separate button for invocations from the UI
        public void OnRealmSwitch()
        {
            if (CanRealmSwitch)
            {
                PhaseComplete(TurnPhase.WaitingForIntent);
                StageState.Instance.OnRealmSwitchFinish.AddListener(MarkRealmSwitchComplete);
                StartCoroutine(StageState.Instance.InitiateRealmSwitch());
            }
        }

        public void OnRealmSwitch(InputAction.CallbackContext context)
        {
            if (!context.action.WasPerformedThisFrame())
            {
                return;
            }

            if (PlayerInputSuppressor.ShouldSuppressPlayerInput)
            {
                return;
            }

            OnRealmSwitch();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (!context.action.WasPerformedThisFrame())
            {
                return;
            }

            if (PlayerInputSuppressor.ShouldSuppressPlayerInput)
            {
                return;
            }

            var direction = context.action.ReadValue<Vector2>();
            var directionOnGrid = m_SelectedCharacter.Movement.GetMove(direction);
            // Not sure if this is possible, but we should ignore it if it happens
            if (directionOnGrid == Vector2Int.zero)
            {
                return;
            }

            var board = StageState.Instance.ActiveBoard;
            var currentSpace = board.GetSpace(m_SelectedCharacter);
            var targetSpace = board.GetSpace(currentSpace + directionOnGrid);

            var intention = targetSpace.HasAny<Oven>()
                ? new Character.Intention(directionOnGrid, targetSpace.GetAllPieces<Oven>().First())
                : new Character.Intention(Character.Intent.Move, directionOnGrid);
            m_SelectedCharacter.ReceiveIntent(intention, false);
            if (CurrentPhase == TurnPhase.WaitingForIntent)
            {
                ResolveIntention();
            }
        }

        void MarkMovementPhaseComplete(Character character)
        {
            Assert.AreEqual(m_SelectedCharacter, character,
                $"{character.name} is not {m_SelectedCharacter.name} - something broke.");
            if (StageState.Instance.ActiveBoard.GetSpace(character).HasAny<Sigil>())
            {
                StageState.Instance.OnRealmSwitchFinish.AddListener(MarkRealmSwitchComplete);
                StartCoroutine(StageState.Instance.InitiateRealmSwitch());
            }
            else
            {
                PhaseComplete(TurnPhase.ResolvingIntent);
            }
        }

        void MarkRealmSwitchComplete()
        {
            PhaseComplete(TurnPhase.ResolvingIntent);
            StageState.Instance.OnRealmSwitchFinish.RemoveListener(MarkRealmSwitchComplete);
        }

        protected override IEnumerator HandleTurn_impl(Turn incomingTurn)
        {
            yield return new WaitUntil(() => CurrentPhase == TurnPhase.Finished);
        }

        #region Public Static Events
        public static event Action<PlayerTurnReceiver> OnPlayerTurnReceiverDiscovered;
        public static event Action<PlayerTurnReceiver> OnPlayerTurnReceiverLost;
        #endregion
    }
}
