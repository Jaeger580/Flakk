using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float speed;

    private int finaleDamage = 0;

    private void Start()
    {
        Destroy(gameObject, 5f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Translate(Vector3.up * Time.deltaTime * speed);
    }

    // Gun will call this method during instantiation. Will get the base damage from the gun.
    public void setDamage(int damage) 
    {
        finaleDamage = damage;
    }
}
