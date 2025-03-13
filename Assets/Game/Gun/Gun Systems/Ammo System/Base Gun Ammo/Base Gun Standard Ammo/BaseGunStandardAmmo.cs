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

        //do vfx/sfx and other cleanup IF it lands correctly
        vfxPrefab.GetComponent<ParticleSystem>().Play();

        CustomAudio.PlayClipAt(this.GetComponent<AudioSource>(), p.CollisionInfo.transform.position);

        //foreach(AudioClip clip in sfxOnImpact) 
        //{
        //    AudioSource.PlayClipAtPoint(clip, p.CollisionInfo.transform.position);
        //}

        Destroy(this.gameObject);
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