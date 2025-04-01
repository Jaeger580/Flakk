using UnityEngine;
using GeneralUtility.VariableObject;
using UnityEngine.UIElements;
using GeneralUtility.GameEventSystem;
using System.Collections.Generic;

public class UpgradeTerminalHandler : MonoBehaviour, IUIScreenRefresh
{
    [SerializeField] private bool resetOnReload;
    [SerializeField] private UpgradeSet upgrades;

    [SerializeField] protected IntReference currentCurrency;
    [SerializeField] protected GameEvent currencyChangedEvent;

    [SerializeField] private VisualTreeAsset upgradeReadoutAsset;
    private Dictionary<UpgradeSO, Button> upgradeEntries = new();

    // Quick Initilizer to reset the bought status on upgrades when restarting game.
    private void Awake()
    {
        foreach (UpgradeSO upgrade in upgrades.items) 
        {
            upgrade.bought = false;
        }
    }

    private VisualElement CreateNewUpgradeReadout(UpgradeSO upg)
    {
        var readout = upgradeReadoutAsset.CloneTree();

        var name = readout.Q<Label>("UpgradeTitle");
        var desc = readout.Q<Label>("UpgradeDesc");
        var cost = readout.Q<Label>("UpgradeCost");
        var buyBtn = readout.Q<Button>("BuyButton");

        name.text = upg.upgradeName;
        desc.text = upg.upgradeDesc;
        cost.text = $"{upg.cost:000}";
        upgradeEntries.Add(upg, buyBtn);
        bool CheckBuyable()
        {
            if (upg.bought)
            {
                buyBtn.SetEnabled(false);
                buyBtn.text = "Bought";
                return false;
            }

            if (upg.cost > currentCurrency.Value)
            {
                buyBtn.SetEnabled(false);
                buyBtn.text = "Not Enough Currency";
                return false;
            }

            buyBtn.SetEnabled(true);
            return true;
        }

        buyBtn.clicked += () =>
        {
            currentCurrency.Value -= upg.cost;
            currencyChangedEvent?.Trigger();
            upg.IncreaseStat();

            RecheckAllBtns();
            if (!CheckBuyable()) return;
            buyBtn.SetEnabled(true);
        };

        CheckBuyable();

        return readout;
    }

    private void RecheckAllBtns()
    {
        foreach(var (upg, btn) in upgradeEntries)
        {
            if (upg.bought)
            {
                btn.SetEnabled(false);
                btn.text = "Bought";
                continue;
            }

            if (upg.cost > currentCurrency.Value)
            {
                btn.SetEnabled(false);
                btn.text = "Not Enough Currency";
                continue;
            }
            btn.SetEnabled(true);
        }
    }

    public void RefreshInfo()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var upgradeContainer = root.Q<VisualElement>($"UpgradeContainer");
        upgradeContainer.Clear();
        upgradeEntries.Clear();
        foreach (var upg in upgrades.items)
        {
            upgradeContainer.Add(CreateNewUpgradeReadout(upg));
        }
    }

    public void RefreshUI()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        if (root == null) return;
        RefreshInfo();
    }

    private void Start()
    {
        if (resetOnReload)
        {
            ResetStats();
        }

        var currencyChangedListener = gameObject.AddComponent<GameEventListener>();
        currencyChangedListener.Events.Add(currencyChangedEvent);
        currencyChangedListener.Response = new();
        currencyChangedListener.Response.AddListener(() => RefreshUI());
        currencyChangedEvent.RegisterListener(currencyChangedListener);

        RefreshInfo();
    }

    private void OnDisable()
    {
        ResetStats();
    }

    public void ResetStats()
    {
        foreach (var upg in upgrades.items)
        {
            upg.ResetStat();
        }
    }
}
