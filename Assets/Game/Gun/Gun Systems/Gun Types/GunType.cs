﻿using Cinemachine;
using GeneralUtility;
using GeneralUtility.EditorQoL;
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
    [SerializeField] private GameEvent inputEvReloadPress, inputEvReloadRelease;
    [SerializeField] private GameEvent inputEvSwapMagPress;
    [SerializeField] private GameEvent inputEvExitGun;

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

    public void ReloadPressed(InputAction.CallbackContext context)
    {
        if (context.started)
            inputEvReloadPress.Trigger();
    }

    public void ReloadRelease(InputAction.CallbackContext context)
    {
        if (context.canceled)
            inputEvReloadRelease.Trigger();
    }

    public void SwapMagPressed(InputAction.CallbackContext context)
    {
        if (context.started)
            inputEvSwapMagPress.Trigger();
    }

    public void ExitGunPressed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            inputEvExitGun.Trigger();
        }
    }
}

public abstract class GunType : MonoBehaviour
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

    [Tooltip("What force should be applied to the bullet? (How hard should it be shot?)")]
    [SerializeField] protected FloatReference shotForce;

    [Tooltip("At what distance should bullet accuracy be measured from?")]
    [SerializeField] protected FloatReference inaccuracyRange;
    [Tooltip("What is the maximum distance any given bullet should be expectetd to stray from the center at its inaccuracy range?")]
    [SerializeField] protected Vector2Reference inaccuracyRadius;

    protected float fireTimer;
    protected float reloadTimer;

    [Header("References")]
    [SerializeField] protected GameObjectReference vfxOnShot;
    [SerializeField] protected AudioSource sfxOnShot;
    [SerializeField] protected AudioSource sfxOnReload;

    protected float onShotStartPitch;
    protected float onReloadStartPitch;



    protected GameObject gunCamera;
    protected CinemachineVirtualCamera vCam;
    protected GameObject gunBase, gunBulletPoint;

    public delegate void OnAmmoChange(float newAmmo);
    public OnAmmoChange AmmoChangeEvent;

    protected GunTypeSetup gunSetup;

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
        InitMag(primaryStockpile, defaultPrimaryMag);
        InitMag(secondaryStockpile, defaultSecondaryMag);

        currentMag = primaryMag;
        currentStockpile = primaryStockpile;

        if(!TryGetComponent(out gunSetup)) { Editor_Utility.ThrowWarning("ERR: Gun Type setup could not be completed.", this); return; }

        var primaryFireListener = gameObject.AddComponent<GameEventListener>();
        primaryFireListener.Events.Add(gunSetup.inputEvPriFire);
        primaryFireListener.Response = new();
        primaryFireListener.Response.AddListener(() => FireInputIntake(true));
        gunSetup.inputEvPriFire.RegisterListener(primaryFireListener);

        var primaryReleaseListener = gameObject.AddComponent<GameEventListener>();
        primaryReleaseListener.Events.Add(gunSetup.inputEvPriRelease);
        primaryReleaseListener.Response = new();
        primaryReleaseListener.Response.AddListener(() => FireInputIntake(false));
        gunSetup.inputEvPriRelease.RegisterListener(primaryReleaseListener);

        var reloadPressListener = gameObject.AddComponent<GameEventListener>();
        reloadPressListener.Events.Add(gunSetup.inputEvReloadPress);
        reloadPressListener.Response = new();
        reloadPressListener.Response.AddListener(() => ReloadInputIntake(true));
        gunSetup.inputEvReloadPress.RegisterListener(reloadPressListener);

        var reloadReleaseListener = gameObject.AddComponent<GameEventListener>();
        reloadReleaseListener.Events.Add(gunSetup.inputEvReloadRelease);
        reloadReleaseListener.Response = new();
        reloadReleaseListener.Response.AddListener(() => ReloadInputIntake(false));
        gunSetup.inputEvReloadRelease.RegisterListener(reloadReleaseListener);

        var magSwapPressListener = gameObject.AddComponent<GameEventListener>();
        magSwapPressListener.Events.Add(gunSetup.inputEvMagSwapPress);
        magSwapPressListener.Response = new();
        magSwapPressListener.Response.AddListener(() => SwapMag());
        gunSetup.inputEvMagSwapPress.RegisterListener(magSwapPressListener);

        var zoomEnterListener = gameObject.AddComponent<GameEventListener>();
        zoomEnterListener.Events.Add(gunSetup.inputEvAdsPress);
        zoomEnterListener.Response = new();
        zoomEnterListener.Response.AddListener(() => { Zoom(true); gunSetup.zoomEnterEvent?.Trigger(); });
        gunSetup.inputEvAdsPress.RegisterListener(zoomEnterListener);

        var zoomExitListener = gameObject.AddComponent<GameEventListener>();
        zoomExitListener.Events.Add(gunSetup.inputEvAdsRelease);
        zoomExitListener.Response = new();
        zoomExitListener.Response.AddListener(() => { Zoom(false); gunSetup.zoomExitEvent?.Trigger(); });
        gunSetup.inputEvAdsRelease.RegisterListener(zoomExitListener);

        //Continued setup using the gunTypeSetup component
        gunBase = gunSetup.gunBase;
        gunCamera = gunSetup.gunCamera;
        gunCamera.TryGetComponent(out vCam);
        gunBulletPoint = gunSetup.gunBulletPoint;

        fireTimer = 1f / fireRate.Value;
        fireTimer += 0.1f;

        // Audio Setup
        onShotStartPitch = sfxOnShot.pitch;
        onReloadStartPitch = sfxOnReload.pitch;

    }

    virtual protected void Start()
    {
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
        var imp = bulletInstance.GetComponent<ImpactBehavior>();
        imp.Initialize(ammoType.damage.Value);

        //Cleanup
        currentMag.Pop();
        AmmoChangeEvent?.Invoke(currentMag.stack.Count);

        fireTimer = 0f;
        //trigger sfx and vfx?
        //if (vfxOnShot != null)
        //{
        //    GameObject vfxInstance = Instantiate(vfxOnShot.Value, bulletInstance.transform.position, bulletInstance.transform.rotation);
        //}

        if (sfxOnShot != null)
        {
            Debug.Log("Playing Audio");

            //// Try randomizing the pitch before playing the clip
            //float minPitch = startPitch - (startPitch * 0.05f);
            //float maxPitch = startPitch + (startPitch * 0.05f);

            //float ranPitch = Random.Range(minPitch, maxPitch);

            //sfxOnShot.pitch = ranPitch;
            //sfxOnShot.PlayOneShot(sfxOnShot.clip);
            CustomAudio.PlayOnceWithPitch(sfxOnShot, onShotStartPitch);
        }
    }

    virtual protected GameObject Fire(AmmoType ammoType)
    {//Should EXCLUSIVELY be creating and returning whatever bullet gets created, NOTHING ELSE
     //Find the exact hit position using a raycast
     //ViewportPointToRay(0.5,0.5,0) = middle of the screen
        Ray ray = new(gunBulletPoint.transform.position, gunBulletPoint.transform.forward);
        Vector3 targetPoint;
        targetPoint = ray.GetPoint(inaccuracyRange.Value); //Just a point far away from the player

        //Calculate direction from attackPoint to targetPoint
        Vector3 directionWithoutSpread = targetPoint - gunBulletPoint.transform.position;
        Vector3 directionWithSpread = directionWithoutSpread + DetermineInaccuracy();

        var bulletInstance = Instantiate(ammoType.bulletObj.Value, gunBulletPoint.transform.position, Quaternion.identity);

        var bulletTrans = bulletInstance.transform;
        var bulletRB = bulletInstance.GetComponent<Rigidbody>();
        bulletTrans.forward = directionWithSpread.normalized;

        if (bulletRB != null)
        {
            bulletRB.velocity = Vector3.zero;
            bulletRB.AddForce(directionWithSpread.normalized * shotForce.Value, ForceMode.Impulse);
        }

        return bulletInstance;
    }

    protected Vector3 DetermineInaccuracy()
    {
        //when you shoot, it should move the gun closer to the camera
        //transform.localPosition -= Vector3.forward * 0.1f;

        float xRecoil = Random.Range(-inaccuracyRadius.Value.x, inaccuracyRadius.Value.x);
        float yRecoil = Random.Range(-inaccuracyRadius.Value.y, inaccuracyRadius.Value.y);

        Vector3 recoilDir = new Vector3(xRecoil, yRecoil, 0f);

        return recoilDir;
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
        if(sfxOnReload != null) 
        {
            CustomAudio.PlayOnceWithPitch(sfxOnReload, onReloadStartPitch);
        }

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