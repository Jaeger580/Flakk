using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Waypoint_Web : MonoBehaviour
{
    public List<Transform> waypoints;
    public float nodeSize;
    public Color32 nodeColor;
    public Color32 webColor;
    public WebSelection currentWeb;

    private void ChildCheck()
    {
        foreach (Transform child in transform)
        {
            if (!waypoints.Contains(child))
            {
                waypoints.Add(child);
            }
        }

        foreach(Transform child in waypoints)
        {
            if(child == null)
            {
                waypoints.Remove(child);
            }
        }

        if (transform.childCount < waypoints.Count)
        {
            int num = waypoints.Count - transform.childCount;

            for (int i = 0; i < num; i++)
            {
                waypoints.RemoveAt(waypoints.Count - 1);
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = webColor;
        ChildCheck();

        foreach (Transform node in waypoints)
        {
            node.localScale = new Vector3(nodeSize, nodeSize, nodeSize);
            node.GetComponent<MeshRenderer>().sharedMaterial.color = nodeColor;
        }

        for (int i = 0; i < waypoints.Count; i++)
        {
            if(i == waypoints.Count - 1)
            {
                waypoints[i].LookAt(waypoints[0].position);
                float distance = Vector3.Distance(waypoints[i].position, waypoints[0].position);
                Gizmos.DrawRay(waypoints[i].position, waypoints[i].forward * distance);
            }
            else
            {
                waypoints[i].LookAt(waypoints[i + 1].position);
                float distance = Vector3.Distance(waypoints[i].position, waypoints[i + 1].position);
                Gizmos.DrawRay(waypoints[i].position, waypoints[i].forward * distance);
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Waypoint_Web))]
public class WebGUIEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Waypoint_Web web = (Waypoint_Web)target;

        EditorGUI.BeginChangeCheck();

        WebSelection selectedWeb = (WebSelection)EditorGUILayout.EnumPopup("Desired Selected", web.currentWeb);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(web, "Change Desired Selected Web");
            web.currentWeb = selectedWeb;

            if (Spider.instance.IsStateAvailable(selectedWeb))
            {
                Debug.Log("Selected " + selectedWeb + " is available.");
            }
            else
            {
                Debug.Log("Selected " + selectedWeb + " is already in use.");
            }
        }

/*
        GUILayout.Label("Used States:");
        foreach (var state in Spider.instance.GetUsedStates())
        {
            GUILayout.Label(state.ToString());
        }
*/
        DrawDefaultInspector();
    }
}
#endif