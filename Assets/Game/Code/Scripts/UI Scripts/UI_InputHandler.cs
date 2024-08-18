using GeneralUtility.GameEventSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class UI_InputHandler : MonoBehaviour
{
    /*
        InputHandler handles all inputs and triggers events accordingly.
        For example, pressing the jump button triggers the jump GameEvent.
    */

    [SerializeField] private PlayerInput playerInput;
    public GameEvent inputEVCancelUI;

    private InputActionMap aMapHub, aMapUI, aMapGun, aMapDev;
    private Dictionary<InputActionMap, bool> aMapList = new();      //Specifically for keeping track of what is SUPPOSED to be enabled

    private int targetFrameRate = 60;

    private void Awake()
    {
        Application.targetFrameRate = targetFrameRate;
        playerInput = GetComponent<PlayerInput>();

        aMapHub = playerInput.actions.FindActionMap("Hub");
        aMapGun = playerInput.actions.FindActionMap("Gun");
        aMapUI = playerInput.actions.FindActionMap("UI");

        aMapList.Add(aMapHub, aMapHub.enabled);
        aMapList.Add(aMapGun, aMapGun.enabled);
        aMapList.Add(aMapUI, aMapUI.enabled);

#if UNITY_EDITOR
        //aMapDev = playerInput.actions.FindActionMap("Developer Events");
        //aMapDev.Enable();
        //aMapList.Add(aMapDev, aMapDev.enabled);
        //aMapList.Add(aMapDev, aMapDev.enabled);
#endif
    }

    public void ExitMonitor(InputAction.CallbackContext context)
    {
        if (context.started)
            inputEVCancelUI.Trigger();
    }
}
