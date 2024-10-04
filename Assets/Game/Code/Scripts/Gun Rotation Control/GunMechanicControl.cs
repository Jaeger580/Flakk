using Cinemachine;
using GeneralUtility;
using GeneralUtility.EditorQoL;
using GeneralUtility.GameEventSystem;
using GeneralUtility.VariableObject;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunMechanicControl : MonoBehaviour
{
    [Header("Bullet Magazines")]
    [ReadOnly] [SerializeField] protected AmmoStack currentMag;
    [ReadOnly] [SerializeField] protected AmmoStack currentStockpile;
    [SerializeField] protected AmmoStack primaryMag, secondaryMag;
    [SerializeField] protected AmmoStack primaryStockpile, secondaryStockpile;

    [Tooltip("Default magazines, mostly for testing and/or before we have stockpiles ready.")]
    [SerializeField] protected AmmoStack defaultPrimaryMag, defaultSecondaryMag;

    [Header("Gun Specifications and Handling")]
    [Tooltip("How many bullets can be fired per second?")]
    [SerializeField] protected FloatReference fireRate;
    [SerializeField] protected BoolReference isFiring;

    [Tooltip("How many bullets can be reloaded per second?")]
    [SerializeField] protected FloatReference reloadRate;
    [SerializeField] protected BoolReference isReloading;
    protected float reloadTimer;
    protected float fireTimer;

    [Header("References")]
    [SerializeField] protected GameObject gunCamera;
    protected CinemachineVirtualCamera vCam;
    [SerializeField] protected Transform gunpoint;
    [SerializeField] protected GameObject gunBase, gunBulletPoint;
    [SerializeField] protected GameObjectReference bulletPrefab;
    [SerializeField] protected GameObject vfxPrefab;

    [Header("Game Events")]
    [SerializeField] protected GameEvent inputEvPriFire, inputEvPriRelease;
    [SerializeField] protected GameEvent inputEvAdsPress, inputEvAdsRelease;
    [SerializeField] protected GameEvent inputEvReloadPress, inputEvReloadRelease;
    [SerializeField] protected GameEvent inputEvMagSwapPress;
    [SerializeField] protected GameEvent zoomEnterEvent, zoomExitEvent;
    //^Probably could've further separated logic and result through extra use of game events? (i.e. once a shot is confirmed, just send a shot event)
    public delegate void OnAmmoChange(float newAmmo);
    public OnAmmoChange AmmoChangeEvent;

    protected void Awake()
    {//When the scene starts, refill the mags (player shouldn't need to know how mags work yet)

        static void InitMag(AmmoStack mag, AmmoStack defaultAmmo)
        {//Fill the mag with default ammo, with a max mag size
            if (mag.stack.Count <= 0)
            {//If the mag is empty,
                foreach (var ammo in defaultAmmo.stack)
                {//DO NOT POP, add each bullet without removing it from the default mag
                    if (mag.stack.Count >= mag.maxStackSize.Value) break;
                    mag.Push(ammo);
                }
            }
        }

        InitMag(primaryMag, defaultPrimaryMag);
        InitMag(secondaryMag, defaultSecondaryMag);

        currentMag = primaryMag;

        var primaryFireListener = gameObject.AddComponent<GameEventListener>();
        primaryFireListener.Events.Add(inputEvPriFire);
        primaryFireListener.Response = new();
        primaryFireListener.Response.AddListener(() => FireInputIntake(true));
        inputEvPriFire.RegisterListener(primaryFireListener);

        var primaryReleaseListener = gameObject.AddComponent<GameEventListener>();
        primaryReleaseListener.Events.Add(inputEvPriRelease);
        primaryReleaseListener.Response = new();
        primaryReleaseListener.Response.AddListener(() => FireInputIntake(false));
        inputEvPriRelease.RegisterListener(primaryReleaseListener);

        var reloadPressListener = gameObject.AddComponent<GameEventListener>();
        reloadPressListener.Events.Add(inputEvReloadPress);
        reloadPressListener.Response = new();
        reloadPressListener.Response.AddListener(() => ReloadInputIntake(true));
        inputEvReloadPress.RegisterListener(reloadPressListener);

        var reloadReleaseListener = gameObject.AddComponent<GameEventListener>();
        reloadReleaseListener.Events.Add(inputEvReloadRelease);
        reloadReleaseListener.Response = new();
        reloadReleaseListener.Response.AddListener(() => ReloadInputIntake(false));
        inputEvReloadRelease.RegisterListener(reloadReleaseListener);

        var magSwapPressListener = gameObject.AddComponent<GameEventListener>();
        magSwapPressListener.Events.Add(inputEvMagSwapPress);
        magSwapPressListener.Response = new();
        magSwapPressListener.Response.AddListener(() => SwapMag());
        inputEvMagSwapPress.RegisterListener(magSwapPressListener);

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

    virtual protected void Start()
    {
        gunCamera.TryGetComponent(out vCam);
        AmmoChangeEvent?.Invoke(currentMag.stack.Count);
    }

    virtual protected void OnDisable()
    {
        AmmoChangeEvent = null;
    }

    virtual protected void Update()
    {
        if (fireTimer < 1f / fireRate.Value)
        {//increment timer if it's below the threshold
            fireTimer += Time.deltaTime;
        }

        if (isReloading.Value)
        {//if I'm pressing reload, handle that
            HandleReload();
        }
        else if (isFiring.Value)
        {//if I'm firing, handle that
            HandleFire();
        }
    }

    #region Firing
    protected void FireInputIntake(bool pressedFire) => isFiring.Value = pressedFire;

    virtual protected void HandleFire()
    {
        if (!currentMag.TryPeek(out var ammoType)) return;      //if there's no bullet in the magazine, return
        if (fireTimer < 1f / fireRate.Value) return;          //if the timer isn't ready, return

        var bulletInstance = Fire(ammoType);

        //Cleanup
        currentMag.Pop();
        AmmoChangeEvent?.Invoke(currentMag.stack.Count);

        fireTimer = 0f;
        //trigger sfx and vfx?
        if (vfxPrefab != null)
        {
            GameObject vfxInstance = Instantiate(vfxPrefab, bulletInstance.transform.position, bulletInstance.transform.rotation);
        }
    }

    virtual protected GameObject Fire(AmmoType ammoType)
    {//Should EXCLUSIVELY be creating and returning whatever bullet gets created, NOTHING ELSE
        Quaternion gunRotation = gunBase.transform.rotation;

        gunRotation *= Quaternion.Euler(90, 0, 0);

        return Instantiate(bulletPrefab.Value, gunBulletPoint.transform.position, gunRotation);
    }

    #endregion

    #region Reloading
    protected void HandleReload()
    {
        if (reloadTimer < 1f / reloadRate.Value)
        {
            reloadTimer += Time.deltaTime;
        }
        else
        {
            ReloadInputIntake(TryReload());
            reloadTimer = 0f;
        }
    }

    protected void ReloadInputIntake(bool pressedReload)
    {
        if (pressedReload)
        {
            isReloading.Value = true;
        }
        else
        {//If you released the reload button, reset the reload timer
            isReloading.Value = false;
            reloadTimer = 0f;
        }
    }

    protected bool TryReload()
    {
        if (!currentStockpile.TryPeek(out var bullet)) { Editor_Utility.ThrowWarning("ERR: Chosen stockpile empty!", this); return false; }
        if (!currentMag.TryPush(bullet)) { Editor_Utility.ThrowWarning("ERR: Chosen mag full!", this); return false; }

        //Handle reload sound

        currentStockpile.Pop(); //Stockpile DOES have ammo, and current mag DID take from it, so pop
        AmmoChangeEvent?.Invoke(currentMag.stack.Count);

        return true;
    }

    #endregion

    #region Misc. Mechanics
    public void Zoom(bool zoomEnter)
    {
        vCam.m_Lens.FieldOfView = zoomEnter ? 36f : 52f;
        Vector3 camPos = gunCamera.transform.localPosition;
        gunCamera.transform.localPosition = new Vector3(camPos.x, camPos.y, camPos.z + (zoomEnter ? 2f : -2f));
    }

    protected void SwapMag()
    {
        if (currentMag == primaryMag)
        {
            currentMag = secondaryMag;
            currentStockpile = secondaryStockpile;
        }
        else
        {
            currentMag = primaryMag;
            currentStockpile = primaryStockpile;
        }
    }
    #endregion
}
