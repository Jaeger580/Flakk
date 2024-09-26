using Cinemachine;
using GeneralUtility.GameEventSystem;
using GeneralUtility.VariableObject;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class InputHandler : MonoBehaviour
{
    [Header("Gun Mechanic Input Events")]
    [SerializeField] private GameEvent inputEvPriFire;
    [SerializeField] private GameEvent inputEvPriRelease;
    [SerializeField] private GameEvent inputEvAdsPress, inputEvAdsRelease;
    [SerializeField] private GameEvent inputEvReleasePress, inputEvReleaseRelease;

    public void PrimaryFire(InputAction.CallbackContext context)
    {
        if (context.started)
            inputEvPriFire.Trigger();
    }

    public void PrimaryRelease(InputAction.CallbackContext context)
    {
        if (context.canceled)
            inputEvPriRelease.Trigger();
    }

    public void AimDownSightsPressed(InputAction.CallbackContext context)
    {
        if (context.started)
            inputEvAdsPress.Trigger();
    }

    public void AimDownSightsRelease(InputAction.CallbackContext context)
    {
        if (context.canceled)
            inputEvAdsRelease.Trigger();
    }
}

public class GunMechanicControl : MonoBehaviour
{
    private Camera mainCamera;

    //private float maxFireRate;
    private float fireRateTimer;
    private int currentClip = 0;
    private float reloadTimer;
    private float currentHeat = 1f;
    private bool overHeating = false;

    public delegate void OnHeatChange(float newHeat);
    public OnHeatChange HeatChangeEvent;
    public delegate void OnAmmoChange(float newAmmo);
    public OnAmmoChange AmmoChangeEvent;

    private bool isShooting = false;

    [Header("Gun Stats")]
    [SerializeField] private IntReference baseDamage;

    [Tooltip("How many bullets can be fired per second?")]
    [SerializeField] private FloatReference fireRate;

    [Tooltip("How many bullets can be reloaded per second?")]
    [SerializeField] private FloatReference reloadRate;
    [SerializeField] private BoolReference isReloading;

    [Tooltip("How many bullets can fit in the clip, at max?")]
    [SerializeField] private IntReference clipSize;
    public int ClipSize => clipSize.Value;

    [Tooltip("How much heat until the gun needs to cool down?")]
    [SerializeField] private FloatReference maxHeat;
    public float MaxHeat => maxHeat.Value;

    [Tooltip("How fast can the gun cool down prior to reaching max overheat? (in overheat per second)")]
    [SerializeField] private FloatReference cooldownSpeed;
    [Tooltip("How fast can the gun cool down after overheating? (in overheat per second)")]
    [SerializeField] private FloatReference overheatSpeed;
    [Tooltip("How much overheat does the gun accumulate per shot?")]
    [SerializeField] private FloatReference overheatRate;

    [Header("References")]
    [SerializeField] private GameObject gunCamera;
    [SerializeField] private GameObject gunBase, gunBulletPoint;
    [SerializeField] private GameObjectReference bulletPrefab;
    [SerializeField] private GameObject vfxPrefab;
    [SerializeField] private GameEvent inputEvAdsPress, inputEvAdsRelease;
    [SerializeField] private GameEvent zoomEnterEvent, zoomExitEvent;

    private CinemachineVirtualCamera vCam;

    private void OnDisable()
    {
        HeatChangeEvent = null;
        AmmoChangeEvent = null;
    }

    private void Awake()
    {
        var zoomEnterListener = gameObject.AddComponent<GameEventListener>();
        zoomEnterListener.Events.Add(inputEvAdsPress);
        zoomEnterListener.Response = new();
        zoomEnterListener.Response.AddListener(() => { Zoom(true); zoomEnterEvent?.Trigger(); });
        inputEvAdsPress.RegisterListener(zoomEnterListener);

        var zoomExitListener = gameObject.AddComponent<GameEventListener>();
        zoomExitListener.Events.Add(inputEvAdsRelease);
        zoomExitListener.Response = new();
        zoomExitListener.Response.AddListener(() => { Zoom(false); zoomExitEvent?.Trigger(); });
        inputEvAdsRelease.RegisterListener(zoomExitListener);
    }

    private void Start()
    {
        mainCamera = Camera.main;
        gunCamera.TryGetComponent(out vCam);
        currentClip = clipSize.Value;
        AmmoChangeEvent?.Invoke(currentClip);
    }

    private void Update()
    {
        if (!isShooting)
        {
            if (currentHeat > 0 & !overHeating)
            {
                currentHeat -= cooldownSpeed.Value * Time.deltaTime;
                HeatChangeEvent?.Invoke(currentHeat);
            }
            else if (overHeating && currentHeat > 0)
            {
                currentHeat -= overheatSpeed.Value * Time.deltaTime;
                HeatChangeEvent?.Invoke(currentHeat);
            }

            if (currentHeat <= 0 && overHeating)
            {
                overHeating = false;
            }
        }

        if (isReloading.Value)
        {
            if (reloadTimer < reloadRate.Value)
            {
                reloadTimer += Time.deltaTime;
            }
            else if (currentClip < clipSize.Value)
            {
                reloadTimer = 0;
                currentClip++;
                AmmoChangeEvent?.Invoke(currentClip);
            }
            else if (currentClip == clipSize.Value)
            {
                isReloading.Value = false;
            }
        }
        else if (isShooting && Time.time >= fireRateTimer + (1 / fireRate.Value) && currentClip > 0)
        {
            Fire();
        }
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (!overHeating)
        {
            if (context.started)
            {
                isShooting = true;
            }

            else if (context.canceled)
            {
                isShooting = false;
            }
        }
    }

    public void Reload(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            reloadTimer = 0f;
            isReloading.Value = true;
        }

        else if (context.canceled)
        {
            isReloading.Value = false;
        }
    }

    public void Zoom(bool zoomEnter)
    {
        vCam.m_Lens.FieldOfView = zoomEnter ? 36f : 52f;
        Vector3 camPos = gunCamera.transform.localPosition;
        gunCamera.transform.localPosition = new Vector3(camPos.x, camPos.y, camPos.z + (zoomEnter ? 2f : -2f));
    }

    private void Fire()
    {
        Quaternion gunRotation = gunBase.transform.rotation;

        gunRotation *= Quaternion.Euler(90, 0, 0);

        Vector3 camCenter = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, 0));
        GameObject bulletInstance = Instantiate(bulletPrefab.Value, gunBulletPoint.transform.position, gunRotation);

        if (vfxPrefab != null)
        {
            GameObject vxfInstance = Instantiate(vfxPrefab, bulletInstance.transform.position, bulletInstance.transform.rotation);
        }

        currentClip--;
        AmmoChangeEvent?.Invoke(currentClip);
        currentHeat += overheatRate.Value;
        HeatChangeEvent?.Invoke(currentHeat);

        if (currentHeat >= maxHeat.Value)
        {
            overHeating = true;
            isShooting = false;
        }

        fireRateTimer = Time.time;

    }
}
