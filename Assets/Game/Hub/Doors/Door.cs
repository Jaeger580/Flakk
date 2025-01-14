using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    private GameObject doorModel;   // Needs moved along Z

    [SerializeField]
    private GameObject doorCollider; // Needs moved along Z

    [SerializeField]
    private float doorSpeed = 5;

    private Transform modelPos;
    private Transform colliderPos;

    private Vector3 modelStart;
    private Vector3 colliderStart;

    private bool nearDoor;

    private void Awake()
    {
        nearDoor = false;
    }

    private void Start()
    {
        modelPos = doorModel.transform;
        colliderPos = doorCollider.transform;

        modelStart = modelPos.position;
        colliderStart = colliderPos.position;
    }

    private void Update()
    {
        var finalSpeed = doorSpeed * Time.deltaTime;

        if (nearDoor)
        {
            doorCollider.transform.position = Vector3.MoveTowards(colliderPos.position, new Vector3(colliderPos.position.x, 3, colliderPos.position.z), finalSpeed);

            doorModel.transform.position = Vector3.MoveTowards(modelPos.position, new Vector3(modelPos.position.x, 3, modelPos.position.z), finalSpeed);
        }
        else if(!nearDoor) 
        {
            doorCollider.transform.position = Vector3.MoveTowards(colliderPos.position, colliderStart, finalSpeed);

            doorModel.transform.position = Vector3.MoveTowards(modelPos.position, modelStart, finalSpeed);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Open();
    }

    private void OnTriggerExit(Collider other)
    {
        Close();
    }

    private void Open() 
    {
        nearDoor = true;
    }

    private void Close() 
    {
        nearDoor = false;
    }
}
