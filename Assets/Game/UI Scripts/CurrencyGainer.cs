using GeneralUtility.GameEventSystem;
using GeneralUtility.VariableObject;
using UnityEngine;

public class CurrencyGainer : MonoBehaviour
{
    [SerializeField] private GameEvent levelEndEvent, currencyChangedEvent;
    [SerializeField] private IntReference currency;
    [SerializeField] private int gainPerLevel;

    private void Start()
    {
        var levelEndListener = gameObject.AddComponent<GameEventListener>();
        levelEndListener.Events.Add(levelEndEvent);
        levelEndListener.Response = new();
        levelEndListener.Response.AddListener(() => GainCurrency());
        levelEndEvent.RegisterListener(levelEndListener);
    }

    private void OnDisable()
    {
        ResetCurrency();
    }

    private void GainCurrency()
    {
        currency.Value += gainPerLevel;
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
