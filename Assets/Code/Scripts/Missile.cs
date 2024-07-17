using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Missile : MonoBehaviour
{
    private float damageOutput;
    public float speed;
    public float lifeTime;
    private Rigidbody rb;
    private Transform target;
    public LayerMask ignoreLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        if (target)
        {
            //transform.LookAt(target.position);
            rb.AddForce(transform.forward * speed * Time.deltaTime, ForceMode.Acceleration);
        }
    }

    public void Fire(Transform _target, float _damageOutput)
    {
        target = _target;
        damageOutput = _damageOutput;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == ignoreLayer)
        {
            return;
        }
        
        if (other.GetComponent<IDamagable>() != null)
        {
            other.GetComponent<IDamagable>().TakeDamage(damageOutput);
        }

        Destroy(gameObject);
    }
}
