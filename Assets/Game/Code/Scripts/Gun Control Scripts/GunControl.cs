using Cinemachine;
using GeneralUtility.GameEventSystem;
using GeneralUtility.VariableObject;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class InputHandler : MonoBehaviour
{
    [Header("Gun Input Events")]
    public GameEvent inputEvPriFire, inputEvAdsPress, inputEvReload, inputEvPriRelease, inputEvAdsRelease;
    public void PrimaryFire(InputAction.CallbackContext context)
    {
        if (context.started)
            inputEvPriFire.Trigger();
    }

    public void AimDownSightsPressed(InputAction.CallbackContext context)
    {
        if (context.started)
            inputEvAdsPress.Trigger();
    }

    public void PrimaryRelease(InputAction.CallbackContext context)
    {
        if (context.canceled)
            inputEvPriRelease.Trigger();
    }

    public void AimDownSightsRelease(InputAction.CallbackContext context)
    {
        if (context.canceled)
            inputEvAdsRelease.Trigger();
    }
}

public class GunControl : MonoBehaviour
{
    private Camera mainCamera;
    private Vector2 mouseInput;

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
    [SerializeField] private BoolReference isReloading;    

    [Header("Gun Stats")]
    [SerializeField] private IntReference baseDamage;
    [SerializeField] private FloatReference fireRate, reloadTime;
    [SerializeField] private int clipSize = 1;
    public int ClipSize => clipSize;
    [SerializeField] private float maxHeat = 100f;
    public float MaxHeat => maxHeat;
    [SerializeField] private float cooldownSpeed = 1f, overheatSpeed = 1f;
    [SerializeField] private FloatReference overheatRate;

    [Header("References")]
    [SerializeField] private GameObject gunCamera;
    [SerializeField] private GameObject gunBase, gunBulletPoint;
    [SerializeField] private GameObjectReference bulletPrefab;
    [SerializeField] private GameObject vfxPrefab;
    [SerializeField] private GameEvent zoomEnterEvent, zoomExitEvent;

    private void OnDisable()
    {
        HeatChangeEvent = null;
        AmmoChangeEvent = null;
    }

    private void Start()
    {
        mainCamera = Camera.main;
        currentClip = clipSize;
        AmmoChangeEvent?.Invoke(currentClip);
    }

    private void Update()
    {
        if (!isShooting)
        {
            if (currentHeat > 0 & !overHeating)
            {
                currentHeat -= cooldownSpeed * Time.deltaTime;
                HeatChangeEvent?.Invoke(currentHeat);
            }
            else if (overHeating && currentHeat > 0)
            {
                currentHeat -= overheatSpeed * Time.deltaTime;
                HeatChangeEvent?.Invoke(currentHeat);
            }

            if (currentHeat <= 0 && overHeating)
            {
                overHeating = false;
            }
        }

        if (isReloading.Value)
        {
            if (reloadTimer < reloadTime.Value)
            {
                reloadTimer += Time.deltaTime;
            }
            else if (currentClip < clipSize)
            {
                reloadTimer = 0;
                currentClip++;
                AmmoChangeEvent?.Invoke(currentClip);
            }
            else if (currentClip == clipSize)
            {
                isReloading.Value = false;
            }
        }
        else if (isShooting && Time.time >= fireRateTimer + (1 / fireRate.Value) && currentClip > 0)
        {
            Fire();
        }
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
            isReloading.Value = true;
        }

        else if (context.canceled)
        {
            isReloading.Value = false;
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

        if (vfxPrefab != null)
        {
            GameObject vxfInstance = Instantiate(vfxPrefab, bulletInstance.transform.position, bulletInstance.transform.rotation);
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

    }
}
