using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hull : DestructablePart
{
    public override void TriggerSpecialDebuff()
    {
        //mainBody.speed--;
        //trigger VFX
        //enable/disable model
        Destroy(this);
    }
}