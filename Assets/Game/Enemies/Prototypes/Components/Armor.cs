using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Armor : DestructablePart
{
    public override void TriggerSpecialDebuff()
    {
        //trigger VFX
        //enable/disable model
        Destroy(this);
    }
}
