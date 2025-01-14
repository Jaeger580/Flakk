using System.Collections;
using System.Collections.Generic;
using GeneralUtility.CombatSystem;
using UnityEngine;

public class ComponentTesting : MonoBehaviour
{
    // Target should be whatever component I want to destroy.
    [SerializeField]
    private GameObject target;

    private void Start()
    {
        StartCoroutine(DamageComponent());
    }

    IEnumerator DamageComponent() 
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(2f);
            var packet = new CombatPacket();
            packet.SetDamage(5, this);
            target.GetComponent<DestructablePart>().ApplyDamage(packet);
            Debug.Log("Damage Proc");
        }
    }
}