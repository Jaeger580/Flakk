using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

[SelectionBase]
public class PathNode : MonoBehaviour
{
    [SerializeField]
    private GameObject[] Inputs;

    [SerializeField]
    private GameObject[] Outputs;   // Primary varible that will be used. Inputs are for specific cases such as reversed movement.

    // Will likely need to add some sort of "Key" argument for choosing paths in the future.
    public SplineContainer GetExit() 
    {
        int rand = Random.Range(0, Outputs.Length);

        return Outputs[rand].GetComponent<SplineContainer>();
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //var sceneCam = SceneView.currentDrawingSceneView.cameraDistance;
        var distance = Vector3.Distance(transform.position, SceneView.currentDrawingSceneView.camera.transform.position);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, distance / 20);
    }
#endif
}