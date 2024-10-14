using GeneralUtility.GameEventSystem;
using GeneralUtility.VariableObject;
using UnityEngine;
using UnityEngine.UIElements;

public class CurrencyManager : MonoBehaviour, IUIScreenRefresh
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

        var currencyChangedListener = gameObject.AddComponent<GameEventListener>();
        currencyChangedListener.Events.Add(currencyChangedEvent);
        currencyChangedListener.Response = new();
        currencyChangedListener.Response.AddListener(() => RefreshUI());
        currencyChangedEvent.RegisterListener(currencyChangedListener);
    }

    private void GainCurrency()
    {
        currency.Value += gainPerLevel;
        RefreshUI();
    }

    [ContextMenu("+1 Currency")]
    private void IncrementCurrency()
    {
        currency.Value += 1;
        RefreshUI();
    }

    [ContextMenu("+5 Currency")]
    private void AddFiveCurrency()
    {
        currency.Value += 5;
        RefreshUI();
    }

    [ContextMenu("Reset Currency")]
    private void ResetCurrency()
    {
        currency.Value = 0;
        RefreshUI();
    }

    public void RefreshUI()
    {

        if(!TryGetComponent<UIDocument>(out var UIDoc)) 
        {
            return;
        }

        var root = UIDoc.rootVisualElement;

        if (root == null)
            return;

        var currencyText = root.Q<Label>($"CurrencyText");

        if (currencyText == null)
            return;

        currencyText.text = $"${currency.Value}";
    }
}