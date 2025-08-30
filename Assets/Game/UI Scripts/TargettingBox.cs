using System;
using System.Collections;
using GeneralUtility.CombatSystem;
using GeneralUtility.GameEventSystem;
using UnityEngine;
using UnityEngine.UI;

public class TargettingBox : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Color og_color;
    [SerializeField] private AnimationCurve rotationCurve;
    [SerializeField] private float rotationAngle;
    [SerializeField] private float rotationDuration;
    [SerializeField] private float rotationDelay;

    [SerializeField] private AnimationCurve visibilityCurve;
    [Tooltip("Lock-On Zone multiplier that represents area where enemies should be visible.")]
    [SerializeField] private float visibilityAsLockonMultiplier = 2f;
    [Tooltip("Threshold percentage of the screen away from the lockon threshold where the reticle disappears.")]
    [SerializeField] private float visibilityThreshold = 0.135f;
    private float visibilityMag;
    [SerializeField] private AnimationCurve lockonCurve;
    [SerializeField] private float lockonDuration;
    [SerializeField] private Vector2 lockonZonePercent;

    [SerializeField] private AnimationCurve damagedCurve;
    [SerializeField] private float damagedDuration;
    [ColorUsage(true, true)]
    [SerializeField] private Color damagedColor;

    [SerializeField] private GameEvent gunEntered, gunExited;

    private const float
        HIDDEN_THRESHOLD = 0.6f,
        LOCKED_OFF_THRESHOLD = 1f,
        LOCKED_ON_THRESHOLD = 1.3f,
        DAMAGED_THRESHOLD = 4f;
    private const float
        LOCKED_OFF_POWER = 0.4f,
        DAMAGED_POWER = 4f;
    private const float
        NEAR_AO_CLIP = 0.05f,
        FAR_AO_CLIP = 0.25f;

    [Header("References")]
    [SerializeField] private RectTransform rect;
    [SerializeField] private SpriteRenderer rend;
    [SerializeField] private Damageable target;
    private MaterialPropertyBlock mat;
    [SerializeField] private Camera gunCam;

    private int radiusScaleID = Shader.PropertyToID("_Radial_Scale"),
        outerColorID = Shader.PropertyToID("_Outer_Color"),
        radialColorID = Shader.PropertyToID("_Radial_Color"),
        radialPowerID = Shader.PropertyToID("_Radial_Power"),
        aoClipID = Shader.PropertyToID("_AO_Clip");

    private Coroutine rotationRoutine;
    private Coroutine lockOnRoutine;
    private Coroutine lockOffRoutine;
    private Coroutine damagedRoutine;
    //misc vals
    private Vector2 screenDims;
    private bool lockedOn;
    static private bool inGun;

    private void OnDestroy()
    {
        mat.SetFloat(aoClipID, FAR_AO_CLIP);
        mat.SetFloat(radiusScaleID, LOCKED_OFF_THRESHOLD);
        mat.SetColor(outerColorID, og_color);
    }

    private void Start()
    {
        mat = new();
        if (rend != null && rend.material != null)
            rend.GetPropertyBlock(mat);
        rotationRoutine = StartCoroutine(RotateBox());

        if (gunCam == null)
        {
            gunCam = Camera.main;
        }

        visibilityMag = (lockonZonePercent).magnitude * visibilityAsLockonMultiplier;

        target.OnDamage += Damage;

        screenDims = new Vector2(Screen.width, Screen.height);
        mat.SetFloat(radiusScaleID, LOCKED_OFF_THRESHOLD);
        mat.SetColor(outerColorID, og_color);
        rend.SetPropertyBlock(mat);

        var gunEnterListener = gameObject.AddComponent<GameEventListener>();
        gunEnterListener.Events.Add(gunEntered);
        gunEnterListener.Response = new();
        gunEnterListener.Response.AddListener(() => inGun = true);
        gunEntered.RegisterListener(gunEnterListener);

        var gunExitListener = gameObject.AddComponent<GameEventListener>();
        gunExitListener.Events.Add(gunExited);
        gunExitListener.Response = new();
        gunExitListener.Response.AddListener(() => inGun = false);
        gunExited.RegisterListener(gunExitListener);
    }

    private void Update()
    {
        if (!inGun)
        {
            mat.SetFloat(radiusScaleID, HIDDEN_THRESHOLD);
            rend.SetPropertyBlock(mat);
            return;
        }
        else if (mat.GetFloat(radiusScaleID) < LOCKED_OFF_THRESHOLD)
        {
            mat.SetFloat(radiusScaleID, LOCKED_OFF_THRESHOLD);
            rend.SetPropertyBlock(mat);
        }

        Vector3 objectScreenPosition = gunCam.WorldToScreenPoint(transform.position);
        //print($"Ratio: {objectScreenPosition.x / Screen.width}/{objectScreenPosition.y / Screen.height}");

        bool inDeadZone =
            Mathf.Abs((objectScreenPosition.x / screenDims.x) - .5f) <= lockonZonePercent.x &&
            Mathf.Abs((objectScreenPosition.y / screenDims.y) - .5f) <= lockonZonePercent.y;

        float actualMag = ((objectScreenPosition / screenDims) - (Vector2.one * 0.5f)).magnitude;

        if (!lockedOn && inDeadZone)
        {
            LockOn();
        }
        else if (lockedOn && !inDeadZone)
        {
            LockOff();
        }
        else if (!lockedOn && actualMag >= visibilityMag)
        {
            //print($"{actualMag} - {lockonMaxMag} = {(actualMag - lockonMaxMag)} => percent = {(actualMag - lockonMaxMag) / visibleThrehold}");
            mat.SetFloat(aoClipID, Mathf.Lerp(NEAR_AO_CLIP, FAR_AO_CLIP, visibilityCurve.Evaluate(actualMag - visibilityMag) / visibilityThreshold));
            rend.SetPropertyBlock(mat);
        }
    }

    private IEnumerator RotateBox()
    {
        lockedOn = false;
        float journey = 0f;
        float duration = rotationDuration;

        while (journey <= rotationDelay)
        {
            journey += Time.deltaTime;
            yield return null;
        }

        journey = 0f;

        while (journey <= duration)
        {
            float rotationPercent = rotationCurve.Evaluate(journey / duration);
            rect.localEulerAngles = Vector3.Lerp(Vector3.zero, Vector3.forward * rotationAngle, rotationPercent);
            journey += Time.deltaTime;
            yield return null;
        }

        if (isActiveAndEnabled)
            rotationRoutine = StartCoroutine(RotateBox());
    }

    public void LockOn()
    {
        if (lockOnRoutine != null) return;

        lockedOn = true;
        if (rotationRoutine != null) CancelRoutine(ref rotationRoutine);
        if (lockOffRoutine != null) CancelRoutine(ref lockOffRoutine);

        lockOnRoutine = StartCoroutine(LockOnRoutine());
    }

    public void LockOff()
    {
        if (lockOffRoutine != null) return;

        lockedOn = false;
        if (rotationRoutine != null) CancelRoutine(ref rotationRoutine);
        if (lockOnRoutine != null) CancelRoutine(ref lockOnRoutine);

        lockOffRoutine = StartCoroutine(LockOffRoutine());
    }

    public void Damage()
    {
        if (damagedRoutine != null) CancelRoutine(ref damagedRoutine);
        damagedRoutine = StartCoroutine(DamageRoutine());
    }

    private void CancelRoutine(ref Coroutine routine)
    {
        StopCoroutine(routine);
        routine = null;
    }

    private IEnumerator LockOnRoutine()
    {
        var og_angles = rect.localEulerAngles;

        Action<float> lockonAction = (float percent) => {
            rect.localEulerAngles = Vector3.Lerp(og_angles, Vector3.zero, percent);
            mat.SetFloat(radiusScaleID, Mathf.Lerp(LOCKED_OFF_THRESHOLD, LOCKED_ON_THRESHOLD, percent));
            rend.SetPropertyBlock(mat);
        };
        yield return GenericRoutine(lockonDuration, lockonCurve, lockonAction);
        //float journey = 0f;
        //float duration = lockonDuration;
        //while (journey <= duration)
        //{
        //    float lockonPercent = lockonCurve.Evaluate(journey / duration);
        //    rect.localEulerAngles = Vector3.Lerp(og_angles, Vector3.zero, lockonPercent);
        //    mat.SetFloat(radiusScaleID, Mathf.Lerp(LOCKED_OFF_THRESHOLD, LOCKED_ON_THRESHOLD, lockonPercent));
        //    //mat.SetFloat(radialPowerID, Mathf.Lerp(LOCKED_OFF_POWER, LOCKED_ON_POWER, lockonPercent));

        //    journey += Time.deltaTime;
        //    yield return null;
        //}
        lockOnRoutine = null;
    }

    private IEnumerator LockOffRoutine()
    {
        rotationRoutine = StartCoroutine(RotateBox());

        Action<float> lockoffAction = (float percent) => {
            mat.SetFloat(radiusScaleID, Mathf.Lerp(LOCKED_ON_THRESHOLD, LOCKED_OFF_THRESHOLD, percent));
            rend.SetPropertyBlock(mat);
        };

        yield return GenericRoutine(lockonDuration, lockonCurve, lockoffAction);

        //float journey = 0f;
        //float duration = lockonDuration;
        //while (journey <= duration)
        //{
        //    float lockoffPercent = lockonCurve.Evaluate(journey / duration);
        //    mat.SetFloat(radiusScaleID, Mathf.Lerp(LOCKED_ON_THRESHOLD, LOCKED_OFF_THRESHOLD, lockoffPercent));
        //    //mat.SetFloat(radialPowerID, Mathf.Lerp(LOCKED_ON_POWER, LOCKED_OFF_POWER, lockoffPercent));

        //    journey += Time.deltaTime;
        //    yield return null;
        //}
        lockOffRoutine = null;
    }

    private IEnumerator DamageRoutine()
    {
        Action<float> damageAction = (float percent) => {
            mat.SetVector(outerColorID, Color.LerpUnclamped(damagedColor, og_color, percent));
            rend.SetPropertyBlock(mat);
        };
        yield return GenericRoutine(damagedDuration, damagedCurve, damageAction);

        damagedRoutine = null;
    }

    private IEnumerator GenericRoutine(float duration, AnimationCurve curve, Action<float> action)
    {
        float journey = 0f;
        while (journey <= duration)
        {
            float curvedPercent = curve.Evaluate(journey / duration);
            action?.Invoke(curvedPercent);
            //mat.SetFloat(radialPowerID, Mathf.Lerp(LOCKED_ON_POWER, LOCKED_OFF_POWER, lockoffPercent));

            journey += Time.deltaTime;
            yield return null;
        }
    }
}
