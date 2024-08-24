using System.Collections;
using Cinemachine;
using GeneralUtility.GameEventSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnterGunTerminal : MonoBehaviour, IInteractable
{
    [SerializeField] private CinemachineVirtualCamera monitorCam;
    private CinemachineVirtualCamera playerVCAM, gunVCAM;

    private GameObject player;
    private PlayerInput playInput;
    [SerializeField] private GameEvent gunEnterEvent, exitMonitorEvent, gunExitEvent;

    private bool monitorEngaged = false;

    private Coroutine gunLookRoutine;

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

        var gunExitListener = gameObject.AddComponent<GameEventListener>();
        gunExitListener.Events.Add(gunExitEvent);
        gunExitListener.Response = new();
        gunExitListener.Response.AddListener(() => TryExitMonitor());
        gunExitEvent.RegisterListener(gunExitListener);
    }

    private void TryExitMonitor()
    {
        if (!monitorEngaged) return;

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        gunVCAM.Priority = 0;
        monitorCam.Priority = 0;

        if (gunLookRoutine != null)
        {
            StopCoroutine(gunLookRoutine);
            gunLookRoutine = null;
        }

        StartCoroutine(nameof(MouseDelay));
    }

    // Will change the players controlls and change there camera view to the gun / turret.
    public void Interact()
    {
        //Debug.Log("It worked!");
        monitorEngaged = true;
        monitorCam.Priority = 100;
        gunLookRoutine = StartCoroutine(nameof(CamCoroutine));
        //playerVCAM.Priority = 0;
        playInput.SwitchCurrentActionMap("Gun");
        var hubInput = playInput.actions.FindActionMap("Hub");
        hubInput.Disable();
        gunEnterEvent?.Trigger();
        AudioManager.instance.SetVolume("BGM", 0.015f);
    }

    private IEnumerator CamCoroutine()
    {
        yield return new WaitForSeconds(1f);
        gunVCAM.Priority = 101;
        playerVCAM.Priority = 0;
        monitorCam.Priority = 0;
        gunVCAM.Priority = 5;
    }

    private IEnumerator MouseDelay()
    {
        monitorEngaged = false;
        playInput.DeactivateInput();
        var gunInput = playInput.actions.FindActionMap("Gun");
        gunInput.Disable();
        yield return new WaitForSeconds(1f);
        playInput.ActivateInput();
        var hubInput = playInput.actions.FindActionMap("Hub");
        hubInput.Enable();
        playInput.SwitchCurrentActionMap("Hub");
    }
}
