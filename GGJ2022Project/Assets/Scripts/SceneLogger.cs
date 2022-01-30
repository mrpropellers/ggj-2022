using UnityEngine;
using UnityEngine.SceneManagement;

namespace GGJ
{
    public static class SceneLogger
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Init()
        {
            SceneManager.sceneUnloaded += (scene) => {
                Debug.LogFormat("SceneManager: Unloaded scene {0}", scene.name);
            };
            SceneManager.sceneLoaded += (scene, loadSceneMode) => {
                Debug.LogFormat("SceneManager: Loaded scene {0} loadSceneMode={1}", scene.name, loadSceneMode);
            };
        }
    }
}
