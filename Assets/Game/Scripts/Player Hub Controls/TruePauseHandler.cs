using GeneralUtility.GameEventSystem;
using GeneralUtility.VariableObject;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class InputHandler : MonoBehaviour
{
    [Header("Pause Input Events")]
    [SerializeField] private GameEvent inputEVPause;

    public void PausePressed(InputAction.CallbackContext context)
    {
        if (context.started)
            inputEVPause.Trigger();
    }
}

public class TruePauseHandler : MonoBehaviour
{
    [SerializeField] private GameEvent inputEVPause;
    [SerializeField] private GameEvent gamePausedEvent, gameUnpausedEvent;
    [SerializeField] private BoolReference isPaused;
    [SerializeField] private FloatReference unpausedTimeScale;

    private void Awake()
    {
        var pauseListener = gameObject.AddComponent<GameEventListener>();
        pauseListener.Events.Add(inputEVPause);
        pauseListener.Response = new();
        pauseListener.Response.AddListener(() => TryPause());
        inputEVPause.RegisterListener(pauseListener);
    }

    private void OnEnable()
    {//just in case
        isPaused.Value = false;
    }

    private void OnDisable()
    {//just in case
        isPaused.Value = false;
    }

    private void TryPause()
    {
        if (!isPaused.Value) HandlePause();
        else HandleUnpause();
    }

    private void HandlePause()
    {
        isPaused.Value = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.Confined;
        //show UI

        gamePausedEvent?.Trigger();
    }

    private void HandleUnpause()
    {
        isPaused.Value = false;
        Time.timeScale = unpausedTimeScale.Value > 0f ? unpausedTimeScale.Value : 1f;
        Cursor.lockState = CursorLockMode.Locked;
        //hide UI

        gameUnpausedEvent?.Trigger();
    }
}