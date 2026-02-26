using UnityEngine;

public class FrogControllerPhysics : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float acceleration = 20f;
    [SerializeField] private float airControlFactor = 0.3f;

    [Header("Jump Settings")]
    [SerializeField] private float baseRegularJumpForce = 7f;
    [SerializeField] private float baseHighJumpForce = 12f;
    [SerializeField] private LayerMask groundLayer = -1;
    [SerializeField] private float groundCheckDistance = 0.2f;

    [Header("Weight System")]
    public float currentWeight = 1f;
    public float maxWeight = 10f;
    public float weightPerBug = 0.5f;
    public float weightLossPerJump = 0.1f;
    public float minWeight = 0.5f;
    [Tooltip("Jump force reduction per weight unit above 1.0")]
    public float jumpWeightPenalty = 0.15f;

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 720f;

    [Header("Components")]
    private Rigidbody rb;
    private ThirdPersonCamera cameraController;
    private Vector3 moveInput;
    private bool jumpPressed;
    private bool isSprinting;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
                        rb = gameObject.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.linearDamping = 0.5f;
            rb.angularDamping = 0.05f;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        // Find camera controller
        GameObject cam = GameObject.FindGameObjectWithTag("MainCamera");
        if (cam != null)
        {
            cameraController = cam.GetComponent<ThirdPersonCamera>();
        }

        Debug.Log("FrogControllerPhysics initialized with weight: " + currentWeight);
    }

void Update()
    {
        // Get input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveInput = new Vector3(horizontal, 0f, vertical).normalized;

        // Check sprint
        isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // Check jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpPressed = true;
        }

        // Check ground
        CheckGrounded();
    }

    void FixedUpdate()
    {
        // Handle movement
        HandleMovement();

        // Handle jump
        if (jumpPressed)
        {
            PerformJump();
            jumpPressed = false;
        }
    }

    void HandleMovement()
    {
        // Get camera-relative movement direction
        Vector3 moveDirection = GetCameraRelativeMovement();

        // Calculate target speed
        float targetSpeed = isSprinting ? sprintSpeed : walkSpeed;
        Vector3 targetVelocity = moveDirection * targetSpeed;

        // Apply movement force
        float currentAcceleration = isGrounded ? acceleration : acceleration * airControlFactor;
        Vector3 velocityChange = targetVelocity - new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        velocityChange = Vector3.ClampMagnitude(velocityChange, currentAcceleration * Time.fixedDeltaTime);

        rb.AddForce(velocityChange, ForceMode.VelocityChange);

        // Rotate towards movement direction
        if (moveDirection.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.fixedDeltaTime
            );
        }
    }

    Vector3 GetCameraRelativeMovement()
    {
        if (moveInput.magnitude < 0.1f)
            return Vector3.zero;

        // Get camera forward and right
        Transform cameraTransform = Camera.main.transform;
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // Project onto horizontal plane
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        // Calculate movement direction relative to camera
        return forward * moveInput.z + right * moveInput.x;
    }

    void PerformJump()
    {
        // Calculate jump force based on weight
        float weightMultiplier = 1f - ((currentWeight - 1f) * jumpWeightPenalty);
        weightMultiplier = Mathf.Max(0.3f, weightMultiplier); // Minimum 30% jump force

        // Choose jump type based on sprint state
        float baseJumpForce = isSprinting ? baseHighJumpForce : baseRegularJumpForce;

        // Apply weight penalty to jump force
        float finalJumpForce = baseJumpForce * weightMultiplier;

        // Apply jump
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, finalJumpForce, rb.linearVelocity.z);

        // Lose weight when jumping
        LoseWeight(weightLossPerJump);

        Debug.Log($"Jump! Type: {(isSprinting ? "HIGH" : "REGULAR")}, Force: {finalJumpForce:F1}, Weight: {currentWeight:F1}");
    }

void CheckGrounded()
    {
        // Raycast down from center of frog to check if grounded
        RaycastHit hit;
        // Start raycast from center of capsule collider
        Vector3 origin = transform.position;
        // Check down for distance equal to half the collider height plus a small margin
        float checkDistance = 0.6f; // Slightly more than half the capsule height
        
        if (Physics.Raycast(origin, Vector3.down, out hit, checkDistance))
        {
            // Check if the hit object is not ourselves
            if (hit.collider.gameObject != gameObject)
            {
                isGrounded = true;
                return;
            }
        }
        
        // Also use a SphereCast for more reliable detection
        if (Physics.SphereCast(origin, 0.3f, Vector3.down, out hit, 0.6f))
        {
            if (hit.collider.gameObject != gameObject)
            {
                isGrounded = true;
                return;
            }
        }
        
        isGrounded = false;
    }

    // Weight management methods
    public void GainWeight(float amount)
    {
        currentWeight += amount;
        currentWeight = Mathf.Clamp(currentWeight, minWeight, maxWeight);
        Debug.Log($"Weight gained! New weight: {currentWeight:F1}");
    }

    void LoseWeight(float amount)
    {
        currentWeight -= amount;
        currentWeight = Mathf.Clamp(currentWeight, minWeight, maxWeight);
    }

    // Getters for UI and other systems
    public float GetWeight()
    {
        return currentWeight;
    }

    public float GetWeightPercentage()
    {
        return (currentWeight - minWeight) / (maxWeight - minWeight);
    }

    void OnDrawGizmos()
    {
        // Draw ground check ray
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Vector3 origin = transform.position + Vector3.up * 0.1f;
        Gizmos.DrawLine(origin, origin + Vector3.down * (groundCheckDistance + 0.1f));
    }
}
