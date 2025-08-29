using GeneralUtility.CombatSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bridge : DestructablePart
{
    [SerializeField]
    private int bonusDamage = 30;

    public override void TriggerSpecialDebuff()
    {
        CombatPacket packet = new CombatPacket();
        packet.SetDamage(bonusDamage, this);


        localResistance = 100;
        mainResistance = 100;

        SwapParts();

        mainBody.GetComponent<Enemy>().ApplyDamage(packet);
    }
}
