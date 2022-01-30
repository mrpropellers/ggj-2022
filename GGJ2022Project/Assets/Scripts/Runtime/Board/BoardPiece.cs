using UnityEngine;
using UnityEngine.Assertions;

namespace GGJ
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class BoardPiece : MonoBehaviour
    {
        public enum Tangibility
        {
            Undefined,
            Physical,
            Spritual,
            Both,
            // TODO?
            Neither
        }

        [SerializeField]
        Tangibility m_Tangibility;

        public Tangibility PieceTangibility
        {
            get => m_Tangibility;
            set
            {
                Assert.IsTrue(m_Tangibility == Tangibility.Undefined,
                    $"{nameof(Tangibility)} was already set for this piece - cannot set it twice");
                m_Tangibility = value;
            }
        }

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
