using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Missile : MonoBehaviour
{
    public float damageOutput;
    public float speed;
    public float lifeTime;
    private Rigidbody rb;
    private Transform target;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        if (target)
        {
            //transform.LookAt(target.position);
            rb.AddForce(transform.forward * speed * Time.deltaTime, ForceMode.Force);
        }
    }

    public void Fire(Transform _target)
    {
        target = _target;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<IDamagable>() != null)
        {
            other.GetComponent<IDamagable>().TakeDamage(damageOutput);
        }

        Destroy(gameObject);
    }
}
