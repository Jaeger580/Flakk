using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyProjectile : MonoBehaviour
{
    [SerializeField]
    protected int damage;

    [SerializeField]
    protected float travelSpeed;

    [SerializeField]
    protected MeshRenderer bulletGraphic;
    [SerializeField]
    protected GameObject[] sonicVFX;
    [SerializeField]
    protected GameObject flash;

    protected bool hasStopped = false;
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
            other.gameObject.GetComponentInParent<MothershipHealth>().ApplyDamage(damage);

            //flash.Clear();
            //flash.Play();

            flash.SetActive(true);
            flash.GetComponent<ParticleSystem>().Clear();
            flash.GetComponent<ParticleSystem>().Play();

            //Destroy(this.gameObject);
            StartCoroutine(DestroySelf());
        }
    }

    protected virtual void FixedUpdate() 
    {
        if (!hasStopped)
        {
            transform.position += transform.forward * travelSpeed * Time.deltaTime;
        }
    }

    protected private IEnumerator DestroySelf()
    {
        
        this.gameObject.GetComponent<Collider>().enabled = false;
        bulletGraphic.enabled = false;
        //Rigidbody rb = this.gameObject.GetComponent<Rigidbody>();
        //rb.velocity = Vector3.zero;
        //rb.isKinematic = true;

        hasStopped = true;

        for(int i = 0; i < sonicVFX.Length; i++) 
        {
            sonicVFX[i].GetComponent<ParticleSystem>().Stop();
        }

        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }
}