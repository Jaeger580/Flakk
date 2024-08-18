using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using GeneralUtility.SaveLoadSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace GeneralUtility
{
    namespace UI
    {
        public class MainMenuHandler : MonoBehaviour
        {
            private Label labelTitle;
            private Button buttonNewGame, buttonLoadGame, buttonContinueGame, buttonQuit;
            private EasingFunction linear = EasingMode.Linear;
            private float pulseSeconds = 3f;
            private List<TimeValue> pulseTime = new();
            private List<EasingFunction> easingFunctions = new();

            [SerializeField] private AudioClip sfxHover, sfxClick;
            [SerializeField] private AudioSource sfxSource;

            //Save/Load stuff
            private List<VisualElement> saveSlotElements = new();
            private VisualElement loadScreen, fileOne, fileTwo, fileThree;
            private Button buttonLoadBack;

            //Delete stuff
            private VisualElement deletePrompt;
            private Button buttonDeleteConfirm, buttonDeleteCancel;

            [SerializeField] private SaveSlot[] saveSlots;
            private bool isLoadingGame;

            private IEnumerator Start()
            {//Called after Awake to ensure the HUD is loaded ahead of time
                pulseTime.Add(new TimeValue(pulseSeconds, TimeUnit.Second));
                easingFunctions.Add(linear);

                var root = GetComponent<UIDocument>().rootVisualElement;    //Get the root object of the HUD
                buttonNewGame = root.Q<Button>("NewGame");                  //Find and set the UI references
                buttonQuit = root.Q<Button>("Quit");
                labelTitle = root.Q<Label>("Title");
                //buttonNewGame.clicked += StartNewGame;  //Add a function to the list of functions triggered on button click
                buttonNewGame.clicked += delegate { isLoadingGame = false; RedrawSaveSlots(); StartCoroutine(UI_Utility.C_FadeIn(loadScreen, true)); };
                buttonQuit.clicked += QuitGame;

                //Delete prompt stuff
                deletePrompt = root.Q<VisualElement>("DeletePrompt");
                deletePrompt.style.display = UI_Utility.hidden;
                buttonDeleteConfirm = deletePrompt.Q<Button>("Confirm");
                buttonDeleteCancel = deletePrompt.Q<Button>("Cancel");
                buttonDeleteCancel.clicked += delegate { ToggleDeletePrompt(null, false); };

                //Save/Load stuff
                loadScreen = root.Q<VisualElement>("LoadScreen");
                loadScreen.style.display = UI_Utility.hidden;
                buttonLoadGame = root.Q<Button>("LoadGame");
                buttonLoadGame.style.display = UI_Utility.hidden;
                buttonContinueGame = root.Q<Button>("ContinueGame");
                buttonLoadBack = loadScreen.Q<Button>("Back");
                buttonLoadBack.clicked += delegate { ToggleDeletePrompt(null, false); StartCoroutine(UI_Utility.C_FadeOut(loadScreen, true)); };

                if (DataManager.instance.HasGameData())
                {
                    //buttonContinueGame.clicked += ContinueGame;
                    buttonContinueGame.clicked += delegate { isLoadingGame = true; RedrawSaveSlots(); ContinueGame(); };
                    //buttonLoadGame.clicked += delegate { isLoadingGame = true; RedrawSaveSlots(); StartCoroutine(UI_Utility.C_FadeIn(loadScreen, true)); };
                }
                else
                {
                    //buttonContinueGame.clickable = null;
                    buttonLoadGame.SetEnabled(false);
                    buttonLoadGame.style.display = UI_Utility.hidden;
                }

                fileOne = loadScreen.Q<VisualElement>("FileOne");
                fileTwo = loadScreen.Q<VisualElement>("FileTwo");
                fileThree = loadScreen.Q<VisualElement>("FileThree");
                saveSlotElements.Add(fileOne);
                saveSlotElements.Add(fileTwo);
                saveSlotElements.Add(fileThree);

                RedrawSaveSlots();

                root.Query<Button>().ForEach(AddSFX);

                //PURELY DECORARTION
                yield return new WaitForSeconds(0.1f);
                labelTitle.RemoveFromClassList("title-start");  //Remove the title-start class from each of these elements, which will animate it

                yield return new WaitForSeconds(5f);
                labelTitle.style.transitionTimingFunction = new StyleList<EasingFunction>(easingFunctions); //Add transition stuff to each Title label
                labelTitle.style.transitionDuration = new StyleList<TimeValue>(pulseTime);

                StartCoroutine(TitlePulseToggle()); //Trigger the effect toggle
            }

            private void RedrawSaveSlots()
            {
                var saves = DataManager.instance.GetAllProfilesGameData();
                for (int i = 0; i < saveSlots.Length; i++)
                {
                    SaveSlot slot = saveSlots[i];
                    saves.TryGetValue(slot.GetProfileId(), out GameData profileData);
                    ShowSaveFile(saveSlotElements[i], profileData, slot.GetProfileId());
                }

                bool allEmpty = true;
                foreach (var slot in saveSlotElements)
                {//Cheaty but whatever
                    var empty = slot.Q<Button>("EMPTY");
                    if (empty.style.display == UI_Utility.hidden)
                    {
                        allEmpty = false;
                    }
                }

                if (allEmpty)
                {
                    buttonContinueGame.SetEnabled(false);
                    buttonContinueGame.style.display = UI_Utility.hidden;
                    buttonLoadGame.SetEnabled(false);
                }
            }

            private IEnumerator TitlePulseToggle()
            {//Add or remove the title-pulse class from each title label
                labelTitle.ToggleInClassList("title-pulse");
                yield return new WaitForSeconds(pulseSeconds);
                StartCoroutine(TitlePulseToggle());
            }

            private void StartNewGame()
            {
                DataManager.instance.NewGame();
                //SceneLoader.instance.LoadScene(MagicStrings.SCENE_TUTORIAL);
            }

            public void ShowSaveFile(VisualElement slot, GameData data, string profileID)
            {
                var empty = slot.Q<Button>("EMPTY");
                var slotInfo = slot.Q<VisualElement>("SlotInfo");
                var fileName = slot.Q<Label>("FileName");
                var filePercent = slot.Q<Label>("FilePercent");
                var btnLoad = slot.Q<Button>("Load");
                var btnDelete = slot.Q<Button>("Delete");
                var btnCopy = slot.Q<Button>("Copy");

                empty.clickable = null;
                btnLoad.clickable = null;
                btnDelete.clickable = null;
                btnCopy.clickable = null;

                if (data == null)
                {
                    empty.style.display = UI_Utility.showing;
                    slotInfo.style.display = UI_Utility.hidden;
                    empty.SetEnabled(false);
                    if (!isLoadingGame)
                    {
                        empty.clicked += delegate { OnSaveSlotClick(profileID); };
                        empty.SetEnabled(true);
                    }
                    return;
                }

                empty.style.display = UI_Utility.hidden;
                slotInfo.style.display = UI_Utility.showing;

                fileName.text = $"{profileID}";
                filePercent.text = $"{data.GetPercentageComplete()}%";

                btnLoad.clicked += delegate { isLoadingGame = true; OnSaveSlotClick(profileID); };
                btnDelete.clicked += delegate { ToggleDeletePrompt(profileID, true); };
            }

            private void OnSaveSlotClick(string profileId)
            {
                DataManager.instance.ChangeSelectedProfileId(profileId);
                if (!isLoadingGame)
                {
                    StartNewGame();
                }
                else
                {
                    ContinueGame();
                }
            }

            private void ToggleDeletePrompt(string profileId, bool show)
            {
                if (show && profileId != null)
                {
                    buttonDeleteConfirm.clicked += delegate { OnDeleteClick(profileId); ToggleDeletePrompt(profileId, false); };
                    deletePrompt.style.display = UI_Utility.showing;
                }
                else
                {
                    buttonDeleteConfirm.clickable = null;
                    deletePrompt.style.display = UI_Utility.hidden;
                }
            }

            private void OnDeleteClick(string profileId)
            {
                DataManager.instance.DeleteProfileData(profileId);
                RedrawSaveSlots();
            }

            private void ContinueGame()
            {
                //SceneManager.LoadSceneAsync(1);
                //DataManager.instance.SaveGame();
                //SceneLoader.instance.LoadScene(MagicStrings.SCENE_LEVELSELECT);
            }

            private void QuitGame()
            {
                Application.Quit();
#if UNITY_EDITOR
                EditorApplication.ExitPlaymode();
#endif
            }

            private void AddSFX(Button btn)
            {//RegisterCallback example. The signature of the method being called must match the callback's return value
                btn.RegisterCallback<MouseOverEvent, AudioClip>(TriggerSFX, sfxHover);
                btn.clicked += delegate { TriggerSFX(null, sfxClick); };
            }

            private void TriggerSFX(EventBase evt, AudioClip clip)
            {
                sfxSource.PlayOneShot(clip);
            }
        }
    }
}