using GeneralUtility.EditorQoL;
using GeneralUtility.GameEventSystem;
using GeneralUtility.VariableObject;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class InputHandler : MonoBehaviour
{
    [Header("Gun Rotation Input Events")]
    [SerializeField] private GameEvent inputEvLook;

    public void Look(InputAction.CallbackContext context)
    {
        if (context.started)
            inputEvLook.Trigger(context.ReadValue<Vector2>());
    }

    public void StopLook(InputAction.CallbackContext context)
    {
        if (context.canceled)
            inputEvLook.Trigger(Vector2.zero);
    }
}

public class GunRotationControl : MonoBehaviour
{
    private Vector2 mouseInput;

    [Header("Weapon Movement")]
    [SerializeField] private float sensitivityScaler;
    [SerializeField] private float xSens, ySens;
    [SerializeField] private GameEvent sensitivityChangedEvent;
    [SerializeField] private FloatReference gunRotateSpeed;
    [SerializeField] private GameEvent inputEvLook;
    [SerializeField] private float angleClamp;

    [Header("References")]
    [SerializeField] private GameObject gunCamera;
    [SerializeField] private GameObject pivotPoint, gunBase;
    [SerializeField] private AnimationCurveReference gunCatchUpCurve;
    [SerializeField] private BoolReference isReloading;

    [Header("Debug")]
    [SerializeField] [ReadOnly] private Vector2 pivotRotation;
    [SerializeField] [ReadOnly] private Vector2 gunRotation;

    private void Awake()
    {
        var sensitivityChangedListener = gameObject.AddComponent<GameEventListener>();
        sensitivityChangedListener.Events.Add(sensitivityChangedEvent);
        sensitivityChangedListener.Response = new();
        sensitivityChangedListener.Response.AddListener(() =>
        xSens = PlayerPrefs.GetFloat(GeneralUtility.MagicStrings.OPTIONS_X_SENS_BASE) * sensitivityScaler);
        sensitivityChangedListener.Response.AddListener(() =>
        ySens = PlayerPrefs.GetFloat(GeneralUtility.MagicStrings.OPTIONS_Y_SENS_BASE) * sensitivityScaler);
        sensitivityChangedEvent.RegisterListener(sensitivityChangedListener);

        var lookListener = gameObject.AddComponent<GameEventListener>();
        lookListener.Events.Add(inputEvLook);
        lookListener.VecResponse = new();
        lookListener.VecResponse.AddListener((input) => Look(input));
        inputEvLook.RegisterListener(lookListener);

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        CamLook(mouseInput);

        if (!isReloading.Value)
        {
            GunLook();
        }
    }

    private void CamLook(Vector2 input)
    {
        input.x = Mathf.Clamp(input.x, -120f, 120f);
        pivotRotation.x += input.x * xSens * sensitivityScaler * Time.fixedDeltaTime;

        pivotRotation.y -= input.y * ySens * sensitivityScaler *  Time.fixedDeltaTime;
        pivotRotation.y = Mathf.Clamp(pivotRotation.y, -85f, 15f);

        var pivotRot = Quaternion.Euler(pivotRotation.y, pivotRotation.x, 0f);
        var truRot = Quaternion.RotateTowards(pivotPoint.transform.localRotation, pivotRot, 20f);

        pivotPoint.transform.localRotation = truRot;
    }

    private void GunLook()
    {//bases its catchup on angular distance from the gun
        var angle = Quaternion.Angle(gunBase.transform.rotation, pivotPoint.transform.rotation);
        var speed = gunCatchUpCurve.Value.Evaluate(angle / angleClamp);

        float deadZone = 0.05f;

        Vector2 tempDirection = new Vector2((pivotRotation.x - gunRotation.x), (pivotRotation.y - gunRotation.y));
        tempDirection.Normalize();
        if (Mathf.Abs(tempDirection.x) <= deadZone) tempDirection.x = 0f;
        if (Mathf.Abs(tempDirection.y) <= deadZone) tempDirection.y = 0f;

        gunRotation.x += tempDirection.x * xSens * sensitivityScaler * Time.fixedDeltaTime * gunRotateSpeed.Value * speed;
        gunRotation.x = Mathf.Clamp(gunRotation.x, pivotRotation.x - angleClamp, pivotRotation.x + angleClamp);

        gunRotation.y += tempDirection.y * ySens * sensitivityScaler * Time.fixedDeltaTime * gunRotateSpeed.Value * speed;
        gunRotation.y = Mathf.Clamp(gunRotation.y, -85f, 15f);

        var gunRot = Quaternion.Euler(gunRotation.y, gunRotation.x, 0f);
        var truRot = Quaternion.RotateTowards(gunBase.transform.localRotation, gunRot, 20f);

        if (Mathf.Abs(gunRotation.x - pivotRotation.x) <= deadZone && Mathf.Abs(gunRotation.y - pivotRotation.y) <= deadZone)
        {
            gunBase.transform.rotation = pivotPoint.transform.rotation;
        }
        else
        {
            gunBase.transform.rotation = truRot;
        }
    }

    public void Look(Vector2 input)
    {
        float deadZone = 0.05f;

        input.x = Mathf.Abs(input.x) > deadZone ? input.x : 0f;
        input.y = Mathf.Abs(input.y) > deadZone ? input.y : 0f;

        mouseInput = input;
    }
}
