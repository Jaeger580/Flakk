using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using GeneralUtility;

public class PlayerLook : MonoBehaviour
{
    [SerializeField]
    private GameObject mainCamera;

    private Vector2 mouseInput;
    private float vertRotation;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        HandleLook(mouseInput);
    }

    private void HandleLook(Vector2 Input) 
    {
        var sensX = PlayerPrefs.GetFloat(GeneralUtility.MagicStrings.OPTIONS_X_SENS_BASE, 1f);
        var sensY = PlayerPrefs.GetFloat(GeneralUtility.MagicStrings.OPTIONS_Y_SENS_BASE, 1f);

        float horizRotation = Input.x * sensX;
        transform.Rotate(0, horizRotation, 0);
        
        vertRotation -= Input.y * sensY;
        vertRotation = Mathf.Clamp(vertRotation, -90, 90);
        
        mainCamera.transform.localRotation = Quaternion.Euler(vertRotation, 0, 0);
    }

    public void Look(InputAction.CallbackContext context) 
    {
        mouseInput = context.ReadValue<Vector2>();
    }
}
