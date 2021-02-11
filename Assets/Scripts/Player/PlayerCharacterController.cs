using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerCharacterController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Reference to the main camera used for the player")]
    public Camera playerCamera;
    [Tooltip("Reference to the body used by this Player")]
    public GameObject body;
    [Header("General")]
    [Tooltip("Force applied downward when in the air")]
    public float gravityDownForce = 20f;
    [Tooltip("Physic layers checked to consider the player grounded")]
    public LayerMask groundCheckLayers = -1;
    [Tooltip("distance from the bottom of the character controller capsule to test for grounded")]
    public float groundCheckDistance = 0.05f;
    [Header("Movement")]
    [Tooltip("Max movement speed when grounded (when not sprinting)")]
    public float maxSpeedOnGround = 8f;
    [Tooltip("Sharpness for the movement when grounded, a low value will make the player accelerate and decelerate slowly, a high value will do the opposite")]
    public float movementSharpnessOnGround = 15;
    [Tooltip("Max movement speed when not grounded")]
    public float maxSpeedInAir = 10f;
    [Tooltip("Acceleration speed when in the air")]
    public float accelerationSpeedInAir = 25f;
    [Tooltip("Multiplicator for the sprint speed (based on grounded speed)")]
    public float sprintSpeedModifier = 2f;
    [Header("Rotation")]
    [Tooltip("Rotation speed for moving the camera")]
    public float rotationSpeed = 200f;
    [Range(0.1f, 1f)]
    [Tooltip("Rotation speed multiplier when aiming")]
    public float aimingRotationMultiplier = 0.4f;

    [Header("Jump")]
    [Tooltip("Force applied upward when jumping")]
    public float jumpForce = 5f;
    [Tooltip("Time between jumps")]
    public float jumpGroundingPreventionTime = 0.85f;
    [Header("Stance")]
    [Tooltip("Height of character when standing")]
    public float capsuleHeightStanding = 1.8f;
    public Vector3 characterVelocity { get; set; }
    public bool isGrounded { get; private set; }
    public bool hasJumpedThisFrame { get; private set; }
    public float RotationMultiplier
    {
        get
        {
            if (playerWeaponsManager.isAiming)
            {
                return aimingRotationMultiplier;
            }

            return 1f;
        }
    }
    PlayerHandler playerHandler;
    CharacterController characterController;
    PlayerWeaponsManager playerWeaponsManager;
    Animator animator;
    ActorHealth actorHealth;
    Vector3 groundVector;
    float lastTimeJumped = 0f;
    float cameraVerticalAngle = 0f;
    
    const float GroundCheckDistanceInAir = 0.12f;
    // Performance improvements, to not perform ground check on every frame.
    const float GroundCheckInterval = 1f;
    private float groundFramePolling = 0f;
    private bool isStanding = false;

    void Start()
    {
        // Setup dependend components.
        characterController = GetComponent<CharacterController>();
        playerHandler = GetComponent<PlayerHandler>();
        playerWeaponsManager = GetComponent<PlayerWeaponsManager>();
        actorHealth = GetComponent<ActorHealth>();
        animator = GetComponentInChildren<Animator>();

        // Register error handlers for components for Debug
        DebugUtility.HandleErrorIfNullGetComponent<CharacterController, PlayerCharacterController>(characterController, this, gameObject);
        DebugUtility.HandleErrorIfNullGetComponent<PlayerHandler, PlayerCharacterController>(playerHandler, this, gameObject);
        DebugUtility.HandleErrorIfNullGetComponent<PlayerWeaponsManager, PlayerCharacterController>(playerWeaponsManager, this, gameObject);
        DebugUtility.HandleErrorIfNullGetComponent<ActorHealth, PlayerCharacterController>(actorHealth, this, gameObject);

        // Tweak some values inside character to match parameters from config.
        characterController.enableOverlapRecovery = true;
        characterController.height = capsuleHeightStanding;
        characterController.center = Vector3.up * characterController.height * 0.5f;
        
        // Register onDie action and call initial obstruction calculation.
        actorHealth.onDie += OnDie;
        actorHealth.onRevive += OnRevive;
        animator.runtimeAnimatorController = Resources.Load("Standing") as RuntimeAnimatorController;
    }

    void Update()
    {
        hasJumpedThisFrame = false;
        groundFramePolling += Time.deltaTime;

        if (GroundCheckInterval >= groundFramePolling)
        {
            GroundCheck();
            groundFramePolling = 0f;
        }
        
        HandleCharacterMovement();
    }

    void OnDie()
    {
        body.SetActive(false);
    }

    void OnRevive()
    {
        body.SetActive(true);
    }

    void GroundCheck()
    {
        // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
        float chosenGroundCheckDistance = isGrounded ? (characterController.skinWidth + groundCheckDistance) : GroundCheckDistanceInAir;

        // reset values before the ground check
        isGrounded = false;
        groundVector = Vector3.up;

        // only try to detect ground if it's been a short amount of time since last jump; otherwise we may snap to the ground instantly after we try jumping
        if (Time.time >= lastTimeJumped + jumpGroundingPreventionTime)
        {
            // if we're grounded, collect info about the ground normal with a downward capsule cast representing our character capsule
            if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(characterController.height), characterController.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, groundCheckLayers, QueryTriggerInteraction.Ignore))
            {
                // storing the upward direction for the surface found
                groundVector = hit.normal;

                // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
                // and if the slope angle is lower than the character controller's limit
                if (Vector3.Dot(hit.normal, transform.up) > 0f &&
                    IsNormalUnderSlopeLimit(groundVector))
                {
                    isGrounded = true;

                    // handle snapping to the ground
                    if (hit.distance > characterController.skinWidth)
                    {
                        characterController.Move(Vector3.down * hit.distance);
                    }
                }
            }
        }
    }

    void HandleCharacterMovement()
    {
        // horizontal character rotation
        // rotate the transform with the input speed around its local Y axis
        transform.Rotate(new Vector3(0f, (playerHandler.GetLookInputsHorizontal() * rotationSpeed * RotationMultiplier), 0f), Space.Self);

        // vertical camera rotation
        // add vertical inputs to the camera's vertical angle
        cameraVerticalAngle += playerHandler.GetLookInputsVertical() * rotationSpeed * RotationMultiplier;

        // limit the camera's vertical angle to min/max
        cameraVerticalAngle = Mathf.Clamp(cameraVerticalAngle, -89f, 89f);

        // apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
        playerCamera.transform.localEulerAngles = new Vector3(cameraVerticalAngle, 0, 0);

        // character movement handling
        bool isSprinting = playerHandler.GetSprintInputHeld();
        float speedModifier = isSprinting ? sprintSpeedModifier : 1f;

        // converts move input to a worldspace vector based on our character's transform orientation
        Vector3 worldspaceMoveInput = transform.TransformVector(playerHandler.GetMoveInput());

        // handle grounded movement
        if (isGrounded)
        {
            if (characterVelocity != Vector3.zero)
            {
                isStanding = false;
                animator.runtimeAnimatorController = Resources.Load("Running") as RuntimeAnimatorController;
            }

            // calculate the desired velocity from inputs, max speed, and current slope
            Vector3 targetVelocity = worldspaceMoveInput * maxSpeedOnGround * speedModifier;
            targetVelocity = GetDirectionReorientedOnSlope(targetVelocity.normalized, groundVector) * targetVelocity.magnitude;

            // smoothly interpolate between our current velocity and the target velocity based on acceleration speed
            characterVelocity = Vector3.Lerp(characterVelocity, targetVelocity, movementSharpnessOnGround * Time.deltaTime);

            // jumping
            if (isGrounded && playerHandler.GetJumpInputDown())
            {
                animator.runtimeAnimatorController = Resources.Load("Jumping") as RuntimeAnimatorController;
                // check if we jumped into obstruction
                if (UpdateObstructions())
                {
                    // start by canceling out the vertical component of our velocity
                    characterVelocity = new Vector3(characterVelocity.x, 0f, characterVelocity.z);

                    // then, add the jumpSpeed value upwards
                    characterVelocity += Vector3.up * jumpForce;

                    // remember last time we jumped because we need to prevent snapping to ground for a short time
                    lastTimeJumped = Time.time;
                    hasJumpedThisFrame = true;

                    // Force grounding to false
                    isGrounded = false;
                    groundVector = Vector3.up;
                }
            }
        }
        // handle air movement
        else
        {
            // add air acceleration
            characterVelocity += worldspaceMoveInput * accelerationSpeedInAir * Time.deltaTime;

            // limit air speed to a maximum, but only horizontally
            float verticalVelocity = characterVelocity.y;
            Vector3 horizontalVelocity = Vector3.ProjectOnPlane(characterVelocity, Vector3.up);
            horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, maxSpeedInAir * speedModifier);
            characterVelocity = horizontalVelocity + (Vector3.up * verticalVelocity);

            // apply the gravity to the velocity
            characterVelocity += Vector3.down * gravityDownForce * Time.deltaTime;
        }

        // stop moving when none of inputs are pressed but jump and also when not in mid-air
        if (playerHandler.GetMoveInput() == Vector3.zero && !playerHandler.GetJumpInputDown() && isGrounded)
        {
            if (!isStanding)
            {
                animator.runtimeAnimatorController = Resources.Load("Standing") as RuntimeAnimatorController;
                body.transform.localEulerAngles = Vector3.zero;
                isStanding = true;
            }

            characterVelocity = Vector3.zero;
        }

        
        characterController.Move(characterVelocity * Time.deltaTime);
    }

    // Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
    bool IsNormalUnderSlopeLimit(Vector3 normal)
    {
        return Vector3.Angle(transform.up, normal) <= characterController.slopeLimit;
    }

    // Gets the center point of the bottom hemisphere of the character controller capsule    
    Vector3 GetCapsuleBottomHemisphere()
    {
        return transform.position + (transform.up * characterController.radius);
    }

    // Gets the center point of the top hemisphere of the character controller capsule    
    Vector3 GetCapsuleTopHemisphere(float atHeight)
    {
        return transform.position + (transform.up * (atHeight - characterController.radius));
    }

    // Gets a reoriented direction that is tangent to a given slope
    Vector3 GetDirectionReorientedOnSlope(Vector3 direction, Vector3 slopeNormal)
    {
        Vector3 directionRight = Vector3.Cross(direction, transform.up);
        return Vector3.Cross(slopeNormal, directionRight).normalized;
    }
    
    // returns false if there was an obstruction
    bool UpdateObstructions()
    {
        Collider[] standingOverlaps = Physics.OverlapCapsule(
            GetCapsuleBottomHemisphere(),
            GetCapsuleTopHemisphere(capsuleHeightStanding),
            characterController.radius,
            -1,
            QueryTriggerInteraction.Ignore);
        

        foreach (Collider c in standingOverlaps)
        {
            if (c != characterController) return false;
        }

        return true;
    }
}
