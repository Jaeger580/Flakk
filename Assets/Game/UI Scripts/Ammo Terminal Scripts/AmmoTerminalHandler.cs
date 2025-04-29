using UnityEngine;
using GeneralUtility.VariableObject;
using UnityEngine.UIElements;
using GeneralUtility.GameEventSystem;

public class AmmoTerminalHandler : MonoBehaviour, IUIScreenRefresh
{
    [SerializeField] private AmmoSet ammoTypes;

    [SerializeField] protected IntReference currentCurrency;
    [SerializeField] protected GameEvent currencyChangedEvent;

    [SerializeField] private VisualTreeAsset ammoReadoutAsset;

    [SerializeField] private AmmoCrateSpawner crateSpawner;

    private VisualElement CreateNewUpgradeReadout(AmmoType ammo)
    {
        var readout = ammoReadoutAsset.CloneTree();

        var name = readout.Q<Label>("AmmoTitle");
        var desc = readout.Q<Label>("AmmoDesc");
        var cost = readout.Q<Label>("AmmoCost");
        var buyBtn = readout.Q<Button>("BuyButton");

        name.text = ammo.ammoName;
        desc.text = ammo.ammoDesc;
        cost.text = $"{ammo.crateCost:000}";

        bool CheckBuyable()
        {
            if (ammo.crateCost > currentCurrency.Value)
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
            currentCurrency.Value -= ammo.crateCost;
            crateSpawner.SpawnCrate(ammo.cratePrefab);
            if (!CheckBuyable()) return;
            buyBtn.SetEnabled(true);
        };

        CheckBuyable();

        return readout;
    }

    public void RefreshInfo()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var ammoContainer = root.Q<VisualElement>($"AmmoContainer");
        ammoContainer.Clear();

        foreach (var ammoType in ammoTypes.items)
        {
            ammoContainer.Add(CreateNewUpgradeReadout(ammoType));
        }
    }

    public void RefreshUI()
    {
        RefreshInfo();
    }

    private void Start()
    {
        RefreshInfo();
    }
}
