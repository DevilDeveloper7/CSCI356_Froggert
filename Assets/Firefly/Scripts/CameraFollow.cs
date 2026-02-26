using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;

    [Header("Camera Positioning")]
    public float distance = 8f; // Distance behind the target
    public float height = 6f;   // Height above the target
    public Vector3 offset = new Vector3(0f, 6f, -8f); // Relative offset (used as fallback)

    [Header("Smoothing")]
    public float positionSmoothSpeed = 8f; // Faster smoothing for more responsive feel
    public float rotationSmoothSpeed = 10f;

    [Header("Look At")]
    public float lookAheadDistance = 2f; // How far ahead of the target to look

    private Vector3 currentVelocity;

    void LateUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("CameraFollow: No target assigned!");
            return;
        }

        // Calculate position behind and above the target, relative to target's rotation
        // This makes the camera follow the target's facing direction
        Vector3 targetForward = target.forward;
        Vector3 targetRight = target.right;

        // Position camera behind (opposite of forward) and above the target
        Vector3 desiredPosition = target.position
            - targetForward * distance  // Behind the target
            + Vector3.up * height;      // Above the target

        // Smooth position movement using SmoothDamp for natural feel
        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref currentVelocity,
            1f / positionSmoothSpeed
        );

        // Look at a point ahead of the target (in the direction it's facing)
        Vector3 lookAtPoint = target.position + targetForward * lookAheadDistance + Vector3.up * 1f;

        // Smooth rotation towards the look target
        Quaternion targetRotation = Quaternion.LookRotation(lookAtPoint - transform.position);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSmoothSpeed * Time.deltaTime
        );
    }
}
