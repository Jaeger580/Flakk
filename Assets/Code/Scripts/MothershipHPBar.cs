using GeneralUtility.GameEventSystem;
using GeneralUtility.VariableObject;
using UnityEngine;
using UnityEngine.UI;

public class MothershipHPBar : MonoBehaviour
{
    [SerializeField] private GameEvent healthChangeEvent;
    [SerializeField] private Image healthBar;
    [SerializeField] private IntReference maxHealth, currentHealth;

    private void Start()
    {
        var healthChangeListener = gameObject.AddComponent<GameEventListener>();
        healthChangeListener.Events.Add(healthChangeEvent);
        healthChangeListener.Response = new();
        healthChangeListener.Response.AddListener(() => UpdateHealthBar());
        healthChangeEvent.RegisterListener(healthChangeListener);

        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthBar.fillAmount = (float)currentHealth.Value / maxHealth.Value;
    }
}