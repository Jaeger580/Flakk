using GeneralUtility.GameEventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTutorial : EnemyProjectile
{
    [SerializeField]
    private GameEvent tutorialTrigger;

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.transform.gameObject.layer == targetLayer)
        {
            //Debug.Log(damage + " damage dealt to weakpoint");
            other.gameObject.GetComponentInParent<MothershipHealth>().ApplyDamage(damage);

            tutorialTrigger.Trigger();

            Destroy(this.gameObject);
        }
    }
}
