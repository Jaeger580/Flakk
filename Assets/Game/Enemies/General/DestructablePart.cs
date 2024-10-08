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

    [SerializeField]
    protected float armorPercent;    // multiplier reduces damage taken by total health

    [SerializeField]
    protected Enemy mainBody;

    public int MaxHealth => localHealth;

    public bool ApplyDamage(CombatPacket p)
    {
        if (localHealth <= 0) return false;     //Provides feedback for whether it dealt any damage or not

        p.AddToActiveModifiers(new DamageMod_PostMitigationMultiplier(armorPercent), this);  //Add armor to the list of considerations
        var finalDamage = CombatManager.DamageCalculator(p);                    //Calculate the actual damage

        // Makes sure enemies armor can completely nullify damage. Not sure how these will skill with different enemies vs new player weapons.
        if (finalDamage <= 0)
            finalDamage = 1;

        localHealth -= finalDamage; //Apply that damage locally

        Debug.Log(finalDamage + " local damage taken.");

        if (localHealth <= 0)
            TriggerSpecialDebuff();
        
        mainBody.ApplyDamage(p);

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
