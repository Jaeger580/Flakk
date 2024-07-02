using System.Collections;
using System.Collections.Generic;
using GeneralUtility.GameEventSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace GeneralUtility
{
    namespace Options
    {
        public class SensitivityController : MonoBehaviour
        {
            private float defaultUnscopedSens = 1f, defaultScopedSens = 0.25f;
            private float xNewUnscopedSens, xNewScopedSens, yNewUnscopedSens, yNewScopedSens;
            private Slider sliderXUnscoped, sliderXScoped, sliderYUnscoped, sliderYScoped;
            private Toggle toggleAxisSync;
            public GameEvent applyOptionsEvent, revertOptionsEvent, defaultOptionsEvent, exitOptionsEvent;

            private void Start()
            {//Called AFTER AWAKE so that everything is set up properly
                var root = FindObjectOfType<UIDocument>().rootVisualElement;

                toggleAxisSync = root.Q<Toggle>("AxisSyncToggle");
                toggleAxisSync.value = PlayerPrefs.GetInt("AxisSyncBool", 1) == 1;

                sliderXUnscoped = root.Q<Slider>("xUnscopedSensitivity");
                sliderXScoped = root.Q<Slider>("xScopedSensitivity");
                sliderXScoped.style.display = UI_Utility.hidden;
                sliderXUnscoped.value = PlayerPrefs.GetFloat(sliderXUnscoped.name, defaultUnscopedSens);
                sliderXScoped.value = PlayerPrefs.GetFloat(sliderXScoped.name, defaultScopedSens);
                PlayerPrefs.SetFloat(sliderXUnscoped.name, sliderXUnscoped.value);
                PlayerPrefs.SetFloat(sliderXScoped.name, sliderXScoped.value);
                xNewUnscopedSens = sliderXUnscoped.value;
                xNewScopedSens = sliderXScoped.value;
                sliderXUnscoped.RegisterValueChangedCallback((evt) => TempSensitivity(evt, ref xNewUnscopedSens));     //Tell the system "when slider changes, call "SetMasterVolume"
                sliderXScoped.RegisterValueChangedCallback((evt) => TempSensitivity(evt, ref xNewScopedSens));

                sliderYUnscoped = root.Q<Slider>("yUnscopedSensitivity");
                sliderYScoped = root.Q<Slider>("yScopedSensitivity");
                sliderYScoped.style.display = UI_Utility.hidden;
                sliderYUnscoped.value = PlayerPrefs.GetFloat(sliderYUnscoped.name, defaultUnscopedSens);
                sliderYScoped.value = PlayerPrefs.GetFloat(sliderYScoped.name, defaultScopedSens);
                PlayerPrefs.SetFloat(sliderYUnscoped.name, sliderYUnscoped.value);
                PlayerPrefs.SetFloat(sliderYScoped.name, sliderYScoped.value);
                yNewUnscopedSens = sliderYUnscoped.value;
                yNewScopedSens = sliderYScoped.value;
                sliderYUnscoped.RegisterValueChangedCallback((evt) => TempSensitivity(evt, ref yNewUnscopedSens));     //Tell the system "when slider changes, call "SetMasterVolume"
                sliderYScoped.RegisterValueChangedCallback((evt) => TempSensitivity(evt, ref yNewScopedSens));

                var applyListener = gameObject.AddComponent<GameEventListener>();
                applyListener.Events.Add(applyOptionsEvent);
                applyListener.Response = new();
                applyListener.Response.AddListener(() => SetSensitivity(sliderXUnscoped.name, xNewUnscopedSens));
                applyListener.Response.AddListener(() => SetSensitivity(sliderXScoped.name, xNewScopedSens));
                applyListener.Response.AddListener(() => SetSensitivity(sliderYUnscoped.name, yNewUnscopedSens));
                applyListener.Response.AddListener(() => SetSensitivity(sliderYScoped.name, yNewScopedSens));
                applyOptionsEvent.RegisterListener(applyListener);

                var revertListener = gameObject.AddComponent<GameEventListener>();
                revertListener.Events.Add(revertOptionsEvent);
                revertListener.Response = new();
                revertListener.Response.AddListener(() => RevertSensitivity(sliderXUnscoped, PlayerPrefs.GetFloat(sliderXUnscoped.name), ref xNewUnscopedSens));
                revertListener.Response.AddListener(() => RevertSensitivity(sliderXScoped, PlayerPrefs.GetFloat(sliderXScoped.name), ref xNewScopedSens));
                revertListener.Response.AddListener(() => RevertSensitivity(sliderYUnscoped, PlayerPrefs.GetFloat(sliderYUnscoped.name), ref yNewUnscopedSens));
                revertListener.Response.AddListener(() => RevertSensitivity(sliderYScoped, PlayerPrefs.GetFloat(sliderYScoped.name), ref yNewScopedSens));
                revertOptionsEvent.RegisterListener(revertListener);

                var defaultListener = gameObject.AddComponent<GameEventListener>();
                defaultListener.Events.Add(defaultOptionsEvent);
                defaultListener.Response = new();
                defaultListener.Response.AddListener(() => DefaultSensitivity());
                defaultOptionsEvent.RegisterListener(defaultListener);

                var exitListener = gameObject.AddComponent<GameEventListener>();
                exitListener.Events.Add(exitOptionsEvent);
                exitListener.Response = new();
                exitListener.Response.AddListener(() => RevertSensitivity(sliderXUnscoped, PlayerPrefs.GetFloat(sliderXUnscoped.name), ref xNewUnscopedSens));
                exitListener.Response.AddListener(() => RevertSensitivity(sliderXScoped, PlayerPrefs.GetFloat(sliderXScoped.name), ref xNewScopedSens));
                exitListener.Response.AddListener(() => RevertSensitivity(sliderYUnscoped, PlayerPrefs.GetFloat(sliderYUnscoped.name), ref yNewUnscopedSens));
                exitListener.Response.AddListener(() => RevertSensitivity(sliderYScoped, PlayerPrefs.GetFloat(sliderYScoped.name), ref yNewScopedSens));
                exitOptionsEvent.RegisterListener(exitListener);
            }

            public void TempSensitivity(ChangeEvent<float> evt, ref float flt)
            {
                flt = evt.newValue;
                float rounded = Mathf.Round(evt.newValue * 100f) / 100f;

                SetSensitivity(((VisualElement)evt.target).name, evt.newValue);
                if (toggleAxisSync.value)
                {
                    if (((VisualElement)evt.target).name == sliderXUnscoped.name || ((VisualElement)evt.target).name == sliderYUnscoped.name)
                    {
                        sliderXUnscoped.value = rounded;
                        sliderYUnscoped.value = rounded;
                        xNewUnscopedSens = rounded;
                        yNewUnscopedSens = rounded;

                        SetSensitivity(sliderXUnscoped.name, rounded);
                        SetSensitivity(sliderYUnscoped.name, rounded);
                    }
                    else if (((VisualElement)evt.target).name == sliderXScoped.name || ((VisualElement)evt.target).name == sliderYScoped.name)
                    {
                        sliderXScoped.value = rounded;
                        sliderYScoped.value = rounded;
                        xNewScopedSens = rounded;
                        yNewScopedSens = rounded;

                        SetSensitivity(sliderXScoped.name, rounded);
                        SetSensitivity(sliderYScoped.name, rounded);
                    }
                }
            }

            public void SetSensitivity(string name, float val)
            {//Parameter: evt is an object that holds data about the recent slider movement
                PlayerPrefs.SetFloat(name, val);
            }

            public void DefaultSensitivity()
            {
                PlayerPrefs.SetFloat(sliderXUnscoped.name, defaultUnscopedSens);
                PlayerPrefs.SetFloat(sliderXScoped.name, defaultScopedSens);
                PlayerPrefs.SetFloat(sliderYUnscoped.name, defaultUnscopedSens);
                PlayerPrefs.SetFloat(sliderYScoped.name, defaultScopedSens);

                sliderXUnscoped.value = PlayerPrefs.GetFloat(sliderXUnscoped.name, defaultUnscopedSens);
                sliderXScoped.value = PlayerPrefs.GetFloat(sliderXScoped.name, defaultScopedSens);
                sliderYUnscoped.value = PlayerPrefs.GetFloat(sliderYUnscoped.name, defaultUnscopedSens);
                sliderYScoped.value = PlayerPrefs.GetFloat(sliderYScoped.name, defaultScopedSens);
            }

            public void RevertSensitivity(Slider sli, float val, ref float flt)
            {
                sli.value = val;
                flt = val;
            }
        }
    }
}