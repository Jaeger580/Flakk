using System.Collections.Generic;
using Cinemachine;
using GeneralUtility.GameEventSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace GeneralUtility
{
    namespace Options
    {
        public class GraphicsController : MonoBehaviour
        {
            protected Toggle toggleVSync, toggleFullScreen;
            protected DropdownField resolutionDropdown;
            public GameEvent applyOptionsEvent, revertOptionsEvent, defaultOptionsEvent, exitOptionsEvent;
            protected bool vsync, fullScreen;
            protected int resolutionIndex, defaultResIndex;
            protected List<Resolution> resolutions = new();
            protected List<string> resolutionNames = new();
            protected (int width, int height) nativeResolution;

            // Jakob's FOV and FPS additions
            protected Slider FOVSlider;
            protected float FOVValue;
            [SerializeField]
            protected CinemachineVirtualCamera hubCam;

            protected void Start()
            {
                nativeResolution.width = Display.main.systemWidth;
                nativeResolution.height = Display.main.systemHeight;

                InitUI();

                QualitySettings.vSyncCount = toggleVSync.value ? 1 : 0;
                Screen.fullScreenMode = toggleFullScreen.value ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
                Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, toggleFullScreen.value);

                var applyListener = gameObject.AddComponent<GameEventListener>();
                applyListener.Events.Add(applyOptionsEvent);
                applyListener.Response = new();
                applyListener.Response.AddListener(() => SetVSync());
                applyListener.Response.AddListener(() => SetFullScreen());
                applyListener.Response.AddListener(() => SetFOV());
                //applyListener.Response.AddListener(() => SetResolution());
                applyOptionsEvent.RegisterListener(applyListener);

                var revertListener = gameObject.AddComponent<GameEventListener>();
                revertListener.Events.Add(revertOptionsEvent);
                revertListener.Response = new();
                revertListener.Response.AddListener(() => RevertVSync());
                revertListener.Response.AddListener(() => RevertFullScreen());
                revertListener.Response.AddListener(() => RevertResolution());
                revertListener.Response.AddListener(() => RevertFOV());
                revertOptionsEvent.RegisterListener(revertListener);

                var defaultListener = gameObject.AddComponent<GameEventListener>();
                defaultListener.Events.Add(defaultOptionsEvent);
                defaultListener.Response = new();
                defaultListener.Response.AddListener(() => DefaultVSync());
                defaultListener.Response.AddListener(() => DefaultFullScreen());
                defaultListener.Response.AddListener(() => DefaultResolution());
                defaultListener.Response.AddListener(() => DefaultFOV());
                defaultOptionsEvent.RegisterListener(defaultListener);

                var exitListener = gameObject.AddComponent<GameEventListener>();
                exitListener.Events.Add(exitOptionsEvent);
                exitListener.Response = new();
                exitListener.Response.AddListener(() => RevertVSync());
                exitListener.Response.AddListener(() => RevertFullScreen());
                exitListener.Response.AddListener(() => RevertResolution());
                exitListener.Response.AddListener(() => RevertFOV());
                exitOptionsEvent.RegisterListener(exitListener);
            }

            protected void InitUI()
            {
                var root = GetComponent<UIDocument>().rootVisualElement;

                toggleVSync = root.Q<Toggle>("VSyncToggle");
                toggleVSync.value = PlayerPrefs.GetInt(MagicStrings.OPTIONS_VSYNC, 0) == 1;
                vsync = toggleVSync.value;
                toggleVSync.RegisterValueChangedCallback((evt) => TempVSync(evt));     //Tell the system "when slider changes, call "SetMasterVolume"

                toggleFullScreen = root.Q<Toggle>("FullScreenToggle");
                toggleFullScreen.value = PlayerPrefs.GetInt(MagicStrings.OPTIONS_FULLSCREEN, 0) == 1;
                fullScreen = toggleFullScreen.value;
                toggleFullScreen.RegisterValueChangedCallback((evt) => TempFullScreen(evt));     //Tell the system "when slider changes, call "SetMasterVolume"

                resolutionDropdown = root.Q<DropdownField>("ResolutionDropdown"); //the name of the element in UI Builder

                FOVSlider = root.Q<Slider>("FieldOfView");
                FOVSlider.value = PlayerPrefs.GetFloat(FOVSlider.name, 90);
                FOVValue = FOVSlider.value;
                FOVSlider.RegisterValueChangedCallback((evt) => TempFOV(evt));


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
                resolutionDropdown.RegisterValueChangedCallback((evt) => TempResolution(evt));
            }

            #region VSync Stuff
            protected void TempVSync(ChangeEvent<bool> evt)
            {
                vsync = evt.newValue;
                SetVSync();
            }

            protected void SetVSync()
            {
                PlayerPrefs.SetInt(MagicStrings.OPTIONS_VSYNC, vsync ? 1 : 0);
                QualitySettings.vSyncCount = PlayerPrefs.GetInt(MagicStrings.OPTIONS_VSYNC, 0);
            }
            protected void RevertVSync()
            {
                toggleVSync.value = PlayerPrefs.GetInt(MagicStrings.OPTIONS_VSYNC, 0) == 1;
                vsync = PlayerPrefs.GetInt(MagicStrings.OPTIONS_VSYNC, 0) == 1;
                QualitySettings.vSyncCount = PlayerPrefs.GetInt(MagicStrings.OPTIONS_VSYNC, 0);
            }
            protected void DefaultVSync()
            {
                PlayerPrefs.SetInt(MagicStrings.OPTIONS_VSYNC, 0);
                toggleVSync.value = PlayerPrefs.GetInt(MagicStrings.OPTIONS_VSYNC, 0) == 1;
                QualitySettings.vSyncCount = PlayerPrefs.GetInt(MagicStrings.OPTIONS_VSYNC, 0);
            }
            #endregion

            #region Full-Screen Stuff
            protected void TempFullScreen(ChangeEvent<bool> evt)
            {
                fullScreen = evt.newValue;
                SetFullScreen();
            }

            protected void SetFullScreen()
            {
                PlayerPrefs.SetInt(MagicStrings.OPTIONS_FULLSCREEN, fullScreen ? 1 : 0);
                Screen.fullScreen = toggleFullScreen.value;
            }
            protected void RevertFullScreen()
            {
                toggleFullScreen.value = PlayerPrefs.GetInt(MagicStrings.OPTIONS_FULLSCREEN, 0) == 1;
                fullScreen = PlayerPrefs.GetInt(MagicStrings.OPTIONS_FULLSCREEN, 0) == 1;
                Screen.fullScreen = PlayerPrefs.GetInt(MagicStrings.OPTIONS_FULLSCREEN, 0) == 1;
            }
            protected void DefaultFullScreen()
            {
                PlayerPrefs.SetInt(MagicStrings.OPTIONS_FULLSCREEN, 0);
                toggleFullScreen.value = PlayerPrefs.GetInt(MagicStrings.OPTIONS_FULLSCREEN, 0) == 1;
                Screen.fullScreen = PlayerPrefs.GetInt(MagicStrings.OPTIONS_FULLSCREEN, 0) == 1;
            }
            #endregion

            #region Resolution Stuff
            protected void TempResolution(ChangeEvent<string> evt)
            {
                resolutionIndex = resolutionNames.IndexOf(evt.newValue);
                SetResolution(resolutionIndex);
            }

            protected void SetResolution(int resolutionIndex)
            {
                PlayerPrefs.SetInt(MagicStrings.OPTIONS_RESOLUTION_INDEX, resolutionIndex);
                Display.main.SetRenderingResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height);
                Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, toggleFullScreen.value);
            }
            protected void RevertResolution()
            {
                resolutionDropdown.index = PlayerPrefs.GetInt(MagicStrings.OPTIONS_RESOLUTION_INDEX, defaultResIndex);
                resolutionDropdown.value = resolutionNames[PlayerPrefs.GetInt(MagicStrings.OPTIONS_RESOLUTION_INDEX, defaultResIndex)];
                resolutionIndex = PlayerPrefs.GetInt(MagicStrings.OPTIONS_RESOLUTION_INDEX, defaultResIndex);
                Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, toggleFullScreen.value);
                print($"Reverting to: {resolutions[resolutionIndex].width}x{resolutions[resolutionIndex].height}");
            }
            protected void DefaultResolution()
            {
                PlayerPrefs.SetInt(MagicStrings.OPTIONS_RESOLUTION_INDEX, defaultResIndex);
                resolutionIndex = PlayerPrefs.GetInt(MagicStrings.OPTIONS_RESOLUTION_INDEX, defaultResIndex);
                print($"Defaulting to: {resolutions[resolutionIndex].width}x{resolutions[resolutionIndex].height}");
                resolutionDropdown.index = PlayerPrefs.GetInt(MagicStrings.OPTIONS_RESOLUTION_INDEX, defaultResIndex);
                resolutionDropdown.value = resolutionNames[PlayerPrefs.GetInt(MagicStrings.OPTIONS_RESOLUTION_INDEX, defaultResIndex)];
                Screen.SetResolution(resolutions[resolutionIndex].width, resolutions[resolutionIndex].height, toggleFullScreen.value);
            }
            #endregion

            #region FOV and FPS
            protected void TempFOV(ChangeEvent<float> evt)
            {
                FOVValue = evt.newValue;
                SetFOV();
            }

            protected void SetFOV()
            {
                PlayerPrefs.SetFloat(MagicStrings.OPTIONS_FOV_HUB, 90);
                // APPLY TO CAMERA HERE?
                hubCam.m_Lens.FieldOfView = PlayerPrefs.GetFloat(MagicStrings.OPTIONS_FOV_HUB, 90);
            }
            protected void RevertFOV()
            {
                FOVSlider.value = PlayerPrefs.GetFloat(MagicStrings.OPTIONS_FOV_HUB, 90);
                FOVValue = PlayerPrefs.GetFloat(MagicStrings.OPTIONS_FOV_HUB, 90);

                // APPLY TO CAMERA HERE?
                hubCam.m_Lens.FieldOfView = PlayerPrefs.GetFloat(MagicStrings.OPTIONS_FOV_HUB, 90);
            }
            protected void DefaultFOV()
            {
                PlayerPrefs.SetFloat(MagicStrings.OPTIONS_FOV_HUB, 90);
                FOVSlider.value = PlayerPrefs.GetFloat(MagicStrings.OPTIONS_FOV_HUB, 90);

                // APPLY TO CAMERA HERE?
                hubCam.m_Lens.FieldOfView = PlayerPrefs.GetFloat(MagicStrings.OPTIONS_FOV_HUB, 90);
            }
            #endregion
        }
    }
}