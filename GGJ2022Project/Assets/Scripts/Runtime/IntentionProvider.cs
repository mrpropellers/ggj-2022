using System;
using GGJ.Utility;
using UnityEngine;

namespace GGJ
{
    /// <summary>
    /// Right now this is effectively an "AIIntentionProvider" aka the place where you define the per-object
    /// behavior of AI-controlled Characters
    /// </summary>
    [RequireComponent(typeof(Character))]
    public class IntentionProvider : MonoBehaviour
    {
        public enum Motivation
        {
            // Wander, aimlessly
            //Wander,
            // Always chase player, even if that means landing on them
            Chase,
            // Always stay within one-two squares of the player
            //Haunt,
            // Always run away from the player
            RunAway
        }

        // Motivation to use when Character is tangible
        [SerializeField]
        Motivation m_MotivationTangible;
        // Motivation to use when Character is in-tangible
        [SerializeField]
        Motivation m_MotivationEthereal;
        Character m_Self;

        public Character Self => m_Self;

        public void Awake()
        {
            m_Self = GetComponent<Character>();
        }

        bool MoveIsNotValid(Board board, Vector2Int move)
        {
            return !m_Self.CanMoveTo(board.GetSpace(m_Self) + move);
        }

        // TODO: Use A* here lol, lmao
        Vector2Int ComputeBestMoveInDirection(
            Board board, CharacterMovementRules movementRules, Vector2 direction)
        {
            var move = movementRules.GetMove(direction);
            if (MoveIsNotValid(board, move))
            {
                move = movementRules.GetFallbackMove(direction);
                if (MoveIsNotValid(board, move))
                {
                    return Vector2Int.zero;
                }
            }

            return move;
        }

        public void ProvideIntention(Character other)
        {
            var motivation = m_Self.IsTangible
                ? m_MotivationTangible
                : m_MotivationEthereal;
            var board = StageState.Instance.ActiveBoard;
            var towardsTarget = other.transform.position - transform.position;
            switch (motivation)
            {
                case Motivation.Chase:
                    m_Self.ReceiveIntent(new Character.Intention(Character.Intent.Move,
                        ComputeBestMoveInDirection(board, m_Self.Movement, towardsTarget)));
                    break;
                case Motivation.RunAway:
                    m_Self.ReceiveIntent(new Character.Intention(Character.Intent.Move,
                        ComputeBestMoveInDirection(board, m_Self.Movement, -towardsTarget)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"No handling for {motivation}");

            }
        }
    }
}
