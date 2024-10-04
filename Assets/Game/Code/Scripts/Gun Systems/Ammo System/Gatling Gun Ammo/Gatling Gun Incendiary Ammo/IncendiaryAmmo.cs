using GeneralUtility.CombatSystem;
using GeneralUtility.VariableObject;
using UnityEngine;

public class IncendiaryAmmo : BaseGunAmmo, IPersistentEffect
{
    public AmmoType ammoVals;
    [SerializeField] private FloatReference tickRate, timeBeforeExpiration;
    [SerializeField] private IntReference maxNumTicks, maxStacks;
    [SerializeField] private BoolReference refreshable, stackable;

    public float GetEffectValue() => ammoVals.damage.Value;
    public float GetTickRate() => tickRate.Value;
    public float GetTimeBeforeExpiration() => timeBeforeExpiration.Value;
    public int GetMaxNumTicks() => maxNumTicks.Value;
    public int GetMaxStacks() => maxStacks.Value;
    public bool GetRefreshable() => refreshable.Value;
    public bool GetStackable() => stackable.Value;

    public void OnEffect(int damage)
    {
        //Trigger event for incendiary shot landing
    }

    public override void OnImpact(CombatPacket p)
    {
        //Gets called when the hit is confirmed
        //For projectiles, this gets called in an OnCollisionEnter or OnTriggerEnter
        //For hitscan, this gets called wherever the raycast hit is confirmed

        //Trigger the effect
        TriggerEffect(p);

        //Handle all the cleanup, like SFX/VFX and other shit
    }

    public void OnKill()
    {
        //trigger incendiary kill event
    }

    public bool TriggerEffect(CombatPacket p)
    {
        bool triggered = false;
        if (p.Target is not DestructablePart d) return triggered;
        var fires = d.GetComponents<DoT_Fire>();
        DoT_Fire newFire = null;

        for (int i = 0; i < fires.Length; i++)
        {
            if (fires[i].Owner is IPersistentEffect == this)
            {
                newFire = fires[i];
            }
        }

        if (newFire == null)
        {
            newFire = d.gameObject.AddComponent<DoT_Fire>();
            newFire.RegisterOwner(this);
        }

        triggered = newFire.ApplyEffect();

        //if (!t.TryGetComponent(out DEBUFF_Weaken weaken))
        //{
        //    var newW = t.gameObject.AddComponent<DEBUFF_Weaken>();
        //    newW.DamageModifier = weakenMultiplier;
        //    newW.RegisterOwner(this);
        //    triggered = newW.ApplyEffect();
        //}
        //else
        //{
        //    triggered = weaken.ApplyEffect();
        //}
        return triggered;
    }
}