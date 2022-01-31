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
        Coroutine m_TurnCoroutine;
        TurnReceiver m_CurrentTurnReceiver;

        int m_CurrentTurnNumber;

        public bool StageIsFinished { get; private set; }

        void OnEnable()
        {
            m_CurrentTurnNumber = 1;
            m_TurnCoroutine = StartCoroutine(ProcessTurns());
        }

        private void OnDisable()
        {
            if (m_TurnCoroutine != null)
            {
                StopCoroutine(m_TurnCoroutine);
                m_TurnCoroutine = null;
            }
        }

        void Start()
        {
            StageIsFinished = false;
            StageState.Instance.OnLevelFailure.AddListener(MarkStageFinished);
            StageState.Instance.OnLevelSuccess.AddListener(MarkStageFinished);
        }

        void MarkStageFinished()
        {
            Debug.Log(
                $"{nameof(TurnSystem)} on {name} acknowledges that this stage is finished.");
            StageIsFinished = true;
        }

        TurnReceiver PickNextTurnReceiver()
        {
            TurnReceiver bestTurnReceiver = null;
            var earliestPreviousTurn = int.MaxValue;

            foreach (var turnReceiver in InstanceTracker<TurnReceiver>.GetInstancesReadOnly())
            {
                if (turnReceiver.PreviousTurnNumber < earliestPreviousTurn)
                {
                    earliestPreviousTurn = turnReceiver.PreviousTurnNumber;
                    bestTurnReceiver = turnReceiver;
                }
            }
            return bestTurnReceiver;
        }

        IEnumerator ProcessTurns()
        {
            while (!StageIsFinished)
            {
                yield return null;

                // Pick the receiver for this turn.
                TurnReceiver currentTurnReceiver = PickNextTurnReceiver();
                Debug.Log($"[Turn {m_CurrentTurnNumber}] It is {currentTurnReceiver.name}'s turn.");

                // Early out if there's nothing to receive a turn.
                if (currentTurnReceiver == null)
                {
                    continue;
                }

                // Dispatch the turn to currentTurnReceiver and run the provided coroutine to completion.
                var turn =
                    new Turn(turnSystem: this, turnReceiver: currentTurnReceiver, turnNumber: m_CurrentTurnNumber);
                onPreEnterTurn?.Invoke(turn);

                var currentTurnCoroutine = currentTurnReceiver.HandleTurn(turn);
                yield return currentTurnCoroutine;

                onPostExitTurn?.Invoke(turn);
                Debug.Log($"[Turn {m_CurrentTurnNumber}] {currentTurnReceiver.name}'s turn is complete.");
                m_CurrentTurnNumber++;
            }
        }

        public static event Action<Turn> onPreEnterTurn;
        public static event Action<Turn> onPostExitTurn;
    }
}
