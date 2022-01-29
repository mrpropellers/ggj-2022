using System;
using UnityEngine;

namespace GGJ
{
    [RequireComponent(typeof(BoardPiece))]
    public class Item : MonoBehaviour
    {
        public Sprite InventorySprite;

        BoardPiece m_Piece;

        public void Awake()
        {
            if (!GlobalConsts.Tags.Items.Contains(tag))
            {
                Debug.LogWarning(
                    $"{name}'s tag, {tag}, is not in the list of known item tags. " +
                    $"Either add {tag} to {nameof(GlobalConsts.Tags.Items)} or change {name}'s tag.");
            }
            m_Piece = GetComponent<BoardPiece>();
        }

        public static implicit operator BoardPiece(Item item) => item.m_Piece;
    }
}
