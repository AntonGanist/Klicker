using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using YG;

namespace SceneManagement
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private GameObject _loadingScreen;
        public void Initialize()
        {

        }
        public void LoadMetaScene(SceneEnterParams enterParams = null)
        {
            EventBus.Raise(new Unsubscribe());
            YG2.InterstitialAdvShow();
            StartCoroutine(LoadAndStartMeta(enterParams));
        }

        public void LoadGameplayScene(SceneEnterParams enterParams = null)
        {
            EventBus.Raise(new Unsubscribe());
            StartCoroutine(LoadAndStartGameplay(enterParams));
        }
        public void LoadInfinityScene(SceneEnterParams enterParams = null)
        {
            EventBus.Raise(new Unsubscribe());
            StartCoroutine(LoadAndStartInfinity(enterParams));
        }
        private IEnumerator LoadAndStartMeta(SceneEnterParams enterParams = null)
        {
            _loadingScreen.SetActive(true);

            yield return LoadScene(Scenes.Loader);
            yield return LoadScene(Scenes.MetaScene);

            var sceneEntryPoint = FindFirstObjectByType<EntryPoint>();
            sceneEntryPoint.Run(enterParams);

            _loadingScreen.SetActive(false);
        }

        private IEnumerator LoadAndStartGameplay(SceneEnterParams enterParams)
        {
            _loadingScreen.SetActive(true);

            yield return LoadScene(Scenes.Loader);
            yield return LoadScene(Scenes.LevelScene);

            var sceneEntryPoint = FindFirstObjectByType<EntryPoint>();
            sceneEntryPoint.Run(enterParams);

            _loadingScreen.SetActive(false);
        }

        private IEnumerator LoadAndStartInfinity(SceneEnterParams enterParams)
        {
            _loadingScreen.SetActive(true);

            yield return LoadScene(Scenes.Loader);
            yield return LoadScene(Scenes.InfinityScene);

            var sceneEntryPoint = FindFirstObjectByType<EntryPoint>();
            sceneEntryPoint.Run(enterParams);

            _loadingScreen.SetActive(false);
        }

        private IEnumerator LoadScene(string sceneName)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
        }
    }
}