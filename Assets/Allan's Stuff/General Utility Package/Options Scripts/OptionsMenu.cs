using GeneralUtility.GameEventSystem;
using UnityEngine;
using UnityEngine.UIElements;

/*******************
Scope: OptionsMenu handles the options menu navigation UI, which includes:
    - Toggling the option menu
    - Switching between each option screen based on which button (tab) you click
        - ex.: Clicking the "Audio" tab should bring up all the audio options while hiding all other options
    - Applying, Reverting, and Exiting the options menu (which each has a related GameEvent to implement the Observer pattern)
********************/

namespace GeneralUtility
{
    namespace Options
    {
        public class OptionsMenu : MonoBehaviour
        {
            public GameEvent applyOptionsEvent, revertOptionsEvent, exitOptionsEvent;
            public GameEvent defaultVisualEvent, defaultSensEvent, defaultAudioEvent, defaultAllEvent;

            private Button buttonOptions;

            private VisualElement screenOptions;

            private Button buttonAudioTab, buttonVisualTab, buttonKeysTab, buttonAccessTab, buttonGameTab;
            private Button buttonOptionsApply, buttonOptionsRevert, buttonOptionsDefault, buttonOptionsExit;

            private VisualElement optionsAudio, optionsVisual, optionsKeys, optionsAccess, optionsGame;

            private string visibleOptionsTab;

            private DisplayStyle showing = DisplayStyle.Flex, hidden = DisplayStyle.None;

            private void Start()
            {
                var root = FindObjectOfType<UIDocument>().rootVisualElement;                //Get the root of the UI
                buttonOptions = root.Q<Button>("Options");                              //Find the "options" button
                buttonOptions.clicked += ToggleOptionsScreen;

                screenOptions = root.Q<VisualElement>("OptionsScreen");
                buttonAudioTab = screenOptions.Q<Button>("Audio");                      //Assign a reference to each tab button
                buttonVisualTab = screenOptions.Q<Button>("Visual");
                buttonKeysTab = screenOptions.Q<Button>("Controls");
                buttonAccessTab = screenOptions.Q<Button>("Accessibility");
                buttonGameTab = screenOptions.Q<Button>("Game");

                optionsAudio = screenOptions.Q<VisualElement>("AudioOptions");          //Assign a reference to each related screen
                optionsVisual = screenOptions.Q<VisualElement>("VisualOptions");
                optionsKeys = screenOptions.Q<VisualElement>("ControlsOptions");
                optionsAccess = screenOptions.Q<VisualElement>("AccessibilityOptions");
                optionsGame = screenOptions.Q<VisualElement>("GameOptions");

                optionsAudio.style.display = showing;
                optionsVisual.style.display = hidden;
                optionsKeys.style.display = hidden;
                optionsAccess.style.display = hidden;
                optionsGame.style.display = hidden;

                buttonOptionsExit = screenOptions.Q<Button>("Exit");
                buttonOptionsApply = screenOptions.Q<Button>("Apply");
                buttonOptionsRevert = screenOptions.Q<Button>("Revert");
                buttonOptionsDefault = screenOptions.Q<Button>("Default");

                buttonAudioTab.clicked += delegate { SelectOptions(optionsAudio); };    //Assign the SelectOptions function to each tab
                buttonVisualTab.clicked += delegate { SelectOptions(optionsVisual); };
                buttonKeysTab.clicked += delegate { SelectOptions(optionsKeys); };
                buttonAccessTab.clicked += delegate { SelectOptions(optionsAccess); };
                buttonGameTab.clicked += delegate { SelectOptions(optionsGame); };

                buttonOptionsExit.clicked += ToggleOptionsScreen;
                buttonOptionsExit.clicked += ExitOptions;
                buttonOptionsApply.clicked += ApplyOptions;
                buttonOptionsRevert.clicked += RevertOptions;
                buttonOptionsDefault.clicked += DefaultOptions;
                screenOptions.style.display = hidden;

                visibleOptionsTab = optionsAudio.name;
            }

            /*** UI-Updating Functions ***/
            private void SelectOptions(VisualElement opt)
            {//Toggle the related options screen on, and all the other ones off
                optionsAudio.style.display = hidden;
                optionsVisual.style.display = hidden;
                optionsKeys.style.display = hidden;
                optionsAccess.style.display = hidden;
                optionsGame.style.display = hidden;
                //Maybe switch the switch to a dictionary TryGet?
                opt.style.display = showing;
                visibleOptionsTab = opt.name;
            }
            private void ToggleOptionsScreen()
            {
                if (screenOptions.style.display != hidden)
                    screenOptions.style.display = hidden;
                else
                    screenOptions.style.display = showing;
            }

            /*** Event-Sending Functions ***/
            private void ApplyOptions() { applyOptionsEvent.Trigger(); }
            private void RevertOptions() { revertOptionsEvent.Trigger(); }
            public void ExitOptions() { exitOptionsEvent.Trigger(); }

            private void DefaultOptions()
            {
                switch (visibleOptionsTab)
                {
                    case "AudioOptions":
                        defaultAudioEvent.Trigger();
                        break;
                    case "VisualOptions":
                        defaultVisualEvent.Trigger();
                        break;
                    case "ControlsOptions":
                        break;
                    case "AccessibilityOptions":
                        break;
                    case "GameOptions":
                        defaultSensEvent.Trigger();
                        break;
                    default:
                        break;
                }
            }
        }
    }
}