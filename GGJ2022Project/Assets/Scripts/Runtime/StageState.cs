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
                    s_Instance = FindObjectOfType<StageState>();
                    if (s_Instance == null)
                    {
                        Debug.LogWarning($"No {nameof(StageState)} instance found in scene, creating one.");
                        var go = new GameObject(nameof(StageState));
                        s_Instance = go.AddComponent<StageState>();
                        Debug.LogWarning($"Pointing at first available {nameof(Board)} - " +
                            $"could lead to undefined behavior (avoid this by manually creating a {nameof(StageState)} object)");
                        s_Instance.ActiveBoard = FindObjectOfType<Board>();
                    }
                }

                return s_Instance;
            }
        }

        public Board ActiveBoard { get; private set; }
    }
}
