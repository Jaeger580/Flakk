using GeneralUtility.CombatSystem;
using GeneralUtility.EditorQoL;
using GeneralUtility.VariableObject;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunScratchPaper : MonoBehaviour { }

public abstract class DamageModifier
{
    public DamageModifier(float _statModifier)
    {
        statModifier = _statModifier;
    }

    public float statModifier;
    abstract public int ApplyStatModification(int incomingStat);
}

public class DamageMod_PreMitigationMultiplier : DamageModifier
{
    public DamageMod_PreMitigationMultiplier(float _statModifier) : base(_statModifier) {}

    public override int ApplyStatModification(int _damageInput)
    {
        float _damageOutput;
        if (statModifier >= 0f) //If stat modifier is positive, then the damage output should be the original val over the percent increase from the stat mod
            _damageOutput = _damageInput / 1f + (statModifier / 100f);
        else //If stat modifier is negative, then the damage output should be 2x original val - original val over percent decrease from stat mod
            _damageOutput = 2f * _damageInput - (_damageInput / (1f - statModifier / 100f));

        return Mathf.CeilToInt(_damageOutput);
    }
}

public class DamageMod_PreMitigationFlat : DamageModifier
{
    public DamageMod_PreMitigationFlat(float _statModifier) : base(_statModifier) { }

    public override int ApplyStatModification(int _damageInput)
    {
        var newDamage = Mathf.CeilToInt(_damageInput + statModifier);
        if (newDamage < 0)
            newDamage = 0;
        return newDamage;
    }
}

public class DamageMod_PostMitigationMultiplier : DamageModifier
{
    public DamageMod_PostMitigationMultiplier(float _statModifier) : base(_statModifier) { }

    public override int ApplyStatModification(int _damageInput)
    {
        float _damageOutput;
        if (statModifier >= 0f) //If stat modifier is positive, then the damage output should be the original val over the percent increase from the stat mod
            _damageOutput = _damageInput / 1f + (statModifier / 100f);
        else //If stat modifier is negative, then the damage output should be 2x original val - original val over percent decrease from stat mod
            _damageOutput = 2f * _damageInput - (_damageInput / (1f - statModifier / 100f));

        return Mathf.CeilToInt(_damageOutput);
    }
}

public class DamageMod_PostMitigationFlatIgnore : DamageModifier
{
    public DamageMod_PostMitigationFlatIgnore(float _statModifier) : base(_statModifier) { }

    public override int ApplyStatModification(int incomingArmor)
    {
        var newArmor = Mathf.CeilToInt(incomingArmor - statModifier);
        if (newArmor < 0)
            newArmor = 0;
        return newArmor;
    }
}

public class DamageMod_PostMitigationFlat : DamageModifier
{
    public DamageMod_PostMitigationFlat(float _statModifier) : base(_statModifier) { }

    public override int ApplyStatModification(int _damageInput)
    {
        var newDamage = Mathf.CeilToInt(_damageInput + statModifier);
        if (newDamage < 0)
            newDamage = 0;
        return newDamage;
    }
}

abstract public class PersistentStatusEffect : MonoBehaviour
{
    protected float currentLifeTime, timeBeforeExpiration;
    [SerializeField] protected int currentStacks, maxStacks;
    protected bool refreshable, stackable;
    public delegate void EffectExpiration();
    public EffectExpiration EffectExpiredEvent;
    [SerializeField] protected AudioClip[] sfxTickSounds;

    [ReadOnly] [SerializeField] protected IEffect owner;
    public IEffect Owner => owner;

    private void OnDestroy()
    {
        EffectExpiredEvent?.Invoke();
        EffectExpiredEvent = null;
    }

    virtual protected void OnDisable()
    {
        EffectExpiredEvent?.Invoke();
        EffectExpiredEvent = null;
        Destroy(this);
    }

    virtual protected void OnEnable()
    {
        StartCoroutine(nameof(RegisterDelay));
    }

    protected IEnumerator RegisterDelay()
    {//Wait until AFTER an owner has been registered
        while (owner == null) yield return null;

        if (!TryGetComponent(out IDamageable _))
        {//If I can't find something to damage, destroy myself
            Destroy(this);
            yield return null;
        }
    }

    virtual public bool RegisterOwner(IEffect affector)
    {
        if (owner != null)
            return false;

        owner = affector;
        if (owner is IPersistentEffect persistentEffect)
        {
            timeBeforeExpiration = persistentEffect.GetTimeBeforeExpiration();
            maxStacks = persistentEffect.GetMaxStacks();
            refreshable = persistentEffect.GetRefreshable();
            stackable = persistentEffect.GetStackable();
        }

        //sfxTickSounds = owner.SFXModEffect;

        return true;
    }

    virtual public bool ApplyEffect()
    {
        bool applied = RefreshEffect();
        if (!applied) applied = StackEffect();
        else StackEffect();
        return applied;
    }

    virtual protected bool RefreshEffect()
    {
        if (refreshable)
        {
            currentLifeTime = 0f;
        }
        return refreshable;
    }

    virtual protected bool StackEffect()
    {
        if (!stackable || currentStacks >= maxStacks) return false;
        currentStacks++;
        return true;
    }
}

