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
        //print("Impacting.");
        StartCoroutine(DestroySelf());

        //if (!TriggerEffect(p)) return;

        var vfx = Instantiate(vfxPrefab, transform.position, Quaternion.identity);
        vfx.transform.parent = null;
        vfx.SetActive(true);
        Destroy(vfx, 0.25f);

        //Check for enemies
        //var newPos = p.HitCollider.ClosestPointOnBounds(transform.position);
        Collider[] affectedColliders = Physics.OverlapSphere(transform.position, explosionRadius.Value, affectableMask);
        List<Enemy> affectedEnemies = new();
        List<DestructablePart> damagedParts = new();

        foreach (var c in affectedColliders)
        {//For each collider, remove duplicate object hits (such as objects with multiple colliders)
            if (!c.TryGetComponent<DestructablePart>(out var d)) continue;
            if (affectedEnemies.Contains(d.MainBody)) continue;

            affectedEnemies.Add(d.MainBody);
            damagedParts.Add(d);
        }

        for (int i = 0; i < damagedParts.Count; i++)
        {
            CombatPacket explosivePacket = new(p);
            explosivePacket.SetTarget(damagedParts[i], this);
            explosivePacket.SetHitCollider(damagedParts[i].GetComponent<Collider>(), this);

            TriggerEffect(explosivePacket);
        }

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