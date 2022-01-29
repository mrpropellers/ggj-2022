using UnityEngine;
using UnityEngine.UI;

namespace GGJ.UI
{
    public class ItemIcon : MonoBehaviour
    {
        #region Inspector Parameters
        public Image ItemImage;
        public Sprite DefaultSprite;
        #endregion

        public void SetItem(Item item)
        {
            Sprite itemSprite = DefaultSprite;

            // Collect the sprites we want to use.
            if (item)
            {
                itemSprite = item.InventorySprite;
            }

            // Apply the collected sprites to their appropriate recipients.
            if (ItemImage)
            {
                ItemImage.sprite = itemSprite;
            }
        }
    }
}
