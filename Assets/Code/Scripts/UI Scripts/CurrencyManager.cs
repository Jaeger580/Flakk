using GeneralUtility.GameEventSystem;
using GeneralUtility.VariableObject;
using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    [SerializeField] private GameEvent levelEndEvent;
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

    private void GainCurrency()
    {
        currency.Value += gainPerLevel;
    }

    [ContextMenu("+1 Currency")]
    private void IncrementCurrency()
    {
        currency.Value += 1;
    }

    [ContextMenu("+5 Currency")]
    private void AddFiveCurrency()
    {
        currency.Value += 5;
    }

    [ContextMenu("Reset Currency")]
    private void ResetCurrency()
    {
        currency.Value = 0;
    }
}