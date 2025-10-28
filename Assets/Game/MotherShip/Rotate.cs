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
        var rotation = thisTransform.rotation;
        thisTransform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotateSpeed * Time.deltaTime);
    }
}
