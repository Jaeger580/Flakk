using GeneralUtility;
using GeneralUtility.GameEventSystem;
using GeneralUtility.VariableObject;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

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
    [Header("Pause Events and Variables")]
    [SerializeField] private GameEvent inputEVPause;
    [SerializeField] private GameEvent gamePausedEvent, gameUnpausedEvent;
    [SerializeField] private BoolReference isPaused;
    [SerializeField] private FloatReference unpausedTimeScale;

    [Header("UI References")]
    [SerializeField] private UIDocument uidoc;
    private VisualElement root, pauseScreen, optionsScreen;
    private Button btnResume, btnOptions, btnReloadScene, btnQuit;
    private Button btnOptionsExit;

    [Header("Other")]
    [SerializeField] private PlayerInput playerInput;
    private InputActionMap lastActionMap;


    private void Awake()
    {
        var pauseListener = gameObject.AddComponent<GameEventListener>();
        pauseListener.Events.Add(inputEVPause);
        pauseListener.Response = new();
        pauseListener.Response.AddListener(() => TryPause());
        inputEVPause.RegisterListener(pauseListener);

        root = uidoc.rootVisualElement;
        pauseScreen = root.Q<VisualElement>("PauseScreen");
        pauseScreen.style.display = DisplayStyle.None;
        optionsScreen = root.Q<VisualElement>("OptionsScreen");

        btnResume = pauseScreen.Q<Button>("Resume");
        btnOptions = pauseScreen.Q<Button>("Options");
        btnReloadScene = pauseScreen.Q<Button>("ReloadScene");
        btnQuit = pauseScreen.Q<Button>("QuitGame");

        btnResume.clicked += Resume;
        btnOptions.clicked += ShowOptions;
        btnReloadScene.clicked += ReloadScene;
        btnQuit.clicked += QuitGame;

        btnOptionsExit = optionsScreen.Q<Button>("Exit");
        btnOptionsExit.clicked += CloseOptions;

        HandleUnpause();
    }

    private void Resume()
    {
        HandleUnpause();
    }
    private void ShowOptions()
    {
        UI_Utility.ToggleContainer(optionsScreen, true);
    }
    private void CloseOptions()
    {
        UI_Utility.ToggleContainer(optionsScreen, false);
    }
    private void ReloadScene()
    {
        SceneManager.LoadSceneAsync(0);
    }

    private void QuitGame()
    {
        HandleUnpause();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#endif
#if !UNITY_EDITOR
        Application.Quit();
#endif
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
        UnityEngine.Cursor.lockState = CursorLockMode.Confined;

        UI_Utility.ToggleContainer(pauseScreen, true);

        // Added to stop player from performing actions while paused.
        lastActionMap = playerInput.currentActionMap;
        playerInput.SwitchCurrentActionMap("UI");

        gamePausedEvent?.Trigger();
    }

    private void HandleUnpause()
    {
        isPaused.Value = false;
        Time.timeScale = unpausedTimeScale.Value > 0f ? unpausedTimeScale.Value : 1f;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        // Added to stop player from performing actions while paused.
        if (lastActionMap != null) 
        {
            playerInput.SwitchCurrentActionMap(lastActionMap.name);
        }

        UI_Utility.ToggleContainer(pauseScreen, false);
        UI_Utility.ToggleContainer(optionsScreen, false);

        gameUnpausedEvent?.Trigger();
    }
}