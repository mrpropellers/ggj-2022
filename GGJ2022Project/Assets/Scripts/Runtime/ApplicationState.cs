using UnityEngine;

namespace GGJ
{
    public class ApplicationState : MonoBehaviour
    {
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

        public Board ActiveBoard { get; set; }

        [RuntimeInitializeOnLoadMethod(loadType: RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            GameObject instanceGameObject = Instantiate(Resources.Load<GameObject>("Prefabs/ApplicationState"));
            s_Instance = instanceGameObject.GetComponent<ApplicationState>();
            DontDestroyOnLoad(instanceGameObject);
        }
    }
}
