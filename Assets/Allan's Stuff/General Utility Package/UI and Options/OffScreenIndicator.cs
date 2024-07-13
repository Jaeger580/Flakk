using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GeneralUtility
{
    namespace UI
    {
        public class OffScreenIndicator : MonoBehaviour
        {
            [SerializeField] private Image img;
            [SerializeField] private Transform target;
            private Camera mainCam;
            private Transform camTrans;
            [SerializeField] private Vector3 offset;
            void Start()
            {
                mainCam = Camera.main;
                camTrans = mainCam.transform;
            }

            void LateUpdate()
            {
                float minX = img.GetPixelAdjustedRect().width / 2;
                float maxX = Screen.width - minX;

                float minY = img.GetPixelAdjustedRect().height / 2;
                float maxY = Screen.height - minY;

                Vector2 pos = mainCam.WorldToScreenPoint(target.position + offset);

                if (Vector3.Dot((target.position - camTrans.position), camTrans.forward) < 0)
                {
                    //Target is behind the player
                    if (pos.x < Screen.width / 2)
                    {
                        pos.x = maxX;
                    }
                    else
                    {
                        pos.x = minX;
                    }
                }

                pos.x = Mathf.Clamp(pos.x, minX, maxX);
                pos.y = Mathf.Clamp(pos.y, minY, maxY);

                img.transform.position = pos;
            }
        }
    }
}