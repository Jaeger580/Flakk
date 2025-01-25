using GeneralUtility.GameEventSystem;
using GeneralUtility.VariableObject;
using UnityEngine;

public class Railgun : GunType
{
    [Header("Railgun Settings")]
    [SerializeField] private BoolReference fullyCharged;
    [Tooltip("Max charge of the gun in \"points\", for lack of a better term.")]
    [SerializeField] private FloatReference maxCharge;
    [Tooltip("Current charge of the gun. Exposed for potential access elsewhere.")]
    [SerializeField] private FloatReference currentCharge;
    [Tooltip("Rate of charge increase, in \"points\" per second.")]
    [SerializeField] private FloatReference chargeRate;
    [SerializeField] private AnimationCurveReference chargeDamageCurve;

    public delegate void OnChargeChange(float newCharge);
    public OnChargeChange ChargeChangeEvent;

    protected override void Start()
    {
        base.Start();
        var primaryReleaseListener = gameObject.AddComponent<GameEventListener>();
        primaryReleaseListener.Events.Add(gunSetup.inputEvPriRelease);
        primaryReleaseListener.Response = new();
        primaryReleaseListener.Response.AddListener(() => HandleFire());
        gunSetup.inputEvPriRelease.RegisterListener(primaryReleaseListener);
    }

    protected override void Update()
    {
        if (isReloading.Value)
        {//if I'm pressing reload, handle that
            HandleReload();
            CurrentCharge = 0f;
        }
        else if (isFiring.Value && CurrentCharge < maxCharge.Value)
        {//if I'm holding fire, increment charge
            CurrentCharge += Time.deltaTime * chargeRate.Value;
        }
        else
        {
            CurrentCharge = 0f;
        }
    }

    protected override void HandleFire()
    {
        if (!currentMag.TryPeek(out var ammoType)) return;      //if there's no bullet in the magazine, return
        if (fireTimer < 1f / fireRate.Value) return;          //if the timer isn't ready, return

        var bulletInstance = Fire(ammoType);
        var imps = bulletInstance.GetComponents<RailGunAmmo>();
        foreach(var imp in imps)
        {
            imp.Initialize(ammoType.damage.Value * chargeDamageCurve.Value.Evaluate(currentCharge.Value / maxCharge.Value), ammoType.damage.Value);
        }

        //Cleanup
        currentMag.Pop();
        if (currentMag == primaryMag)
        {
            PrimaryMagAmmoChangeEvent?.Invoke(primaryMag.stack.Count, primaryMag.maxStackSize.Value);
        }
        else
        {
            SecondaryMagAmmoChangeEvent?.Invoke(secondaryMag.stack.Count, secondaryMag.maxStackSize.Value);
        }
        CurrentCharge = 0f;

        //trigger sfx and vfx?
        //if (vfxOnShot != null)
        //{
        //    GameObject vfxInstance = Instantiate(vfxOnShot.Value, bulletInstance.transform.position, bulletInstance.transform.rotation);
        //}
    }

    public float CurrentCharge
    {
        get { return currentCharge.Value; }
        set
        {
            value = Mathf.Clamp(value, 0f, maxCharge.Value);
            if(currentCharge.Value != value)
            {
                currentCharge.Value = value;
                ChargeChangeEvent?.Invoke(currentCharge.Value);
            }

            if(currentCharge.Value >= maxCharge.Value)
            {
                fullyCharged.Value = true;
                //trigger max charge event?
            }
        }
    }
}