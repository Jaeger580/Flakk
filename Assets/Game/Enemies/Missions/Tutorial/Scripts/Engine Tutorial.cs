using GeneralUtility.GameEventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineTutorial : DestructablePart
{
    [SerializeField]
    private float speedMultipler = 0.75f;

    [SerializeField]
    private GameEvent eventTrigger;

    public override void TriggerSpecialDebuff()
    {
        //mainBody.speed--;

        SwapParts();

        mainBody.GetComponent<Enemy>().SpeedMulti(speedMultipler);

        localResistance = 100;
        mainResistance = 100;

        eventTrigger.Trigger();
    }
}
