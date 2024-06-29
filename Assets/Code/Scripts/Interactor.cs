using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/* Jakob Jaeger
 * 06/26/2024
 * Handles the player to interact with anything that inherits the Interactable interface
 */

public class Interactor : MonoBehaviour
{
    [SerializeField]
    private Transform RayCastSource;
    [SerializeField]
    private float RayCastDistance;

    public void TryInteract(InputAction.CallbackContext context) 
    {
        if (context.started)
        {
            Ray ray = new Ray(RayCastSource.position, RayCastSource.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, RayCastDistance))
            {
                if (hit.collider.gameObject.TryGetComponent(out IInteractable target))
                {
                    target.Interact();
                }
            }
        }
    }
}

// Want to check this naming convention
interface IInteractable
{
    void Interact();
}