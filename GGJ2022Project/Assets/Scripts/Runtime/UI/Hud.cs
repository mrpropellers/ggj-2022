using System;
using UnityEngine;

namespace GGJ.UI
{
    public class Hud : MonoBehaviour
    {
        #region Inspector Parameters
        public GameObject GameObjectToActivateWhileObserving;
        #endregion

        #region Engine Messages
        private void Update()
        {
            if (GameObjectToActivateWhileObserving)
            {
                GameObjectToActivateWhileObserving.SetActive((bool)ObservedCharacter);
            }
        }
        #endregion

        #region Observed Character
        private Character m_ObservedCharacter;
        public Character ObservedCharacter {
            get => m_ObservedCharacter;
            set
            {
                if (ReferenceEquals(m_ObservedCharacter, value))
                {
                    return;
                }
                if (!ReferenceEquals(m_ObservedCharacter, null))
                {
                    HandleObservedCharacterLost(m_ObservedCharacter);
                }
                m_ObservedCharacter = value;
                if (!ReferenceEquals(m_ObservedCharacter, null))
                {
                    HandleObservedCharacterDiscovered(m_ObservedCharacter);
                }
            }
        }

        private void HandleObservedCharacterDiscovered(Character observedCharacter)
        {
            try
            {
                OnObservedCharacterDiscovered?.Invoke(observedCharacter);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void HandleObservedCharacterLost(Character observedCharacter)
        {
            try
            {
                OnObservedCharacterLost?.Invoke(observedCharacter);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        #endregion

        public event Action<Character> OnObservedCharacterDiscovered;
        public event Action<Character> OnObservedCharacterLost;
    }
}
