using UnityEngine;

public class ExplosiveBullet : Bullet
{
    private float explosionRadius = 1f;

    public override void SetDamage(int damage)
    {
        finalDamage = damage * 2;
    }

    private void OnTriggerEnter(Collider other)
    {
        var cols = Physics.OverlapSphere(other.ClosestPoint(transform.position), explosionRadius);
        foreach (var col in cols)
        {
            if (col.TryGetComponent(out IDamagable enemyHurtbox))
            {
                enemyHurtbox.TakeDamage(finalDamage);
            }
        }
    }
}
