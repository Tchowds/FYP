using UnityEngine;

// Script to attach to the menu UI element to follow the player's view in VR
public class MenuFollower : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The Transform for the VR Camera's center eye (player view).")]
    public Transform centerEyeAnchor;

    [Header("Position Settings")]
    [Tooltip("Distance from the camera to place the Canvas.")]
    public float distanceFromPlayer = 2.0f;
    [Tooltip("Additional offset (x, y, z) to adjust the canvas placement.")]
    public Vector3 positionOffset;
    [Tooltip("Only update the canvas position if the difference is greater than this threshold.")]
    public float positionThreshold = 0.1f;
    [Tooltip("Time for smoothing the canvas movement (larger values = slower movement).")]
    public float positionSmoothTime = 0.3f;

    private Vector3 velocity = Vector3.zero;

    [Header("Rotation Settings")]
    [Tooltip("If true, the Canvas will always face the camera.")]
    public bool lookAtCamera = true;
    [Tooltip("Time for smoothing the rotation (larger values = slower rotation).")]
    public float rotationSmoothTime = 0.3f;

    private void LateUpdate()
    {
        // Calculate the desired position in front of the player's view.
        Vector3 desiredPosition = centerEyeAnchor.position +
                                  centerEyeAnchor.forward * distanceFromPlayer +
                                  positionOffset;

        // Only update the position if it's further than the specified threshold.
        if (Vector3.Distance(transform.position, desiredPosition) > positionThreshold)
        {
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, positionSmoothTime);
        }

        // If the canvas should always face the camera, smoothly rotate it.
        if (lookAtCamera)
        {
            // Calculate the rotation so that the canvas faces away from the camera.
            Quaternion targetRotation = Quaternion.LookRotation(transform.position - centerEyeAnchor.position);
            // Smoothly interpolate from the current rotation to the target rotation.
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime / rotationSmoothTime);
        }
    }
}
