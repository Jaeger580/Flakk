using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BomberController : Enemy
{
    [SerializeField]
    private int hitsToStun = 3;
    private int hitCount = 0;

    private bool burstReady = false;

    MaterialPropertyBlock propertyBlock;

    [SerializeField]
    MeshRenderer meshRenderer;
    [SerializeField]
    MeshRenderer meshRendererTwo;
    [SerializeField]
    MeshRenderer meshRendererThree;

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
    }

    private void OnTriggerEnter(Collider collider)
    {
        // if we collide with this node, get ready for a sweep attack.
        if (collider.gameObject.GetComponent<BomberNode>() != null)
        {
            propertyBlock.SetColor("_Color_Main", Color.yellow);
            propertyBlock.SetColor("_Color_Shift", Color.yellow);
            meshRenderer.SetPropertyBlock(propertyBlock);
            meshRendererTwo.SetPropertyBlock(propertyBlock);
            meshRendererThree.SetPropertyBlock(propertyBlock);

            StartSweep();
        }
    }

    protected override void FixedUpdate()
    {
        if (canShoot) 
        {
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, attackRadius, -transform.up, out hit, attackRange))
            {
                if (hit.transform.gameObject.layer == targetLayer)
                {
                    // If burst ready, shoot as normal
                    if (burstReady) 
                    {
                        propertyBlock.SetColor("_Color_Main", Color.red);
                        propertyBlock.SetColor("_Color_Shift", Color.red);
                        meshRenderer.SetPropertyBlock(propertyBlock);
                        meshRendererTwo.SetPropertyBlock(propertyBlock);
                        meshRendererThree.SetPropertyBlock(propertyBlock);

                        Attack(hit, gunsList);
                        fireRateTimer = 0;
                        canShoot = false;
                        burstReady = false;
                    }
                }
            }
        }
        
    }

    protected override void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;

        Debug.DrawLine(transform.position, transform.position + -transform.up * attackRange);
        Gizmos.DrawWireSphere(transform.position + -transform.up * attackRange, attackRadius);
    }

    // sets a boolean to tell the enemy to attack the next chance they get. Resets counter of the number of hits taken before sweep is cancelled.
    private void StartSweep() 
    {
        //fireRateTimer = 0;
        //canShoot = false;
        burstReady = true;

        //propertyBlock.SetColor("_Color_Main", Color.red);
        //propertyBlock.SetColor("_Color_Shift", Color.red);
        //meshRenderer.SetPropertyBlock(propertyBlock);
        //meshRendererTwo.SetPropertyBlock(propertyBlock);
        //meshRendererThree.SetPropertyBlock(propertyBlock);

        hitCount = 0;
    }

    protected override void OnHit()
    {
        if (burstReady) 
        {
            hitCount++;

            if (hitCount >= hitsToStun)
            {
                burstReady = false;
                propertyBlock.SetColor("_Color_Main", Color.red);
                propertyBlock.SetColor("_Color_Shift", Color.red);
                meshRenderer.SetPropertyBlock(propertyBlock);
                meshRendererTwo.SetPropertyBlock(propertyBlock);
                meshRendererThree.SetPropertyBlock(propertyBlock);
            }
        }
    }

    // 
    public bool Sweeping { get { return burstReady; } }
}