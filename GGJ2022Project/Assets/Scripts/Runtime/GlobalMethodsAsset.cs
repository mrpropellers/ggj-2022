using UnityEngine;
using GGJ.Utility;
using UnityEngine.Events;

namespace GGJ
{
    [CreateAssetMenu(menuName = "GGJ/GlobalMethodsAsset")]
    public class GlobalMethodsAsset : ScriptableObject
    {
        public UnityEvent OnEnterApplication;
        public UnityEvent OnGameStart;
        public UnityEvent OnGameQuit;
        public UnityEvent OnGameStop;

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

        public void EnterApplication()
        {
            OnEnterApplication?.Invoke();
        }

        public void StartGame(string entrySceneName)
        {
            OnGameStart?.Invoke();
            GameObject inGameStateGameObject = Instantiate(InGameState.GetPrefab());
            inGameStateGameObject.GetComponent<InGameState>().EntrySceneName = entrySceneName;
        }

        public void StopGame()
        {
            OnGameStop?.Invoke();
            Destroy(SingletonHelper<InGameState>.Singleton?.gameObject);
        }

        public void QuitFromStageMenu()
        {
            OnGameQuit?.Invoke();
            StopGame();
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
