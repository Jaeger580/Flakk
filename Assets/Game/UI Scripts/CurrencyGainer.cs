using GeneralUtility.GameEventSystem;
using GeneralUtility.VariableObject;
using UnityEngine;

public class CurrencyGainer : MonoBehaviour
{
    [SerializeField] private GameEvent levelEndEvent, currencyChangedEvent;
    [SerializeField] private IntReference currency;

    private void OnDisable()
    {
        ResetCurrency();
    }

    public void GainCurrency(int val)
    {
        currency.Value += val;
        currencyChangedEvent?.Trigger();
    }

    [ContextMenu("+1 Currency")]
    private void IncrementCurrency()
    {
        currency.Value += 1;
        currencyChangedEvent?.Trigger();
    }

    [ContextMenu("+5 Currency")]
    private void AddFiveCurrency()
    {
        currency.Value += 5;
        currencyChangedEvent?.Trigger();
    }

    [ContextMenu("Reset Currency")]
    private void ResetCurrency()
    {
        currency.Value = 0;
        currencyChangedEvent?.Trigger();
    }
}
