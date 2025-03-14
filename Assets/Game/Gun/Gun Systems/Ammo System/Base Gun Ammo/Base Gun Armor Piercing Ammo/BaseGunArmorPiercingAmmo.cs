using GeneralUtility.CombatSystem;
using GeneralUtility.VariableObject;
using UnityEngine;

public class BaseGunArmorPiercingAmmo : BaseGunAmmo, IEffect
{
    [SerializeField] private FloatReference armorPierceValue;

    public float GetEffectValue()
    {
        return effectValue;
    }

    public void OnEffect(int damage)
    {
        //trigger effect event
    }

    public override void OnImpact(CombatPacket p)
    {
        if (!TriggerEffect(p)) return;

        Destroy(this.gameObject);
        //do vfx/sfx and other cleanup IF it lands correctly
    }

    public void OnKill()
    {
        //trigger kill event
    }

    public bool TriggerEffect(CombatPacket p)
    {
        if (p.Target is not DestructablePart d) return false;

        p.SetDamage(Mathf.CeilToInt(effectValue), this);
        return d.ApplyDamage(p);
    }
}

public class BaseGunArmorShreddingAmmo : BaseGunAmmo, IEffect
{
    [SerializeField] private FloatReference armorShredValue;

    public float GetEffectValue()
    {
        return effectValue;
    }

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
        if (p.Target is not DestructablePart d) return false;

        p.AddResistance(armorShredValue.Value, this);

        p.SetDamage(Mathf.CeilToInt(effectValue), this);
        return d.ApplyDamage(p);
    }
}