using GeneralUtility.GameEventSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLook : MonoBehaviour
{
    [SerializeField]
    private GameObject mainCamera;

    private Vector2 mouseInput;
    private float vertRotation;

    public GameEvent inputEVLook;

    private float SensX => PlayerPrefs.GetFloat(GeneralUtility.MagicStrings.OPTIONS_X_SENS_BASE, 1f);
    private float SensY => PlayerPrefs.GetFloat(GeneralUtility.MagicStrings.OPTIONS_Y_SENS_BASE, 1f);

    // Start is called before the first frame update
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        var lookListener = gameObject.AddComponent<GameEventListener>();
        lookListener.Events.Add(inputEVLook);
        lookListener.VecResponse = new();
        lookListener.VecResponse.AddListener((Vector2 v) => mouseInput = v);
        inputEVLook.RegisterListener(lookListener);

        print($"SensX = {SensX} :: SensY = {SensY}");
    }

    private void Update()
    {
        HandleLook(mouseInput);
    }

    private void HandleLook(Vector2 Input)
    {
        float horizRotation = Input.x * SensX;
        transform.Rotate(0, horizRotation, 0);

        vertRotation -= Input.y * SensY;
        vertRotation = Mathf.Clamp(vertRotation, -90, 90);

        mainCamera.transform.localRotation = Quaternion.Euler(vertRotation, 0, 0);
    }
}



