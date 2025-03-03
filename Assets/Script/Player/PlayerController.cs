using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;
    public float airControlFactor = 0.5f;

    [Header("Jump Settings")]
    public float jumpForce = 8f;
    public float gravityMultiplier = 2f;
    public float groundCheckDistance = 0.2f;
    public float sphereRadius = 0.5f;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;
    public float jumpCD = 0.2f;

    [Header("Input Reference")]
    public PlayerInput playerInput; // Assign the PlayerInput component here
    public Vector2 joyStickInput;
    public bool isJoystickInput;
    public UltimateJoystick JoystickPA;

    [Header("Unity Event Settings")]
    public UnityEvent onDie; // Unity Event to invoke when the player dies
    public UnityEvent onReachedDes; // Unity Event to invoke when the player dies

    private Rigidbody rb;
    public Vector2 movementInput;
    public bool isGrounded;
    public bool isJumping;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private float jumpTimer;
    public PlayerAnimManager playerAnimManager;

    private InputAction moveAction;
    private InputAction jumpAction;

    private bool isAlive = true; // Track if the player is alive
    private bool isMovingToDestination = false; // Track if the player is moving to a destination

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Get references to the input actions
        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
    }

    void Update()
    {
        if (!isAlive || isMovingToDestination) return; // Stop updating if the player is dead or moving to a destination

        HandleMovementInput();
        JoyStickInput();
        HandleJumpInput();
        UpdateAnimation();
    }

    void JoyStickInput()
    {
        joyStickInput.x = JoystickPA.GetHorizontalAxis();
        joyStickInput.y = JoystickPA.GetVerticalAxis();

        isJoystickInput = (joyStickInput != Vector2.zero);

        if (joyStickInput != Vector2.zero && joyStickInput != movementInput)
            movementInput = joyStickInput;
    }

    void HandleMovementInput()
    {
        // Read movement input from the Input System
        movementInput = moveAction.ReadValue<Vector2>();
    }

    void HandleJumpInput()
    {
        // Read jump input from the Input System
        if (jumpAction.triggered)
        {
            ApplyJump();
        }
        UpdateJumpCD();
    }

    void FixedUpdate()
    {
        if (!isAlive || isMovingToDestination) return; // Stop physics updates if the player is dead or moving to a destination

        // Ground check
        isGrounded = CheckGrounded();
        Debug.Log("Is grounded: " + isGrounded);

        // Apply gravity
        if (!isGrounded)
        {
            rb.AddForce(Physics.gravity * gravityMultiplier, ForceMode.Acceleration);
        }

        // Move the player
        MovePlayer(new Vector3(movementInput.x, 0f, movementInput.y));
    }

    void MovePlayer(Vector3 direction)
    {
        // Normalize and apply speed
        Vector3 moveVelocity = direction.normalized * moveSpeed;

        // Reduce control while in the air
        if (!isGrounded)
        {
            moveVelocity *= airControlFactor;
        }

        if (movementInput.magnitude > 0.1f)
        {
            RotatePlayer(new Vector3(movementInput.x, 0f, movementInput.y));
        }

        // Apply movement
        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
    }

    void RotatePlayer(Vector3 direction)
    {
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    IEnumerator Jump()
    {
        // Buffer the jump input
        jumpBufferCounter = jumpBufferTime;

        // Wait for coyote time or grounded state
        while (jumpBufferCounter > 0)
        {
            // Check if the player is grounded or within coyote time
            if (isGrounded || coyoteTimeCounter > 0)
            {
                playerAnimManager.TriggerJump();
                jumpTimer = jumpCD;
                // Apply upward force for jumping
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // Reset vertical velocity
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

                // Reset jump buffer and coyote time
                jumpBufferCounter = 0f;
                coyoteTimeCounter = 0f;

                // Set jumping state
                isJumping = true;

                // Exit the coroutine
                yield break;
            }

            // Decrease jump buffer timer
            jumpBufferCounter -= Time.deltaTime;
            yield return null;
        }
    }

    public void ApplyJump()
    {
        if (isJumping) return;
        StartCoroutine(Jump());
    }

    bool CheckGrounded()
    {
        Vector3 spherePosition = transform.position + Vector3.up * sphereRadius; // Start slightly above the player

        // Perform a SphereCast downward
        bool hit = Physics.SphereCast(
            spherePosition, // Start position
            sphereRadius,   // Radius of the sphere
            Vector3.down,   // Direction of the cast
            out RaycastHit hitInfo, // Store hit information
            sphereRadius + groundCheckDistance // Distance to cast (sphere radius + ground check distance)
        );

        // Check if the hit object is ground
        if (hit)
        {
            coyoteTimeCounter = coyoteTime; // Reset coyote time
            return true;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime; // Decrease coyote time
            return false;
        }
    }

    void UpdateJumpCD()
    {
        if (jumpTimer > 0)
        {
            jumpTimer -= Time.deltaTime;
        }
        else
        {
            isJumping = false;
        }
    }

    void UpdateAnimation()
    {
        bool isRunning = movementInput != Vector2.zero;
        playerAnimManager.SetRunning(isRunning);
    }

    // Die method to disable player controller and invoke Unity Event
    public void Die()
    {
        if (!isAlive) return;
        StartCoroutine(EnumDie());
    }
    IEnumerator EnumDie()
    {
        isAlive = false; // Mark the player as dead

        // Disable player input and movement
        playerInput.enabled = false;
        playerAnimManager.TriggerDie2();
        yield return new WaitForSeconds(1f);
        // Invoke the Unity Event
        onDie.Invoke();

        Debug.Log("Player has died.");
    }

    // Move to a specific destination
    public void MoveToDestination(Transform destination)
    {
        if (isMovingToDestination) return; // Prevent multiple calls

        StartCoroutine(MoveToDestinationCoroutine(destination));
    }

    private IEnumerator MoveToDestinationCoroutine(Transform destination)
    {
        isMovingToDestination = true; // Disable player input
        playerInput.enabled = false; // Disable input system
        rb.velocity = Vector3.zero;
        playerAnimManager.SetRunning(false);
        yield return new WaitForSeconds(1f);
        while (Vector3.Distance(transform.position, destination.position) > 0.1f)
        {
            // Calculate direction to the destination
            Vector3 direction = (destination.position - transform.position).normalized;
            playerAnimManager.SetRunning(true);
            // Move towards the destination
            rb.velocity = direction * moveSpeed;

            // Rotate towards the destination
            RotatePlayer(direction);

            yield return null; // Wait for the next frame
        }

        // Stop movement when reaching the destination
        rb.velocity = Vector3.zero;
        isMovingToDestination = false;
        playerInput.enabled = true; // Re-enable input system
        onReachedDes.Invoke();
        Debug.Log("Player has reached the destination.");
    }

    // Optional: Draw a debug line to visualize the ground check
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 spherePosition = transform.position + Vector3.up * sphereRadius;

        // Draw the sphere
        Gizmos.DrawWireSphere(spherePosition, sphereRadius);

        // Draw the cast direction
        Gizmos.DrawLine(spherePosition, spherePosition + Vector3.down * (sphereRadius + groundCheckDistance));
    }
}