using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player; // Player's Transform in the inspector
    public float smoothSpeed = 0.125f; // Adjust for smoother or tighter camera movement
    public Vector3 offset; // Set this in the inspector based on desired camera positioning relative to the player

    void LateUpdate()
    {
        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        transform.LookAt(player);
    }
}
