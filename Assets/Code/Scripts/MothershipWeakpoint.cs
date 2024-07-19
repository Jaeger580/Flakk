using GeneralUtility.EditorQoL;
using UnityEngine;

public class MothershipWeakpoint : PROTO_DamageRedirect
{
    [SerializeField] private int maxWeakpointHealth = 50;
    [ReadOnly] [SerializeField] private int currentHealth;

    protected override void Start()
    {
        base.Start();
        currentHealth = maxWeakpointHealth;
    }

    override public void TakeDamage(int _damage)
    {
        base.TakeDamage(_damage);
        currentHealth -= _damage;

        if (currentHealth > 0) return;

        currentHealth = 0;
        Debug.Log("They just hit the nth tower!");
        Destroy(gameObject);
    }
}
