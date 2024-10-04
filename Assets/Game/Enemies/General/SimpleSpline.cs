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

    [SerializeField]
    private SplineContainer secondSpline;

    private SplineAnimate spAnim;

    // Start is called before the first frame update
    void Start()
    {
        spAnim = GetComponent<SplineAnimate>();

        spAnim.Container = entrySpline;
        spAnim.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if(spAnim.NormalizedTime >= 1.0f && spAnim.Container == entrySpline) 
        {
            spAnim.Container = secondSpline;
            spAnim.Loop = SplineAnimate.LoopMode.Loop;
            spAnim.Restart(true);
            //spAnim.Play();
        }
    }
}
