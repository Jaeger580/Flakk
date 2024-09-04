using UnityEngine;
using UnityEngine.UI;

public class OverheatUI : MonoBehaviour
{
    [SerializeField] private Image overheatBar;
    //[SerializeField] private GunControl gunControl;

    private void Start()
    {
        //gunControl.HeatChangeEvent += UpdateOverheatBar;
    }

    private void UpdateOverheatBar(float newHeat)
    {
        //overheatBar.fillAmount = (float)(newHeat / gunControl.MaxHeat);
    }
}