using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace GeneralUtility
{
    namespace SaveLoadSystem
    {
        public class DataManager : MonoBehaviour
        {
            [Header("Debugging")]
            [SerializeField] private bool disableDataPersistence = false;
            [SerializeField] private bool initializeDataIfNull = false;
            [SerializeField] private bool overrideSelectedProfileId = false;
            [SerializeField] private string testSelectedProfileId = "test";

            [Header("File Storage Config")]
            [SerializeField] private string fileName;
            [SerializeField] private bool useEncryption;

            private GameData gameData;
            public GameData GameData => gameData;
            private List<IData> dataObj = new();
            private FileDataHandler dataHandler;
            private string selectedProfileId = "";
            public static DataManager instance { get; private set; }

            public bool startingNewFile = false;

            private void Awake()
            {
                if (instance != null)
                {
                    Debug.Log("Found more than one Data Manager in the scene. Destroying the newest one.");
                    Destroy(gameObject);
                    return;
                }
                instance = this;
                DontDestroyOnLoad(gameObject);

                if (disableDataPersistence)
                {
                    Debug.LogWarning("Data persistence is disabled!");
                }

                dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, useEncryption);

                InitializeSelectedProfileId();
            }

            private void OnEnable()
            {
                SceneManager.sceneLoaded += OnSceneLoaded;
            }

            private void OnDisable()
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
            }

            public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
            {
                //if (scene.name == defaultSceneName or something?) return;

                dataObj = FindAllDataObjects();

                LoadGame();
            }

            public void ChangeSelectedProfileId(string newProfileId)
            {
                //update the profile to use for saving and loading
                selectedProfileId = newProfileId;
                // load the game, which will use that profile, updating our game data accordingly
                print($"Switching selected profile: {newProfileId}");
                LoadGame();
            }

            public void NewGame()
            {
                startingNewFile = true;
                gameData = new GameData();
                dataObj = FindAllDataObjects();
            }

            public void LoadGame()
            {
                if (disableDataPersistence) return;

                //Load any saved data from a file using the data handler
                gameData = dataHandler.Load(selectedProfileId);

                //start a new game if the data is null and we're configured to intialize data for debugging purposes
                if (gameData == null && initializeDataIfNull)
                {
                    print("Starting new game.");
                    NewGame();
                }

                //if no data can be loaded, don't continue.
                if (gameData == null)
                {
                    Debug.Log("No data was found. A New Game needs to be started before data can be loaded.");
                    return;
                }
                //push the loaded data to all other scripts that need it
                foreach (IData dataObjects in dataObj)
                {
                    dataObjects.LoadData(gameData);
                }
            }

            public void SaveGame()
            {
                StartCoroutine(nameof(C_SaveGame));
            }

            public IEnumerator C_SaveGame()
            {
                if (disableDataPersistence) yield break;

                print("Trying to save game");

                if (gameData == null)
                {
                    Debug.LogWarning("No data was found. A New Game needs to be started before data can be saved.");
                    yield break;
                }
                //pass the data to other scripts so they can update it
                int i;
                for (i = 0; i < dataObj.Count; i++)
                {
                    IData dataObjects = dataObj[i];
                    dataObjects.SaveData(gameData);
                }

                gameData.lastUpdated = System.DateTime.Now.ToBinary();

                //save that data to a file using the data handler
                yield return new WaitUntil(() => SavedAllDataObjects(i));
                print("Now I saved everything!");
                dataHandler.Save(gameData, selectedProfileId);
            }

            public bool SavedAllDataObjects(int index)
            {
                return index >= dataObj.Count;
            }

            public void DeleteProfileData(string profileId)
            {
                dataHandler.Delete(profileId);
                InitializeSelectedProfileId();
                print($"Deleting profile: {profileId}");
                LoadGame();
            }

            private void InitializeSelectedProfileId()
            {
                selectedProfileId = dataHandler.GetMostRecentlyUpdatedProfileId();
                if (overrideSelectedProfileId)
                {
                    selectedProfileId = testSelectedProfileId;
                    Debug.LogWarning($"Overrode selected profile id with a test id: {testSelectedProfileId}");
                }
            }

            private void OnApplicationQuit()
            {
                SaveGame();
            }

            private List<IData> FindAllDataObjects()
            {
                IEnumerable<IData> dataObj = FindObjectsOfType<MonoBehaviour>().OfType<IData>();
                return new List<IData>(dataObj);
            }

            public bool HasGameData()
            {
                //Debug.Log("gameData");
                return gameData != null;
            }

            public Dictionary<string, GameData> GetAllProfilesGameData()
            {
                return dataHandler.LoadAllProfiles();
            }
        }
    }
}