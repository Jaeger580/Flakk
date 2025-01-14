using Cinemachine;
using GeneralUtility.GameEventSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class TEMP_RestockTerminal : MonoBehaviour, IInteractable
{
    [SerializeField] private CinemachineVirtualCamera monitorCam;

    [SerializeField] private AmmoStack ammoCrateToLoad, stockpileToLoadInto;
    [SerializeField] private GameEvent restockEvent, exitMonitorEvent;

    public delegate void WhileStocking(float loadingBar);
    public WhileStocking LoadingBarChangedEvent;
    public delegate void OnAmmoStocked(float newStockpileAmt, float newMagAmt);
    public OnAmmoStocked AmmoStockedEvent;

    private bool monitorEngaged = false;

    private float restockTimer = 0f, restockRate = 1f;

    public int MaxAmmo => stockpileToLoadInto.maxStackSize.Value;

    private PlayerInput playInput;

    private void Start()
    {
        playInput = FindObjectOfType<PlayerInput>();

        var exitMonitorListener = gameObject.AddComponent<GameEventListener>();
        exitMonitorListener.Events.Add(exitMonitorEvent);
        exitMonitorListener.Response = new();
        exitMonitorListener.Response.AddListener(() => TryExitMonitor());
        exitMonitorEvent.RegisterListener(exitMonitorListener);

        while(ammoCrateToLoad.stack.Count < ammoCrateToLoad.maxStackSize.Value)
        {
            ammoCrateToLoad.Push(ammoCrateToLoad.Peek());
        }
    }

    private void TryExitMonitor()
    {
        if (!monitorEngaged) return;
        restockTimer = 0f;
        monitorCam.Priority = 0;

        Cursor.lockState = CursorLockMode.Locked;
        playInput.SwitchCurrentActionMap("Hub");

        AmmoStockedEvent(ammoCrateToLoad.stack.Count, stockpileToLoadInto.stack.Count);
        LoadingBarChangedEvent?.Invoke(restockTimer / (1f / restockRate));
        monitorEngaged = false;
    }

    private void Update()
    {
        if (monitorEngaged && stockpileToLoadInto.stack.Count < stockpileToLoadInto.maxStackSize.Value)
        {
            restockTimer += Time.deltaTime;
            LoadingBarChangedEvent?.Invoke(restockTimer/(1f/restockRate));
            if (restockTimer < 1f/restockRate) return;

            if (ammoCrateToLoad.stack.Count <= 1)
            {//TEMP FOR INFINITE AMMO
                for (int i = 0; i < ammoCrateToLoad.maxStackSize.Value; i++)
                {
                    ammoCrateToLoad.Push(ammoCrateToLoad.Peek());
                }
            }

            restockTimer = 0f;
            stockpileToLoadInto.Push(ammoCrateToLoad.Pop());
            AmmoStockedEvent?.Invoke(ammoCrateToLoad.stack.Count, stockpileToLoadInto.stack.Count);
            LoadingBarChangedEvent?.Invoke(restockTimer / (1f / restockRate));
        }
    }

    public void Interact(object _)
    {
        playInput.SwitchCurrentActionMap("UI");
        monitorEngaged = true;
        monitorCam.Priority = 100;
        AmmoStockedEvent?.Invoke(ammoCrateToLoad.stack.Count, stockpileToLoadInto.stack.Count);
        LoadingBarChangedEvent?.Invoke(restockTimer / (1f / restockRate));
    }
}
