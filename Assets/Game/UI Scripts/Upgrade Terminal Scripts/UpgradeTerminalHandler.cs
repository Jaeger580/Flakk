﻿using UnityEngine;
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

    const string
        FULLY_BOUGHT_TEXT = "COMPLETE",
        INSUFFICIENT_CURRENCY_TEXT = "NOT ENOUGH CURRENCY";


    // Quick Initilizer to reset the bought status on upgrades when restarting game.
    private void Awake()
    {
        foreach (UpgradeSO upgrade in upgrades.items) 
        {
            upgrade.currentUpgradeIndex = 0;
        }
    }

    private VisualElement CreateNewUpgradeReadout(UpgradeSO upg)
    {
        var readout = upgradeReadoutAsset.CloneTree();

        var name = readout.Q<Label>("UpgradeTitle");
        var desc = readout.Q<Label>("UpgradeDesc");
        var cost = readout.Q<Label>("UpgradeCost");
        var buyBtn = readout.Q<Button>("BuyButton");

        var nextUpgrade = upg.CheckNextUpgrade();

        name.text = nextUpgrade?.upgradeName;
        desc.text = nextUpgrade?.upgradeDesc;
        cost.text = $"{nextUpgrade?.cost:000}";
        upgradeEntries.Add(upg, buyBtn);
        bool CheckBuyable()
        {
            if (nextUpgrade == null)
            {
                buyBtn.SetEnabled(false);
                buyBtn.text = FULLY_BOUGHT_TEXT;
                return false;
            }

            if (nextUpgrade?.cost > currentCurrency.Value)
            {
                buyBtn.SetEnabled(false);
                buyBtn.text = INSUFFICIENT_CURRENCY_TEXT;
                return false;
            }

            buyBtn.SetEnabled(true);
            return true;
        }

        buyBtn.clicked += () =>
        {
            if (upg.BuyUpgrade(currentCurrency)) currencyChangedEvent?.Trigger();

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
            var nextUpg = upg.CheckNextUpgrade();
            if (nextUpg == null)
            {
                btn.SetEnabled(false);
                btn.text = FULLY_BOUGHT_TEXT;
                continue;
            }

            if (nextUpg?.cost > currentCurrency.Value)
            {
                btn.SetEnabled(false);
                btn.text = INSUFFICIENT_CURRENCY_TEXT;
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
