using System.Collections;
using Cinemachine;
using GeneralUtility.GameEventSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnterGunTerminal : MonoBehaviour, IInteractable
{
    private CinemachineVirtualCamera playerVCAM, gunVCAM;

    private GameObject player;
    private PlayerInput playInput;
    [SerializeField] private GameEvent gunEnterEvent, gunExitEvent, exitMonitorEvent;

    private bool monitorEngaged = false;

    private void Start()
    {
        playInput = FindObjectOfType<PlayerInput>();
        player = playInput.gameObject;
        playerVCAM = player.GetComponentInChildren<CinemachineVirtualCamera>();

        gunVCAM = FindObjectOfType<GunCamIdentifier>().GetComponent<CinemachineVirtualCamera>();

        var exitMonitorListener = gameObject.AddComponent<GameEventListener>();
        exitMonitorListener.Events.Add(exitMonitorEvent);
        exitMonitorListener.Response = new();
        exitMonitorListener.Response.AddListener(() => TryExitMonitor());
        exitMonitorEvent.RegisterListener(exitMonitorListener);
    }

    private void TryExitMonitor()
    {
        if (!monitorEngaged) return;

        gunExitEvent.Trigger();
        Cursor.lockState = CursorLockMode.Locked;
        //gunVCAM.Priority = 101;
        playerVCAM.Priority = 5;
        //gunVCAM.Priority = 0;
        playInput.SwitchCurrentActionMap("Hub");

        monitorEngaged = false;
    }

    // Will change the players controlls and change there camera view to the gun / turret.
    public void Interact(object _)
    {
        //Debug.Log("It worked!");
        monitorEngaged = true;
        //monitorCam.Priority = 100;
        StartCoroutine(nameof(CamCoroutine));
        //playerVCAM.Priority = 0;
        playInput.SwitchCurrentActionMap("Gun");
        gunEnterEvent.Trigger();
        //AudioManager.instance.SetVolume("BGM", 0.015f);
    }

    private IEnumerator CamCoroutine()
    {
        yield return null;
        //yield return new WaitForSeconds(1.8f);
        //gunVCAM.Priority = 101;
        playerVCAM.Priority = 0;
        //gunVCAM.Priority = 5;
    }
}