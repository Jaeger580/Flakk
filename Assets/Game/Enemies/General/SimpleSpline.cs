/* Jakob Jaeger
 * 09/17/2024
 * 
 * 
 * This script contains an abstract class that will be used as the base for all future enemies.
 * Contains values for health, move speed, ect...
 * Will contain methods handling, attacking, and other AI behaviors.
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class SimpleSpline : MonoBehaviour
{
    [SerializeField]
    private SplineContainer entrySpline;

    //[SerializeField]
    //private SplineContainer secondSpline
    
    [SerializeField]
    private SplineContainer targetSpline;

    private SplineAnimate spAnim;

    // Start is called before the first frame update
    void Start()
    {
        spAnim = GetComponent<SplineAnimate>();

        if(targetSpline == null) 
        {
            targetSpline = entrySpline;
        }

        spAnim.Container = entrySpline;
        spAnim.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(spAnim.NormalizedTime >= 1.0f) 
        {
            spAnim.Container = targetSpline;
            //spAnim.Loop = SplineAnimate.LoopMode.Loop;
            spAnim.Restart(true);
            //spAnim.Play();
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        //Debug.Log("Collision Happened");

        if (collider.gameObject.GetComponent<PathNode>() != null) 
        {
            PathNode nextNode = collider.gameObject.GetComponent<PathNode>();

            Debug.Log(nextNode.GetExit());

            // If the node belongs to the same set of splines
            if (nextNode.gameObject.transform.parent.Equals(targetSpline.gameObject.transform.parent)) 
            {

                targetSpline = nextNode.GetExit();
            }
        }
    }

    //private void SetNextSpline(SplineContainer SplineCon) 
    //{
    //    targetSpline = SplineCon;
    //}
}
