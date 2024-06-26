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

    private CharacterController charControl;
    private Vector3 moveDir = Vector3.zero;

    //private float jumpDuration = 0.25f;
    //private float jumpTime = 0f;
    private bool jumped = false;

    private float jumpValue = 3f;
    private float vel = 0f;

    // Start is called before the first frame update
    void Start()
    {
         charControl = this.GetComponent<CharacterController>();
    }

    private void Update()
    {
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

        charControl.Move(transform.rotation * moveDir * Time.deltaTime);
    }

    public void Move(InputAction.CallbackContext context)
    {
        Vector2 rawInput = context.ReadValue<Vector2>();
        moveDir = new Vector3(rawInput.x, 0, rawInput.y);
        moveDir.x *= moveSpeed;
        moveDir.z *= moveSpeed;
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
}
