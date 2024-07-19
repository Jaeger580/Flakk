using GeneralUtility.EditorQoL;
using GeneralUtility.GameEventSystem;
using GeneralUtility.VariableObject;
using UnityEngine;

public class MothershipHealth : MonoBehaviour, IDamagable
{
    [SerializeField] private IntReference maxHealth, currentHealth;
    [SerializeField] private GameEvent healthChangeEvent;

    private void Start()
    {
        currentHealth.Value = maxHealth.Value;
        healthChangeEvent?.Trigger(currentHealth.Value);
    }

    public void TakeDamage(int _damage)
    {
        currentHealth.Value -= _damage;
        if (currentHealth.Value <= 0) currentHealth.Value = 0;

        healthChangeEvent?.Trigger(currentHealth.Value);

        if (currentHealth.Value <= 0) Debug.Log("You lose!");
        else Debug.Log($"Mothership now at {currentHealth.Value} health.");
    }
}
