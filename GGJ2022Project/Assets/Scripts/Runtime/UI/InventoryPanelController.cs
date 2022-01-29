using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ.UI
{
    public class InventoryPanelController : MonoBehaviour
    {
        #region Inspector Parameters
        public ItemIcon[] itemIcons = Array.Empty<ItemIcon>();
        #endregion

        #region Engine Messages
        private void OnEnable()
        {
            Rebuild();
        }

        private void OnDestroy()
        {
            // Clean up.
            inventory = null;
        }
        #endregion

        public void Rebuild()
        {
            if (!enabled)
            {
                return;
            }

            IEnumerator<Item> allItems = inventory?.AllHeldItems ?? emptyEnumerator;
            allItems.Reset();
            for (int i = 0; i < itemIcons.Length; ++i)
            {
                Item item = null;
                if (allItems.MoveNext())
                {
                    item = allItems.Current;
                }
                itemIcons[i].SetItem(item);
            }
        }

        private static readonly IEnumerator<Item> emptyEnumerator = System.Linq.Enumerable.Empty<Item>().GetEnumerator();

        #region Inventory Link
        private IHoldItems m_inventory;
        public IHoldItems inventory
        {
            get => m_inventory;
            set
            {
                if (ReferenceEquals(inventory, value))
                {
                    return;
                }
                if (!ReferenceEquals(inventory, null))
                {
                    inventory.OnItemAdd.RemoveListener(OnInventoryItemAdded);
                    inventory.OnItemRemove.RemoveListener(OnInventoryItemRemove);
                }
                inventory = value;
                if (!ReferenceEquals(inventory, null))
                {
                    inventory.OnItemAdd.AddListener(OnInventoryItemAdded);
                    inventory.OnItemRemove.AddListener(OnInventoryItemRemove);
                }
                Rebuild();
            }
        }

        private void OnInventoryItemAdded(Item item)
        {
            Rebuild();
        }

        private void OnInventoryItemRemove(Item item)
        {
            Rebuild();
        }
        #endregion
    }
}
