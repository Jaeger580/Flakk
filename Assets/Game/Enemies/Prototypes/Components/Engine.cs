using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : DestructablePart
{
    public override void TriggerSpecialDebuff()
    {
        //mainBody.speed--;

        mainBody.GetComponent<Enemy>().SpeedMulti(0.5f);

        localResistance = 100;
        mainResistance = 50;
        //trigger VFX
        //enable/disable model
        //Destroy(this);
    }
}
