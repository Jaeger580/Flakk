using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : DestructablePart
{
    public override void TriggerSpecialDebuff()
    {
        //mainBody.speed--;

        mainBody.GetComponent<Enemy>().speedMulti(0.5f);

        //trigger VFX
        //enable/disable model
        Destroy(this);
    }
}
