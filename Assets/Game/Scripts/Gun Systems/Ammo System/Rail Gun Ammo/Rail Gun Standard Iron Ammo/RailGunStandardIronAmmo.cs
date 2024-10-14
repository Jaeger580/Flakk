using GeneralUtility.CombatSystem;
using GeneralUtility.VariableObject;
using UnityEngine;

public class RailGunStandardIronAmmo : RailGunAmmo, IEffect
{
    [Tooltip("Value as a percent, where 100 = 100%.")]
    [SerializeField] private FloatReference hullMultiplier;
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

        //trigger feedback, like VFX/SFX, likely at the point of impact/contact
    }

    public void OnKill()
    {
        //trigger kill event
    }

    public bool TriggerEffect(CombatPacket p)
    {
        if (p.Target is not DestructablePart d) return false;
        if (p.Target is Hull)
        {//If it hit the hull, make sure you ignore any resistance and specifically add a multiplier
            p.SetIgnoreMainResistance(true, this);
            p.AddResistance(-hullMultiplier.Value, this);
        }

        p.SetDamage(Mathf.CeilToInt(effectValue), this);
        return d.ApplyDamage(p);
    }
}