using UnityEngine;

// Put this on the Main Camera (NOT as a child of the player).
// Position the camera how you like it in the editor first; the script keeps that same
// offset from the player and smoothly follows it around.
public class CameraFollow : MonoBehaviour
{
    [Tooltip("The small box (the object with PlayerHopper).")]
    public Transform target;

    [Tooltip("Roughly how many seconds the camera takes to catch up. 0 = locked on instantly.")]
    public float smoothTime = 0.2f;

    [Tooltip("If off, the camera ignores the hop arc and up/down movement, so it doesn't bob.")]
    public bool followHeight = false;

    Vector3 offset;
    Vector3 velocity;
    float fixedY;

    void Start()
    {
        // No target dragged in? Look for the object that has PlayerHopper on it.
        if (target == null)
        {
            PlayerHopper player = FindAnyObjectByType<PlayerHopper>();
            if (player != null) target = player.transform;
        }

        if (target == null)
        {
            Debug.LogWarning("CameraFollow: no target assigned, and no PlayerHopper found in the scene.");
            enabled = false;
            return;
        }

        // Remember where the camera sits relative to the player right now.
        offset = transform.position - target.position;
        fixedY = transform.position.y;
    }

    // LateUpdate runs after the player has moved this frame, which avoids jitter.
    void LateUpdate()
    {
        Vector3 desired = target.position + offset;
        if (!followHeight) desired.y = fixedY;

        transform.position = smoothTime <= 0f
            ? desired
            : Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
    }
}