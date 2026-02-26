using UnityEngine;

public class FrogController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 10f;
    public float rotationSpeed = 10f;
    private float currentSpeed;

    [Header("Jump Settings")]
    public float baseRegularJumpForce = 7f;  // Regular jump when standing
    public float baseHighJumpForce = 12f;    // High jump when sprinting (Shift + Space)

    [Header("Weight System")]
    public float currentWeight = 1f;         // Current weight (starts at 1)
    public float maxWeight = 10f;            // Maximum weight limit
    public float weightPerBug = 0.5f;        // Weight gained per bug eaten
    public float weightLossPerJump = 0.1f;   // Weight lost per jump
    public float minWeight = 0.5f;           // Minimum weight (can't go below)

    // Weight affects jump height - heavier = lower jumps
    [Header("Weight Jump Penalties")]
    public float jumpWeightPenalty = 0.15f;  // Jump force multiplier reduction per weight unit

    [Header("Components")]
    private CharacterController characterController;
    private Vector3 moveDirection;
    private float gravity = -20f;
    private float verticalVelocity = 0f;
    private bool isJumping = false;
    private bool isSprinting = false;

    void Start()
    {
        // Get or add CharacterController component
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
            characterController.radius = 0.5f;
            characterController.height = 1f;
            characterController.center = new Vector3(0, 0.5f, 0);
        }

        currentSpeed = walkSpeed;
    }

    void Update()
    {
        // Check for sprint input (Shift key)
        isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // Set current speed based on sprint state
        currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        // Get input from WASD keys
        float horizontal = Input.GetAxis("Horizontal"); // A/D keys
        float vertical = Input.GetAxis("Vertical");     // W/S keys

        // Calculate movement direction relative to world space
        Vector3 move = new Vector3(horizontal, 0f, vertical).normalized;

        if (move.magnitude >= 0.1f)
        {
            // Rotate the frog to face movement direction
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // Move the frog at current speed
            moveDirection = move * currentSpeed;
        }
        else
        {
            moveDirection = Vector3.zero;
        }

        // Jump input (Space key)
        if (characterController.isGrounded)
        {
            if (isJumping)
            {
                isJumping = false;
            }

            if (Input.GetButtonDown("Jump")) // Space key
            {
                PerformJump();
            }
            else
            {
                verticalVelocity = -2f; // Small downward force to keep grounded
            }
        }
        else
        {
            // Apply gravity when in air
            verticalVelocity += gravity * Time.deltaTime;
        }

        // Combine horizontal movement and vertical velocity
        Vector3 finalMove = moveDirection + Vector3.up * verticalVelocity;

        // Move the character
        characterController.Move(finalMove * Time.deltaTime);
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

        verticalVelocity = finalJumpForce;
        isJumping = true;

        // Lose weight when jumping (burning energy)
        LoseWeight(weightLossPerJump);

        Debug.Log($"Jump! Type: {(isSprinting ? "HIGH" : "REGULAR")}, Force: {finalJumpForce:F1}, Weight: {currentWeight:F1}");
    }

    // Called by BugEater when a bug is eaten
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

    // Getter for UI and other systems
    public float GetWeight()
    {
        return currentWeight;
    }

    public float GetWeightPercentage()
    {
        return (currentWeight - minWeight) / (maxWeight - minWeight);
    }
}
