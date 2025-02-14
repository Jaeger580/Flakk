using GeneralUtility.CombatSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guns : DestructablePart
{
    [SerializeField]
    private int bonusDamage = 15;

    // Need this object to remove it from the main body later
    [SerializeField]
    private GameObject attackPoint;

    [SerializeField]
    private bool isLeft = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void TriggerSpecialDebuff()
    {
        CombatPacket packet = new CombatPacket();
        packet.SetDamage(bonusDamage, this);


        localResistance = 100;
        mainResistance = 100;


        SwapParts();

        mainBody.GetComponent<Enemy>().ApplyDamage(packet);

        mainBody.GetComponent<HeavyFighterController>().RemoveGun(isLeft, attackPoint);

        Debug.Log("Gun DETROYED");
    }
}
