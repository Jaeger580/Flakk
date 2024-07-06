using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint_Web : MonoBehaviour
{
    public List<Transform> waypoints;
    public float nodeSize;
    public Color32 nodeColor;
    public Color32 webColor;

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
