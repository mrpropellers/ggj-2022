using System;
using System.Collections.Generic;
using System.Linq;
using GGJ.GlobalConsts;
using GGJ.Utility;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace GGJ
{
    // TODO: This entire class should really be a state machine - we just don't have time to implement a framework
    /// <summary>
    /// Right now this is effectively an "AIIntentionProvider" aka the place where you define the per-object
    /// behavior of AI-controlled Characters
    /// </summary>
    [RequireComponent(typeof(Character))]
    public class IntentionProvider : MonoBehaviour
    {
        public enum Motivation
        {
            // Move towards closest ingredient
            Eat,
            // Wander, aimlessly
            Wander,
            // Always chase player, even if that means landing on them
            Chase,
            // Always stay within one-two squares of the player
            //Haunt,
            // Always run away from the player
            RunAway
        }


        #region Member Variables

        Board m_Board;
        Motivation m_MostRecentMotivation;
        BoardPiece m_MostRecentFocus;
        BoardSpace m_HomeSpace;
        Character m_Self;
        Character m_Player;
        List<Item> m_Ingredients;

        #endregion

        #region Inspector Fields

        // Motivation to use when Character is tangible
        [SerializeField]
        Motivation m_MotivationTangible;
        // Motivation to use when Character is in-tangible
        [SerializeField]
        Motivation m_MotivationEthereal;
        [SerializeField]
        Motivation m_MotivationFallback = Motivation.Wander;
        // How likely the Character is to wander away from its starting space
        [SerializeField]
        [Range(0f, 1f)]
        float m_WanderFreedom;

        #endregion

        #region Properties

        Motivation PreviousMotivation => m_MostRecentMotivation;
        public Character Self
        {
            get
            {
                if (m_Self == null)
                {
                    m_Self = GetComponent<Character>();
                }
                return m_Self;
            }
        }

        #endregion

        #region Events

        UnityEvent<IntentionProvider, Motivation> OnMotivationChange;

        #endregion

        #region Engine Messages

        void Start()
        {
            m_Board = StageState.Instance.ActiveBoard;
            m_Player = StageState.Instance.PlayerCharacter;
            m_HomeSpace = m_Board.GetSpace(Self);
            m_Ingredients = new List<Item>();
            m_Ingredients.AddRange(FindObjectsOfType<Item>()
                .Where(item => item.CompareTag(Tags.Ingredient)));
        }

        #endregion

        #region Private Methods
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

        bool MotivationIsAchievable(Motivation motivation)
        {
            switch (motivation)
            {
                case Motivation.Chase:
                case Motivation.RunAway:
                case Motivation.Wander:
                    return true;
                case Motivation.Eat:
                    return m_Ingredients.Any(item => item.isActiveAndEnabled);
                 default:
                     throw new ArgumentOutOfRangeException($"No handling for {motivation}");
            }
        }

        // TODO: This should check the Character's inventory and filter closest Items by Inventory tags
        BoardPiece FindClosestIngredient()
        {
            var board = StageState.Instance.ActiveBoard;
            var selfCoordinates = board.GetSpace(Self).CoordinatesGrid;
            BoardPiece target = null;
            var minimumDistance = int.MaxValue;
            foreach (var ingredient in m_Ingredients)
            {
                if (!ingredient.isActiveAndEnabled)
                {
                    continue;
                }

                var itemCoordinates = board.GetSpace(ingredient.Piece).CoordinatesGrid;
                var distance = QuickMaths.GridDistance(selfCoordinates, itemCoordinates);
                if (distance < minimumDistance)
                {
                    target = ingredient;
                    minimumDistance = distance;
                }
                else if (distance == minimumDistance)
                {
                    target = m_MostRecentFocus == target ? target : ingredient;
                }
            }
            Assert.IsNotNull(target,
                $"Failed to find a valid {nameof(Item)} to eat, " +
                $"but {nameof(MotivationIsAchievable)} had returned true...");
            return target;
        }

        void HandleMotivationChange(Motivation newMotivation)
        {
            m_MostRecentMotivation = newMotivation;
        }

        Vector2Int GenerateWanderVector()
        {
            var coordinatesSelfWorld = m_Board.GetWorldCoordinates(Self);
            var coordinatesHomeWorld = m_Board.GetWorldCoordinates(m_HomeSpace);
            var homePull = (Vector2)(coordinatesHomeWorld - coordinatesSelfWorld) * (1f - m_WanderFreedom);
            var wanderPull = QuickMaths.RandomDirection() * m_WanderFreedom;
            var direction = homePull + wanderPull;
            return Self.Movement.GetMove(direction);
        }

        #endregion


        #region Public Methods
        public void ProvideIntention()
        {
            var motivation = Self.IsTangible
                ? m_MotivationTangible
                : m_MotivationEthereal;
            if (!MotivationIsAchievable(motivation))
            {
                motivation = m_MotivationFallback;
            }

            if (motivation != m_MostRecentMotivation)
            {
                OnMotivationChange?.Invoke(this, motivation);
                HandleMotivationChange(motivation);
            }

            var board = StageState.Instance.ActiveBoard;
            var towardsPlayer = m_Player.transform.position - transform.position;
            Vector2Int move;
            switch (motivation)
            {
                case Motivation.Chase:
                    m_MostRecentFocus = m_Player.Piece;
                    move = ComputeBestMoveInDirection(board, m_Self.Movement, towardsPlayer);
                    break;
                case Motivation.RunAway:
                    m_MostRecentFocus = m_Player.Piece;
                    move = ComputeBestMoveInDirection(board, m_Self.Movement, -towardsPlayer);
                    break;
                case Motivation.Eat:
                    m_MostRecentFocus = FindClosestIngredient();
                    var towardsItem = m_MostRecentFocus.transform.position - transform.position;
                    move = ComputeBestMoveInDirection(board, m_Self.Movement, towardsItem);
                    break;
                case Motivation.Wander:
                    move = GenerateWanderVector();
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"No handling for {motivation}");
            }

            Self.ReceiveIntent(new Character.Intention(Character.Intent.Move, move));
        }
        #endregion
    }
}
