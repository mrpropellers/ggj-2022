using System;
using UnityEngine;
using GGJ.Utility;

namespace GGJ.UI
{
    public class PauseMenuPanel : MonoBehaviour
    {
        #region Engine Messages
        private void OnEnable()
        {
            SingletonHelper<PauseMenuPanel>.HandleInstanceEnabled(this);
        }

        private void OnDisable()
        {
            SingletonHelper<PauseMenuPanel>.HandleInstanceDisabled(this);
        }
        #endregion

        public static GameObject GetPrefab()
        {
            return Resources.Load<GameObject>("Prefabs/UI/PauseMenuPanel");
        }

        public static void TogglePauseMenu()
        {
            var existingInstance = SingletonHelper<PauseMenuPanel>.Singleton;
            if (existingInstance)
            {
                Destroy(existingInstance.gameObject);
            }
            else
            {
                Instantiate(original: GetPrefab(), parent: ApplicationState.Instance.MainCanvasTransform);
            }
        }
    }
}
