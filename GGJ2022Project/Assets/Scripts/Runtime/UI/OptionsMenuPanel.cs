using UnityEngine;
using UnityEngine.UI;

namespace GGJ.Assets.Scripts.Runtime.UI
{
    public class OptionsMenuPanel : MonoBehaviour
    {
        #region Inspector Parameters
        [Header("Window Mode Options")]
        public Toggle WindowModeWindowToggle;
        public Toggle WindowModeFullscreenToggle;
        #endregion

        #region Engine Messages
        private void OnEnable()
        {
            var fullScreenMode = UnityEngine.Screen.fullScreenMode;
            WindowModeWindowToggle.isOn = (fullScreenMode == FullScreenMode.Windowed);
            WindowModeFullscreenToggle.isOn = (fullScreenMode == FullScreenMode.FullScreenWindow);

            WindowModeWindowToggle.onValueChanged.AddListener((newValue) =>
            {
                if (newValue)
                {
                    Screen.fullScreenMode = FullScreenMode.Windowed;
                }
            });

            WindowModeFullscreenToggle.onValueChanged.AddListener((newValue) =>
            {
                if (newValue)
                {
                    Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                }
            });
        }
        #endregion
    }
}
