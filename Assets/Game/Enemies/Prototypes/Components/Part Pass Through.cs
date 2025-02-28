using GeneralUtility.CombatSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartPassThrough : DestructablePart
{
    [SerializeField] private DestructablePart part; //here's where you'd plug in the wing or whatever needs to be hit

    

    public override bool ApplyDamage(CombatPacket p)
    {
        return part.ApplyDamage(p);
        //Basically, call the wing using the same combat packet
    }

    public override void TriggerSpecialDebuff() 
    {
    }
}