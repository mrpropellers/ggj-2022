using System;
using System.Collections;
using GGJ.Utility;
using UnityEngine;

namespace GGJ
{
    [CreateAssetMenu]
    public class CharacterMovementRules : ScriptableObject
    {
        public enum Flavor
        {
            Simple
        }

        [SerializeField]
        Flavor m_MovementFlavor = Flavor.Simple;

        [SerializeField]
        [Range(1f, 100f)]
        float m_MovementSpeed = float.PositiveInfinity;

        public float Speed => m_MovementSpeed;

        static Vector2Int QuantizeToFallbackDirection(Vector2 floatVector)
        {
            if (QuickMaths.VectorIsZero(floatVector))
            {
                return Vector2Int.zero;
            }
            var xAbs = Mathf.Abs(floatVector.x);
            var yAbs = Mathf.Abs(floatVector.y);
            if (xAbs < yAbs)
            {
                return xAbs > floatVector.x ? Vector2Int.left : Vector2Int.right;
            }

            return yAbs > floatVector.y ? Vector2Int.down : Vector2Int.up;
        }

        static Vector2Int QuantizeToDirection(Vector2 floatVector)
        {
            if (QuickMaths.VectorIsZero(floatVector))
            {
                return Vector2Int.zero;
            }
            var xAbs = Mathf.Abs(floatVector.x);
            var yAbs = Mathf.Abs(floatVector.y);
            if (xAbs > yAbs)
            {
                return xAbs > floatVector.x ? Vector2Int.left : Vector2Int.right;
            }

            return yAbs > floatVector.y ? Vector2Int.down : Vector2Int.up;
        }

        public Vector2Int GetMove(Vector2 direction)
        {
            return m_MovementFlavor switch
            {
                Flavor.Simple => QuantizeToDirection(direction),
                _ => throw new ArgumentOutOfRangeException($"No handling for {m_MovementFlavor}")
            };
        }

        public Vector2Int GetFallbackMove(Vector2 direction)
        {
            return m_MovementFlavor switch
            {
                Flavor.Simple => QuantizeToFallbackDirection(direction),
                _ => throw new ArgumentOutOfRangeException($"No handling for {m_MovementFlavor}")
            };
        }
    }
}
