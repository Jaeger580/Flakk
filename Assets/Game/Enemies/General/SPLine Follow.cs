/*  Jakob Jaeger
 *  09/06/2024
 *  Custom Script that will move along splines.
 *  Original Layout of this script from git-amend's tutorial on youtube: https://youtu.be/ipKeYqYB4oY?si=-AtATrjU_GfE52_V
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Splines;



public class SPLineFollow : MonoBehaviour
{
    // custom classes for making spline data serializable in editor
    // Note: these classes may be better to define in a different location. Needs Testing.
    [Serializable]
    public class SplinePathData
    {
        public SliceData[] slices;
    }

    [Serializable]
    public class SliceData
    {
        public int splineIndex;
        public SplineRange range;

        // Can store more useful information
        public bool isEnabled = true;
        //public bool loop = false;

        public float sliceLength;
        public float distanceFromStart;
    }

    [SerializeField]
    private SplineContainer splineContainer;
    [SerializeField]
    private float moveSpeed = 0.1f;

    [SerializeField]
    private SplinePathData pathData;

    SplinePath path;

    float progressRatio;
    //float progress;
    float totalLength;

    private void Start()
    {
        path = new SplinePath(CalculatePath());

        StartCoroutine(FollowCoroutine());  
    }

    private List<SplineSlice<Spline>> CalculatePath()
    {
        // Get the container's transform matrix
        var localToWorldMatrix = splineContainer.transform.localToWorldMatrix;

        // Get all the enabled Slices using LINQ
        var enabledSlices = pathData.slices.Where(slice => slice.isEnabled).ToList();

        var slices = new List<SplineSlice<Spline>>();

        totalLength = 0f;

        int num = 0;

        foreach (var sliceData in enabledSlices)
        {
            
            var spline = splineContainer.Splines[sliceData.splineIndex];
            var slice = new SplineSlice<Spline>(spline, sliceData.range, localToWorldMatrix);
            slices.Add(slice);

            // this if check was add to skip the first section of spline. Not very modular and should be replaced with another solution
            // Another solution may be difficult with out full script rewrite
            if(num != 0) 
            {
                // Calculate the slice details
                sliceData.distanceFromStart = totalLength;
                sliceData.sliceLength = slice.GetLength();
                totalLength += sliceData.sliceLength;
                num++;
            }

        }



        return slices;
    }


    // Coroutine that handles the gameobject movement
    IEnumerator FollowCoroutine() 
    {
        // This makes it so the script loops trough the spline after following the spawning spline.
        // Would be better to replace with something like a bool 
        if (pathData.slices[0].isEnabled)
            pathData.slices[0].isEnabled = false;

        for (var n = 0;; ++n)   // This syntax causes the for loop to loop forever.
        {
            progressRatio = 0;

            while (progressRatio <= 1f) 
            {
                // Get the gameobject's posiotion on the path
                var pos = path.EvaluatePosition(progressRatio);
                var direction = path.EvaluateTangent(progressRatio);

                // Currently this sets the object's position and rotation
                transform.position = pos;
                transform.LookAt(pos + direction);
                transform.rotation = Quaternion.LookRotation(path.EvaluateUpVector(progressRatio), -transform.forward);
                transform.Rotate(Vector3.right, 90f);

                // Increment the progress ratio
                progressRatio += moveSpeed * Time.deltaTime;

                // Calculate the current distance travelled
                //progress = progressRatio * totalLength;
                yield return null;
            }


            // Should need to auto enable paths. Seems reduntant when we can decide which are enabled
            //// Enable all paths
            //foreach (var sliceData in pathData.slices) 
            //{
            //    sliceData.isEnabled = true;
            //}

            // Calculate the new path
            path = new SplinePath(CalculatePath());
        }
    }
}
