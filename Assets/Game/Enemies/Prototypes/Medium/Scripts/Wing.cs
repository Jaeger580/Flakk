using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wing : DestructablePart
{
    public override void TriggerSpecialDebuff()
    {
        //mainBody.speed--;
        //trigger VFX
        //enable/disable model
        Destroy(this);
    }
}