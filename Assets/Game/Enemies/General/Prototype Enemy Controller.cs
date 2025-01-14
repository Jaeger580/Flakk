/* Jakob Jaeger
 * 09/06/2024
 * This is a prototype script that SHOULD NOT BE USED AFTER TESTING. A new script should be rewritten as the framework for future enemies. 
 * 
 * This script handles the control of A SINGULAR ENEMY.
 * Matching its position with the leading point which so be a sibiling object to whatever this is attached to.
 * Uses Late update as the LeadPoint needs to update before this script.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrototypeEnemyController : MonoBehaviour
{
    [SerializeField]
    private GameObject LeadPoint;

    [SerializeField]
    private Vector3 offSet;

    private void LateUpdate()
    {
        var targetPos = LeadPoint.transform.position;
        var finalPos = new Vector3(targetPos.x - offSet.x, targetPos.y - offSet.y, targetPos.z - offSet.z);

        // Matches postion to the leading point
        this.gameObject.transform.position = finalPos;

        // Matches rotation to the leading point. Note this sibling relationship was set up so this exact feature would be optional.
        this.gameObject.transform.rotation = LeadPoint.transform.rotation;
    }
}
