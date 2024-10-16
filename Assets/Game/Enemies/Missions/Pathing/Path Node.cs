using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class PathNode : MonoBehaviour
{
    [SerializeField]
    private GameObject[] Inputs;

    [SerializeField]
    private GameObject[] Outputs;   // Primary varible that will be used. Inputs are for specific cases such as refersed movement.

    // Will likely need to add some sort of "Key" argument for choosing paths in the future.
    public SplineContainer GetExit() 
    {
        int rand = Random.Range(0, Outputs.Length);

        return Outputs[rand].GetComponent<SplineContainer>();
    }
}
