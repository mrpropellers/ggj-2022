using System.Collections;
using UnityEngine;
using GGJ.Utility;
using System;

namespace GGJ
{
    /// <summary>
    /// Dispatches and manages game turns.
    /// </summary>
    public class TurnSystem : MonoBehaviour
    {
        private Coroutine turnCoroutine;

        private int currentTurnNumber;

        private void OnEnable()
        {
            turnCoroutine = StartCoroutine(ProcessTurns());
        }

        private void OnDisable()
        {
            if (turnCoroutine != null)
            {
                StopCoroutine(turnCoroutine);
                turnCoroutine = null;
            }
        }

        private TurnReceiver PickNextTurnReceiver()
        {
            TurnReceiver bestTurnReceiver = null;
            int earliestPreviousTurn = int.MaxValue;

            foreach (var turnReceiver in InstanceTracker<TurnReceiver>.GetInstancesReadOnly())
            {
                if (turnReceiver.previousTurnNumber < earliestPreviousTurn)
                {
                    earliestPreviousTurn = turnReceiver.previousTurnNumber;
                    bestTurnReceiver = turnReceiver;
                }
            }
            return bestTurnReceiver;
        }

        private IEnumerator ProcessTurns()
        {
            while (true)
            {
                yield return null;

                // Pick the receiver for this turn.
                TurnReceiver currentTurnReciever = PickNextTurnReceiver();

                // Early out if there's nothing to receive a turn.
                if (currentTurnReciever == null)
                {
                    continue;
                }

                // Dispatch the turn to currentTurnReceiver and run the provided coroutine to completion.
                Turn turn = new Turn(turnSystem: this, turnReceiver: currentTurnReciever, turnNumber: currentTurnNumber);
                onPreEnterTurn?.Invoke(turn);

                IEnumerable currentTurnCoroutine = currentTurnReciever.HandleTurn(turn);
                foreach (var _ in currentTurnCoroutine)
                {
                    yield return null;
                }

                onPostExitTurn?.Invoke(turn);
            }
        }

        public static event Action<Turn> onPreEnterTurn;
        public static event Action<Turn> onPostExitTurn;
    }
}
