using UnityEngine;

public class TargetPractice : MonoBehaviour, IDamagable
{
    [SerializeField] private Renderer rend;
    [SerializeField] private Material hitMat;

    public void TakeDamage(int _damage)
    {
        rend.material = hitMat;
        //Debug.Log($"{name} (parent: {transform.parent.name}) was hit!");
    }
}
