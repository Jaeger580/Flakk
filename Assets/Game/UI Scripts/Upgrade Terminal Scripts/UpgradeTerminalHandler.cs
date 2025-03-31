using UnityEngine;
using GeneralUtility.VariableObject;
using UnityEngine.UIElements;
using GeneralUtility.GameEventSystem;

public class UpgradeTerminalHandler : MonoBehaviour, IUIScreenRefresh
{
    [SerializeField] private bool resetOnReload;
    [SerializeField] private UpgradeSet upgrades;

    [SerializeField] protected IntReference currentCurrency;
    [SerializeField] protected GameEvent currencyChangedEvent;

    [SerializeField] private VisualTreeAsset upgradeReadoutAsset;

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

        bool CheckBuyable()
        {
            if (upg.bought)
            {
                buyBtn.SetEnabled(false);
                buyBtn.text = "Bought";
                return false ;
            }

            if (upg.cost > currentCurrency.Value)
            {
                buyBtn.SetEnabled(false);
                buyBtn.text = "Not Enough Currency";
                return false ;
            }

            buyBtn.SetEnabled(true);
            return true;
        }

        buyBtn.clicked += () =>
        {
            currentCurrency.Value -= upg.cost;
            upg.IncreaseStat();

            if (!CheckBuyable()) return;
            buyBtn.SetEnabled(true);
        };

        CheckBuyable();

        return readout;
    }

    public void RefreshInfo()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var upgradeContainer = root.Q<VisualElement>($"UpgradeContainer");
        upgradeContainer.Clear();

        foreach (var upg in upgrades.items)
        {
            upgradeContainer.Add(CreateNewUpgradeReadout(upg));
        }
    }

    public void RefreshUI()
    {
        RefreshInfo();
    }

    private void Start()
    {
        if (resetOnReload)
        {
            ResetStats();
        }
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
