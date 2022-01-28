using UnityEngine;

namespace GGJ
{
    public class ApplicationState : MonoBehaviour
    {
        #region Inspector Parameters
        public GameObject InGameStatePrefab;
        #endregion

        #region Instance Management
        static ApplicationState s_Instance;

        public static ApplicationState Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    Debug.LogWarning($"{nameof(ApplicationState)} was not initialized before being accessed.");
                }

                return s_Instance;
            }
        }

        [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            GameObject instanceGameObject = Instantiate(Resources.Load<GameObject>("Prefabs/ApplicationState"));
            s_Instance = instanceGameObject.GetComponent<ApplicationState>();
            DontDestroyOnLoad(instanceGameObject);
        }
        #endregion

        #region Engine Messages
        private void Start()
        {
            // TODO: Instantiate InGameStatePrefab from someplace more appropriate, like the button on the main menu that starts the game.
            var inGameStateInstance = Instantiate(InGameStatePrefab);
            DontDestroyOnLoad(inGameStateInstance);
        }
        #endregion

        public Board ActiveBoard { get; set; }



        
    }
}