public class DEBUFF_DamageOverTime : PersistentStatusEffect
{
    private float tickRate;
    private int damagePerTick, currentNumTicks, maxNumTicks;

    private float internalTickTimer = 0;
    private IDamageable iHealth;

    private AudioSource sfxSource;

    private void Start()
    {
        if (!TryGetComponent(out iHealth)) Destroy(this);
        currentStacks = 1;
        internalTickTimer = tickRate + 0.1f;
        //sfxSource = Audio_Utility.AddAudioSource(gameObject, true, true);
    }

    override protected void OnDisable()
    {
        Destroy(sfxSource);
        base.OnDisable();
    }

    override public bool RegisterOwner(IEffect affector)
    {
        if (owner != null)
            return false;

        owner = affector;

        if (affector is IPersistentEffect dot)
        {
            damagePerTick = Mathf.CeilToInt(dot.GetEffectValue());
            tickRate = dot.GetTickRate();
            timeBeforeExpiration = dot.GetTimeBeforeExpiration();
            maxNumTicks = dot.GetMaxNumTicks();
            maxStacks = dot.GetMaxStacks();
            refreshable = dot.GetRefreshable();
            stackable = dot.GetStackable();

            //sfxTickSounds = dot.SFXTickSounds;

            return true;
        }
        return false;
    }

    private void Update()
    {
        if (maxNumTicks > 0 && currentNumTicks >= maxNumTicks) { Destroy(this); return; }
        if (timeBeforeExpiration > 0 && currentLifeTime >= timeBeforeExpiration) { Destroy(this); return; }
        if (internalTickTimer > tickRate/1f)
        {
            CombatPacket p = new();
            p.SetDamage(damagePerTick * currentStacks, this);
            p.SetAffector(owner, this);
            p.SetIgnoreLocalResistance(true, this);

            iHealth.ApplyDamage(p);
            //Trigger DoT event?
            internalTickTimer = 0;
            currentNumTicks++;
            if (sfxSource != null && sfxTickSounds != null && sfxTickSounds.Length > 0)
            {
                sfxSource.Stop();
                sfxSource.clip = sfxTickSounds[Random.Range(0, sfxTickSounds.Length)];
                sfxSource.Play();
            }
        }

        internalTickTimer += Time.deltaTime;
        currentLifeTime += Time.deltaTime;
    }

    override public bool ApplyEffect()
    {
        bool applied = RefreshEffect();
        if (!applied) applied = StackEffect();
        else StackEffect();
        return applied;
    }

    override protected bool RefreshEffect()
    {
        if (refreshable)
        {
            currentLifeTime = 0f;
            currentNumTicks = 0;
        }
        return refreshable;
    }
}

abstract public class RailGunAmmo : ImpactBehavior
{
    protected bool triggered;
    protected float og_effectValue;
    public float OG_EffectValue
    {
        get => og_effectValue;
        set
        {
            og_effectValue = value;
        }
    }
    [SerializeField] protected float radiusScalar = 0f;

    [SerializeField] protected TrailRenderer bulletTrail;
    [SerializeField] protected float trailToPoint, trailLifetime;
    [SerializeField] protected GradientReference gradient;
    [SerializeField] protected AnimationCurve trailPosCurve, trailWeightCurve;
    [ReadOnly] [SerializeField] protected float journey;

    public void Initialize(float effectVal, float og_effectVal)
    {
        effectValue = effectVal;
        og_effectValue = og_effectVal;
    }

    protected void LateUpdate()
    {
        if (triggered) return;
        triggered = true;

        Physics.Raycast(transform.position, transform.forward, out var hit, 1000f, affectableMask);
        var allHits = Physics.SphereCastAll(transform.position, 2f, transform.forward, 1000f, affectableMask);

        HandleTrailVFX(allHits.Length > 0, hit);

        //if (!hitSomething) { Destroy(this); return; }
        if (allHits.Length <= 0) { return; }

        foreach(var hitInfo in allHits)
        {
            if (!hitInfo.collider.TryGetComponent<IDamageable>(out var d)) continue;  //If it's not damageable

            //TrailRenderer trail = Instantiate(bulletTrail, transform.position, Quaternion.identity);

            CombatPacket p = new();
            p.SetTarget(d, this);
            p.SetHitCollider(hitInfo.collider, this);

            OnImpact(p);
        }
    }

    protected void HandleTrailVFX(bool hitSomething, RaycastHit hit)
    {
        if (bulletTrail == null) return;
        TrailRenderer trail = Instantiate(bulletTrail, transform.position, Quaternion.identity);
        Ray ray = new(transform.position, transform.forward);

        if (hitSomething)
        {
            StartCoroutine(SpawnTrail(trail, hit));
        }
        else
        {
            StartCoroutine(SpawnTrail(trail, ray.GetPoint(500)));
        }
    }

