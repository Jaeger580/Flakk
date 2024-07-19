using GeneralUtility;
using GeneralUtility.VariableObject;
using JO.AI;
using UnityEngine;

public class PROTO_DamageRedirect : MonoBehaviour, IDamagable
{
    [SerializeField] protected GameObject actualHealthHolder;
    protected IDamagable actualHealthComponent;

    virtual protected void Start()
    {
        if (actualHealthHolder != null) actualHealthHolder.TryGetComponent(out actualHealthComponent);
    }

    virtual public void TakeDamage(int _damage)
    {
        if (actualHealthComponent == null) { Editor_Utility.ThrowWarning("ERR: No health component found to redirect to.",this); return; }
        Debug.Log($"{gameObject.name} got hit, redirecting to {actualHealthHolder.name}");
        actualHealthComponent.TakeDamage(_damage);
    }
}