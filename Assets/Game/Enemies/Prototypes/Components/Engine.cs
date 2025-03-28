using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : DestructablePart
{
    [SerializeField]
    private float speedMultipler = 0.75f;

    [SerializeField]
    private ParticleSystem trail;

    public override void TriggerSpecialDebuff()
    {
        //mainBody.speed--;

        SwapParts();

        mainBody.GetComponent<Enemy>().SpeedMulti(speedMultipler); 

        localResistance = 100;
        mainResistance = 100;

        //trigger VFX
        if(trail != null) 
        {
            trail.Stop();
        }

        //enable/disable model
        Destroy(this.gameObject);
    }
}
