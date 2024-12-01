using System;
using GeneralUtility.GameEventSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class SCRATCH_PlayerController { }

public partial class InputHandler : MonoBehaviour
{
    [Header("Hub Player Look Events")]
    [SerializeField] private GameEvent inputEVHubLook;

    public void HubLook(InputAction.CallbackContext context)
    {
        if (context.started)
            inputEVHubLook.Trigger(context.ReadValue<Vector2>());
    }
}