using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint_Web : MonoBehaviour
{
    public Transform[] waypoints;
    public float nodeSize;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        foreach(Transform node in waypoints)
        {
            node.localScale = new Vector3(nodeSize, nodeSize, nodeSize);
        }

        for (int i = 0; i < waypoints.Length; i++)
        {
            if(i == waypoints.Length - 1)
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
