using System.Collections.Generic;
using GeneralUtility.CombatSystem;
using GeneralUtility.VariableObject;
using UnityEngine;

public class BaseGunHighExplosiveAmmo : BaseGunAmmo, IEffect
{
    [SerializeField] private FloatReference explosionRadius;

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
        //Check for enemies
        var newPos = p.HitCollider.ClosestPointOnBounds(transform.position);
        Collider[] affectedColliders = Physics.OverlapSphere(newPos, explosionRadius.Value, affectableMask);
        List<GameObject> affectedObjs = new();

        foreach (var c in affectedColliders)
        {//For each collider, remove duplicate object hits (such as objects with multiple colliders)
            if (affectedObjs.Contains(c.gameObject)) continue;
            affectedObjs.Add(c.gameObject);
        }

        for (int i = 0; i < affectedObjs.Count; i++)
        {//For each affected object,
            if (affectedObjs[i].TryGetComponent<IDamageable>(out var d))
            {//If that object implements the IDamageable interface
                if (d is DestructablePart part)
                {//If it's an enemy,
                    CombatPacket explosivePacket = new(p);
                    explosivePacket.SetTarget(part, this);
                    explosivePacket.SetHitCollider(part.GetComponent<Collider>(), this);

                    TriggerEffect(explosivePacket);
                }
                else
                {//Otherwise it's not an enemy, just get any collider
                    CombatPacket explosivePacket = new(p);
                    explosivePacket.SetTarget(d, this);
                    explosivePacket.SetHitCollider(affectedObjs[i].GetComponent<Collider>(), this);
                }
            }
        }

        Destroy(this.gameObject);
        //if (triggered)
        //{

        //}
        //ImpactFeedback();
        //WaitForEffectEnd();
        //if (triggered) return;  //Temp for avoiding the annoying warning that triggered isn't used
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