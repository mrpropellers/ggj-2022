using System;
using System.Collections;
using GGJ;
using UnityEngine;

public class AITurnReceiver : TurnReceiver
{
    IntentionProvider[] m_ControlledIntentions;
    int m_NumMovementFinished;

    int NumControlled => m_ControlledIntentions.Length;
    public override int StartingTurnNumber => -1;

    void Awake()
    {
        m_ControlledIntentions = FindObjectsOfType<IntentionProvider>();
        foreach (var unit in m_ControlledIntentions)
        {
            unit.Self.OnMovementFinished.AddListener(NoteMovementFinished);
        }
    }

    void NoteMovementFinished(Character _)
    {
        // TODO: Add error-checking to ensure we're in correct phase
        m_NumMovementFinished++;
    }

    protected override IEnumerator HandleTurn_impl(Turn incomingTurn)
    {
        m_NumMovementFinished = 0;
        foreach (var unit in m_ControlledIntentions)
        {
            unit.ProvideIntention();
        }
        PhaseComplete(TurnPhase.WaitingForIntent);
        yield return new WaitUntil(() => NumControlled == m_NumMovementFinished);
        PhaseComplete(TurnPhase.ResolvingIntent);
    }
}
