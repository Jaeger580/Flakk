using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bomb : MonoBehaviour
{
    private int damageOutput;
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
            transform.LookAt(target.position);
            rb.AddForce(speed * Time.deltaTime * transform.forward, ForceMode.Acceleration);
        }
    }

    public void Fire(Transform _target, int _damageOutput)
    {
        target = _target;
        damageOutput = _damageOutput;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((ignoreLayer.value & 1 << other.gameObject.layer) > 0)
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