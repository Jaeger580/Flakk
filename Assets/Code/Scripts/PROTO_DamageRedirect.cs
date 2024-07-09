using JO.AI;
using UnityEngine;

public class PROTO_DamageRedirect : MonoBehaviour, IDamagable
{
    [SerializeField] private Light_Infantry actualHealthComponent;

    public void TakeDamage(float _damage)
    {
        Debug.Log($"{gameObject.name} got hit, redirecting to {actualHealthComponent.gameObject.name}");
        actualHealthComponent.TakeDamage(_damage);
    }
}