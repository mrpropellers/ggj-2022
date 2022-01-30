using UnityEngine;

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

        public void DestroyObject(UnityEngine.Object obj)
        {
            Destroy(obj);
        }
    }
}
