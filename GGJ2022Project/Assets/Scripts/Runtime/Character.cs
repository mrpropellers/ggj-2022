using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using GGJ;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoardPiece))]
public class Character : MonoBehaviour
{
    public enum Intent
    {
        Move
    }

    public struct Intention
    {
        public Intent Intent { get; }
        public Vector3 Direction { get; internal set; }

        // TODO: Sanitize input by using different constructors for different Intents
        //       e.g. an Intent to stand still should never require a direction
        public Intention(Intent intent, Vector3 direction)
        {
            Intent = intent;
            Direction = direction;
        }
    }

    struct Memory
    {
        float TimeGenerated { get; }
        internal Intention intention { get; }

        internal float TimeElapsed => Time.time - TimeGenerated;

        public Memory(Intention intention)
        {
            TimeGenerated = Time.time;
            this.intention = intention;
        }
    }

    BoardPiece m_BoardPiece;
    // TODO? Could log both timestamp and Turn # when intent was received
    Queue<Memory> m_IntentBuffer;
    Intention? m_ActiveIntent;

    public UnityEvent OnMovementFinished;

    [SerializeField]
    CharacterMovementRules m_MovementRules;

    // How long a character will hold onto Intents before discarding
    // TODO? Differentiate between time-based and turn-based intentions?
    //      Right now this is used primarily to buffer player inputs in real-time, but might also
    //      make sense to use as a way to muddle AI directives if processed by turn?
    [SerializeField]
    float m_AttentionSpan = 0.2f;

    public BoardPiece Piece => m_BoardPiece;
    public bool IsMoving { get; private set; }
    public bool IsActing => m_ActiveIntent != null;

    public void Awake()
    {
        m_BoardPiece = GetComponent<BoardPiece>();
        m_IntentBuffer = new Queue<Memory>();
        OnMovementFinished.AddListener(ClearActiveIntent);
    }

    public void Update()
    {
        ProcessIntents();
    }

    void ProcessIntents()
    {
        while (m_IntentBuffer.TryPeek(out var next) && next.TimeElapsed > m_AttentionSpan)
        {
            m_IntentBuffer.Dequeue();
        }

        if (IsActing)
        {
            return;
        }

        if (m_IntentBuffer.TryDequeue(out var entry))
        {
            Act(entry.intention);
        }
    }

    void Act(Intention intention)
    {
        m_ActiveIntent = intention;
        switch (intention.Intent)
        {
            case Intent.Move:
                if (!TryMove(intention.Direction))
                {
                    ClearActiveIntent();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException($"No handling for {intention.Intent}.");
        }
    }

    // TODO?
    // This could be more robust by having events pass their Intent value in here and only clearing when
    // the Intent matches the active intention, but this is already way over-engineered, so....
    void ClearActiveIntent()
    {
        m_ActiveIntent = null;
        ProcessIntents();
    }

    IEnumerator MoveInScene(Vector3 start, Vector3 end)
    {
        var direction = (end - start);
        var distance = direction.magnitude;
        if (Mathf.Approximately(distance, 0f))
        {
            Debug.LogWarning($"{name} attempted to move to where it already was");
            IsMoving = false;
            yield break;
        }
        var timeNeeded = distance / m_MovementRules.Speed;
        var timeElapsed = 0f;
        while (timeElapsed < timeNeeded)
        {
            var t = timeElapsed / timeNeeded;
            transform.position = Vector3.Lerp(start, end, t);
            yield return null;
            timeElapsed += Time.deltaTime;
        }

        transform.position = end;
        IsMoving = false;
        OnMovementFinished?.Invoke();
    }

    void PerformMovement(Vector3 from, Vector3 to)
    {
        IsMoving = true;
        StartCoroutine(MoveInScene(from, to));
    }

    bool TryMove(Vector2 direction)
    {
        if (IsMoving)
        {
            Debug.LogWarning($"{name} is still moving from the last move command.");
            return false;
        }
        var startPosition = transform.position;
        if (BoardNavigation.TryMove(this, m_MovementRules, direction, out var space))
        {
            // TODO: Fire this as a callback to a "TurnExecutor"
            PerformMovement(startPosition, space.CoordinatesWorld);
            return true;
        }

        return false;
    }

    public void ReceiveIntent(Intention intention)
    {
        if (IsActing)
        {
            m_IntentBuffer.Enqueue(new Memory(intention));
        }
        else
        {
            Act(intention);
        }

    }
}
