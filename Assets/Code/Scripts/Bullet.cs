using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float speed;

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(Vector3.up * Time.deltaTime);
    }
}
