using System.Collections;
using GeneralUtility.GameEventSystem;
using GeneralUtility.VariableObject;
using UnityEngine;
using UnityEngine.UI;

public class MothershipHPBar : MonoBehaviour
{
    [SerializeField] private GameEvent healthChangeEvent;
    [SerializeField] private Image healthBarOne, healthBarTwo;
    [SerializeField] private IntReference maxHealth, currentHealth;

    private IEnumerator Start()
    {
        var healthChangeListener = gameObject.AddComponent<GameEventListener>();
        healthChangeListener.Events.Add(healthChangeEvent);
        healthChangeListener.Response = new();
        healthChangeListener.Response.AddListener(() => UpdateHealthBar());
        healthChangeEvent.RegisterListener(healthChangeListener);

        yield return new WaitForSeconds(0.01f);

        UpdateHealthBar();
    }

    private void UpdateHealthBar()
    {
        healthBarOne.fillAmount = (float)currentHealth.Value / maxHealth.Value;
        healthBarTwo.fillAmount = (float)currentHealth.Value / maxHealth.Value;
    }
}