using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : DestructablePart
{
    [SerializeField]
    private float speedMultipler = 0.75f;
    public override void TriggerSpecialDebuff()
    {
        //mainBody.speed--;

        SwapParts();

        mainBody.GetComponent<Enemy>().SpeedMulti(speedMultipler); 

        localResistance = 100;
        mainResistance = 50;
        //trigger VFX
        //enable/disable model
        //Destroy(this);
    }
}
