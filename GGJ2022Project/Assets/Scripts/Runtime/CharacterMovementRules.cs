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

        [SerializeField]
        bool m_CanPickUpItems;

        public float Speed => m_MovementSpeed;

        public Vector2Int GetMove(Vector2 direction)
        {
            return m_MovementFlavor switch
            {
                Flavor.Simple => QuickMaths.QuantizeToDirection(direction),
                _ => throw new ArgumentOutOfRangeException($"No handling for {m_MovementFlavor}")
            };
        }
    }
}
