using System.Collections;
using GeneralUtility;
using GeneralUtility.EditorQoL;
using GeneralUtility.GameEventSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public partial class InputHandler : MonoBehaviour
{
    [Header("Hub Movement Input Events")]
    [SerializeField] private GameEvent inputEVMove;
    [SerializeField] private GameEvent inputEVJumpPress, inputEVJumpRelease;
    [SerializeField] private GameEvent inputEVSprintPress, inputEVSprintRelease;

    public void Move(InputAction.CallbackContext context)
    {
        inputEVMove.Trigger(context.ReadValue<Vector2>());
    }

    public void JumpPressed(InputAction.CallbackContext context)
    {
        if (context.started)
            inputEVJumpPress.Trigger();
    }

    public void JumpReleased(InputAction.CallbackContext context)
    {
        if (context.canceled)
            inputEVJumpRelease.Trigger();
    }

    public void SprintPressed(InputAction.CallbackContext context)
    {
        if (context.started)
            inputEVSprintPress.Trigger();
    }

    public void SprintReleased(InputAction.CallbackContext context)
    {
        if (context.canceled)
            inputEVSprintRelease.Trigger();
    }
}

[SelectionBase]
public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CapsuleCollider playerCollider;
    private Rigidbody rb;

    [Header("Camera Settings")]
    [SerializeField] private Camera[] cameras;

    #region Movement
    [System.Serializable]
    class Movement
    {
        public float walkSpeed = 12f;
        public float sprintSpeed = 24f;
        public float jumpCooldown;
        public float groundDrag, airDrag;
        public float sprintFOV = 80f, normalFOV = 60f, sprintTransitionDuration = 0.5f;
    }

    [SerializeField] private Movement movement;
    private float sprintSpeed => movement.sprintSpeed;
    private float sprintFOV => movement.sprintFOV;
    private float normalFOV => movement.normalFOV;
    private float sprintTransitionDuration => movement.sprintTransitionDuration;
    private float walkSpeed => movement.walkSpeed;
    private float groundDrag => movement.groundDrag;
    private float airDrag => movement.airDrag;
    private float jumpCooldown => movement.jumpCooldown;
    [ReadOnly] [SerializeField] private bool readyToJump, jumpPressed, jumpStarted;

    private Coroutine startSprint, stopSprint;
    [ReadOnly] [SerializeField] private float actualSpeed;
    #endregion

    #region Slope Handling
    [System.Serializable]
    class SlopeHandling
    {
        public float maxSlopeAngle;
        public GameObject stepRayLower;
        public GameObject stepRayUpper;
        public float stepSmooth = 0.3f;
        public float maxDownwardVelocity = -100f;
    }

    [SerializeField] private SlopeHandling slopeHandling;
    private float maxSlopeAngle => slopeHandling.maxSlopeAngle;
    private GameObject stepRayLower => slopeHandling.stepRayLower;
    private GameObject stepRayUpper => slopeHandling.stepRayUpper;
    private float stepSmooth => slopeHandling.stepSmooth;
    private float maxDownwardVelocity => slopeHandling.maxDownwardVelocity;

    private RaycastHit slopeHit;
    #endregion

    #region Ground Check
    [System.Serializable]
    class GroundCheck
    {
        public LayerMask groundMask;
        public float playerHeight = 2f;
        public float maxCoyoteTime = 0.25f;
    }

    [SerializeField] private GroundCheck groundCheck;

    private LayerMask groundMask => groundCheck.groundMask;
    private float playerHeight => groundCheck.playerHeight;
    private float maxCoyoteTime => groundCheck.maxCoyoteTime;

    [ReadOnly] [SerializeField] private bool isGrounded, coyoteGrounded;
    private float coyoteTimer;
    #endregion


    [Header("Sound Effects")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip[] sfxJump;

    [Header("Input Events")]
    [SerializeField] private GameEvent playerMoveEvent;
    [SerializeField] private GameEvent playerStopEvent, playerSprintEvent;
    [SerializeField] private GameEvent inputEVMove, inputEVJumpPress, inputEVJumpRelease, inputEVSprintPress, inputEVSprintRelease;

    [Header("Other")]
    //And input variables
    private Vector3 rawMove, normalizedMove, delayedForceToApply;
    private bool pressSprint;
    private Quaternion originalRotation;

    [ReadOnly] [SerializeField] private float angle;
    [ReadOnly] [SerializeField] private bool exitingSlope;

    [SerializeField] private float heightPreDeceleration, timePreDeceleration, decelerationDistance, decelerationTime;
    [SerializeField] private float maxSlideTime = 1f;
    private float isKinematicTimer;

    private void Awake()
    {
        var moveListener = gameObject.AddComponent<GameEventListener>();
        moveListener.Events.Add(inputEVMove);
        moveListener.VecResponse = new();
        moveListener.VecResponse.AddListener((Vector2 v) => RawMovement(v));
        inputEVMove.RegisterListener(moveListener);

        var jumpListener = gameObject.AddComponent<GameEventListener>();
        jumpListener.Events.Add(inputEVJumpPress);
        jumpListener.Response = new();
        jumpListener.Response.AddListener(() => JumpPress());
        jumpListener.Response.AddListener(() => Jump());
        inputEVJumpPress.RegisterListener(jumpListener);

        var jumpReleaseListener = gameObject.AddComponent<GameEventListener>();
        jumpReleaseListener.Events.Add(inputEVJumpRelease);
        jumpReleaseListener.Response = new();
        jumpReleaseListener.Response.AddListener(() => JumpRelease());
        inputEVJumpRelease.RegisterListener(jumpReleaseListener);

        var sprintListener = gameObject.AddComponent<GameEventListener>();
        sprintListener.Events.Add(inputEVSprintPress);
        sprintListener.Response = new();
        sprintListener.Response.AddListener(() => StartSprint());
        inputEVSprintPress.RegisterListener(sprintListener);

        var sprintReleaseListener = gameObject.AddComponent<GameEventListener>();
        sprintReleaseListener.Events.Add(inputEVSprintRelease);
        sprintReleaseListener.Response = new();
        sprintReleaseListener.Response.AddListener(() => StopSprint());
        inputEVSprintRelease.RegisterListener(sprintReleaseListener);

        originalRotation = transform.rotation;
        //stepRayUpper.transform.position = new Vector3(stepRayUpper.transform.position.x, stepHeight, stepRayUpper.transform.position.z);
        readyToJump = true; //Could just be defaulted?
    }

    private void Start()
    {
        if (playerCollider == null || !playerCollider.TryGetComponent(out rb))
        {
            Editor_Utility.ThrowWarning($"ERR: {GetType()}.Start() cannot find a capsule, or it can't find a rigidbody.", this);
            return;
        }

        //rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {//Move is here because it's physics-based
        Move();
        SpeedHandler();
        SpeedControl();
        StepClimb();

        if (isGrounded && !jumpStarted)
        {
            coyoteTimer = 0;
            rb.drag = groundDrag;
            ResetJump();
        }
        else
        {
            rb.drag = airDrag;
        }

        //If x and z movement are within a threshold, apply an opposing force similar to drag to slow them to a stop
        if (Mathf.Abs(rb.velocity.x) < 0.01f && Mathf.Abs(rb.velocity.z) < 0.01f)
        {
            var v = rb.velocity;
            v.y = 0f;
            v = -v;
            rb.AddForce(-v * 1000f, ForceMode.Force);
        }

        if (isGrounded &&
            !jumpStarted &&
            Mathf.Approximately(Mathf.Abs(rawMove.x), 0f) && Mathf.Approximately(Mathf.Abs(rawMove.z), 0f))
        {
            /* If:
             * - on the ground AND
             * - not doing a movement thing that requires you not to be kinematic AND
             * - you're not inputting a movement
             * Then:
             * - become isKinematic based on a timer
             */
            if (isKinematicTimer >= maxSlideTime)
            {
                rb.isKinematic = true;
            }
            else
            {
                isKinematicTimer += Time.deltaTime;
                rb.drag += Mathf.Pow(isKinematicTimer * 25f, 2);
            }
        }
        else
        {
            isKinematicTimer = 0;
            rb.isKinematic = false;
        }
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(transform.position + Vector3.down, 0.25f, groundMask);
        coyoteGrounded = coyoteTimer < maxCoyoteTime;
        coyoteTimer += Time.deltaTime;
    }

    private void StartSprint()
    {
        pressSprint = true;

        startSprint = StartCoroutine(C_SFOV(cameras[0].fieldOfView, sprintFOV, sprintTransitionDuration));
        if (stopSprint != null)
            StopCoroutine(stopSprint);
        playerSprintEvent.Trigger();
    }

    private void StopSprint()
    {
        pressSprint = false;

        stopSprint = StartCoroutine(C_SFOV(cameras[0].fieldOfView, normalFOV, sprintTransitionDuration));
        if (startSprint != null)
            StopCoroutine(startSprint);
    }

    private void SpeedHandler()
    {//Exclusively affects the player's speed
        //if (actualSpeed > sprintSpeed)
        //    actualSpeed -= 1.5f;
        //else
        //    actualSpeed = sprintSpeed;

        if (!coyoteGrounded)
            return;
        else if (pressSprint)
            actualSpeed = sprintSpeed;
        else if (actualSpeed > walkSpeed)
            actualSpeed -= 1.5f;
        else
            actualSpeed = walkSpeed;
    }

    private void Move()
    {
        normalizedMove = (transform.right * rawMove.x + transform.forward * rawMove.z).normalized;
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(50f * actualSpeed * GetSlopeMoveDirection(), ForceMode.Force);
            if (rb.velocity.y > 0f)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        else if (isGrounded && !exitingSlope)
        {
            rb.AddForce(50f * actualSpeed * normalizedMove, ForceMode.Force);
            if (rb.velocity.y > maxDownwardVelocity && !rb.isKinematic)
            {
                var down = rb.velocity;
                down.y = maxDownwardVelocity;
                rb.velocity = down;
            }
        }
        else
        {
            var forceToApply = 20f * actualSpeed * normalizedMove;
            var drag = airDrag * actualSpeed * -normalizedMove;
            if (drag.magnitude > forceToApply.magnitude)
            {
                drag = -forceToApply;
            }
            rb.AddForce(forceToApply, ForceMode.Force);
            rb.AddForce(drag, ForceMode.Force);
        }

        rb.useGravity = !OnSlope();
    }

    private void RawMovement(Vector2 vec)
    {//Listens for the movement input GameEvent
        rawMove.x = vec.x;                  //Assign the raw movement's vectors, to be used in Move()
        rawMove.z = vec.y;
        if (vec.x != 0 || vec.y != 0)
        {
            playerMoveEvent.Trigger();
        }
        else
        {
            playerStopEvent.Trigger();
        }
    }

    private void SpeedControl()
    {//Limits the horizontal velocity if need be
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > actualSpeed)
            {
                rb.velocity = rb.velocity.normalized * actualSpeed;
            }
        }
        else
        {
            Vector3 flatVelocity = new(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVelocity.magnitude > actualSpeed)
            {
                Vector3 limitedVelocity = Vector3.Normalize(flatVelocity) * actualSpeed;
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            }
        }
    }
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.magenta;
    //    Gizmos.DrawRay(stepRayLower.transform.position, transform.TransformDirection(rawMove + new Vector3(0, 0, 1)));
    //    Gizmos.DrawRay(stepRayLower.transform.position, transform.TransformDirection(rawMove + new Vector3(1.5f, 0, 1)));
    //    Gizmos.DrawRay(stepRayLower.transform.position, transform.TransformDirection(rawMove + new Vector3(-1.5f, 0, 1)));
    //}

    private void StepClimb()
    {
        //something in front of my foot? if not, exit early
        if (!Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(rawMove), out RaycastHit _, 1f, groundMask)) return;

        //am I on a slope? if not, exit early
        if (OnSlope()) return;

        //okay but fr, is there something I might trip on? if not, exit early
        if (!Physics.Raycast(stepRayLower.transform.position + (Vector3.up * 0.25f), transform.TransformDirection(rawMove), out RaycastHit _, 1f, groundMask)) return;

        //well I guess there's something, so I better check to make sure it's close enough in the direction I'm moving before trying to adjust
        if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(rawMove), out RaycastHit hit, 6f, groundMask))
        {
            rb.position -= new Vector3(0f, -stepSmooth, 0f);
        }
    }
    private void JumpRelease()
    {
        jumpPressed = false;
        rb.drag = 0;
    }

    private void JumpPress()
    {
        jumpPressed = true;
    }

    private void Jump()
    {//Listens for jump input GameEvent
        if (!readyToJump) return;  //Guard clause: If player isn't ready to jump

        if (!coyoteGrounded) return; //If player is in the air
        jumpStarted = true;
        rb.isKinematic = false;
        isKinematicTimer = 0;

        if (coyoteGrounded) //If player is on ground
        {
            Invoke(nameof(ResetExitingSlope), jumpCooldown);
        }

        exitingSlope = true;

        var initialJumpForce = transform.up * (heightPreDeceleration / (timePreDeceleration * timePreDeceleration));
        var decelerationForce = (decelerationDistance / (decelerationTime * decelerationTime)) * -1f * transform.up;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        //rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        rb.AddForce(initialJumpForce, ForceMode.VelocityChange);
        StopCoroutine(nameof(C_JumpDecelerator));
        StartCoroutine(nameof(C_JumpDecelerator), decelerationForce);
        //sfxSource.clip = sfxJump[UnityEngine.Random.Range(0, sfxJump.Length)];
        //sfxSource.PlayOneShot(sfxSource.clip);
    }

    private IEnumerator C_JumpDecelerator(Vector3 decelForce)
    {
        float journey = 0f;
        while (journey < timePreDeceleration)
        {
            if (journey > (timePreDeceleration / 2))
                rb.AddForce(decelForce, ForceMode.Acceleration);
            journey += Time.fixedDeltaTime;
            yield return null;
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
        exitingSlope = false;
        jumpStarted = false;
    }

    private void ResetExitingSlope()
    {
        exitingSlope = false;
        jumpStarted = false;
    }

    private bool OnSlope()
    {
        /*
         * when player reaches top of a steep slope
         *  > slide down a little and then kinda stop
         *  
         *  if maxSlopeAngle > angle, then
         *      (slide slightly)
         */

        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight + 0.3f, groundMask))
        {
            float currAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
            angle = currAngle;
            //print("Checking slope.");
            //print($"{angle < maxSlopeAngle}, {angle != 0}");
            //if (angle < maxSlopeAngle && angle != 0) print("ON SLOPE!");
            return currAngle < maxSlopeAngle && currAngle > 5f;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {//Normalizes the player's sloped movement
        return Vector3.Normalize(Vector3.ProjectOnPlane(normalizedMove, slopeHit.normal));
    }

    private IEnumerator C_SFOV(float startPoint, float endPoint, float duration)
    {
        float journey = 0f;
        while (journey <= duration)
        {
            journey += Time.smoothDeltaTime;
            float percent = journey / duration;
            for (int i = 0; i < cameras.Length; i++)
            {
                Camera camera = cameras[i];
                camera.fieldOfView = Mathf.Lerp(startPoint, endPoint, percent);
            }
            yield return null;
        }
    }

    public float Speed
    {
        get { return actualSpeed; }

        set
        {
            if (value < 0)
                throw new System.Exception("Cannot have negative speed!");
            actualSpeed = value;
        }
    }
}