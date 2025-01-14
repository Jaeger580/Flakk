using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UI;

namespace GeneralUtility
{
    public class UI_Utility : MonoBehaviour
    {
        public const DisplayStyle showing = DisplayStyle.Flex, hidden = DisplayStyle.None;
        private const float DEFAULT_PREPAUSE = 0f, DEFAULT_DURATION = 0.05f;
        static private WaitForSeconds defaultWait = new(DEFAULT_PREPAUSE);
        static private WaitForSecondsRealtime defaultWaitRealtime = new(DEFAULT_PREPAUSE);

        //static private List<Coroutine> currentlyRunning = new();

        //static public void RecordElementFadeIn(Coroutine c)
        //{
        //    if (currentlyRunning.Contains(c)) return;
        //    currentlyRunning.Add(c);
        //}
        //static public void RecordElementFadeOut(Coroutine c)
        //{
        //    if (currentlyRunning.Contains(c)) return;
        //    currentlyRunning.Add(c);
        //}

        #region FadeIn and Overloads
        static public IEnumerator C_FadeIn(VisualElement element, float duration, AnimationCurve posEaseIn, AnimationCurve opaEaseIn, Vector2 inFromPosition, bool ignorePause)
        {//Considered reducing to only one function with Fade going either way, but decided it better to keep separate
            if (ignorePause)
                yield return defaultWaitRealtime;
            else
                yield return defaultWait;
            element.style.display = showing;
            Vector2 posOrigin = new(element.style.translate.value.x.value + inFromPosition.x,
                                    element.style.translate.value.y.value + inFromPosition.y);
            //Vector2 posTarget = new(element.style.translate.value.x.value, element.style.translate.value.y.value);
            Vector2 posTarget = new(0, 0);
            float opaOrigin = element.style.opacity.value;
            float opaTarget = 1f;

            float journey = 0f;
            while (journey <= duration)
            {
                if (ignorePause)
                    journey += Time.unscaledDeltaTime;
                else
                    journey += Time.deltaTime;

                float percent = Mathf.Clamp01(journey / duration);
                float posEasePercent = posEaseIn.Evaluate(percent);
                float opaEasePercent = opaEaseIn.Evaluate(percent);

                element.style.translate = new Translate(Mathf.LerpUnclamped(posOrigin.x, posTarget.x, posEasePercent), Mathf.LerpUnclamped(posOrigin.y, posTarget.y, posEasePercent));
                element.style.opacity = Mathf.LerpUnclamped(opaOrigin, opaTarget, opaEasePercent);

                yield return null;
            }
        }

        static public IEnumerator C_FadeIn(VisualElement element, float duration, Vector2 inFromPosition, bool ignorePause)
        {//Considered reducing to only one function with Fade going either way, but decided it better to keep separate
            if (ignorePause)
                yield return defaultWaitRealtime;
            else
                yield return defaultWait;

            element.style.display = showing;
            Vector2 posOrigin = new(element.style.translate.value.x.value + inFromPosition.x,
                                    element.style.translate.value.y.value + inFromPosition.y);
            //Vector2 posTarget = new(element.style.translate.value.x.value, element.style.translate.value.y.value);
            Vector2 posTarget = new(0, 0);
            float opaOrigin = element.style.opacity.value;
            float opaTarget = 1f;

            float journey = 0f;
            while (journey <= duration)
            {
                if (ignorePause)
                    journey += Time.unscaledDeltaTime;
                else
                    journey += Time.deltaTime;

                float percent = Mathf.Clamp01(journey / duration);

                element.style.translate = new Translate(Mathf.Lerp(posOrigin.x, posTarget.x, percent), Mathf.Lerp(posOrigin.y, posTarget.y, percent));
                element.style.opacity = Mathf.Lerp(opaOrigin, opaTarget, percent);

                yield return null;
            }
        }

