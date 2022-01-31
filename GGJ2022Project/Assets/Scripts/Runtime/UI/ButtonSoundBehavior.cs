using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GGJ.UI
{
    [RequireComponent(typeof(Selectable))]
    public class ButtonSoundBehavior : MonoBehaviour, ISelectHandler, IPointerEnterHandler
    {
        #region Inspector Parameters
        public AudioSource HoverAudioSource;
        public AudioSource ClickAudioSource;
        #endregion

        #region Cached Components
        private Selectable m_Selectable;
        #endregion

        #region Engine Messages
        private void Awake()
        {
            m_Selectable = GetComponent<Selectable>();
            if (m_Selectable is Button button)
            {
                button.onClick.AddListener(HandleButtonClicked);
            }
        }
        #endregion

        #region ISelectHandler
        public void OnSelect(BaseEventData eventData)
        {
            if (HoverAudioSource)
            {
                HoverAudioSource.Play();
            }
        }
        #endregion

        #region IPointerEnterHandler
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (HoverAudioSource)
            {
                HoverAudioSource.Play();
            }
        }
        #endregion

        private void HandleButtonClicked()
        {
            if (ClickAudioSource)
            {
                ClickAudioSource.Play();
            }
        }
    }
}
