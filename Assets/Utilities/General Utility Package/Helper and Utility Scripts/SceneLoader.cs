using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GeneralUtility.GameEventSystem;
using GeneralUtility.SaveLoadSystem;

namespace GeneralUtility
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private GameObject loadingScreen;
        [SerializeField] private Slider loadingBar;
        [SerializeField] private GameObject loadingText;
        [SerializeField] private GameObject pressAnyKeyText;
        public static SceneLoader instance { private set; get; } // so we are saying here that any script can access this but there can only be 1 sceneloader that can get accessed 

        [SerializeField] private RenderTexture hudTexture;

        [SerializeField] private GameEvent loadingScreenExitEvent;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void LoadScene(int levelIndex)
        {
            loadingScreen.SetActive(true);
            StartCoroutine(LoadSceneAsynchronously(levelIndex));
        }

        public void LoadScene(string levelName)
        {
            loadingScreen.SetActive(true);
            StartCoroutine(LoadSceneAsynchronously(levelName));
        }

        private IEnumerator LoadSceneAsynchronously(int levelIndex)
        {
            Time.timeScale = 0f;
            if (DataManager.instance != null)
                DataManager.instance.SaveGame();
            AsyncOperation operation = SceneManager.LoadSceneAsync(levelIndex);

            loadingBar.gameObject.SetActive(true);
            loadingText.SetActive(true);
            pressAnyKeyText.SetActive(false);

            while (!operation.isDone)
            {
                loadingBar.value = operation.progress;
                yield return null;
            }

            hudTexture.Release();

            loadingBar.gameObject.SetActive(false);
            loadingText.SetActive(false);
            pressAnyKeyText.SetActive(true);

            yield return new WaitUntil(CheckForKey);
            //yield return new WaitForSecondsRealtime(2f); //USE FOR TESTING PURPOSES
            pressAnyKeyText.SetActive(false);
            loadingScreen.SetActive(false);

            loadingScreenExitEvent.Trigger();
            Time.timeScale = 1f;
        }

        private IEnumerator LoadSceneAsynchronously(string levelName)
        {
            Time.timeScale = 0f;
            if (DataManager.instance != null)
                DataManager.instance.SaveGame();
            AsyncOperation operation = SceneManager.LoadSceneAsync(levelName);

            loadingBar.gameObject.SetActive(true);
            loadingText.SetActive(true);
            pressAnyKeyText.SetActive(false);

            while (!operation.isDone)
            {
                loadingBar.value = operation.progress;
                yield return null;
            }

            hudTexture.Release();

            loadingBar.gameObject.SetActive(false);
            loadingText.SetActive(false);
            pressAnyKeyText.SetActive(true);

            yield return new WaitUntil(CheckForKey);
            //yield return new WaitForSecondsRealtime(2f); USE FOR TESTING PURPOSES
            pressAnyKeyText.SetActive(false);
            loadingScreen.SetActive(false);
            loadingScreenExitEvent.Trigger();
            Time.timeScale = 1f;
        }

        private bool CheckForKey()
        {
            return Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape);
        }
    }
}