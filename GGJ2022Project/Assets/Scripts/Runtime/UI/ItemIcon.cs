using UnityEngine;
using UnityEngine.UI;

namespace GGJ.UI
{
    public class ItemIcon : MonoBehaviour
    {
        public Image backgroundImage;
        public Image itemImage;

        public Sprite emptyBackgroundSprite;
        public Sprite occupiedBackgroundSprite;

        public void SetItem(Item item)
        {
            Sprite itemSprite = null;
            Sprite backgroundSprite;

            // Collect the sprites we want to use.
            if (item)
            {
                var itemSpriteRenderer = item.GetComponentInChildren<SpriteRenderer>();
                if (itemSpriteRenderer)
                {
                    itemSprite = itemSpriteRenderer.sprite;
                }
            }
            backgroundSprite = (itemSprite ? occupiedBackgroundSprite : emptyBackgroundSprite);

            // Apply the collected sprites to their appropriate recipients.
            if (backgroundImage)
            {
                backgroundImage.sprite = backgroundSprite;
            }
            if (itemImage)
            {
                itemImage.sprite = itemSprite;
            }
        }
    }
}
