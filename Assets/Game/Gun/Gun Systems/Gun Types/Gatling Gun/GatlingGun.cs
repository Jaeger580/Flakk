using GeneralUtility.VariableObject;
using UnityEngine;

public class GatlingGun : GunType
{
    [Header("Overheat Settings")]
    [Tooltip("How much heat until the gun needs to cool down?")]
    [SerializeField] private FloatReference maxHeat;
    public float MaxHeat => maxHeat.Value;
    [Tooltip("How fast can the gun cool down prior to reaching max overheat? (in overheat per second)")]
    [SerializeField] private FloatReference preOverheatCoolingSpeed;
    [Tooltip("How fast can the gun cool down after overheating? (in overheat per second)")]
    [SerializeField] private FloatReference postOverheatCoolingSpeed;
    [Tooltip("How much overheat does the gun accumulate per shot?")]
    [SerializeField] private FloatReference overheatRate;

    private float currentHeat = 0f;
    [SerializeField] protected BoolReference overHeating;

    public delegate void OnHeatChange(float newHeat);
    public OnHeatChange HeatChangeEvent;

    protected override void OnDisable()
    {
        base.OnDisable();
        HeatChangeEvent = null;
    }

    protected override void Update()
    {
        if (fireTimer < 1f / fireRate.Value)
        {//increment timer if it's below the threshold
            fireTimer += Time.deltaTime;
        }

        if (isReloading.Value)
        {//if I'm pressing reload, handle that
            HandleReload();
        }
        else if (overHeating.Value)
        {//if I'm overheating, prevent firing and handle overheat
            HandleOverheat();
        }
        else if (isFiring.Value)
        {//if I'm firing, handle that
            HandleFire();
        }
        else
        {//otherwise also consider handling overheat
            HandleOverheat();
        }
    }

    protected override void HandleFire()
    {
        if (overHeating.Value) { Debug.Log("OVERHEATING"); return; }                          //if I'm overheating, return (TODO: add feedback)
        if (!currentMag.TryPeek(out var ammoType)) return;        //if there's no bullet in the magazine, return (TODO: add feedback)
        if (fireTimer < 1f / fireRate.Value) return;            //if the timer isn't ready, return

        var bulletInstance = Fire(ammoType);
        var imp = bulletInstance.GetComponent<ImpactBehavior>();
        imp.Initialize(ammoType.damage.Value);

        //Cleanup
        currentMag.Pop();
        fireTimer = 0f;
        //(TODO: add feedback)
        currentHeat += overheatRate.Value;
        HeatChangeEvent?.Invoke(currentHeat);

        if (currentHeat >= maxHeat.Value)
        {
            overHeating.Value = true;
            isFiring.Value = false;
        }
    }

    protected void HandleOverheat()
    {
        //if I have heat, reduce my currentHeat based on whether I'm overheating or not
        if (currentHeat > 0) currentHeat -= Time.deltaTime * (overHeating.Value ? postOverheatCoolingSpeed.Value : preOverheatCoolingSpeed.Value);

        //if I'm at 0 heat or below, zero-out heat and set overheating to false
        if (currentHeat <= 0 && overHeating.Value) { currentHeat = 0; overHeating.Value = false; }
    }

    override protected GameObject Fire(AmmoType ammoType)
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

        bulletRB.velocity = Vector3.zero;
        bulletTrans.forward = directionWithSpread.normalized;

        bulletRB.AddForce(directionWithSpread.normalized * shotForce.Value, ForceMode.Impulse);

        return bulletInstance;
    }
}