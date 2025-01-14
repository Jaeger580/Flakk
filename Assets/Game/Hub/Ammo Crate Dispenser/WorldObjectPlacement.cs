using UnityEngine;

public class WorldObjectPlacement : MonoBehaviour
{
    public static bool isValid;

    [SerializeField]
    private Material isValidMaterial, isInvalidMaterial;

    public Renderer[] renders;
    private float overlapRadius = 3f;
    //private GameObject player;

    [SerializeField] private LayerMask groundMask, blockingMask;

    private void Start()
    {
        //var pcols = player.GetComponentsInChildren<Collider>();
        //foreach (var pcol in pcols)
        //    Physics.IgnoreCollision(GetComponent<Collider>(), pcol, true);

        blockingMask |= 1 << LayerMask.NameToLayer("Player");
        blockingMask |= 1 << LayerMask.NameToLayer("PlaceableObject");

        groundMask |= 1 << LayerMask.NameToLayer("Ground");
    }

    private void OnCollisionStay(Collision collision)
    {
        if ((groundMask.value & 1 << collision.gameObject.layer) > 0)
        {
            isValid = true;
            if (Physics.CheckSphere(transform.position, overlapRadius, blockingMask))
                isValid = false;

            ChangeRender();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if ((groundMask.value & 1 << collision.gameObject.layer) > 0)
            isValid = false;
        ChangeRender();
    }

    private void ChangeRender()
    {
        if (isValid)
        {
            foreach (Renderer rend in renders)
            {
                rend.material = isValidMaterial;
            }
        }
        else
        {
            foreach (Renderer rend in renders)
            {
                rend.material = isInvalidMaterial;
            }
        }
    }
}