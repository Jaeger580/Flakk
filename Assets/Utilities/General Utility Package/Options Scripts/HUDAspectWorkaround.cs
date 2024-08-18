using System.Collections;
using System.Collections.Generic;
using GeneralUtility.GameEventSystem;
using UnityEngine;

namespace GeneralUtility
{
    namespace Options
    {
        public class HUDAspectWorkaround : MonoBehaviour
        {
            [SerializeField] private GameEvent applyOptionsEvent, revertOptionsEvent, defaultOptionsEvent, exitOptionsEvent;
            [SerializeField] private GameObject visorSixteenByNine, visorSixteenByTen;

#if !UNITY_EDITOR
    private void Start()
    {
        var causeHUDUpdateListener = gameObject.AddComponent<GameEventListener>();
        causeHUDUpdateListener.Events.Add(applyOptionsEvent);
        causeHUDUpdateListener.Events.Add(revertOptionsEvent);
        causeHUDUpdateListener.Events.Add(defaultOptionsEvent);
        causeHUDUpdateListener.Events.Add(exitOptionsEvent);
        causeHUDUpdateListener.Response = new();
        causeHUDUpdateListener.Response.AddListener(() => UpdateHUDResolution());
        foreach (var e in causeHUDUpdateListener.Events)
        {
            e.RegisterListener(causeHUDUpdateListener);
        }

        UpdateHUDResolution();
    }
#endif

            //https://forum.unity.com/threads/get-aspect-ratio.211255/
            //Ugly and not my code, but fuck this whole situation in the first place so

            private void UpdateHUDResolution()
            {
                visorSixteenByNine.SetActive(false);
                visorSixteenByTen.SetActive(false);

                float r = (float)Screen.currentResolution.width / Screen.currentResolution.height;
                string _r = r.ToString("F2");
                string ratio = _r.Substring(0, 4);
                string trueRatio = string.Empty;

                switch (ratio)
                {
                    case "2.37":
                    case "2.39":
                        trueRatio = "21:9";
                        break;
                    case "1.25":
                        trueRatio = ("  5:4");
                        break;
                    case "1.33":
                        trueRatio = "  4:3";
                        break;
                    case "1.50":
                        trueRatio = "  3:2";
                        break;
                    case "1.60":
                    case "1.56":
                        trueRatio = "  16:10";
                        visorSixteenByTen.SetActive(true);
                        break;
                    case "1.67":
                    case "1.78":
                    case "1.77":
                        trueRatio = "  16:9";
                        visorSixteenByNine.SetActive(true);
                        break;
                    case "0.67":
                        trueRatio = "  2:3";
                        break;
                    case "0.56":
                        trueRatio = "  9:16";
                        break;
                    default:
                        break;
                }

                print($"True aspect ratio: {trueRatio}");
            }
        }
    }
}