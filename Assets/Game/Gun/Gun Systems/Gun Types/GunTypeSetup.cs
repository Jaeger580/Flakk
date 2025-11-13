using GeneralUtility.GameEventSystem;
using UnityEngine;

public class GunTypeSetup : MonoBehaviour
{
    [Header("References")]
    public GameObject gunCamera;
    public GameObject gunBase, gunBulletPoint;

    [Header("Game Events")]
    public GameEvent inputEvPriFire;
    public GameEvent inputEvPriRelease;
    public GameEvent inputEvAdsPress, inputEvAdsRelease;
    public GameEvent inputEvReloadPress, inputEvReloadRelease;
    public GameEvent inputEvMagSwapPress;
    public GameEvent zoomEnterEvent, zoomExitEvent;
    public GameEvent gunEnterEvent, gunExitEvent, exitMonitorEvent;
    public GameEvent magSwapTriggered;
    public GameEvent[] ammoUpdatedEvents;
}
