using GeneralUtility.CombatSystem;
using GeneralUtility.EditorQoL;
using GeneralUtility.GameEventSystem;
using GeneralUtility.VariableObject;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class MothershipHealth : Damageable<MothershipHealth>
{
    [SerializeField] private IntReference maxHealth, currentHealth;
    [SerializeField] private GameEvent healthChangeEvent, deathEvent, winEvent;
    private bool hasLost = false;

    // Variables for over heat system
    [SerializeField] private IntReference maxHeat, currentHeat, overHeatThresholdPercent;
    [SerializeField] private float currentCoolDown, coolDownDelay, coolDownRate;
    [SerializeField] private float damageRate; // damage per second.
    [SerializeField] private bool isOverHeating;

    [SerializeField]
    private TMP_Text heatText;
    [SerializeField]
    private TMP_Text hullText;

    override public int MaxHealth => maxHealth.Value;

    override public Action OnDamage { get; set; }

    private void Start()
    {
        var winListener = gameObject.AddComponent<GameEventListener>();
        winListener.Events.Add(winEvent);
        winListener.Response = new();
        winListener.Response.AddListener(() => ResetHealth());
        winEvent.RegisterListener(winListener);

        ResetHealth();
        StartCoroutine(HullDamage(damageRate));
        StartCoroutine(CoolDownTracker(coolDownRate));
    }

    private void FixedUpdate()
    {
        // heat values are ints that needed to be calculated as floats
        float currentHeatPercent = ((float)currentHeat.Value / maxHeat.Value) * 100f;

        // if the current heat is greater than a specified percent of the max heat, then we are over heating.
        if ((currentHeatPercent >= overHeatThresholdPercent.Value))
        {
            if (!isOverHeating)
            {
                isOverHeating = true;
                Debug.Log("SHIP OVERHEATING!!!!");
            }
        }
        else if(isOverHeating)
        {
            isOverHeating = false;
        }

        // currentCoolDown is reset whenever the player is hit. If they go long enough without getting hit, they begin to cool down.
        if (currentCoolDown < coolDownDelay)
        {
            currentCoolDown += Time.deltaTime;
        }
    }

    private void ResetHealth()
    {
        StopAllCoroutines();
        hasLost = false;
        currentHealth.Value = maxHealth.Value;
        currentHeat.Value = 0;
        healthChangeEvent?.Trigger();

        // May want to change currentHeatPercent into a private variable since it is used so often.
        float currentHeatPercent = ((float)currentHeat.Value / maxHeat.Value) * 100f;
        heatText.text = "HEAT: " + currentHeatPercent + "%";
        hullText.text = "HULL: " + currentHealth.Value + "%";

    }

    public void ApplyDamage(int _damage)
    {
        CombatPacket p = new();
        p.SetDamage(_damage, this);
        ApplyDamage(p);
    }

    override public bool ApplyDamage(CombatPacket p)
    {
        currentHeat.Value += p.Damage;
        if (currentHeat.Value > maxHeat.Value) currentHeat.Value = maxHeat.Value;

        currentCoolDown = 0f;

        healthChangeEvent?.Trigger();

        //Debug.Log("Current Heat raised to " + currentHeat.Value);
        float currentHeatPercent = ((float)currentHeat.Value / maxHeat.Value) * 100f;

        heatText.text = "HEAT: " + currentHeatPercent + "%";
        return true;
        // Old health system before over heat system:
        //currentHealth.Value -= _damage;
        //if (currentHealth.Value <= 0) currentHealth.Value = 0;

        //healthChangeEvent?.Trigger();

        //if (currentHealth.Value <= 0)
        //{
        //    Debug.Log("You lose!");
        //    deathEvent?.Trigger();
        //}
        //else Debug.Log($"Mothership now at {currentHealth.Value} health.");
    }

    // Co-routine that deals damage to the main health of the mothership if it is currently over heating.
    private IEnumerator HullDamage(float healthLossRate) 
    {
        while (true) 
        {
            if (isOverHeating)
            {
                currentHealth.Value -= 1;

                if (currentHealth.Value <= 0 && !hasLost)
                {
                    Debug.Log("You lose!");
                    deathEvent?.Trigger();
                    hasLost = true;
                }
                else 
                {
                    //Debug.Log($"Mothership now at {currentHealth.Value} health.");
                    //Debug.Log("Hull integrity at " + currentHealth.Value + "%.");
                    hullText.text = "HULL: " + currentHealth.Value + "%";

                }

                yield return new WaitForSeconds(healthLossRate);
            }
            else
            {
                yield return null;
            }
        }
    }

    private IEnumerator CoolDownTracker(float heatCoolDownRate) 
    {
        while (true) 
        {
            // If the cool down delay time has passed, cool down at set rate.
            if (currentCoolDown >= coolDownDelay && currentHeat.Value > 0) 
            {
                currentHeat.Value -= 1;

                float currentHeatPercent = ((float)currentHeat.Value / maxHeat.Value) * 100f;
                heatText.text = "HEAT: " + currentHeatPercent + "%";

                yield return new WaitForSeconds(heatCoolDownRate);
            }
            else
            {
                yield return null;
            }
        }
    }
}
