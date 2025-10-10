using System.Collections;
using System.Collections.Generic;
using GeneralUtility.GameEventSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class InputHandler : MonoBehaviour
{
    [Header("Hub Interact Input Events")]
    [SerializeField] private GameEvent inputEVInteractPress;
    [SerializeField] private GameEvent inputEVInteractRelease;
    [SerializeField] private GameEvent inputEVUICancel;

    public void InteractPressed(InputAction.CallbackContext context)
    {
        if (context.started)
            inputEVInteractPress.Trigger();
    }

    public void InteractReleased(InputAction.CallbackContext context)
    {
        if (context.canceled)
            inputEVInteractRelease.Trigger();
    }

    public void UICancel(InputAction.CallbackContext context)
    {
        if (context.started)
            inputEVUICancel.Trigger();
    }
}

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform RayCastSource;
    [SerializeField] private float RayCastDistance;
    [SerializeField] private GameEvent inputEVInteractPress;

    [SerializeField] private LayerMask IgnorePlayer;

    private void Start()
    {
        var interactListener = gameObject.AddComponent<GameEventListener>();
        interactListener.Events.Add(inputEVInteractPress);
        interactListener.Response = new();
        interactListener.Response.AddListener(() => TryInteract());
        inputEVInteractPress.RegisterListener(interactListener);
    }

    private void TryInteract() 
    {
        Ray ray = new Ray(RayCastSource.position, RayCastSource.forward);

        if (!Physics.Raycast(RayCastSource.position, RayCastSource.forward, out var hit, RayCastDistance, ~IgnorePlayer)) return;

        if (hit.collider.gameObject.TryGetComponent(out IInteractable target))
        {
            target.Interact(this);
        }
    }
}

// Want to check this naming convention
public interface IInteractable
{
    public void Interact(object interactor);
}