        static public IEnumerator C_FadeIn(VisualElement element, Vector2 inFromPosition, bool ignorePause)
        {//Considered reducing to only one function with Fade going either way, but decided it better to keep separate
            if (ignorePause)
                yield return defaultWaitRealtime;
            else
                yield return defaultWait;

            element.style.display = showing;
            Vector2 posOrigin = new(element.style.translate.value.x.value + inFromPosition.x,
                                    element.style.translate.value.y.value + inFromPosition.y);
            //Vector2 posTarget = new(element.style.translate.value.x.value, element.style.translate.value.y.value);
            Vector2 posTarget = new(0, 0);
            float opaOrigin = element.style.opacity.value;
            float opaTarget = 1f;

            float journey = 0f;
            while (journey <= DEFAULT_DURATION)
            {
                if (ignorePause)
                    journey += Time.unscaledDeltaTime;
                else
                    journey += Time.deltaTime;

                float percent = Mathf.Clamp01(journey / DEFAULT_DURATION);

                element.style.translate = new Translate(Mathf.Lerp(posOrigin.x, posTarget.x, percent), Mathf.Lerp(posOrigin.y, posTarget.y, percent));
                element.style.opacity = Mathf.Lerp(opaOrigin, opaTarget, percent);

                yield return null;
            }
        }

        static public IEnumerator C_FadeIn(VisualElement element, bool ignorePause)
        {//Considered reducing to only one function with Fade going either way, but decided it better to keep separate
            if (ignorePause)
                yield return defaultWaitRealtime;
            else
                yield return defaultWait;

            element.style.display = showing;
            float opaOrigin = element.style.opacity.value;
            float opaTarget = 1f;

            float journey = 0f;
            while (journey <= DEFAULT_DURATION)
            {
                if (ignorePause)
                    journey += Time.unscaledDeltaTime;
                else
                    journey += Time.deltaTime;

                float percent = Mathf.Clamp01(journey / DEFAULT_DURATION);

                element.style.opacity = Mathf.Lerp(opaOrigin, opaTarget, percent);

                yield return null;
            }
        }

        static public IEnumerator C_FadeIn_OnlyOpacity(VisualElement element, bool ignorePause)
        {//Considered reducing to only one function with Fade going either way, but decided it better to keep separate
            if (ignorePause)
                yield return defaultWaitRealtime;
            else
                yield return defaultWait;

            float opaOrigin = element.style.opacity.value;
            float opaTarget = 1f;

            float journey = 0f;
            while (journey <= DEFAULT_DURATION)
            {
                if (ignorePause)
                    journey += Time.unscaledDeltaTime;
                else
                    journey += Time.deltaTime;

                float percent = Mathf.Clamp01(journey / DEFAULT_DURATION);

                element.style.opacity = Mathf.Lerp(opaOrigin, opaTarget, percent);

                yield return null;
            }
        }

        static public IEnumerator C_FadeIn_OnlyOpacity(VisualElement element, float duration, AnimationCurve posEaseIn, AnimationCurve opaEaseIn, Vector2 inFromPosition, bool ignorePause)
        {//Considered reducing to only one function with Fade going either way, but decided it better to keep separate

            if (ignorePause)
                yield return defaultWaitRealtime;
            else
                yield return defaultWait;
            Vector2 posOrigin = new(element.style.translate.value.x.value + inFromPosition.x,
                                    element.style.translate.value.y.value + inFromPosition.y);
            //Vector2 posTarget = new(element.style.translate.value.x.value, element.style.translate.value.y.value);
            Vector2 posTarget = new(0, 0);
            float opaOrigin = element.style.opacity.value;
            float opaTarget = 1f;

            float journey = 0f;
            while (journey <= duration)
            {
                if (ignorePause)
                    journey += Time.unscaledDeltaTime;
                else
                    journey += Time.deltaTime;

                float percent = Mathf.Clamp01(journey / duration);
                float posEasePercent = posEaseIn.Evaluate(percent);
                float opaEasePercent = opaEaseIn.Evaluate(percent);

                element.style.translate = new Translate(Mathf.LerpUnclamped(posOrigin.x, posTarget.x, posEasePercent), Mathf.LerpUnclamped(posOrigin.y, posTarget.y, posEasePercent));
                element.style.opacity = Mathf.LerpUnclamped(opaOrigin, opaTarget, opaEasePercent);

                yield return null;
            }
        }
        #endregion

