using System;
using System.Collections;
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

        void ResolveIntention()
        {
            Assert.AreEqual(CurrentPhase, TurnPhase.WaitingForIntent);
            PhaseComplete(TurnPhase.WaitingForIntent);
            m_SelectedCharacter.ProcessIntents();
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            if (!context.action.WasPerformedThisFrame())
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

            var intention = new Character.Intention(Character.Intent.Move, directionOnGrid);
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
            PhaseComplete(TurnPhase.ResolvingIntent);
        }

        protected override IEnumerator HandleTurn_impl(Turn incomingTurn)
        {
            yield return new WaitUntil(() => CurrentPhase == TurnPhase.Finished);
        }
    }
}