    protected IEnumerator SpawnTrail(TrailRenderer trail, Vector3 hitPoint)
    {
        if (hitPoint == null) yield break;

        journey = 0f;
        var trailTrans = trail.transform;
        var trailRend = trail.GetComponent<TrailRenderer>();
        trailRend.colorGradient = gradient.Variable == null ? trailRend.colorGradient : gradient.Value;

        Vector3 startPos = trailTrans.position;
        //trailTrans.position = hitInfo.point;
        while (journey < trailLifetime)
        {
            float pointPercent = journey / trailToPoint;
            float lifePercent = journey / trailLifetime;
            float posEasePercent = trailPosCurve.Evaluate(pointPercent);
            float weightEasePercent = trailWeightCurve.Evaluate(lifePercent);

            trailTrans.position = Vector3.LerpUnclamped(startPos, hitPoint, posEasePercent);
            float scale = radiusScalar * effectValue/og_effectValue * (1f - weightEasePercent);
            trailRend.widthMultiplier = scale;
            //trailRend.startWidth = scale;
            //trailRend.endWidth = scale;
            journey += Time.deltaTime / trail.time;
            yield return null;
        }
        trailTrans.position = hitPoint;
        //if (impactParticleSystem != null)
        //{
        //    Instantiate(impactParticleSystem, hitPoint, Quaternion.identity);
        //}
        Destroy(trail.gameObject, trail.time);
    }

    protected IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hitInfo)
    {
        journey = 0f;
        var trailTrans = trail.transform;
        var trailRend = trail.GetComponent<TrailRenderer>();
        trailRend.colorGradient = gradient.Variable == null ? trailRend.colorGradient : gradient.Value;

        Vector3 startPos = trailTrans.position;
        //trailTrans.position = hitInfo.point;
        while (journey < trailLifetime)
        {
            float pointPercent = journey / trailToPoint;
            float lifePercent = journey / trailLifetime;
            float posEasePercent = trailPosCurve.Evaluate(pointPercent);
            float weightEasePercent = trailWeightCurve.Evaluate(lifePercent);

            trailTrans.position = Vector3.LerpUnclamped(startPos, hitInfo.point, posEasePercent);
            float scale = radiusScalar * effectValue / og_effectValue * (1f - weightEasePercent);
            trailRend.widthMultiplier = scale;
            //trailRend.startWidth = scale;
            //trailRend.endWidth = scale;
            journey += Time.deltaTime / trail.time;
            yield return null;
        }
        trailTrans.position = hitInfo.point;
        //if (impactParticleSystem != null)
        //{
        //    Instantiate(impactParticleSystem, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
        //}
        Destroy(trail.gameObject, trail.time);
    }

    abstract public override void OnImpact(CombatPacket p);
}

public class DoT_Fire : DEBUFF_DamageOverTime
{}

public class DoT_Uranium : DEBUFF_DamageOverTime
{ }

abstract public class BaseGunAmmo : ImpactBehavior
{
    abstract public override void OnImpact(CombatPacket p);
    protected void OnCollisionEnter(Collision collision)
    {
        var collided = collision.gameObject;
        if (!((affectableMask & 1 << collided.layer) > 0))
        {
            CombatPacket blank = new();
            OnImpact(blank);
            return;      //If it's not on the right layer
        }
        if (!collided.TryGetComponent<IDamageable>(out var d)) return;  //If it's not damageable

        CombatPacket p = new();
        p.SetTarget(d, this);
        p.SetHitCollider(collision, this);
        //p.SetDamage(Mathf.CeilToInt(effectValue), this);
        //foreach (var modif in tracker.TempEffects)
        //    p.AddToActiveModifiers(modif, this);

        OnImpact(p);
    }
}

public abstract class ImpactBehavior : MonoBehaviour
{
    [Tooltip("VFX to trigger at the point of collision, for feedback.")]
    [SerializeField] protected GameObject vfxPrefab;
    [Tooltip("SFX to trigger on impact, for feedback.")]
    [SerializeField] protected AudioClip[] sfxOnImpact;
    protected float effectValue;

    [Tooltip("Graphic to disable on destroy.")]
    [SerializeField] protected MeshRenderer bulletGraphic;
    [SerializeField] protected GameObject trailVFX;
    [SerializeField] protected GameObject sonicVFX;

    static public LayerMask affectableMask => EnemyMask();

    static public LayerMask EnemyMask()
    {
        return 1 << LayerMask.NameToLayer("Enemy");
    }

    protected void Awake()
    {
        Destroy(gameObject, 5f);
    }

    public void Initialize(float effectVal)
    {
        effectValue = effectVal;
    }

    abstract public void OnImpact(CombatPacket p);

    protected private IEnumerator DestroySelf()
    {
        this.gameObject.GetComponent<Collider>().enabled = false;
        bulletGraphic.enabled = false;
        Rigidbody rb = this.gameObject.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;


        sonicVFX.GetComponent<ParticleSystem>().Stop();

        yield return new WaitForSeconds(1f);
        Destroy(this.gameObject);
    }
}

/*
 * 
 * hitscan bulletPrefab example
 * Start()
 * -> send raycast/spherecast forward looking exclusively for enemies
 * -> die after a second
 * 
 * abstract BulletBase : Monobehaviour, IEffect
 * - TriggerEffect()
 * - OnCollisionEnter()
 * 
*/