        #region FadeOut and Overloads
        static public IEnumerator C_FadeOut(VisualElement element, float duration, AnimationCurve posEaseIn, AnimationCurve opaEaseIn, Vector2 outToPosition, bool ignorePause)
        {
            //Vector2 posOrigin = new(element.style.translate.value.x.value, element.style.translate.value.y.value);
            Vector2 posOrigin = new(0, 0);
            Vector2 posTarget = new(element.style.translate.value.x.value + outToPosition.x,
                                    element.style.translate.value.y.value + outToPosition.y);
            float opaOrigin = element.style.opacity.value;
            float opaTarget = 0f;

            float journey = 0f;
            while (journey <= duration)
            {
                if (ignorePause)
                    journey += Time.unscaledDeltaTime;
                else
                    journey += Time.deltaTime;

                float percent = Mathf.Clamp01(journey / duration);
                float posEasePercent = posEaseIn.Evaluate(percent);
                float opaEasePercent = opaEaseIn.Evaluate(percent);

                element.style.translate = new Translate(Mathf.LerpUnclamped(posOrigin.x, posTarget.x, posEasePercent), Mathf.LerpUnclamped(posOrigin.y, posTarget.y, posEasePercent));
                element.style.opacity = Mathf.LerpUnclamped(opaOrigin, opaTarget, opaEasePercent);

                yield return null;
            }
            element.style.display = hidden;
        }

        static public IEnumerator C_FadeOut(VisualElement element, float duration, Vector2 outToPosition, bool ignorePause)
        {
            //Vector2 posOrigin = new(element.style.translate.value.x.value, element.style.translate.value.y.value);
            Vector2 posOrigin = new(0, 0);
            Vector2 posTarget = new(element.style.translate.value.x.value + outToPosition.x,
                                    element.style.translate.value.y.value + outToPosition.y);
            float opaOrigin = element.style.opacity.value;
            float opaTarget = 0f;

            float journey = 0f;
            while (journey <= duration)
            {
                if (ignorePause)
                    journey += Time.unscaledDeltaTime;
                else
                    journey += Time.deltaTime;

                float percent = Mathf.Clamp01(journey / duration);

                element.style.translate = new Translate(Mathf.Lerp(posOrigin.x, posTarget.x, percent), Mathf.Lerp(posOrigin.y, posTarget.y, percent));
                element.style.opacity = Mathf.Lerp(opaOrigin, opaTarget, percent);

                yield return null;
            }
            element.style.display = hidden;
        }

        static public IEnumerator C_FadeOut(VisualElement element, Vector2 outToPosition, bool ignorePause)
        {
            //Vector2 posOrigin = new(element.style.translate.value.x.value, element.style.translate.value.y.value);
            Vector2 posOrigin = new(0, 0);
            Vector2 posTarget = new(element.style.translate.value.x.value + outToPosition.x,
                                    element.style.translate.value.y.value + outToPosition.y);
            float opaOrigin = element.style.opacity.value;
            float opaTarget = 0f;

            float journey = 0f;
            while (journey <= DEFAULT_DURATION)
            {
                if (ignorePause)
                    journey += Time.unscaledDeltaTime;
                else
                    journey += Time.deltaTime;

                float percent = Mathf.Clamp01(journey / DEFAULT_DURATION);

                element.style.translate = new Translate(Mathf.Lerp(posOrigin.x, posTarget.x, percent), Mathf.Lerp(posOrigin.y, posTarget.y, percent));
                element.style.opacity = Mathf.Lerp(opaOrigin, opaTarget, percent);

                yield return null;
            }
            element.style.display = hidden;
        }

        static public IEnumerator C_FadeOut(VisualElement element, bool ignorePause)
        {
            //Vector2 posOrigin = new(element.style.translate.value.x.value, element.style.translate.value.y.value);
            float opaOrigin = element.style.opacity.value;
            float opaTarget = 0f;

            float journey = 0f;
            while (journey <= DEFAULT_DURATION)
            {
                if (ignorePause)
                    journey += Time.unscaledDeltaTime;
                else
                    journey += Time.deltaTime;

                float percent = Mathf.Clamp01(journey / DEFAULT_DURATION);

                element.style.opacity = Mathf.Lerp(opaOrigin, opaTarget, percent);

                yield return null;
            }
            element.style.display = hidden;
        }

