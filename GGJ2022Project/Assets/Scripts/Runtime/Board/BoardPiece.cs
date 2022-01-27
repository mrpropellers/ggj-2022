using UnityEngine;

namespace GGJ
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BoardPiece : MonoBehaviour
    {
        void Awake()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer.sortingLayerID != GlobalConsts.SortingLayers.Foreground)
            {
                Debug.Log(
                    $"Changing {name}'s sorting layer to {nameof(GlobalConsts.SortingLayers.Foreground)}");
                spriteRenderer.sortingLayerID = GlobalConsts.SortingLayers.Foreground;
            }
        }
    }
}
