using System;
using System.Collections;
using System.Collections.Generic;
using GGJ;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

[RequireComponent(typeof(BoardPiece))]
public class Character : MonoBehaviour, IBoardPiece
{
    // TODO? Intentions could probably be simplified -
    //      Remove this enum and just store memories as a vector corresponding to the move the character
    //      wants to make. "Intent" here is better represented by "Motivation" in the IntentionProvider
    public enum Intent
    {
        Move,
        InteractInPlace
    }

    public struct Intention
    {
        public Intent Intent { get; }
        public Vector2Int Direction { get; internal set; }
        public BoardPiece Target { get; internal set; }

        // TODO: Sanitize input by using different constructors for different Intents
        //       e.g. an Intent to stand still should never require a direction
        public Intention(Intent intent, Vector2Int direction)
        {
            Intent = intent;
            Direction = direction;
            Target = null;
        }

        // Making big assumptions here... but doing this right is OUT OF SCOPE
        public Intention(Vector2Int direction, BoardPiece target)
        {
            Intent = Intent.InteractInPlace;
            Direction = direction;
            Target = target;
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
    // NOTE: This may be null if a Character can't carry objects
    // TODO? Add separate Hands component for items visibly carried by Character?
    Inventory m_Inventory;
    // TODO? Could log both timestamp and Turn # when intent was received
    Queue<Memory> m_IntentBuffer;
    Intention? m_ActiveIntent;

    public UnityEvent OnMovementAnimationStart;
    public UnityEvent OnMovementAnimationFinish;
    public UnityEvent OnInteract;
    public UnityEvent OnIdleWave;
    public UnityEvent<Character> OnMovementFinished;

    [SerializeField]
    CharacterMovementRules m_MovementRules;

    // How long a character will hold onto Intents before discarding
    // TODO? Differentiate between time-based and turn-based intentions?
    //      Right now this is used primarily to buffer player inputs in real-time, but might also
    //      make sense to use as a way to muddle AI directives if processed by turn?
    [SerializeField]
    float m_AttentionSpan = 0.2f;

    public BoardPiece Piece => m_BoardPiece;
    public CharacterMovementRules Movement => m_MovementRules;
    public bool IsMoving { get; private set; }
    public bool IsActing => m_ActiveIntent != null;
    public bool HasInventory => m_Inventory != null;
    public bool HasIntentionsQueued
    {
        get
        {
            PruneIntents();
            return m_IntentBuffer.Count > 0;
        }
    }

    public bool IsTangible => StageState.Instance.IsTangible(this);

    public bool CanPickUp(Item item) => item.isActiveAndEnabled && IsTangible
        && StageState.Instance.IsTangible(item)
        && HasInventory && m_Inventory.CanHold(item);

    // TODO: This should depend on the characters movement rules, but we're locked into "Simple" for now
    public bool CanMoveTo(Vector2Int cellCoordinates) =>
        StageState.Instance.ActiveBoard.TryGetSpace(cellCoordinates, out var space)
        && BoardNavigation.CharacterCanMoveTo(this, space);


    public static implicit operator BoardPiece(Character character) => character?.Piece;

    public void Awake()
    {
        m_BoardPiece = GetComponent<BoardPiece>();
        m_IntentBuffer = new Queue<Memory>();
        m_Inventory = GetComponent<Inventory>();
        if (m_MovementRules == null)
        {
            Debug.LogWarning($"{name} has no {nameof(m_MovementRules)} assigned!");
        }
    }

    public void Update()
    {
        PruneIntents();
    }

    // TODO? Should this be registered as an intention?
    public void PickUp(Item item)
    {
        m_Inventory.Add(item);
    }

    void PruneIntents()
    {
        while (m_IntentBuffer.TryPeek(out var next) && next.TimeElapsed > m_AttentionSpan)
        {
            m_IntentBuffer.Dequeue();
        }
    }

    public void ProcessIntents()
    {
        PruneIntents();

        Assert.IsFalse(IsActing, $"Tried to {nameof(ProcessIntents)} while {name} was already acting" +
            $"You must wait until previous intent is resolved before calling again.");


        // Get the most recent intent entered
        while (m_IntentBuffer.TryDequeue(out var entry))
        {
            m_ActiveIntent = entry.intention;
        }

        if (m_ActiveIntent != null)
        {
            Act();
        }
    }

    void Act()
    {
        Assert.IsFalse(m_ActiveIntent == null,
            $"Can't call the parameterless {nameof(Act)} if no {nameof(m_ActiveIntent)} is set.");
        var intention = m_ActiveIntent.Value;
        // Can't use IsNotNull here because Nullable<T> it technically still a value type
        switch (intention.Intent)
        {
            case Intent.Move:
                if (intention.Direction == Vector2Int.zero ||
                    !TryMove(intention.Direction))
                {
                    MarkMovementFinished();
                }
                break;
            case Intent.InteractInPlace:
                // TODO: Error check to ensure this intent is still valid
                OnInteract?.Invoke();
                intention.Target.OnInteractedWith?.Invoke();
                MarkMovementFinished();
                break;
            default:
                throw new ArgumentOutOfRangeException($"No handling for {intention.Intent}.");
        }
    }

    void Act(Intention intention)
    {
        m_ActiveIntent = intention;
        Act();
    }

    // TODO?
    // This could be more robust by having events pass their Intent value in here and only clearing when
    // the Intent matches the active intention, but this is already way over-engineered, so....
    void MarkMovementFinished()
    {
        IsMoving = false;
        m_ActiveIntent = null;
        OnMovementFinished?.Invoke(this);
        PruneIntents();
    }

    IEnumerator MoveInWorld(Vector3 start, Vector3 end)
    {
        OnMovementAnimationStart?.Invoke();
        var direction = (end - start);
        var distance = direction.magnitude;
        if (Mathf.Approximately(distance, 0f))
        {
            Debug.LogWarning($"{name} attempted to move to where it already was");
            MarkMovementFinished();
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
        OnMovementAnimationFinish?.Invoke();
        MarkMovementFinished();
    }

    void PerformWorldMovement(Vector3 from, Vector3 to)
    {
        IsMoving = true;
        StartCoroutine(MoveInWorld(from, to));
    }

    bool TryMove(Vector2Int direction)
    {
        if (IsMoving)
        {
            Debug.LogWarning($"{name} is still moving from the last move command.");
            return false;
        }
        var startPosition = transform.position;
        if (BoardNavigation.TryMove(this, direction, out var space))
        {
            PerformWorldMovement(startPosition, space.CoordinatesWorld);
            return true;
        }

        return false;
    }

    public void ReceiveIntent(Intention intention, bool canAct = true)
    {
        if (IsActing || !canAct)
        {
            m_IntentBuffer.Enqueue(new Memory(intention));
        }
        else
        {
            Act(intention);
        }

    }
}
