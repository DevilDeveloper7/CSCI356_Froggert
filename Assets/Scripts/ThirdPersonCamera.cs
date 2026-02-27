using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 targetOffset = new Vector3(0f, 1.5f, 0f);

    [Header("Distance")]
    [SerializeField] private float distance = 8f;
    [SerializeField] private float minDistance = 3f;
    [SerializeField] private float maxDistance = 12f;

    [Header("Rotation")]
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float minPitch = -20f;
    [SerializeField] private float maxPitch = 60f;
    [SerializeField] private bool invertY = false;

    [Header("Smoothing")]
    [SerializeField] private float positionSmoothTime = 0.08f;
    [SerializeField] private float rotationSmoothTime = 0.05f;

    [Header("Collision")]
    [SerializeField] private LayerMask collisionMask = ~0;
    [SerializeField] private float collisionRadius = 0.3f;
    [SerializeField] private float collisionPadding = 0.1f;

    [Header("Input")]
    [SerializeField] private bool lockCursorOnStart = true;
    [SerializeField] private bool useMouseControl = true;

    private float yaw;
    private float pitch;
    private float yawVelocity;
    private float pitchVelocity;
    private Vector3 positionVelocity;
    private Transform cachedTransform;

        

        private void Start()
    {
        cachedTransform = transform;
        Vector3 angles = cachedTransform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x;

        if (lockCursorOnStart && useMouseControl)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        // Auto-find target if not set
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
        }
    }

    private void Update()
    {
        // Allow unlocking cursor with Escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        // Relock cursor on click
        if (Input.GetMouseButtonDown(0) && useMouseControl)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        // Get mouse input only if cursor is locked
        if (useMouseControl && Cursor.lockState == CursorLockMode.Locked)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            float deltaYaw = mouseX * mouseSensitivity;
            float deltaPitch = mouseY * mouseSensitivity;

            if (invertY)
            {
                deltaPitch = -deltaPitch;
            }

            yaw += deltaYaw;
            pitch = Mathf.Clamp(pitch - deltaPitch, minPitch, maxPitch);
        }

        // Smooth rotation
        float smoothYaw = Mathf.SmoothDampAngle(transform.eulerAngles.y, yaw, ref yawVelocity, rotationSmoothTime);
        float smoothPitch = Mathf.SmoothDampAngle(transform.eulerAngles.x, pitch, ref pitchVelocity, rotationSmoothTime);

        Quaternion rotation = Quaternion.Euler(smoothPitch, smoothYaw, 0f);

        // Calculate desired position
        float clampedDistance = Mathf.Clamp(distance, minDistance, maxDistance);
        Vector3 focusPoint = target.position + targetOffset;
        Vector3 desiredPosition = focusPoint - rotation * Vector3.forward * clampedDistance;

        // Collision detection
        if (Physics.SphereCast(focusPoint, collisionRadius, (desiredPosition - focusPoint).normalized,
            out RaycastHit hit, clampedDistance, collisionMask, QueryTriggerInteraction.Ignore))
        {
            float hitDistance = Mathf.Max(hit.distance - collisionPadding, minDistance);
            desiredPosition = focusPoint - rotation * Vector3.forward * hitDistance;
        }

        // Smooth position and apply rotation
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref positionVelocity, positionSmoothTime);
        transform.rotation = rotation;
    }

    // Public method to get camera yaw for character rotation
    public float GetCameraYaw()
    {
        return yaw;
    }
}
