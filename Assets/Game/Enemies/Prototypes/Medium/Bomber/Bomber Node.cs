using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

[SelectionBase]
public class BomberNode : MonoBehaviour
{
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //var sceneCam = SceneView.currentDrawingSceneView.cameraDistance;
        var distance = Vector3.Distance(transform.position, SceneView.currentDrawingSceneView.camera.transform.position);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, distance / 20);
    }
#endif
}
