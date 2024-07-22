using Cinemachine;
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
    private float vertRotation;
    private float horizRotation;

    private float maxFireRate;
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
    [SerializeField]
    private float sensitivity = 5f;
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
    public ParticleSystem[] vfx=new ParticleSystem[5];

    private void OnDisable()
    {
        HeatChangeEvent = null;
        AmmoChangeEvent = null;
    }

    private void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        maxFireRate = fireRate.Value;
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

            if(currentHeat <= 0 && overHeating) 
            {
                overHeating = false;
            }
        }

        // Move the camera
        CamLook(mouseInput);

        // Realign the gun
        if (!isReloading)
        {
            GunLook();
        }

        if (isShooting && Time.time >= fireRateTimer + (1 / maxFireRate) && currentClip > 0)
        {
            Fire();
        }
        else if(isReloading)
        {
            if(reloadTimer < reloadTime.Value) 
            {
                reloadTimer += Time.deltaTime;
            }
            else if(currentClip < clipSize)
            {
                reloadTimer = 0;
                currentClip++;
                AudioManager.instance.ForcePlay("Reload");
                AmmoChangeEvent?.Invoke(currentClip);
            }
            else if(currentClip == clipSize) 
            {
                isReloading = false;
            }
        }
    }

    private void CamLook(Vector2 Input)
    {        
        horizRotation += Input.x * sensitivity * Time.deltaTime;
        horizRotation = Mathf.Repeat(horizRotation, 360f);

        vertRotation -= Input.y * sensitivity * Time.deltaTime;
        vertRotation = Mathf.Clamp(vertRotation, -85f, 15f);

        var gunBaseY = gunBase.transform.rotation.eulerAngles.y;
        if (Mathf.Abs(horizRotation - gunBaseY) > 180f)
        {//If the difference is too big, we know that horizRotation wrapped when gunBaseY didn't (or vice versa)
            if (horizRotation > gunBaseY)
            {
                //print($"Clamping! {horizRotation} > {gunBaseY}After Clamp: {Mathf.Clamp(horizRotation, 330f, 360f)}");
                horizRotation = Mathf.Clamp(horizRotation, 330f, 360f);   //if horiz passed left over 360 line, clamp it to a minimum of 330f
            }
            else if (horizRotation < gunBaseY)
            {
                //print($"Clamping! {horizRotation} < {gunBaseY} || After Clamp: {Mathf.Clamp(horizRotation, 0f, 30f)}");
                horizRotation = Mathf.Clamp(horizRotation, 0f, 30f); //if horiz passed right over 0 line, clamp it to a max of 30f
            }
        }
        else if (Mathf.Abs(horizRotation - gunBaseY) > 25f)
        {
            //print($"Clamping! {horizRotation} --- {gunBaseY} After Clamp: {Mathf.Clamp(horizRotation, gunBaseY - 30f, gunBaseY + 30f)}");
            horizRotation = Mathf.Clamp(horizRotation, gunBaseY - 30f, gunBaseY + 30f);
        }

        var pivotRot = Quaternion.Euler(vertRotation, horizRotation, 0f);

        pivotPoint.transform.localRotation = pivotRot;
    }

    // Starts off faster than slows down to normal pace as approaches center.
    private void GunLook()
    {
        var angle = Quaternion.Angle(gunBase.transform.rotation, pivotPoint.transform.rotation);

        gunBase.transform.rotation = Quaternion.RotateTowards(gunBase.transform.rotation, pivotPoint.transform.rotation, gunRotateSpeed.Value * Time.deltaTime * angle);
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
        }

        else if (context.canceled)
        {
            CinemachineVirtualCamera vCam = gunCamera.GetComponent<CinemachineVirtualCamera>();

            vCam.m_Lens.FieldOfView = 52f; // Calculated as Vertical for some reason. needs fixed

            Vector3 camPos = gunCamera.transform.localPosition;
            gunCamera.transform.localPosition = new Vector3(camPos.x, camPos.y, camPos.z - 2);
        }
    }

    private void Fire() 
    {
        Quaternion gunRotation = gunBase.transform.rotation;

        gunRotation *= Quaternion.Euler(90, 0, 0);

        Vector3 camCenter = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, 0));
        GameObject bulletInstance = Instantiate(bulletPrefab.Value, gunBulletPoint.transform.position, gunRotation);
        bulletInstance.GetComponent<Bullet>().SetDamage(baseDamage.Value);

        if(AudioManager.instance != null) 
        {
            AudioManager.instance.ForcePlay("Shoot");
        }

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

        foreach(ParticleSystem ps in vfx)
        {
            ps.Play();
        }
        
    }
}
