using System;
using System.Collections;
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

    [SerializeField] private AnimationCurve lockonCurve;
    [SerializeField] private float lockonDuration;
    [SerializeField] private Vector2 lockonZonePercent;

    [SerializeField] private AnimationCurve damagedCurve;
    [SerializeField] private float damagedDuration;
    [ColorUsage(true, true)]
    [SerializeField] private Color damagedColor;

    private const float
        LOCKED_OFF_THRESHOLD = 1f,
        LOCKED_ON_THRESHOLD = 1.3f,
        DAMAGED_THRESHOLD = 4f;
    private const float
        LOCKED_OFF_POWER = 0.4f,
        DAMAGED_POWER = 4f;

    [Header("References")]
    [SerializeField] private RectTransform rect;
    [SerializeField] private Image rend;
    [SerializeField] private Enemy enemy;
    private Material mat;
    private Camera gunCam;

    private int radiusScaleID = Shader.PropertyToID("_Radial_Scale"),
        outerColorID = Shader.PropertyToID("_Outer_Color"),
        radialColorID = Shader.PropertyToID("_Radial_Color"),
        radialPowerID = Shader.PropertyToID("_Radial_Power");

    private Coroutine rotationRoutine;
    private Coroutine lockOnRoutine;
    private Coroutine lockOffRoutine;
    private Coroutine damagedRoutine;
    //misc vals
    private Vector2 screenDims;
    private bool lockedOn;

    private void Start()
    {
        if (rend != null && rend.material != null)
            mat = rend.material;
        rotationRoutine = StartCoroutine(RotateBox());

        if (gunCam == null)
        {
            gunCam = Camera.main;
        }

        enemy.OnDamage += Damage;

        screenDims = new Vector2(Screen.width, Screen.height);
        mat.SetFloat(radiusScaleID, LOCKED_OFF_THRESHOLD);
        mat.SetColor(outerColorID, og_color);
    }

    private void Update()
    {
        Vector3 objectScreenPosition = gunCam.WorldToScreenPoint(transform.position);
        //print($"Ratio: {objectScreenPosition.x / Screen.width}/{objectScreenPosition.y / Screen.height}");

        bool inDeadZone =
            Mathf.Abs((objectScreenPosition.x / screenDims.x) - .5f) <= lockonZonePercent.x &&
            Mathf.Abs((objectScreenPosition.y / screenDims.y) - .5f) <= lockonZonePercent.y;

        if (!lockedOn && inDeadZone)
        {
            LockOn();
        }
        else if (lockedOn && !inDeadZone)
        {
            LockOff();
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
