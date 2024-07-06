using Cinemachine;
using GeneralUtility.GameEventSystem;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

public class MonitorInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private CinemachineVirtualCamera monitorCam;
    [SerializeField] private GameEvent exitMonitorEvent;

    private PlayerInput playInput;
    private bool monitorEngaged = false;

    private void Start()
    {
        playInput = FindObjectOfType<PlayerInput>();

        var root = GetComponent<UIDocument>().rootVisualElement;
        var exitBtn = root.Q<Button>("Exit");
        exitBtn.clickable.clicked += TryExitMonitor;

        var exitMonitorListener = gameObject.AddComponent<GameEventListener>();
        exitMonitorListener.Events.Add(exitMonitorEvent);
        exitMonitorListener.Response = new();
        exitMonitorListener.Response.AddListener(() => TryExitMonitor());
        exitMonitorEvent.RegisterListener(exitMonitorListener);
    }

    public void Interact()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;
        monitorEngaged = true;
        monitorCam.Priority = 100;
        playInput.SwitchCurrentActionMap("UI");
    }

    private void TryExitMonitor()
    {
        if (!monitorEngaged) return;

        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        monitorCam.Priority = 0;
        playInput.SwitchCurrentActionMap("Hub");
    }
}