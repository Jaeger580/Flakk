using UnityEngine;
using UnityEngine.UIElements;

public class UI_InputMapper : MonoBehaviour
{
    UIDocument doc;

    private void OnEnable()
    {
        doc = GetComponent<UIDocument>();
        var mainCam = Camera.main;
        doc.panelSettings.SetScreenToPanelSpaceFunction((Vector2 screenPos) =>
        {
            var invalidPosition = new Vector2(float.NaN, float.NaN);

            var cameraRay = mainCam.ScreenPointToRay(Input.mousePosition);  //Fix later
            Debug.DrawRay(cameraRay.origin, cameraRay.direction * 100f, Color.magenta);

            RaycastHit hit;
            if (!Physics.Raycast(cameraRay, out hit, 100f, LayerMask.GetMask("UI")))
            {
                //Debug.Log("Invalid position.");
                return invalidPosition;
            }

            //Debug.Log($"{hit.collider.transform.parent.name}");

            Vector2 pixelUV = hit.textureCoord;

            pixelUV.y = 1 - pixelUV.y;
            pixelUV.x *= doc.panelSettings.targetTexture.width;
            pixelUV.y *= doc.panelSettings.targetTexture.height;

            //var cursor = doc.rootVisualElement.Q<VisualElement>("Cursor");
            //if (cursor != null)
            //{
            //    cursor.style.left = pixelUV.x;
            //    cursor.style.top = pixelUV.y;
            //}

            return pixelUV;
        });
    }
}