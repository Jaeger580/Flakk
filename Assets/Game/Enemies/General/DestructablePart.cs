/*  Jakob Jaeger
 *  09/26/2024
 *  Script containing the abstract class that will be used by all damagable parts.
 *  Base script written Allan, but copied into Unity by me. (I did all the REAL work).
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class DestructablePart : MonoBehaviour, IDamagable
{
    // Tracks the local health of the part that was hit, and then applies damage 
    [SerializeField]
    protected int localHealth;

    // Need a more complex fomrula for resistance / armor to account for different weapons / ammo types
    // especially since some might have armor / resistance penetrating ability.
    //[SerializeField]
    //protected float resistance;   // multiplier reduces damage taken locally

    [SerializeField]
    protected int armor;    // multiplier reduces damage taken by total health

    [SerializeField]
    protected Enemy mainBody;

    public void TakeDamage(int _damage)
    {
        if (localHealth <= 0) return;
        localHealth -= (_damage);

        Debug.Log(_damage + " local damage taken.");

        if (localHealth <= 0)
            TriggerSpecialDebuff();
        
        var finalDamage = _damage - armor;

        // Makes sure enemies armor can completely nullify damage. Not sure how these will skill with different enemies vs new player weapons.
        if (finalDamage <= 0)
            finalDamage = 1;
        mainBody.TakeDamage((finalDamage));
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
