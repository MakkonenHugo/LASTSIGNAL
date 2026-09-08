using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerAdvanced : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 9f;
    public float acceleration = 14f;
    public float deceleration = 10f;
    public float airControlMultiplier = 0.4f;
    public float gravity = -15f;

    [Header("Jump")]
    public float jumpVelocity = 6f;
    public float coyoteTime = 0.12f;
    public float jumpBufferTime = 0.12f;

    [Header("Camera Shake")]
    public CameraShake cameraShake;
    public float landShakeIntensity = 0.08f;
    public float landShakeDuration = 0.15f;
    public float sprintShakeIntensity = 0.03f;
    public float jumpShakeIntensity = 0.05f;

    private CharacterController controller;
    private Vector3 horizontalVelocity;
    private float verticalVelocity;

    private float coyoteTimer;
    private float jumpBufferTimer;
    private bool wasGroundedLastFrame;
    private bool isSprinting;
    private bool queuedJump;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (DoorCodePanel.IsAnyPanelOpen)
            return;

        ReadInput();
        Move();
        UpdateSprintShake();
        DetectLanding();
    }

    void ReadInput()
    {
        Vector2 moveInput = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
            if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
            if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;
            if (Keyboard.current.dKey.isPressed) moveInput.x += 1;

            if (Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                queuedJump = true;
                jumpBufferTimer = jumpBufferTime;
            }

            bool shiftHeld = Keyboard.current.leftShiftKey.isPressed
                && moveInput.sqrMagnitude > 0.01f;

            if (controller.isGrounded)
            {
                isSprinting = shiftHeld;
            }
            else
            {
                isSprinting = isSprinting && shiftHeld;
            }
        }

        moveInput = Vector2.ClampMagnitude(moveInput, 1f);

        Vector3 wishDir = transform.right * moveInput.x + transform.forward * moveInput.y;
        float targetSpeed = isSprinting ? sprintSpeed : walkSpeed;
        Vector3 wishVelocity = wishDir * targetSpeed;

        bool grounded = controller.isGrounded;
        float rate = wishVelocity.sqrMagnitude > 0.01f ? acceleration : deceleration;

        if (!grounded)
            rate *= airControlMultiplier;

        horizontalVelocity = Vector3.MoveTowards(horizontalVelocity, wishVelocity, rate * Time.deltaTime);
    }

    void Move()
    {
        bool grounded = controller.isGrounded;

        if (grounded)
        {
            coyoteTimer = coyoteTime;

            if (verticalVelocity < 0f)
                verticalVelocity = -2f;
        }
        else
        {
            coyoteTimer = Mathf.Max(0f, coyoteTimer - Time.deltaTime);
        }

        if (jumpBufferTimer > 0f)
        {
            jumpBufferTimer -= Time.deltaTime;
        }

        bool canJump = coyoteTimer > 0f && jumpBufferTimer > 0f;

        if (canJump)
        {
            verticalVelocity = jumpVelocity;
            coyoteTimer = 0f;
            jumpBufferTimer = 0f;
            queuedJump = false;

            if (cameraShake != null)
                cameraShake.TriggerPulse(jumpShakeIntensity, 0.15f);
        }

        verticalVelocity += gravity * Time.deltaTime;

        Vector3 fullVelocity = horizontalVelocity;
        fullVelocity.y = verticalVelocity;

        controller.Move(fullVelocity * Time.deltaTime);
    }

    void UpdateSprintShake()
    {
        if (cameraShake == null)
            return;

        bool grounded = controller.isGrounded;

        if (isSprinting && grounded)
        {
            if (!cameraShake.IsShaking())
            {
                cameraShake.intensity = sprintShakeIntensity;
                cameraShake.StartShake(CameraShake.ShakeSourceType.Sprint);
            }
        }
        else if (cameraShake.IsShaking() && cameraShake.ShakeSource() == CameraShake.ShakeSourceType.Sprint)
        {
            cameraShake.StopShake();
        }
    }

    void DetectLanding()
    {
        bool grounded = controller.isGrounded;

        if (grounded && !wasGroundedLastFrame && verticalVelocity <= -0.1f)
        {
            if (cameraShake != null)
                cameraShake.TriggerPulse(landShakeIntensity, landShakeDuration);
        }

        wasGroundedLastFrame = grounded;
    }
}