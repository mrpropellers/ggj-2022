using System.Collections.Generic;
using UnityEngine.Events;

namespace GGJ
{
    public interface IHoldItems
    {
        public IEnumerator<Item> AllHeldItems { get; }
        public UnityEvent<Item> OnItemAdd { get; }
        public UnityEvent<Item> OnItemRemove { get; }
        public void Add(Item item);

        public void Remove(Item item);

        public bool CanHold(Item item);

        public bool IsHolding(Item item);

    }
}
