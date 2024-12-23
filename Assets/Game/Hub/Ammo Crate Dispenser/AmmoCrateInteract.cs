using System.Collections;
using System.Collections.Generic;
using GeneralUtility;
using GeneralUtility.EditorQoL;
using UnityEngine;

public class AmmoCrateHolder : MonoBehaviour
{
    [ReadOnly] [SerializeField] private Stack<(AmmoCrateInteract physicalCrate, AmmoStack ammoCrate)> registeredAmmoCrates = new();

    public void PushCrateOnTop(AmmoCrateInteract physicalCrate, AmmoStack ammoCrate)
    {
        registeredAmmoCrates.Push((physicalCrate, ammoCrate));
    }

    public bool TryPopTopCrate(out (AmmoCrateInteract physicalCrate, AmmoStack ammoCrate) registeredCrate)
    {
        return registeredAmmoCrates.TryPop(out registeredCrate);
    }

    public void DropAllCrates()
    {
        foreach(var (physicalCrate, _) in registeredAmmoCrates)
        {
            physicalCrate.DetachFromPlayer();
        }
        registeredAmmoCrates.Clear();
    }
}

public class StockpileBusInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private AmmoStack stockpileToLoad;

    [Tooltip("How long should a single ammo crate load into this bus (in seconds per crate)?")]
    [SerializeField] private float loadingRate = 1f;

    public void Interact(object interactor)
    {
        if (interactor is not Interactor playerInteractor) return;
        if (!playerInteractor.TryGetComponent(out AmmoCrateHolder ammoCrateHolder)) return;

        if (!ammoCrateHolder.TryPopTopCrate(out var poppedCrate)) return;
    }

    private IEnumerator LoadStockpile(AmmoStack crateToLoad)
    {
        int ammoInCrate = crateToLoad.stack.Count;
        var waitPerAmmo = new WaitForSeconds(loadingRate/ammoInCrate);

        while(ammoInCrate > 0)
        {//While there's still ammo in this crate,
            if (stockpileToLoad.TryPush(crateToLoad.Peek()))
            {//If I can fit it, try pushing ammo into the stockpile
                ammoInCrate--;  //If you did, register that you did
            }
            else
            {//otherwise, wait until I CAN fit ammo into the stockpile
                yield return new WaitUntil(() => stockpileToLoad.stack.Count < stockpileToLoad.maxStackSize.Value);
            }

            yield return waitPerAmmo;
        }


    }
}

public class AmmoCrateInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private BoxCollider col;
    [SerializeField] private Rigidbody rb;

    private GameObject currentPlaceableObject;
    [SerializeField] private Transform spawnPos;
    private Transform currentTrans;
    [SerializeField] private Camera mainCam;
    private Vector3 screenCenter = new(0.5f, 0.5f);
    private float maxRange = 10f, groundY = 0f;
    [SerializeField] private LayerMask groundMask, blockingMask;
    private LayerMask playerMask;

    private float objectRotation;

    private bool attached;

    private void Start()
    {
        //mainCam = Camera.main;

        blockingMask |= 1 << LayerMask.NameToLayer("Player");
        playerMask |= 1 << LayerMask.NameToLayer("Player");
        groundMask |= 1 << LayerMask.NameToLayer("Ground");

        currentTrans = transform;
    }

    public void Interact(object interactor)
    {
        if (interactor is not Interactor player)
        {
            Editor_Utility.ThrowWarning("ERR: Interactor not valid.", this);
            return;
        }

        AttachToPlayer(player.transform);
    }

    private void AttachToPlayer(Transform playerTrans)
    {
        transform.parent = playerTrans;
        var pos = Vector3.zero;
        pos.z += 1f;
        transform.localPosition = pos;
        transform.localRotation = Quaternion.identity;
        if (col == null)
        {
            Editor_Utility.ThrowWarning("ERR: Missing collider on ammo crate.", this);
            return;
        }
        attached = true;
        //Physics.IgnoreCollision(col, playerTrans.GetComponent<Collider>(), true);
        col.enabled = false;
        rb.isKinematic = true;
        //col.excludeLayers = (playerMask);
    }

    private void Update()
    {
        if (!attached) return;
        //MoveCurrentPlaceableObjectToMouse();
    }

    //private void MoveCurrentPlaceableObjectToMouse()
    //{
    //    currentTrans.eulerAngles = new Vector3(0f, 0f, 0f);
    //    currentTrans.Rotate(Vector3.up, objectRotation, Space.Self);

    //    Vector3 potentialPos;

    //    Ray ray = mainCam.ViewportPointToRay(screenCenter);

    //    if (Physics.Raycast(ray, out RaycastHit hit, maxRange, groundMask))
    //    {//If you're looking at ground within a range, place the object there
    //        potentialPos = hit.point;
    //        potentialPos.y += 0.5f;
    //        groundY = hit.point.y;
    //    }
    //    else
    //    {//Otherwise, place the object at the max range in the direction you're looking
    //        potentialPos = ray.GetPoint(maxRange);
    //        float newRange = maxRange;
    //        if (potentialPos.y > spawnPos.position.y)
    //        {
    //            newRange += potentialPos.y - spawnPos.position.y;
    //        }
    //        //print($"YPos1 = {currentTrans.position.y}");
    //        var tempPos = potentialPos;
    //        tempPos.y += 5;
    //        Ray downRay = new(tempPos, Vector3.down);
    //        if (Physics.Raycast(downRay, out hit, newRange, groundMask))
    //        {//If there's ground beneath this object,
    //            var tempGround = hit.point;                 //Store the position of the ground hit
    //            var tempObjPos = ray.GetPoint(maxRange);    //Store the max-range position
    //            tempObjPos.y = tempGround.y + 0.5f;         //Max range position's y = y of the ground hit
    //            potentialPos = tempObjPos; //move it to that position
    //            //print($"YPos2.1 = {currentTrans.position.y}");
    //            groundY = potentialPos.y;
    //        }
    //        else
    //        {
    //            groundY = spawnPos.position.y - 3f;
    //            potentialPos = new Vector3(potentialPos.x, groundY, potentialPos.z);
    //            //print($"YPos2.2 = {currentTrans.position.y}");
    //        }
    //    }
    //    currentTrans.position = potentialPos;
    //}

    public void DetachFromPlayer()
    {
        transform.parent = null;
        col.enabled = true;
        rb.isKinematic = false;

        //if(Physics.Raycast(spawnPos.position, -transform.up, out hit,  groundMask))
        //{

        //}

        //var bounds = col.bounds;
        //var pointOffset = bounds.extents;
        //var directionToPoint = transform.position - hitPoint;
        
        //transform.position = hitPoint;
    }
}