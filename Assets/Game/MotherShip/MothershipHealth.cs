﻿using GeneralUtility.EditorQoL;
using GeneralUtility.GameEventSystem;
using GeneralUtility.VariableObject;
using UnityEngine;

public class MothershipHealth : MonoBehaviour, IDamagable
{
    [SerializeField] private IntReference maxHealth, currentHealth;
    [SerializeField] private GameEvent healthChangeEvent, deathEvent, winEvent;

    private void Start()
    {
        var winListener = gameObject.AddComponent<GameEventListener>();
        winListener.Events.Add(winEvent);
        winListener.Response = new();
        winListener.Response.AddListener(() => ResetHealth());
        winEvent.RegisterListener(winListener);

        ResetHealth();
    }

    private void ResetHealth()
    {
        currentHealth.Value = maxHealth.Value;
        healthChangeEvent?.Trigger();
    }

    public void TakeDamage(int _damage)
    {
        currentHealth.Value -= _damage;
        if (currentHealth.Value <= 0) currentHealth.Value = 0;

        healthChangeEvent?.Trigger();

        if (currentHealth.Value <= 0)
        {
            Debug.Log("You lose!");
            deathEvent?.Trigger();
        }
        else Debug.Log($"Mothership now at {currentHealth.Value} health.");
    }
}
