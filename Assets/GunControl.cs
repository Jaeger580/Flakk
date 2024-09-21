using Cinemachine;
using GeneralUtility.EditorQoL;
using GeneralUtility.GameEventSystem;
using GeneralUtility.VariableObject;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunControl : MonoBehaviour
{
    private Camera mainCamera;
    private Vector2 mouseInput;
    private float vertRotation, gunVertRotation;
    private float horizRotation, gunHorizRotation;

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
    private bool isReloading = false;

    [Header("Game Objects")]
    [SerializeField]
    private GameObject gunCamera;
    [SerializeField]
    private GameObject pivotPoint;
    [SerializeField]
    private GameObject gunBase;
    //[SerializeField]
    //private GameObject gunBarrel;
    [SerializeField]
    private GameObject gunBulletPoint;
    [SerializeField]
    private GameObjectReference bulletPrefab;

    [Header("Weapon Movement")]
    [SerializeField] //[ReadOnly]
    private float xSens, ySens;
    [SerializeField]
    private float sensitivityScaler;
    [SerializeField] private GameEvent sensitivityChangedEvent;
    [SerializeField]
    private FloatReference gunRotateSpeed;

    [Header("Gun Stats")]
    [SerializeField]
    private IntReference baseDamage;
    [SerializeField]
    private FloatReference fireRate;
    [SerializeField]
    private FloatReference reloadTime;
    [SerializeField]
    private int clipSize = 1;
    public int ClipSize => clipSize;
    [SerializeField]
    private float maxHeat = 100f;
    public float MaxHeat => maxHeat;
    [SerializeField]
    private float coolDownSpeed = 1f;
    [SerializeField]
    private float overHeatSpeed = 1f;
    [SerializeField]
    private FloatReference overheatRate;

    [SerializeField]
    public GameObject vfxPrefab;

    [SerializeField] private GameEvent zoomEnterEvent, zoomExitEvent;

    [SerializeField] private AnimationCurve gunCatchUpCurve;

    private void OnDisable()
    {
        HeatChangeEvent = null;
        AmmoChangeEvent = null;
    }

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

    private void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        //maxFireRate = fireRate.Value;
        //fireRateTimer = Time.time;
        currentClip = clipSize;
        AmmoChangeEvent?.Invoke(currentClip);
    }

    private void Update()
    {
        //if (Time.time >= fireRateTimer + maxFireRate)
        //{
        //    fireRateTimer = Time.time;
        //}

        if (!isShooting)
        {
            if (currentHeat > 0 & !overHeating)
            {
                currentHeat -= coolDownSpeed * Time.deltaTime;
                HeatChangeEvent?.Invoke(currentHeat);
            }
            else if (overHeating && currentHeat > 0)
            {
                currentHeat -= overHeatSpeed * Time.deltaTime;
                HeatChangeEvent?.Invoke(currentHeat);
            }

            if (currentHeat <= 0 && overHeating)
            {
                overHeating = false;
            }
        }

        if (isReloading)
        {
            if (reloadTimer < reloadTime.Value)
            {
                reloadTimer += Time.deltaTime;
            }
            else if (currentClip < clipSize)
            {
                reloadTimer = 0;
                currentClip++;
                //AudioManager.instance.ForcePlay("Reload");
                AmmoChangeEvent?.Invoke(currentClip);
            }
            else if (currentClip == clipSize)
            {
                isReloading = false;
            }
        }
        else if (isShooting && Time.time >= fireRateTimer + (1 / fireRate.Value) && currentClip > 0)
        {
            Fire();
        }
    }

    private void LateUpdate()
    {
        
    }

    private void FixedUpdate()
    {
        // Move the camera
        CamLook(mouseInput);
        Vector2 random = new(Random.Range(-180f, -140f), 0f);
        //CamLook(random);

        // Realign the gun
        if (!isReloading)
        {
            GunLook(mouseInput);
            //GunLook(random);
        }
    }

    private void CamLook(Vector2 input)
    {
        input.x = Mathf.Clamp(input.x, -80f, 80f);

        horizRotation += input.x * xSens * Time.fixedDeltaTime;
        //while (horizRotation > 180f) { horizRotation -= 360f; }
        //while (horizRotation < -180f) { horizRotation += 360f; }
        //horizRotation = Mathf.Repeat(horizRotation, 360f) - 180f;

        //print(horizRotation);

        vertRotation -= input.y * ySens * Time.fixedDeltaTime;
        vertRotation = Mathf.Clamp(vertRotation, -85f, 15f);

        var gunBaseY = gunBase.transform.rotation.eulerAngles.y;
        if (gunBaseY > 180f) gunBaseY -= 360f;
        else if (gunBaseY < -180f) gunBaseY += 360f;
        //print($"{gunBaseY}"); 

        float angleOffset = 45f;
        float negativeDiff = Mathf.Abs(-180f - gunBaseY);
        float positiveDiff = Mathf.Abs(180f - gunBaseY);

        //if (Mathf.Abs(horizRotation - gunBaseY) > 180f)
        //{//If the difference is too big, we know that horizRotation wrapped when gunBaseY didn't (or vice versa)
        //    if (horizRotation > gunBaseY)
        //    {
        //        print($"Clamping! {horizRotation} > {gunBaseY} || Min {180f - angleOffset - negativeDiff} || After Clamp: {Mathf.Clamp(horizRotation, 180f - angleOffset - negativeDiff, 180f)}");
        //        horizRotation = Mathf.Clamp(horizRotation, 180f - negativeDiff - angleOffset, 180f);   //if horiz passed left over 360 line, clamp it to a minimum of 330f
        //    }
        //    else if (horizRotation < gunBaseY)
        //    {
        //        //print($"Clamping! {horizRotation} < {gunBaseY} || Diff {Mathf.Abs(horizRotation - gunBaseY)} || After Clamp: {Mathf.Clamp(horizRotation, -180f, -180f + angleOffset + diff)}");
        //        horizRotation = Mathf.Clamp(horizRotation, -180f, -180f + angleOffset + negativeDiff); //if horiz passed right over 0 line, clamp it to a max of 30f
        //    }
        //}
        //else if (Mathf.Abs(horizRotation - gunBaseY) > 25f)
        //{
        //    //print($"Clamping! {horizRotation} --- {gunBaseY} After Clamp: {Mathf.Clamp(horizRotation, gunBaseY - angleOffset, gunBaseY + angleOffset)}");
        //    horizRotation = Mathf.Clamp(horizRotation, gunBaseY - angleOffset, gunBaseY + angleOffset);
        //}

        var pivotRot = Quaternion.Euler(vertRotation, horizRotation, 0f);
        var truRot = Quaternion.RotateTowards(pivotPoint.transform.localRotation, pivotRot, 20f);
        //pivotPoint.transform.localRotation.Rot

        //var truEul = truRot.eulerAngles;
        //if(truEul.y > 180f)
        //{
        //    truEul.y -= 360f;
        //}
        //else if (truEul.y < -180f)
        //{
        //    truEul.y += 360f;
        //}

        //truRot = Quaternion.Euler(truEul);

        pivotPoint.transform.localRotation = truRot;

        //print(horizRotation);
    }

    // Starts off faster than slows down to normal pace as approaches center.
    private void GunLook(Vector2 input)
    {//TODO: CURRENT ISSUE: when crossing the 180-line, RotateTowards fucks up and starts going the opposite direction
     //because it thinks the faster way to rotate to the correct rotation is in the opposite direction
        var angle = Quaternion.Angle(gunBase.transform.rotation, pivotPoint.transform.rotation);
        float clamp = 32f;
        var speed = gunCatchUpCurve.Evaluate(angle / clamp);
        gunHorizRotation += Mathf.Sign(horizRotation - gunHorizRotation) * xSens * Time.fixedDeltaTime * gunRotateSpeed.Value * speed;
        gunHorizRotation = Mathf.Clamp(gunHorizRotation, horizRotation - clamp, horizRotation + clamp);
        gunVertRotation -= Mathf.Sign(horizRotation - gunHorizRotation) * ySens * Time.fixedDeltaTime * gunRotateSpeed.Value * speed;
        gunVertRotation = Mathf.Clamp(vertRotation, -85f, 15f);

        //directional input * sensitivity * time since last physics frame * how fast the gun should rotate * distanceScalar

        //var newRot = Quaternion.RotateTowards(gunBase.transform.rotation, pivotPoint.transform.rotation, gunRotateSpeed.Value * speed).eulerAngles;
        //var nRot = Quaternion.Slerp(gunBase.transform.rotation, pivotPoint.transform.rotation, speed * gunRotateSpeed.Value * Time.fixedDeltaTime);
        //if(Quaternion.Angle(gunBase.transform.rotation, ))
        //var lookRot = Quaternion.LookRotation(pivotPoint.transform.forward, pivotPoint.transform.up);
        var gunRot = Quaternion.Euler(gunVertRotation, gunHorizRotation, 0f);
        var truRot = Quaternion.RotateTowards(gunBase.transform.localRotation, gunRot, 20f);

        if(input.x <= 0.01f && input.y <= 0.01f)
        {
            gunBase.transform.rotation = pivotPoint.transform.rotation;
        }
        else
        {
            gunBase.transform.rotation = truRot;
        }
        //gunBase.transform.rotation = Quaternion.RotateTowards(gunBase.transform.rotation, pivotPoint.transform.rotation, gunRotateSpeed.Value * speed);

        //find the desired up direction:
        //Vector3 targetUpDirection = pivotPoint.transform.up;
        //face forward/upward via black magic
        //gunBase.transform.rotation = Quaternion.LookRotation(targetUpDirection, gunBase.transform.forward);
        //gunBase.transform.rotation = Quaternion.LookRotation(gunBase.transform.up, gunBase.transform.forward);
        //gunBase.transform.rotation = Quaternion.RotateTowards(gunBase.transform.rotation, lookRot, gunRotateSpeed.Value * speed);
    }

    //private void HandleLook(Vector2 Input)
    //{
    //    horizRotation += Input.x * sensitivity * Time.deltaTime;
    //    horizRotation = Mathf.Repeat(horizRotation, 360f);

    //    vertRotation -= Input.y * sensitivity * Time.deltaTime;
    //    vertRotation = Mathf.Clamp(vertRotation, -85f, 15f);

    //    var pivotRot = Quaternion.Euler(vertRotation, horizRotation, 0f);

    //    var angle = Quaternion.Angle(gunBase.transform.rotation, pivotPoint.transform.rotation);

    //    gunBase.transform.rotation = Quaternion.RotateTowards(gunBase.transform.rotation, pivotRot, gunRotateSpeed.Value * Time.deltaTime * angle);

    //    //vertRotation = Mathf.Clamp(vertRotation, gunBase.transform.rotation.eulerAngles.x - 25f, gunBase.transform.rotation.eulerAngles.x + 25f);

    //    var gunBaseY = gunBase.transform.rotation.eulerAngles.y;
    //    if (Mathf.Abs(horizRotation - gunBaseY) > 180f)
    //    {//If the difference is too big, we know that horizRotation wrapped when gunBaseY didn't (or vice versa)
    //        if (horizRotation > gunBaseY)
    //        {
    //            //print($"Clamping! {horizRotation} > {gunBaseY}After Clamp: {Mathf.Clamp(horizRotation, 330f, 360f)}");
    //            horizRotation = Mathf.Clamp(horizRotation, 330f, 360f);   //if horiz passed left over 360 line, clamp it to a minimum of 330f
    //        }
    //        else if (horizRotation < gunBaseY)
    //        {
    //            //print($"Clamping! {horizRotation} < {gunBaseY} || After Clamp: {Mathf.Clamp(horizRotation, 0f, 30f)}");
    //            horizRotation = Mathf.Clamp(horizRotation, 0f, 30f); //if horiz passed right over 0 line, clamp it to a max of 30f
    //        }
    //    }
    //    else if (Mathf.Abs(horizRotation - gunBaseY) > 25f)
    //    {
    //        //print($"Clamping! {horizRotation} --- {gunBaseY} After Clamp: {Mathf.Clamp(horizRotation, gunBaseY - 30f, gunBaseY + 30f)}");
    //        horizRotation = Mathf.Clamp(horizRotation, gunBaseY - 30f, gunBaseY + 30f);
    //    }

    //    pivotRot = Quaternion.Euler(vertRotation, horizRotation, 0f);

    //    pivotPoint.transform.localRotation = pivotRot;
    //}

    public void Look(InputAction.CallbackContext context)
    {
        mouseInput = context.ReadValue<Vector2>();
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
            isReloading = true;
        }

        else if (context.canceled)
        {
            isReloading = false;
        }
    }

    public void Zoom(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            CinemachineVirtualCamera vCam = gunCamera.GetComponent<CinemachineVirtualCamera>();

            vCam.m_Lens.FieldOfView = 36f; // Calculated as Vertical for some reason. needs fixed

            Vector3 camPos = gunCamera.transform.localPosition;
            gunCamera.transform.localPosition = new Vector3(camPos.x, camPos.y, camPos.z + 2);

            zoomEnterEvent?.Trigger();
        }

        else if (context.canceled)
        {
            CinemachineVirtualCamera vCam = gunCamera.GetComponent<CinemachineVirtualCamera>();

            vCam.m_Lens.FieldOfView = 52f; // Calculated as Vertical for some reason. needs fixed

            Vector3 camPos = gunCamera.transform.localPosition;
            gunCamera.transform.localPosition = new Vector3(camPos.x, camPos.y, camPos.z - 2);

            zoomExitEvent?.Trigger();
        }
    }

    private void Fire()
    {
        Quaternion gunRotation = gunBase.transform.rotation;

        gunRotation *= Quaternion.Euler(90, 0, 0);

        Vector3 camCenter = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, 0));
        GameObject bulletInstance = Instantiate(bulletPrefab.Value, gunBulletPoint.transform.position, gunRotation);
        //bulletInstance.GetComponent<Bullet>().SetDamage(baseDamage.Value);


        if (vfxPrefab != null)
        {
            GameObject vxfInstance = Instantiate(vfxPrefab, bulletInstance.transform.position, bulletInstance.transform.rotation);
        }

        //if (AudioManager.instance != null)
        //{
        //    AudioManager.instance.ForcePlay("Shoot");
        //}

        currentClip--;
        AmmoChangeEvent?.Invoke(currentClip);
        currentHeat += overheatRate.Value;
        HeatChangeEvent?.Invoke(currentHeat);

        if (currentHeat >= maxHeat)
        {
            overHeating = true;
            isShooting = false;
        }

        fireRateTimer = Time.time;

    }
}
