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

        public void StartGame(string entrySceneName)
        {
            GameObject inGameStateGameObject = Instantiate(InGameState.GetPrefab());
            inGameStateGameObject.GetComponent<InGameState>().EntrySceneName = entrySceneName;
        }

        public void StopGame()
        {
            Destroy(SingletonHelper<InGameState>.Singleton?.gameObject);
        }

        public void ResetGameStage()
        {
            SingletonHelper<InGameState>.Singleton?.ResetStage();
        }

        public void DestroyObject(UnityEngine.Object obj)
        {
            Destroy(obj);
        }

    }
}
