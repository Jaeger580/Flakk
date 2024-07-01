using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunControl : MonoBehaviour
{
    [SerializeField]
    private float sensitivity = 5f;

    [SerializeField]
    private float gunRotateSpeed = 5f;

    private Camera mainCamera;

    [SerializeField]
    private GameObject gunCamera;

    [SerializeField]
    private GameObject gunBase;
    [SerializeField]
    private GameObject gunBarrel;
    [SerializeField]
    private GameObject gunBulletPoint;

    private Vector2 mouseInput;
    private float vertRotation;
    private float horizRotation;

    [SerializeField]
    private GameObject bulletPrefab;

    private void Start()
    {
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    private void Update()
    {
        HandleLook(mouseInput);
    }

    private void HandleLook(Vector2 Input)
    {
        horizRotation += Input.x * sensitivity * Time.deltaTime;
  
        //transform.Rotate(0, horizRotation, 0);
        //gunBase.transform.Rotate(0, horizRotation, 0);

        vertRotation -= Input.y * sensitivity * Time.deltaTime;
        vertRotation = Mathf.Clamp(vertRotation, -85, 0);

        //mainCamera.transform.Rotate(0, horizRotation, 0);
        gunCamera.transform.localRotation = Quaternion.Euler(vertRotation, horizRotation, 0);

        gunBase.transform.rotation = Quaternion.RotateTowards(gunBase.transform.rotation, gunCamera.transform.rotation, gunRotateSpeed * Time.deltaTime);
    }

    public void Look(InputAction.CallbackContext context)
    {
        mouseInput = context.ReadValue<Vector2>();
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        Quaternion gunRotation = gunBase.transform.rotation;

        gunRotation *= Quaternion.Euler(90,0,0);

        Vector3 camCenter = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, 0));
        GameObject bulletInstance = Instantiate(bulletPrefab, gunBulletPoint.transform.position, gunRotation);
    }
}
