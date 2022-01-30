using UnityEngine;
using GGJ.Utility;

namespace GGJ
{
    public class InGameState : MonoBehaviour
    {
        #region Inspector Parameters
        #endregion

        public string EntrySceneName { get; set; }
        public string ExitSceneName { get; set; } = "MainMenu";

        #region Engine Messages
        private void Start()
        {
            // Hacky condition to prevent immediate scene switch when entering a specific scene directly from entering play mode in editor.
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu")
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName: EntrySceneName);
            }
        }

        private void OnDestroy()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName: ExitSceneName);
        }

        private void OnEnable()
        {
            SingletonHelper<InGameState>.HandleInstanceEnabled(this);
        }

        private void OnDisable()
        {
            SingletonHelper<InGameState>.HandleInstanceDisabled(this);
        }

        private void Update()
        {
            bool pausePressed = false;
            foreach (var playerInput in UnityEngine.InputSystem.PlayerInput.all)
            {
                if (playerInput.actions.FindActionMap("Player").FindAction("Pause").WasPressedThisFrame())
                {
                    pausePressed = true;
                    break;
                }
            }
            if (pausePressed)
            {
                UI.PauseMenuPanel.TogglePauseMenu();
            }
        }
        #endregion

        public static InGameState Singleton => SingletonHelper<InGameState>.Singleton;

        public static GameObject GetPrefab()
        {
            return Resources.Load<GameObject>("Prefabs/InGameState");
        }
    }
}
