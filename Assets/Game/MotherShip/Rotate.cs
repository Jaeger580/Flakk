using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField]
    private float rotateSpeed;

    private Transform thisTransform;

    private void Start()
    {
        thisTransform = this.gameObject.transform;
    }

    private void FixedUpdate()
    {
        //var rotation = thisTransform.rotation;
        //thisTransform.rotation = Quaternion.Euler(rotation.x, rotateSpeed * Time.deltaTime, rotation.z);

        thisTransform.Rotate(new Vector3(0, rotateSpeed, 0) * Time.deltaTime);
    }
}
