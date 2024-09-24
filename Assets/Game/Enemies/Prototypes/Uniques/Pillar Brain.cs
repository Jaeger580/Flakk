/*  Jakob Jaeger
 *  09/19/2024
 *  Script that will go onto the brain of pillar enemies
 *  Will hold an array of all the pillars it is managing
 *  Will be the one to handle the whatever happens when a pillar segement is defeated
*/

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PillarBrain : Enemy
{
    private GameObject topParent;

    [SerializeField]
    private GameObject segementPrefab;

    [SerializeField]
    private int startSize = 7;

    [SerializeField]
    private Vector3 segementOffSet;

    private List<GameObject> segements;

    private List<GameObject> targets;

    protected override void Start()
    {
        // Initialization
        segements = new List<GameObject>();
        targets = new List<GameObject>();

        // Add a follow point
        topParent = transform.parent.gameObject;

        var randDir = Random.insideUnitSphere;
        randDir *= 2;

        GameObject newFollow = Instantiate(new GameObject("Follow Target"), this.transform.position + randDir, Quaternion.identity, this.transform);

        targets.Add(newFollow);

        for (int i = 0; i < startSize; i++) 
        {
            segements.Add(Instantiate(segementPrefab, this.transform.position, Quaternion.identity, topParent.transform));
            if (i == 0) 
            {
                segements[i].transform.position = this.transform.position + segementOffSet;

                segements[i].AddComponent<PillarEnemy>();
                segements[i].GetComponent<PillarEnemy>().setLeadPoint(targets[0]);
            }
            else if (i != 0) 
            {
                segements[i].transform.position = segements[i - 1].transform.localPosition + segementOffSet;
            }
        }
    }

    protected override void Move()
    {
        // By default, this method will make the enemy follow the leadPoint at a 1:1 rate.
        // Both for position and rotation.
        var targetPos = leadPoint.transform.position;

        // Matches postion to the leading point
        this.gameObject.transform.position = targetPos;
    }
}