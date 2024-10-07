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
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

public class SimpleSpline : MonoBehaviour
{
    //[SerializeField]
    //private SplineContainer entrySpline;

    //[SerializeField]
    //private SplineContainer secondSpline;

    private SplineAnimate spAnim;

    [SerializeField]
    private GameObject spContainerObject;
    private SplineContainer splineContainer;

    [SerializeField]
    private PathPoint startPoint;
    private PathPoint nextPoint;

    // Start is called before the first frame update
    void Start()
    {
        spAnim = GetComponent<SplineAnimate>();
        splineContainer = spContainerObject.GetComponent<SplineContainer>();

        nextPoint = startPoint.GetNextTarget();

        var knots = new UnityEngine.Splines.BezierKnot[2];
        knots[0] = new BezierKnot(startPoint.transform.position);
        knots[1] = new BezierKnot(nextPoint.transform.position);

        splineContainer[0].Knots = knots;

        //spAnim.Container = entrySpline;
        spAnim.Container = splineContainer;
        spAnim.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(spAnim.NormalizedTime >= 1.0f) 
        {
            //Create a new spline
            //var sp = new SplineContainer();
            //var spline = splineContainer.AddSpline();

            //Remove old Spline
            //splineContainer.RemoveSpline(splineContainer[0]);
            spAnim.Pause();

            splineContainer[0].Clear();

            //if(nextPoint == null) 
            //{
            //    nextPoint = startPoint.GetNextTarget();
            //}

            startPoint = nextPoint;
            nextPoint = nextPoint.GetNextTarget();

            var knots = new UnityEngine.Splines.BezierKnot[2];
            knots[0] = new BezierKnot(startPoint.transform.position);
            knots[1] = new BezierKnot(nextPoint.transform.position);

            splineContainer[0].Knots = knots;

            //spAnim.Container = splineContainer;

            //spAnim.Container = secondSpline;
            //spAnim.Loop = SplineAnimate.LoopMode.Loop;
            spAnim.Restart(true);
            //spAnim.Play();
        }
    }
}
