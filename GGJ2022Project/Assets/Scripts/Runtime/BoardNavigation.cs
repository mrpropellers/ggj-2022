using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ
{
    public static class BoardNavigation
    {
        // TODO? May want to figure out how to fit a "PerformMove" coroutine in here
        //       It's likely we want to play an animation or resolve effects during a move - should probably trigger
        //       and wait for the resolution of any time-based components before resolving the move
        public static bool TryMove(Character character, CharacterMovementRules movementRules, Vector2 direction,
            out BoardSpace targetSpace)
        {
            var board = StageState.Instance.ActiveBoard;
            var delta = movementRules.GetMove(direction);
            if (!board.TryGetSpace(character.Piece, out var currentSpace))
            {
                Debug.LogError($"Something went wrong when trying to move {character.name}.");
                targetSpace = null;
                return false;
            }

            var moveTarget = currentSpace.CoordinatesGrid + delta;
            if (board.TryGetSpace(moveTarget, out targetSpace) &&
                targetSpace.IsAvailableFor(character.Piece))
            {
                board.PlacePiece(character.Piece, targetSpace);

                return true;
            }

            Debug.Log($"Couldn't move {character.name} in direction {delta}");
            return false;
        }
    }
}
