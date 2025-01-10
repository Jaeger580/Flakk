using Cinemachine;
using GeneralUtility.GameEventSystem;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;

public class MonitorInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private CinemachineVirtualCamera monitorCam;
    [SerializeField] private GameEvent inputEventExitMonitor, exittedMonitorEvent;
    [SerializeField] private bool startInTerminal;

    private PlayerInput playInput;
    private bool monitorEngaged = false;

    private UIDocument doc;
    private UI_InputMapper mapper;
    private List<IUIScreenRefresh> toRefresh = new();

    private IEnumerator Start()
    {
        playInput = FindObjectOfType<PlayerInput>();
        doc = GetComponent<UIDocument>();
        mapper = GetComponent<UI_InputMapper>();

        var exitMonitorListener = gameObject.AddComponent<GameEventListener>();
        exitMonitorListener.Events.Add(inputEventExitMonitor);
        exitMonitorListener.Response = new();
        exitMonitorListener.Response.AddListener(() => TryExitMonitor());
        inputEventExitMonitor.RegisterListener(exitMonitorListener);

        foreach(var refresh in GetComponents<IUIScreenRefresh>())
        {
            toRefresh.Add(refresh);
        }

        yield return new WaitForSeconds(0.01f);


        if (!startInTerminal)
            DisableMonitor();
        else
            Interact(this);
    }

    private void EnableMonitor()
    {
        doc.enabled = true;
        mapper.enabled = true;

        var root = doc.rootVisualElement;
        var exitBtn = root.Q<Button>("Exit");
        exitBtn.clickable.clicked += TryExitMonitor;

        foreach(var refresh in toRefresh)
        {
            refresh.RefreshUI();
        }
    }

    private void DisableMonitor()
    {
        doc.enabled = false;
        mapper.enabled = false;
    }

    public void Interact(object _)
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        monitorEngaged = true;
        monitorCam.Priority = 100;
        playInput.SwitchCurrentActionMap("UI");
        var hubInput = playInput.actions.FindActionMap("Hub");
        hubInput.Disable();

        EnableMonitor();
    }

    private void TryExitMonitor()
    {
        if (!monitorEngaged) return;

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        monitorCam.Priority = 0;
        var hubInput = playInput.actions.FindActionMap("Hub");
        hubInput.Enable();
        playInput.SwitchCurrentActionMap("Hub");

        exittedMonitorEvent?.Trigger();

        DisableMonitor();
    }
}