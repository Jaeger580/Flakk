using GeneralUtility.GameEventSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FighterTutorial : Enemy
{
    [SerializeField]
    private GameEvent deathEvent;

    protected override void Death()
    {
        deathEvent.Trigger();

        if (vfxTrail != null)
        {
            vfxTrail.GetComponent<ParticleSystem>().Stop();
        }

        if (vfxDeath != null)
        {
            GameObject vfxD = Instantiate(vfxDeath, transform.position, transform.rotation);
            //Debug.Log("PE NAMe " + pe.name);
            Destroy(vfxD, 3);
            //Debug.Log("pe deleted");
        }

        CustomAudio.PlayClipAt(sfxDeath, transform.position);

        // Proper death needs added later
        StopAllCoroutines();

        WaveManager.ReduceCount();

        Destroy(transform.parent.gameObject);
    }
}
