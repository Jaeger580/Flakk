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

    private float targetHP;

    //private IEnumerator Start()
    //{
    //    var healthChangeListener = gameObject.AddComponent<GameEventListener>();
    //    healthChangeListener.Events.Add(healthChangeEvent);
    //    healthChangeListener.Response = new();
    //    healthChangeListener.Response.AddListener(() => UpdateHealthBar());
    //    healthChangeEvent.RegisterListener(healthChangeListener);

    //    yield return new WaitForSeconds(0.01f);

    //    UpdateHealthBar();
    //}
    private void Update()
    {
        //UpdateHealthBar((float)currentHealth.Value / (float)maxHealth.Value);
        targetHP = (float)currentHealth.Value / (float)maxHealth.Value;
        //healthBarOne.fillAmount = targetHealth;
        //healthBarTwo.fillAmount = targetHealth;

        //float barOneValue = Mathf.MoveTowards(healthBarOne.fillAmount, targetHP, 0.01f * Time.deltaTime);
        //float barTwoValue = Mathf.MoveTowards(healthBarTwo.fillAmount, targetHP, 0.01f * Time.deltaTime);

        healthBarOne.fillAmount = targetHP;
        healthBarTwo.fillAmount = targetHP;
    }

    private void UpdateHealthBar(float targetHealth)
    {
        targetHP = targetHealth;
        //healthBarOne.fillAmount = targetHealth;
        //healthBarTwo.fillAmount = targetHealth;

        float barOneValue = Mathf.MoveTowards(healthBarOne.fillAmount, targetHP, 1 * Time.deltaTime);
        float barTwoValue = Mathf.MoveTowards(healthBarTwo.fillAmount, targetHP, 1 * Time.deltaTime);

        healthBarOne.fillAmount = barOneValue;
        healthBarTwo.fillAmount = barTwoValue;
    }
}