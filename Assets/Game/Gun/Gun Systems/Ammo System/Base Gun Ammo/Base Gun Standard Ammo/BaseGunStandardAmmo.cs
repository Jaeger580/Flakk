using GeneralUtility.CombatSystem;
using UnityEngine;

public class BaseGunStandardAmmo : BaseGunAmmo, IEffect
{
    public float GetEffectValue()
    {
        return effectValue;
    }

    public void OnEffect(int damage)
    {
        //trigger effect event
    }

    public override void OnImpact(CombatPacket p)
    {
        if (!TriggerEffect(p)) return;

        Destroy(this.gameObject);

        //do vfx/sfx and other cleanup IF it lands correctly
        if (vfxPrefab != null) 
        {
            vfxPrefab.TryGetComponent<ParticleSystem>(out ParticleSystem particle);
            particle.Play();
        }

        //vfxPrefab.GetComponent<ParticleSystem>().Play();
        
        
        // choose a random clip and play it.
        int coinFlip = Random.Range(0, sfxOnImpact.Length);
        AudioSource audioSource = this.GetComponent<AudioSource>();

        audioSource.clip = sfxOnImpact[coinFlip];

        CustomAudio.PlayClipAt(audioSource, p.CollisionInfo.transform.position);

        //foreach(AudioClip clip in sfxOnImpact) 
        //{
        //    AudioSource.PlayClipAtPoint(clip, p.CollisionInfo.transform.position);
        //}

    }

    public void OnKill()
    {
        //trigger kill event
    }

    public bool TriggerEffect(CombatPacket p)
    {
        if (p.Target is not DestructablePart d) return false;

        p.SetDamage(Mathf.CeilToInt(effectValue), this);
        return d.ApplyDamage(p);
    }
}