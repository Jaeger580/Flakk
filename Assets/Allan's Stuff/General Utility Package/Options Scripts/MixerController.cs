using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Audio;
using GeneralUtility.GameEventSystem;

namespace GeneralUtility
{
    namespace Options
    {
        public class MixerController : MonoBehaviour
        {
            [SerializeField] protected AudioMixer myAudioMixer;
            [SerializeField] protected GameEvent defaultAudioEvent;
            protected float defaultVolume = 0.5f;
            protected Slider masterSlider, musicSlider, sfxSlider;

            protected void Start()
            {//Called AFTER AWAKE so that everything is set up properly
                InitUI();

                myAudioMixer.SetFloat(masterSlider.name, Mathf.Log10(masterSlider.value) * 20);      //Set volume properly based on previously set volume
                myAudioMixer.SetFloat(musicSlider.name, Mathf.Log10(musicSlider.value) * 20);
                myAudioMixer.SetFloat(sfxSlider.name, Mathf.Log10(sfxSlider.value) * 20);

                var defaultListener = gameObject.AddComponent<GameEventListener>();
                defaultListener.Events.Add(defaultAudioEvent);
                defaultListener.Response = new();
                defaultListener.Response.AddListener(() => DefaultVolume());
                defaultAudioEvent.RegisterListener(defaultListener);
            }

            protected void InitUI()
            {
                var root = FindObjectOfType<UIDocument>().rootVisualElement;
                masterSlider = root.Q<Slider>("MasterSlider");
                musicSlider = root.Q<Slider>("MusicSlider");
                sfxSlider = root.Q<Slider>("SFXSlider");
                masterSlider.RegisterValueChangedCallback(SetVolume);     //Tell the system "when slider changes, call "SetMasterVolume"
                musicSlider.RegisterValueChangedCallback(SetVolume);
                sfxSlider.RegisterValueChangedCallback(SetVolume);
                masterSlider.value = PlayerPrefs.GetFloat(masterSlider.name, defaultVolume);
                musicSlider.value = PlayerPrefs.GetFloat(musicSlider.name, defaultVolume);
                sfxSlider.value = PlayerPrefs.GetFloat(sfxSlider.name, defaultVolume);
            }

            public void SetVolume(ChangeEvent<float> evt)
            {//Parameter: evt is an object that holds data about the recent slider movement
                myAudioMixer.SetFloat(((VisualElement)evt.currentTarget).name, Mathf.Log10(evt.newValue) * 20);
                PlayerPrefs.SetFloat(((VisualElement)evt.currentTarget).name, evt.newValue);
            }

            public void DefaultVolume()
            {
                PlayerPrefs.SetFloat(masterSlider.name, defaultVolume);
                PlayerPrefs.SetFloat(musicSlider.name, defaultVolume);
                PlayerPrefs.SetFloat(sfxSlider.name, defaultVolume);
                masterSlider.value = PlayerPrefs.GetFloat(masterSlider.name, defaultVolume);
                musicSlider.value = PlayerPrefs.GetFloat(musicSlider.name, defaultVolume);
                sfxSlider.value = PlayerPrefs.GetFloat(sfxSlider.name, defaultVolume);
                myAudioMixer.SetFloat(masterSlider.name, Mathf.Log10(defaultVolume) * 20);      //Set volume properly based on previously set volume
                myAudioMixer.SetFloat(musicSlider.name, Mathf.Log10(defaultVolume) * 20);
                myAudioMixer.SetFloat(sfxSlider.name, Mathf.Log10(defaultVolume) * 20);
            }
        }
    }
}