using GeneralUtility.EditorQoL;
using GeneralUtility.GameEventSystem;
using GeneralUtility.VariableObject;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunRotationControl : MonoBehaviour
{
    private Vector2 mouseInput;

    [Header("Weapon Movement")]
    [SerializeField] private float sensitivityScaler;
    [SerializeField] private float xSens, ySens;
    [SerializeField] private GameEvent sensitivityChangedEvent;
    [SerializeField] private FloatReference gunRotateSpeed;

    [Header("References")]
    [SerializeField] private GameObject gunCamera;
    [SerializeField] private GameObject pivotPoint, gunBase;
    [SerializeField] private GameEvent zoomEnterEvent, zoomExitEvent;
    [SerializeField] private AnimationCurve gunCatchUpCurve;
    [SerializeField] private BoolReference isReloading;

    [Header("Debug")]
    [SerializeField] [ReadOnly] private Vector2 pivotRotation, gunRotation;

    private void Awake()
    {
        var sensitivityChangedListener = gameObject.AddComponent<GameEventListener>();
        sensitivityChangedListener.Events.Add(sensitivityChangedEvent);
        sensitivityChangedListener.Response = new();
        sensitivityChangedListener.Response.AddListener(() =>
        xSens = PlayerPrefs.GetFloat(GeneralUtility.MagicStrings.OPTIONS_X_SENS_ZOOM) * sensitivityScaler);
        sensitivityChangedListener.Response.AddListener(() =>
        ySens = PlayerPrefs.GetFloat(GeneralUtility.MagicStrings.OPTIONS_Y_SENS_ZOOM) * sensitivityScaler);
        sensitivityChangedEvent.RegisterListener(sensitivityChangedListener);

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
        input.x = Mathf.Clamp(input.x, -80f, 80f);

        pivotRotation.x += input.x * xSens * Time.fixedDeltaTime;

        pivotRotation.y -= input.y * ySens * Time.fixedDeltaTime;
        pivotRotation.y = Mathf.Clamp(pivotRotation.y, -85f, 15f);

        var pivotRot = Quaternion.Euler(pivotRotation.y, pivotRotation.x, 0f);
        var truRot = Quaternion.RotateTowards(pivotPoint.transform.localRotation, pivotRot, 20f);

        pivotPoint.transform.localRotation = truRot;
    }

    private void GunLook()
    {//bases its catchup on angular distance from the gun
        var angle = Quaternion.Angle(gunBase.transform.rotation, pivotPoint.transform.rotation);
        float clamp = 32f;
        var speed = gunCatchUpCurve.Evaluate(angle / clamp);

        float deadZone = 0.05f;

        Vector2 tempDirection = new Vector2((pivotRotation.x - gunRotation.x), (pivotRotation.y - gunRotation.y));
        tempDirection.Normalize();
        if (Mathf.Abs(tempDirection.x) <= deadZone) tempDirection.x = 0f;
        if (Mathf.Abs(tempDirection.y) <= deadZone) tempDirection.y = 0f;

        gunRotation.x += tempDirection.x * xSens * Time.fixedDeltaTime * gunRotateSpeed.Value * speed;
        gunRotation.x = Mathf.Clamp(gunRotation.x, pivotRotation.x - clamp, pivotRotation.x + clamp);

        gunRotation.y += tempDirection.y * ySens * Time.fixedDeltaTime * gunRotateSpeed.Value * speed;
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

    public void Look(InputAction.CallbackContext context)
    {
        mouseInput = context.ReadValue<Vector2>();
    }
}
