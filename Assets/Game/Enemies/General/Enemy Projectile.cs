using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyProjectile : MonoBehaviour
{
    [SerializeField]
    protected int damage;

    [SerializeField]
    protected float travelSpeed;


    protected int targetLayer;
    protected virtual void Start() 
    {
        Destroy(gameObject, 50f);
        targetLayer = LayerMask.NameToLayer("Weakpoint (Player)");
    }

    protected virtual void OnTriggerEnter(Collider other) 
    {
        if (other.transform.gameObject.layer == targetLayer) 
        {
            //Debug.Log(damage + " damage dealt to weakpoint");
            other.gameObject.GetComponentInParent<MothershipHealth>().TakeDamage(damage);
            Destroy(this.gameObject);
        }
    }

    protected virtual void FixedUpdate() 
    {
        transform.position += transform.forward * travelSpeed * Time.deltaTime;
    }
}