        static public IEnumerator C_FadeOut_OnlyOpacity(VisualElement element, bool ignorePause)
        {
            //Vector2 posOrigin = new(element.style.translate.value.x.value, element.style.translate.value.y.value);
            float opaOrigin = element.style.opacity.value;
            float opaTarget = 0f;

            float journey = 0f;
            while (journey <= DEFAULT_DURATION)
            {
                if (ignorePause)
                    journey += Time.unscaledDeltaTime;
                else
                    journey += Time.deltaTime;

                float percent = Mathf.Clamp01(journey / DEFAULT_DURATION);

                element.style.opacity = Mathf.Lerp(opaOrigin, opaTarget, percent);

                yield return null;
            }
        }
        static public IEnumerator C_FadeOut_OnlyOpacity(VisualElement element, float duration, AnimationCurve posEaseIn, AnimationCurve opaEaseIn, Vector2 outToPosition, bool ignorePause)
        {
            //Vector2 posOrigin = new(element.style.translate.value.x.value, element.style.translate.value.y.value);
            Vector2 posOrigin = new(0, 0);
            Vector2 posTarget = new(element.style.translate.value.x.value + outToPosition.x,
                                    element.style.translate.value.y.value + outToPosition.y);
            float opaOrigin = element.style.opacity.value;
            float opaTarget = 0f;

            float journey = 0f;
            while (journey <= duration)
            {
                if (ignorePause)
                    journey += Time.unscaledDeltaTime;
                else
                    journey += Time.deltaTime;

                float percent = Mathf.Clamp01(journey / duration);
                float posEasePercent = posEaseIn.Evaluate(percent);
                float opaEasePercent = opaEaseIn.Evaluate(percent);

                element.style.translate = new Translate(Mathf.LerpUnclamped(posOrigin.x, posTarget.x, posEasePercent), Mathf.LerpUnclamped(posOrigin.y, posTarget.y, posEasePercent));
                element.style.opacity = Mathf.LerpUnclamped(opaOrigin, opaTarget, opaEasePercent);

                yield return null;
            }
        }
        #endregion

        #region UGUI STUFF

        static public IEnumerator C_FadeIn_UGUI_Image(UnityEngine.UI.Image image, float duration, AnimationCurve posEaseIn, AnimationCurve opaEaseIn,
            Vector2 inFromPosition, Vector2 desiredLocalOrigin, float desiredOpacity, bool ignorePause)
        {//Considered reducing to only one function with Fade going either way, but decided it better to keep separate
            if (ignorePause)
                yield return defaultWaitRealtime;
            else
                yield return defaultWait;

            image.gameObject.SetActive(true);

            Vector2 posOrigin = new(image.rectTransform.localPosition.x + inFromPosition.x,
                                    image.rectTransform.localPosition.y + inFromPosition.y);
            //Vector2 posTarget = new(element.style.translate.value.x.value, element.style.translate.value.y.value);
            Vector2 posTarget = desiredLocalOrigin;
            float opaOrigin = 0f;
            float opaTarget = desiredOpacity;

            float journey = 0f;
            while (journey <= duration)
            {
                if (ignorePause)
                    journey += Time.unscaledDeltaTime;
                else
                    journey += Time.deltaTime;

                float percent = Mathf.Clamp01(journey / duration);
                float posEasePercent = posEaseIn.Evaluate(percent);
                float opaEasePercent = opaEaseIn.Evaluate(percent);

                image.rectTransform.SetLocalPositionAndRotation(new Vector3(Mathf.LerpUnclamped(posOrigin.x, posTarget.x, posEasePercent),
                    Mathf.LerpUnclamped(posOrigin.y, posTarget.y, posEasePercent), image.rectTransform.localPosition.z), Quaternion.identity);
                var newColor = image.color;
                newColor.a = Mathf.LerpUnclamped(opaOrigin, opaTarget, opaEasePercent);
                image.color = newColor;

                yield return null;
            }
        }

        static public IEnumerator C_FadeOut_UGUI_Image(UnityEngine.UI.Image image, float duration, AnimationCurve posEaseIn, AnimationCurve opaEaseIn,
            Vector2 outToPosition, Vector2 desiredLocalOrigin, bool ignorePause)
        {//local because we want it simple - base it off where the designer put the thing in the first place
            Vector2 posOrigin = desiredLocalOrigin;
            Vector2 posTarget = new(image.rectTransform.localPosition.x + outToPosition.x, image.rectTransform.localPosition.y + outToPosition.y);
            //Vector2 posTarget = new(element.style.translate.value.x.value, element.style.translate.value.y.value);
            float opaOrigin = image.color.a;
            float opaTarget = 0f;

            float journey = 0f;
            while (journey <= duration)
            {
                if (ignorePause)
                    journey += Time.unscaledDeltaTime;
                else
                    journey += Time.deltaTime;

                float percent = Mathf.Clamp01(journey / duration);
                float posEasePercent = posEaseIn.Evaluate(percent);
                float opaEasePercent = opaEaseIn.Evaluate(percent);

                image.rectTransform.SetLocalPositionAndRotation(new Vector3(Mathf.LerpUnclamped(posOrigin.x, posTarget.x, posEasePercent),
                    Mathf.LerpUnclamped(posOrigin.y, posTarget.y, posEasePercent), image.rectTransform.localPosition.z), Quaternion.identity);
                var newColor = image.color;
                newColor.a = Mathf.LerpUnclamped(opaOrigin, opaTarget, opaEasePercent);
                image.color = newColor;

                yield return null;
            }
            image.gameObject.SetActive(false);
        }

