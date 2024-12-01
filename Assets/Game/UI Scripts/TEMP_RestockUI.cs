using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TEMP_RestockUI : MonoBehaviour
{
    [SerializeField] private Image ammoBar;
    [SerializeField] private TMP_Text ammoCrateText, stockpileText;
    [SerializeField] private TEMP_RestockTerminal restockTerminal;

    private void Start()
    {
        restockTerminal.AmmoStockedEvent += UpdateAmmoText;
        restockTerminal.LoadingBarChangedEvent += UpdateAmmoBar;
    }

    private void UpdateAmmoText(float newCrateAmt, float newStockpileAmt)
    {
        ammoBar.fillAmount = ((float)newStockpileAmt / restockTerminal.MaxAmmo);
        ammoCrateText.text = $"{(int)newCrateAmt}";
        stockpileText.text = $"{(int)newStockpileAmt}";
    }

    private void UpdateAmmoBar(float newLoadingPercent)
    {
        ammoBar.fillAmount = newLoadingPercent;
    }
}
