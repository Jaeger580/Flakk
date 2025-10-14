using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskHighlight : MonoBehaviour
{
    public GameObject targetPositionObject;


    void Update()
    {
        var targetPos = targetPositionObject.transform.position;
        var screenPos = Camera.main.WorldToScreenPoint(targetPos);
        targetPos.y += 5f;
        var recticlePos = this.transform.position;

        // Stop duplicate UI
        if(screenPos.z < 0)
        {
            screenPos *= -1;
        }

        this.transform.position = screenPos;
    }
}
