using GeneralUtility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AmmoCounterUI : MonoBehaviour
{
    [SerializeField] private TMP_Text primaryMagAmmo, secondaryMagAmmo, primaryStockpileAmmo, secondaryStockpileAmmo;
    [SerializeField] private Image primaryMagBar, secondaryMagBar, primaryStockpileBar, secondaryStockpileBar;
    [SerializeField] private Image reloadBar;
    [SerializeField] private RectTransform primaryCanvas, secondaryCanvas;

    private void Awake()
    {
        if (!TryGetComponent(out GunType gun)) { Editor_Utility.ThrowWarning("ERR: NO GUN TYPE FOUND.", this); return; }

        gun.PrimaryMagAmmoChangeEvent += (newAmmo, maxAmmo) => HandleAmmoChange(newAmmo, maxAmmo, primaryMagAmmo, primaryMagBar);
        gun.SecondaryMagAmmoChangeEvent += (newAmmo, maxAmmo) => HandleAmmoChange(newAmmo, maxAmmo, secondaryMagAmmo, secondaryMagBar);
        gun.PrimaryStockpileAmmoChangeEvent += (newAmmo, maxAmmo) => HandleAmmoChange(newAmmo, maxAmmo, primaryStockpileAmmo, primaryStockpileBar);
        gun.SecondaryStockpileAmmoChangeEvent += (newAmmo, maxAmmo) => HandleAmmoChange(newAmmo, maxAmmo, secondaryStockpileAmmo, secondaryStockpileBar);
        gun.ReloadTimerChangeEvent += (newReloadTimer, maxReloadTimer) => HandleReloadTimerChange(newReloadTimer, maxReloadTimer, reloadBar);

        gun.MagSwapEvent += (primary) => HandleMagSwap(primary);
    }

    private void HandleMagSwap(bool primary)
    {
        if (primary)
        {
            primaryCanvas.localScale = Vector3.one;
            secondaryCanvas.localScale = Vector3.one * 0.85f;
        }
        else
        {
            secondaryCanvas.localScale = Vector3.one;
            primaryCanvas.localScale = Vector3.one * 0.85f;
        }
    }

    private void HandleAmmoChange(float newAmmo, float maxAmmo, TMP_Text ammoElement, Image ammoBar)
    {
        ammoElement.text = $"{newAmmo}/{maxAmmo}";
        if(ammoBar != null)
            ammoBar.fillAmount = newAmmo / maxAmmo;
    }

    private void HandleReloadTimerChange(float newReloadTimer, float maxReloadTimer, Image reloadBar)
    {
        if (reloadBar != null)
            reloadBar.fillAmount = newReloadTimer / maxReloadTimer;
    }
}