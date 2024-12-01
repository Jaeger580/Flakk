using GeneralUtility.CombatSystem;
using GeneralUtility.VariableObject;
using UnityEngine;

public class RailGunActiveUraniumAmmo : RailGunAmmo, IPersistentEffect
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
        //trigger effect event
    }

    public override void OnImpact(CombatPacket p)
    {
        if (!TriggerEffect(p)) return;

        //do vfx/sfx and other cleanup IF it lands correctly
    }

    public void OnKill()
    {
        //trigger kill event
    }

    public bool TriggerEffect(CombatPacket p)
    {
        bool triggered = false;
        if (p.Target is not DestructablePart d) return triggered;
        var fires = d.GetComponents<DoT_Uranium>();
        DoT_Uranium newUranium = null;

        for (int i = 0; i < fires.Length; i++)
        {
            if (fires[i].Owner is IPersistentEffect == this)
            {
                newUranium = fires[i];
            }
        }

        if (newUranium == null)
        {
            newUranium = d.gameObject.AddComponent<DoT_Uranium>();
            newUranium.RegisterOwner(this);
        }

        triggered = newUranium.ApplyEffect();

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