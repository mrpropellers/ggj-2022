using UnityEngine;

namespace GGJ
{
    public class StageState : MonoBehaviour
    {
        static StageState s_Instance;

        public static StageState Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    var go = new GameObject(nameof(StageState));
                    s_Instance = go.AddComponent<StageState>();
                }

                return s_Instance;
            }
        }

        public Board CurrentBoard { get; }
    }
}
