using System.Collections;
using System.Collections.Generic;
using GeneralUtility.VariableObject;
using UnityEditor;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private FloatReference speed;

    private int finalDamage = 0;

    private void Start()
    {
        Destroy(gameObject, 5f);
    }

    private void FixedUpdate()
    {
        transform.Translate(speed.Value * Time.deltaTime * Vector3.up);
    }

    // Gun will call this method during instantiation. Will get the base damage from the gun.
    public void SetDamage(int damage) 
    {
        finalDamage = damage;
    }
}
