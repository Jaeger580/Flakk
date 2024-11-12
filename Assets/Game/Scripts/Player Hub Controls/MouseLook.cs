using GeneralUtility.GameEventSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    // A mouselook behaviour with constraints which operate relative to
    // this gameobject's initial rotation.
    // Only rotates around local X and Y.
    // Works in local coordinates, so if this object is parented
    // to another moving gameobject, its local constraints will
    // operate correctly
    // To have no constraints on an axis, set the rotationRange to 360 or greater.
    public Vector2 rotationRange = new Vector3(70, 70);
    public float rotationSpeed = 10;
    public float dampingTime = 0.2f;
    public bool autoZeroVerticalOnMobile = true;
    public bool autoZeroHorizontalOnMobile = false;
    public bool relative = true;

    private Vector3 m_TargetAngles;
    private Vector3 m_FollowAngles;
    private Vector3 m_FollowVelocity;
    private Quaternion m_PlayerOriginalRotation, m_CameraOriginalRotation;

    private Vector2 sensitivity;
    public Transform playerBody, cameraBody;
    public GameEvent exitOptionsEvent, inputEVLook;

    //float xRotation = 0f, yRotation;

    private void Start()
    {
        m_PlayerOriginalRotation = playerBody.localRotation;
        m_CameraOriginalRotation = cameraBody.localRotation;

        Cursor.lockState = CursorLockMode.Locked;
        Sensitivity = new Vector2(PlayerPrefs.GetFloat("xUnscopedSensitivity", 1f), PlayerPrefs.GetFloat("yUnscopedSensitivity", 1f));
        var senslistener = gameObject.AddComponent<GameEventListener>();
        senslistener.Events.Add(exitOptionsEvent);
        senslistener.Response = new();
        senslistener.Response.AddListener(UpdateSens);
        exitOptionsEvent.RegisterListener(senslistener);

        //var lookListener = gameObject.AddComponent<GameEventListener>();
        //lookListener.Event = inputEVLook;
        //lookListener.VecResponse = new();
        //lookListener.VecResponse.AddListener((Vector2 v) => Look(v));
        //lookListener.Event.RegisterListener(lookListener);

        //yRotation = playerBody.rotation.y;
    }

    private void Update()
    {
        // we make initial calculations from the original local rotation
        playerBody.localRotation = m_PlayerOriginalRotation;
        cameraBody.localRotation = m_CameraOriginalRotation;

        // read input from mouse or mobile controls
        Vector2 targetMouseDelta = Mouse.current.delta.ReadValue() * Time.smoothDeltaTime;

        float inputH = targetMouseDelta.x * sensitivity.x;
        float inputV = targetMouseDelta.y * sensitivity.y;
        if (relative)
        {
            // wrap values to avoid springing quickly the wrong way from positive to negative
            if (m_TargetAngles.y > 180)
            {
                m_TargetAngles.y -= 360;
                m_FollowAngles.y -= 360;
            }
            if (m_TargetAngles.x > 180)
            {
                m_TargetAngles.x -= 360;
                m_FollowAngles.x -= 360;
            }
            if (m_TargetAngles.y < -180)
            {
                m_TargetAngles.y += 360;
                m_FollowAngles.y += 360;
            }
            if (m_TargetAngles.x < -180)
            {
                m_TargetAngles.x += 360;
                m_FollowAngles.x += 360;
            }
            // with mouse input, we have direct control with no springback required.
            m_TargetAngles.y += inputH * rotationSpeed;
            m_TargetAngles.x += inputV * rotationSpeed;

            // clamp values to allowed range
            m_TargetAngles.y = Mathf.Clamp(m_TargetAngles.y, -rotationRange.y * 0.5f, rotationRange.y * 0.5f);
            m_TargetAngles.x = Mathf.Clamp(m_TargetAngles.x, -rotationRange.x * 0.5f, rotationRange.x * 0.5f);
        }
        else
        {
            // set values to allowed range
            m_TargetAngles.y = Mathf.Lerp(-rotationRange.y * 0.5f, rotationRange.y * 0.5f, inputH / Screen.width);
            m_TargetAngles.x = Mathf.Lerp(-rotationRange.x * 0.5f, rotationRange.x * 0.5f, inputV / Screen.height);
        }

        // smoothly interpolate current values to target angles
        m_FollowAngles = Vector3.SmoothDamp(m_FollowAngles, m_TargetAngles, ref m_FollowVelocity, dampingTime);

        // update the actual gameobject's rotation
        playerBody.localRotation = m_PlayerOriginalRotation * Quaternion.Euler(0, m_FollowAngles.y, 0);
        cameraBody.localRotation = m_CameraOriginalRotation * Quaternion.Euler(-m_FollowAngles.x, 0, 0);
    }

    private void UpdateSens()
    {
        Sensitivity = new Vector2(PlayerPrefs.GetFloat("xUnscopedSensitivity", 1f), PlayerPrefs.GetFloat("yUnscopedSensitivity", 1f));
    }

    public Vector2 Sensitivity
    {
        get { return sensitivity; }

        set
        {
            if (value.x >= 0f)
                sensitivity.x = value.x;
            if (value.y >= 0f)
                sensitivity.y = value.y;
        }
    }
}



