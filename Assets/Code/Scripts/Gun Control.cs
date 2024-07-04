using Cinemachine;
using GeneralUtility.VariableObject;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunControl : MonoBehaviour
{

    private Camera mainCamera;
    private Vector2 mouseInput;
    private float vertRotation;
    private float horizRotation;

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
    [SerializeField]
    private GameObject gunBarrel;
    [SerializeField]
    private GameObject gunBulletPoint;
    [SerializeField]
    private GameObject bulletPrefab;

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






    private void OnDisable()
    {
        HeatChangeEvent = null;
        AmmoChangeEvent = null;
    }

    private void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
        fireRateTimer = fireRate.Value;
        currentClip = clipSize;
        AmmoChangeEvent.Invoke(currentClip);
    }

    private void Update()
    {
        if (fireRateTimer < fireRate.Value)
        {
            fireRateTimer += Time.deltaTime;
        }

        if (!isShooting)
        {
            if (currentHeat > 0 & !overHeating)
            {
                currentHeat -= coolDownSpeed * Time.deltaTime;
                HeatChangeEvent.Invoke(currentHeat);
            }
            else if (overHeating && currentHeat > 0) 
            {
                currentHeat -= overHeatSpeed * Time.deltaTime;
                HeatChangeEvent.Invoke(currentHeat);
            }

            if(currentHeat <= 0 && overHeating) 
            {
                overHeating = false;
            }
        }

        if (!isReloading) 
        {
            HandleLook(mouseInput);

            if (isShooting && fireRateTimer >= fireRate.Value && currentClip > 0)
            {
                Fire();
            }
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
                AmmoChangeEvent.Invoke(currentClip);
            }
            else if(currentClip == clipSize) 
            {
                isReloading = false;
            }
        }
    }

    // Starts off faster than slows down to normal pace as approaches center.

    private void HandleLook(Vector2 Input)
    {
        horizRotation += Input.x * sensitivity * Time.deltaTime;
  
        //transform.Rotate(0, horizRotation, 0);
        //gunBase.transform.Rotate(0, horizRotation, 0);

        vertRotation -= Input.y * sensitivity * Time.deltaTime;
        vertRotation = Mathf.Clamp(vertRotation, -85, 0);

        //mainCamera.transform.Rotate(0, horizRotation, 0);
        pivotPoint.transform.localRotation = Quaternion.Euler(vertRotation, horizRotation, 0);

        var angle = Quaternion.Angle(gunBase.transform.rotation, pivotPoint.transform.rotation);

        gunBase.transform.rotation = Quaternion.RotateTowards(gunBase.transform.rotation, pivotPoint.transform.rotation, gunRotateSpeed.Value * Time.deltaTime * angle);
    }

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

    private void Fire() 
    {
        Quaternion gunRotation = gunBase.transform.rotation;

        gunRotation *= Quaternion.Euler(90, 0, 0);

        Vector3 camCenter = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, 0));
        GameObject bulletInstance = Instantiate(bulletPrefab, gunBulletPoint.transform.position, gunRotation);
        bulletInstance.GetComponent<Bullet>().SetDamage(baseDamage.Value);

        currentClip--;
        AmmoChangeEvent.Invoke(currentClip);
        currentHeat += overheatRate.Value;
        HeatChangeEvent.Invoke(currentHeat);

        if (currentHeat >= maxHeat) 
        {
            overHeating = true;
            isShooting = false;
        }

        fireRateTimer = 0;
    }
}
