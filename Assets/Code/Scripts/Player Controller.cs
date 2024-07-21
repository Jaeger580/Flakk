using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1f;

    [SerializeField]
    private float gravityValue = -9.81f;

    [SerializeField]
    private float jumpValue = 2f;

    private CharacterController charControl;
    private Vector3 moveDir = Vector3.zero;

    private bool jumped = false;
    private float vel = 0f;

    private PlayerInput playInput;
    private CinemachineVirtualCamera virtualCamera;

    private bool isMoving; //for audio
    private float footStepTimer;
    private float footStepMax = 0.75f;

    // Start is called before the first frame update
    void Start()
    {
        charControl = this.GetComponent<CharacterController>();
        playInput = this.GetComponent<PlayerInput>();
        virtualCamera = this.GetComponentInChildren<CinemachineVirtualCamera>();

        footStepTimer = footStepMax;
    }

    private void Update()
    {
        if(footStepTimer < footStepMax) 
        {
            footStepTimer += Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        vel += gravityValue * Time.deltaTime;

        if (jumped) 
        {
            vel = Mathf.Sqrt(jumpValue * -2 * gravityValue);
            jumped = false;
        }

        moveDir.y = vel;

        if (isMoving && footStepTimer >= footStepMax) 
        {
            AudioManager.instance.PlayRando();
            footStepTimer = 0;
        }

        charControl.Move(transform.rotation * moveDir * Time.deltaTime);
        
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 rawInput = context.ReadValue<Vector2>();
        moveDir = new Vector3(rawInput.x, 0, rawInput.y);
        moveDir.x *= moveSpeed;
        moveDir.z *= moveSpeed;

        if(context.started)
            isMoving = true;
        
        if(context.canceled)
            isMoving = false;

    }

    public void Jump(InputAction.CallbackContext context) 
    {
        if (context.started) 
        {
            if (charControl.isGrounded && !jumped)
            {
                jumped = true;
            }
        }   
    }

    public void ExitGun()
    {
        playInput.SwitchCurrentActionMap("Hub");
        virtualCamera.Priority = 10;
        AudioManager.instance.setVolume("BGM", 0.05f);
    }
}
