using UnityEngine;

namespace GGJ
{
    public class ApplicationState
    {
        static ApplicationState s_Instance;

        public static ApplicationState Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    Debug.LogWarning($"{nameof(ApplicationState)} was not initialized before being accessed.");
                    s_Instance = new ApplicationState();
                }

                return s_Instance;
            }
        }

        public Board ActiveBoard;
    }
}
