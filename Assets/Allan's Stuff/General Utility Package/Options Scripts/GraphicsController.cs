using System.Collections.Generic;
using GeneralUtility.GameEventSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace GeneralUtility
{
    namespace Options
    {
        public class GraphicsController : MonoBehaviour
        {
            private Toggle toggleVSync, toggleFullScreen;
            private DropdownField resolutionDropdown;
            public GameEvent applyOptionsEvent, revertOptionsEvent, defaultOptionsEvent, exitOptionsEvent;
            private bool vsync, fullScreen;
            private int resolutionIndex, defaultResIndex;
            private List<Resolution> resolutions = new();
            private List<string> resolutionNames = new();
            private (int width, int height) nativeResolution;

            private void Start()
            {
                nativeResolution.width = Display.main.systemWidth;
                nativeResolution.height = Display.main.systemHeight;

                var root = FindObjectOfType<UIDocument>().rootVisualElement;

                toggleVSync = root.Q<Toggle>("VSyncToggle");
                toggleVSync.value = PlayerPrefs.GetInt(MagicStrings.OPTIONS_VSYNC, 0) == 1;
                QualitySettings.vSyncCount = toggleVSync.value ? 1 : 0;
                vsync = toggleVSync.value;
                toggleVSync.RegisterValueChangedCallback((evt) => TempVSync(evt));     //Tell the system "when slider changes, call "SetMasterVolume"

                toggleFullScreen = root.Q<Toggle>("FullScreenToggle");
                toggleFullScreen.value = PlayerPrefs.GetInt(MagicStrings.OPTIONS_FULLSCREEN, 0) == 1;
                Screen.fullScreenMode = toggleFullScreen.value ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
                fullScreen = toggleFullScreen.value;
                toggleFullScreen.RegisterValueChangedCallback((evt) => TempFullScreen(evt));     //Tell the system "when slider changes, call "SetMasterVolume"

                resolutionDropdown = root.Q<DropdownField>("ResolutionDropdown"); //the name of the element in UI Builder
                for (int i = Screen.resolutions.Length - 1; i >= 0; i--)
                {
                    resolutions.Add(Screen.resolutions[i]);
                }
                resolutionDropdown.choices.Clear();
                for (int i = 0; i < resolutions.Count; i++)
                {
                    var resolutionName = $"{resolutions[i].width}x{resolutions[i].height} {Mathf.Round(((float)resolutions[i].refreshRateRatio.value) * 100f) / 100f}Hz";
                    resolutionDropdown.choices.Add(resolutionName);
                    resolutionNames.Add(resolutionName);
                    if (resolutions[i].width == nativeResolution.width && resolutions[i].height == nativeResolution.height)
                    {
                        defaultResIndex = i + 1;
                    }
                    else if (resolutions[i].width == nativeResolution.width && defaultResIndex == 0)
                    {
                        defaultResIndex = i;
                    }
                    else if (resolutions[i].height == nativeResolution.height && defaultResIndex == 0)
                    {
                        defaultResIndex = i;
                    }
                }
                resolutionIndex = PlayerPrefs.GetInt(MagicStrings.OPTIONS_RESOLUTION_INDEX, defaultResIndex);
                resolutionDropdown.value = resolutionNames[PlayerPrefs.GetInt(MagicStrings.OPTIONS_RESOLUTION_INDEX, defaultResIndex)];
                Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, toggleFullScreen.value);
                resolutionDropdown.RegisterValueChangedCallback((evt) => TempResolution(evt));

                var applyListener = gameObject.AddComponent<GameEventListener>();
                applyListener.Events.Add(applyOptionsEvent);
                applyListener.Response = new();
                applyListener.Response.AddListener(() => SetVSync());
                applyListener.Response.AddListener(() => SetFullScreen());
                //applyListener.Response.AddListener(() => SetResolution());
                applyOptionsEvent.RegisterListener(applyListener);

                var revertListener = gameObject.AddComponent<GameEventListener>();
                revertListener.Events.Add(revertOptionsEvent);
                revertListener.Response = new();
                revertListener.Response.AddListener(() => RevertVSync());
                revertListener.Response.AddListener(() => RevertFullScreen());
                revertListener.Response.AddListener(() => RevertResolution());
                revertOptionsEvent.RegisterListener(revertListener);

                var defaultListener = gameObject.AddComponent<GameEventListener>();
                defaultListener.Events.Add(defaultOptionsEvent);
                defaultListener.Response = new();
                defaultListener.Response.AddListener(() => DefaultVSync());
                defaultListener.Response.AddListener(() => DefaultFullScreen());
                defaultListener.Response.AddListener(() => DefaultResolution());
                defaultOptionsEvent.RegisterListener(defaultListener);

                var exitListener = gameObject.AddComponent<GameEventListener>();
                exitListener.Events.Add(exitOptionsEvent);
                exitListener.Response = new();
                exitListener.Response.AddListener(() => RevertVSync());
                exitListener.Response.AddListener(() => RevertFullScreen());
                exitListener.Response.AddListener(() => RevertResolution());
                exitOptionsEvent.RegisterListener(exitListener);
            }

            #region VSync Stuff
            private void TempVSync(ChangeEvent<bool> evt)
            {
                vsync = evt.newValue;
                SetVSync();
            }

            private void SetVSync()
            {
                PlayerPrefs.SetInt(MagicStrings.OPTIONS_VSYNC, vsync ? 1 : 0);
                QualitySettings.vSyncCount = PlayerPrefs.GetInt(MagicStrings.OPTIONS_VSYNC, 0);
            }
            private void RevertVSync()
            {
                toggleVSync.value = PlayerPrefs.GetInt(MagicStrings.OPTIONS_VSYNC, 0) == 1;
                vsync = PlayerPrefs.GetInt(MagicStrings.OPTIONS_VSYNC, 0) == 1;
                QualitySettings.vSyncCount = PlayerPrefs.GetInt(MagicStrings.OPTIONS_VSYNC, 0);
            }
            private void DefaultVSync()
            {
                PlayerPrefs.SetInt(MagicStrings.OPTIONS_VSYNC, 0);
                toggleVSync.value = PlayerPrefs.GetInt(MagicStrings.OPTIONS_VSYNC, 0) == 1;
                QualitySettings.vSyncCount = PlayerPrefs.GetInt(MagicStrings.OPTIONS_VSYNC, 0);
            }
            #endregion

            #region Full-Screen Stuff
            private void TempFullScreen(ChangeEvent<bool> evt)
            {
                fullScreen = evt.newValue;
                SetFullScreen();
            }

            private void SetFullScreen()
            {
                PlayerPrefs.SetInt(MagicStrings.OPTIONS_FULLSCREEN, fullScreen ? 1 : 0);
                Screen.fullScreen = toggleFullScreen.value;
            }
            private void RevertFullScreen()
            {
                toggleFullScreen.value = PlayerPrefs.GetInt(MagicStrings.OPTIONS_FULLSCREEN, 0) == 1;
                fullScreen = PlayerPrefs.GetInt(MagicStrings.OPTIONS_FULLSCREEN, 0) == 1;
                Screen.fullScreen = PlayerPrefs.GetInt(MagicStrings.OPTIONS_FULLSCREEN, 0) == 1;
            }
            private void DefaultFullScreen()
            {
                PlayerPrefs.SetInt(MagicStrings.OPTIONS_FULLSCREEN, 0);
                toggleFullScreen.value = PlayerPrefs.GetInt(MagicStrings.OPTIONS_FULLSCREEN, 0) == 1;
                Screen.fullScreen = PlayerPrefs.GetInt(MagicStrings.OPTIONS_FULLSCREEN, 0) == 1;
            }
            #endregion

            #region Resolution Stuff
            private void TempResolution(ChangeEvent<string> evt)
            {
                resolutionIndex = resolutionNames.IndexOf(evt.newValue);
                SetResolution(resolutionIndex);
            }

            private void SetResolution(int resolutionIndex)
            {
                PlayerPrefs.SetInt(MagicStrings.OPTIONS_RESOLUTION_INDEX, resolutionIndex);
                //Display.main.SetRenderingResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height);
                Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, toggleFullScreen.value);
            }
            private void RevertResolution()
            {
                //resolutionDropdown.index = PlayerPrefs.GetInt(MagicStrings.OPTIONS_RESOLUTION_INDEX, defaultResIndex);
                resolutionDropdown.value = resolutionNames[PlayerPrefs.GetInt(MagicStrings.OPTIONS_RESOLUTION_INDEX, defaultResIndex)];
                resolutionIndex = PlayerPrefs.GetInt(MagicStrings.OPTIONS_RESOLUTION_INDEX, defaultResIndex);
                Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, toggleFullScreen.value);
                print($"Reverting to: {resolutions[resolutionIndex].width}x{resolutions[resolutionIndex].height}");
            }
            private void DefaultResolution()
            {
                PlayerPrefs.SetInt(MagicStrings.OPTIONS_RESOLUTION_INDEX, defaultResIndex);
                resolutionIndex = PlayerPrefs.GetInt(MagicStrings.OPTIONS_RESOLUTION_INDEX, defaultResIndex);
                print($"Defaulting to: {resolutions[resolutionIndex].width}x{resolutions[resolutionIndex].height}");
                resolutionDropdown.index = PlayerPrefs.GetInt(MagicStrings.OPTIONS_RESOLUTION_INDEX, defaultResIndex);
                resolutionDropdown.value = resolutionNames[PlayerPrefs.GetInt(MagicStrings.OPTIONS_RESOLUTION_INDEX, defaultResIndex)];
                Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, toggleFullScreen.value);
            }
            #endregion
        }
    }
}