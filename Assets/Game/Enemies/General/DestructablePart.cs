/*  Jakob Jaeger
 *  09/26/2024
 *  Script containing the abstract class that will be used by all damagable parts.
 *  Base script written Allan, but copied into Unity by me. (I did all the REAL work).
*/

using System.Collections;
using System.Collections.Generic;
using GeneralUtility.CombatSystem;
using UnityEngine;

abstract public class DestructablePart : MonoBehaviour, IDamageable
{
    // Tracks the local health of the part that was hit, and then applies damage 
    [SerializeField]
    protected int localHealth;

    // Need a more complex fomrula for resistance / armor to account for different weapons / ammo types
    // especially since some might have armor / resistance penetrating ability.
    //[SerializeField]
    //protected float resistance;   // multiplier reduces damage taken locally

    [Tooltip("Value in percentage resistance increase, where 100 = 100%.")]
    [SerializeField] protected float localResistance;    // multiplier reduces damage taken by total health
    [Tooltip("Value in percentage resistance increase, where 100 = 100%.")]
    [SerializeField] protected float mainResistance;    // multiplier reduces damage taken by total health

    [SerializeField]
    protected Enemy mainBody;

    public int MaxHealth => localHealth;

    public bool ApplyDamage(CombatPacket p)
    {
        if (localHealth <= 0)
        {
            var mainPacket = new CombatPacket(p);
            mainPacket.SetTarget(mainBody, this);
            mainPacket.AddResistance(mainResistance, this);
            mainBody.ApplyDamage(mainPacket);

            return false;   //Tells whether I did damage to this part or not
        }

        p.AddResistance(localResistance, this);
        var finalDamage = CombatManager.DamageCalculator(p);                    //Calculate the actual damage

        localHealth -= finalDamage; //Apply that damage locally

        //Debug.Log(finalDamage + " local damage taken.");

        if (localHealth <= 0)
            TriggerSpecialDebuff();

        var mainBodyPacket = new CombatPacket(p);
        mainBodyPacket.SetTarget(mainBody, this);
        //Add res here? unsure, depends on the part ig
        mainBody.ApplyDamage(mainBodyPacket);

        return true;    //eventually could switch to: return mainBody.ApplyDamage(p); if we just want to know that it applied damage to the main body
    }

    abstract public void TriggerSpecialDebuff();
}

////bullet takes care of triggering damage

// Example class
//public class WingPart : DestructablePart
//{
//    public override void TriggerSpecialDebuff()
//    {
//        //mainBody.speed--;
//        //trigger VFX
//        //enable/disable model
//        Destroy(this);
//    }
//}
