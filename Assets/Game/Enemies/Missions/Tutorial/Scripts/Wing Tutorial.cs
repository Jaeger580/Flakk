using GeneralUtility.CombatSystem;
using GeneralUtility.GameEventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WingTutorial : DestructablePart
{
    [SerializeField]
    private float speedMultipler = 0.90f;

    [SerializeField]
    private int bonusDamage = 2;

    [SerializeField]
    private GameEvent eventTrigger;

    public override void TriggerSpecialDebuff()
    {
        CombatPacket packet = new CombatPacket();
        packet.SetDamage(bonusDamage, this);


        SwapParts();

        mainBody.GetComponent<Enemy>().SpeedMulti(speedMultipler);
        mainBody.GetComponent<Enemy>().ApplyDamage(packet);

        Debug.Log("Wing DETROYED");

        eventTrigger.Trigger();
    }
}
