/* Jakob Jaeger
 * 11/21/2024
 * Script that will move the main gun up and down.
 */

using GeneralUtility.GameEventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GunVerticalMovement : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1;

    [SerializeField]
    private float maxHeight = 100;
    [SerializeField]
    private float minHeight = 0;

    [SerializeField]
    private GameEvent moveUpPress, moveUpRelease;
    [SerializeField]
    private GameEvent moveDownPress, moveDownRelease;

    private float moveValue = 0f;

    public void Move(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            moveValue = context.ReadValue<float>();
            Debug.Log(moveValue);
        }
        else if (context.canceled)
        {
            moveValue = 0;
        }
    }

    private void FixedUpdate()
    {
        var speed = moveSpeed * Time.deltaTime;

        if (moveValue == 1)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, maxHeight, 0), moveSpeed);
        }
        else if (moveValue == -1)
        {
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, minHeight, 0), moveSpeed);
        }

        //if(moveValue == 1) 
        //{
        //    transform.position = new Vector3(0, Mathf.Lerp(minHeight, maxHeight, speed), 0);
        //}
        //else if(moveValue == -1) 
        //{
        //    transform.position = new Vector3(0, Mathf.Lerp(maxHeight, minHeight, speed), 0);
        //}
    }
}
