using GeneralUtility.CombatSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wing : DestructablePart
{
    [SerializeField]
    private float speedMultipler = 0.90f;

    [SerializeField]
    private int bonusDamage = 2;

    public override void TriggerSpecialDebuff()
    {
        CombatPacket packet = new CombatPacket();
        packet.SetDamage(bonusDamage, this);


        //localResistance = 100;
        //mainResistance = 100;

        //packet.AddResistance(localResistance, this);

        SwapParts();

        mainBody.GetComponent<Enemy>().SpeedMulti(speedMultipler);
        mainBody.GetComponent<Enemy>().ApplyDamage(packet);

        Debug.Log("Wing DETROYED");

        //mainBody.speed--;
        //trigger VFX
        //enable/disable model
        Destroy(this.gameObject);
    }
}