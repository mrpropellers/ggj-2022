using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace GGJ
{
    public class Inventory : MonoBehaviour, IHoldItems
    {
        HashSet<Item> m_HeldItems;

        // TODO: Change to a TagField using custom inspector
        [SerializeField]
        List<string> m_AcceptedTags;

        [SerializeField]
        UnityEvent<Item> m_OnItemAdd;
        [SerializeField]
        UnityEvent<Item> m_OnItemRemove;

        public UnityEvent<Item> OnItemAdd => m_OnItemAdd;
        public UnityEvent<Item> OnItemRemove => m_OnItemRemove;

        public bool CanHold(Item item) => m_AcceptedTags.Contains(item.tag);
        public bool IsHolding(Item item) => m_HeldItems.Contains(item);
        public IEnumerator<Item> AllHeldItems => m_HeldItems.GetEnumerator();

        public void Awake()
        {
            m_HeldItems = new HashSet<Item>();
        }

        public void Add(Item item)
        {
            Assert.IsTrue(m_AcceptedTags.Contains(item.tag),
                $"{name} doesn't accept items with tag {item.tag}.");
            Assert.IsFalse(m_HeldItems.Contains(item),
                $"Tried to add {item.name} to {name}'s {nameof(Inventory)} twice...");
            item.gameObject.SetActive(false);
            m_HeldItems.Add(item);
            OnItemAdd?.Invoke(item);
        }

        public void Remove(Item item)
        {
            Assert.IsTrue(m_HeldItems.Contains(item),
                $"Tried to remove {item.name} which isn't held by {name}");
            m_HeldItems.Remove(item);
            item.gameObject.SetActive(true);
            OnItemRemove?.Invoke(item);
        }

    }
}
