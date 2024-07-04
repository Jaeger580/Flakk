using UnityEngine;
using UnityEngine.UI;

public class AmmoUI : MonoBehaviour
{
    [SerializeField] private Image ammoBar;
    [SerializeField] private GunControl gunControl;

    private void Start()
    {
        gunControl.AmmoChangeEvent += UpdateAmmoBar;
    }

    private void UpdateAmmoBar(float newAmmo)
    {
        ammoBar.fillAmount = ((float)newAmmo / gunControl.ClipSize);
    }
}