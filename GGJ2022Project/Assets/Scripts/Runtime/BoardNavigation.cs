using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace GGJ
{
    public static class BoardNavigation
    {
        static HashSet<Character> s_RegisteredCharacters;

        static void EnsureCharacterRegistered(Character character)
        {
            if (s_RegisteredCharacters == null)
            {
                s_RegisteredCharacters = new HashSet<Character>();
            }

            if (!s_RegisteredCharacters.Contains(character))
            {
                s_RegisteredCharacters.Add(character);
                character.OnMovementFinished.AddListener(ResolveMovement);
            }
        }

        public static Vector2Int ComputeMovement(Character character, Vector2 rawDirection)
        {
            var move = character.Movement.GetMove(rawDirection);
            var board = StageState.Instance.ActiveBoard;

            if (!board.TryGetSpace(character.Piece, out var currentSpace))
            {
                Assert.IsTrue(false,
                    $"Failed to find {character.name} on {board.name} even though " +
                    $"{nameof(StageState)} said that's where it is.");
            }

            if (board.TryGetSpace(currentSpace + move, out var targetSpace)
                && targetSpace.IsAvailableFor(character.Piece))
            {
                return move;
            }

            return Vector2Int.zero;
        }

        // TODO? May want to figure out how to fit a "PerformMove" coroutine in here
        //       It's likely we want to play an animation or resolve effects during a move - should probably trigger
        //       and wait for the resolution of any time-based components before resolving the move
        public static bool TryMove(Character character, Vector2Int move, out BoardSpace targetSpace)
        {
            EnsureCharacterRegistered(character);

            var board = StageState.Instance.ActiveBoard;

            var moveTarget = board.GetSpace(character) + move;
            if (board.TryGetSpace(moveTarget, out targetSpace)
                && targetSpace.IsAvailableFor(character.Piece))
            {
                board.PlacePiece(character.Piece, targetSpace);
                return true;
            }

            Debug.Log($"Couldn't move {character.name} in direction {move}");
            return false;
        }

        static void ResolveMovement(Character character)
        {
            var board = StageState.Instance.ActiveBoard;
            if (!board.TryGetSpace(character.Piece, out var space))
            {
                Assert.IsNotNull(space,
                    $"Failed to get space for {character.name} which shouldn't be possible since it just moved.");
            }

            // allocate list for this so we don't change the enumerator's state by removing items
            var itemsToPickUp =
                space.GetAll<Item>().Where(character.CanPickUp).ToList();
            foreach (var item in itemsToPickUp)
            {
                // TODO? See note in Character class - this might be better if treated as an intention
                //       Character could resolve its own movement, and use a helper from BoardNavigation
                //       to find the items on its space, but it should be responsible for deciding whether
                //       or not to pick the item up, probably...
                board.RemovePiece(item);
                character.PickUp(item);
            }
        }
    }
}
