using GeneralUtility.GameEventSystem;
using GeneralUtility.VariableObject;
using UnityEngine;
using UnityEngine.UIElements;

public class CurrencyManager : MonoBehaviour, IUIScreenRefresh
{
    [SerializeField] private GameEvent currencyChangedEvent;
    [SerializeField] private IntReference currency;

    private void Start()
    {
        var currencyChangedListener = gameObject.AddComponent<GameEventListener>();
        currencyChangedListener.Events.Add(currencyChangedEvent);
        currencyChangedListener.Response = new();
        currencyChangedListener.Response.AddListener(() => RefreshUI());
        currencyChangedEvent.RegisterListener(currencyChangedListener);
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