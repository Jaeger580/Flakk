using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComponentTesting : MonoBehaviour
{
    // Target should be whatever component I want to destroy.
    [SerializeField]
    private GameObject target;

    private void Start()
    {
        StartCoroutine(damageComponent());
    }

    IEnumerator damageComponent() 
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(2f);
            target.GetComponent<DestructablePart>().TakeDamage(5);
            Debug.Log("Damage Proc");
        }
    }
}