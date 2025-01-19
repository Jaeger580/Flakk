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

        localResistance = 100;
        mainResistance = 100;

        SwapParts();
        mainBody.GetComponent<Enemy>().SpeedMulti(speedMultipler);
        mainBody.GetComponent<Enemy>().ApplyDamage(packet);

        //mainBody.speed--;
        //trigger VFX
        //enable/disable model
        //Destroy(this);
    }
}