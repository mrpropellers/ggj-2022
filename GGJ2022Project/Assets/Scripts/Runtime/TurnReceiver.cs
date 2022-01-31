using System.Collections;
using UnityEngine;
using GGJ.Utility;
using UnityEngine.Assertions;

namespace GGJ
{
    /// <summary>
    /// Base class for behaviors that can take a game turn.
    /// </summary>
    public abstract class TurnReceiver : MonoBehaviour
    {
        protected enum TurnPhase
        {
            InActive,
            WaitingForIntent,
            ResolvingIntent,
            Finished
        }

        protected TurnPhase CurrentPhase { get; private set; }
        protected bool IsHandlingTurn => CurrentPhase != TurnPhase.InActive;

        public int PreviousTurnNumber { get; private set; }

        // Should be a negative number - lowest number goes first, and so on
        public abstract int StartingTurnNumber { get; }

        protected void PhaseComplete(TurnPhase phaseJustCompleted)
        {
            // NOTE: You could just cheat and always call this function with CurrentPhase as the argument
            //       but that would defeat the purpose of this check so please don't
            Assert.IsTrue(phaseJustCompleted == CurrentPhase,
                $"{name} said they just finished {phaseJustCompleted} - but they should have " +
                $"been on {CurrentPhase}");
            CurrentPhase++;
        }

        protected void OnEnable()
        {
            PreviousTurnNumber = StartingTurnNumber;
            InstanceTracker<TurnReceiver>.Add(this);
        }

        protected void OnDisable()
        {
            InstanceTracker<TurnReceiver>.Remove(this);
        }

        public IEnumerator HandleTurn(Turn incomingTurn)
        {
            PreviousTurnNumber = incomingTurn.turnNumber;
            CurrentPhase = TurnPhase.WaitingForIntent;
            // We could probably just WaitUntil(CurrentPhase == TurnPhase.Finished),
            // but that would inject an extra frame into every turn -
            //  which our speed-running community would complain about
            yield return HandleTurn_impl(incomingTurn);
            yield return null;
            CurrentPhase = TurnPhase.InActive;

        }

        protected abstract IEnumerator HandleTurn_impl(Turn incomingTurn);
    }
}
