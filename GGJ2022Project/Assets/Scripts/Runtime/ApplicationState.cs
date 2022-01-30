using UnityEngine;
using GGJ.Utility;

namespace GGJ
{
    public class ApplicationState : MonoBehaviour
    {
        #region Inspector Parameters
        public RectTransform MainCanvasTransform;
        #endregion

        #region Instance Management
        public static ApplicationState Instance => SingletonHelper<ApplicationState>.Singleton;

        [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            Instantiate(Resources.Load<GameObject>("Prefabs/ApplicationState"));
        }
        #endregion

        #region Engine Messages
        private void Start()
        {
            StartedInMenu = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "MainMenu";
            // TODO: Instantiate InGameStatePrefab from someplace more appropriate, like the button on the main menu that starts the game.
            if (!StartedInMenu)
            {
                Instantiate(InGameState.GetPrefab());
            }
        }

        private void OnEnable()
        {
            SingletonHelper<ApplicationState>.HandleInstanceEnabled(this);
        }

        private void OnDisable()
        {
            SingletonHelper<ApplicationState>.HandleInstanceDisabled(this);
        }
        #endregion

        public Board ActiveBoard { get; set; }
        public bool StartedInMenu { get; private set; }
    }
}
