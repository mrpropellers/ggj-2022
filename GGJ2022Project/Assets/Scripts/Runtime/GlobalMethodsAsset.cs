using UnityEngine;
using GGJ.Utility;

namespace GGJ
{
    [CreateAssetMenu(menuName = "GGJ/GlobalMethodsAsset")]
    public class GlobalMethodsAsset : ScriptableObject
    {
        public void QuitApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }

        public void SetScene(string sceneName)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName: sceneName);
        }

        public void StartGame()
        {
            Instantiate(InGameState.GetPrefab());
        }

        public void StopGame()
        {
            Destroy(SingletonHelper<InGameState>.Singleton);
        }

        public void DestroyObject(UnityEngine.Object obj)
        {
            Destroy(obj);
        }
    }
}