        static public IEnumerator C_FadeIn_UGUI_Image(UnityEngine.UI.Image image, float duration, AnimationCurve posEaseIn, AnimationCurve opaEaseIn, float desiredOpacity, bool ignorePause)
        {//Considered reducing to only one function with Fade going either way, but decided it better to keep separate
            if (ignorePause)
                yield return defaultWaitRealtime;
            else
                yield return defaultWait;

            image.gameObject.SetActive(true);

            float opaOrigin = 0f;
            float opaTarget = desiredOpacity;

            float journey = 0f;
            while (journey <= duration)
            {
                if (ignorePause)
                    journey += Time.unscaledDeltaTime;
                else
                    journey += Time.deltaTime;

                float percent = Mathf.Clamp01(journey / duration);
                float posEasePercent = posEaseIn.Evaluate(percent);
                float opaEasePercent = opaEaseIn.Evaluate(percent);

                var newColor = image.color;
                newColor.a = Mathf.LerpUnclamped(opaOrigin, opaTarget, opaEasePercent);
                image.color = newColor;

                yield return null;
            }
        }

        static public IEnumerator C_FadeOut_UGUI_Image(UnityEngine.UI.Image image, float duration, AnimationCurve posEaseIn, AnimationCurve opaEaseIn, bool ignorePause)
        {//local because we want it simple - base it off where the designer put the thing in the first place
            float opaOrigin = image.color.a;
            float opaTarget = 0f;

            float journey = 0f;
            while (journey <= duration)
            {
                if (ignorePause)
                    journey += Time.unscaledDeltaTime;
                else
                    journey += Time.deltaTime;

                float percent = Mathf.Clamp01(journey / duration);
                float posEasePercent = posEaseIn.Evaluate(percent);
                float opaEasePercent = opaEaseIn.Evaluate(percent);

                var newColor = image.color;
                newColor.a = Mathf.LerpUnclamped(opaOrigin, opaTarget, opaEasePercent);
                image.color = newColor;

                yield return null;
            }
            image.gameObject.SetActive(false);
        }

        #endregion

        static public void ToggleContainer(VisualElement container)
        {
            container.style.display = container.style.display == showing ? hidden : showing;
        }

        static public void ToggleContainer(VisualElement container, bool shouldBeVisible)
        {
            container.style.display = shouldBeVisible ? showing : hidden;
        }

        static public void AddSFX(UnityEngine.UIElements.Button btn, AudioSource sfxSource, AudioClip sfxHover, AudioClip sfxClick)
        {//RegisterCallback example. The signature of the method being called must match the callback's return value
            btn.RegisterCallback<MouseOverEvent, (AudioSource, AudioClip)>(TriggerSFX, (sfxSource, sfxHover));
            btn.RegisterCallback<ClickEvent, (AudioSource, AudioClip)>(TriggerSFX, (sfxSource, sfxClick));
        }

        static public void TriggerSFX(EventBase evt, (AudioSource, AudioClip) sfxSourceAndClip)
        {
            sfxSourceAndClip.Item1.PlayOneShot(sfxSourceAndClip.Item2);
        }

        static public void AddSFXWorkaround(UnityEngine.UIElements.Button btn, AudioSource sfxSource, AudioClip sfxHover, AudioClip sfxClick)
        {//stupid bullshit, loadout options refuse to trigger their click sfx for some reason so w/e this will do
            btn.RegisterCallback<MouseOverEvent, (AudioSource, AudioClip)>(TriggerSFX, (sfxSource, sfxHover));
            btn.clicked += () => TriggerSFX((sfxSource, sfxClick));
        }

        static public void TriggerSFX((AudioSource, AudioClip) sfxSourceAndClip)
        {
            sfxSourceAndClip.Item1.PlayOneShot(sfxSourceAndClip.Item2);
        }
    }
}