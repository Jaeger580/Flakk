using GeneralUtility.CombatSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pilot : DestructablePart
{
    // When destroyed, kill enemy.
    public override void TriggerSpecialDebuff()
    {
        // Need a way to apply damage or directly trigger death. Need to speak to allan.
        CombatPacket packet = new CombatPacket();

        packet.SetDamage(9999, this);
        packet.SetTarget(mainBody, this);
        mainBody.ApplyDamage(packet);
    }
}
