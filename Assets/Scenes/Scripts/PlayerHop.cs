using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

// Put this on the small box (the player).
// W / Up    = hop to the nearest platform away from the camera
// S / Down  = hop to the nearest platform toward the camera
// A / Left  = hop left,  D / Right = hop right
// After every hop the box is snapped to the exact top-center of the platform.
public class PlayerHopper : MonoBehaviour
{
    [Header("Hop")]
    [Tooltip("Seconds the hop takes. Set to 0 for an instant teleport.")]
    public float hopDuration = 0.25f;
    [Tooltip("How high the arc goes above the straight line between platforms.")]
    public float hopHeight = 1.5f;
    [Tooltip("Platforms farther than this are ignored.")]
    public float maxHopDistance = 10f;
    [Tooltip("How far off the pressed direction a platform can be and still count (degrees).")]
    [Range(1f, 89f)] public float searchConeAngle = 45f;

    [Header("Direction")]
    [Tooltip("If on, W/S/A/D are relative to the camera's view. If off, they use world +Z/-Z/-X/+X.")]
    public bool cameraRelative = true;
    public Transform cameraTransform;

    // Fires every time the player starts a hop. Other scripts (like GameTimer) can listen to it.
    public event System.Action Hopped;

    Platform current;
    bool isHopping;
    float halfHeight;

    void Start()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        // Physics would fight the hop and push the box off-center, so make it kinematic.
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        halfHeight = GetHalfHeight();

        // Start centered on whichever platform the box is closest to.
        current = FindNearestPlatform();
        if (current != null) SnapTo(current);
    }

    void Update()
    {
        if (isHopping) return;

        Keyboard kb = Keyboard.current;
        if (kb == null) return; // no keyboard connected

        Vector3 input = Vector3.zero;
        if (kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame)         input = Vector3.forward;
        else if (kb.sKey.wasPressedThisFrame || kb.downArrowKey.wasPressedThisFrame)  input = Vector3.back;
        else if (kb.aKey.wasPressedThisFrame || kb.leftArrowKey.wasPressedThisFrame)  input = Vector3.left;
        else if (kb.dKey.wasPressedThisFrame || kb.rightArrowKey.wasPressedThisFrame) input = Vector3.right;

        if (input == Vector3.zero) return;

        Platform target = FindPlatformInDirection(ToWorldDirection(input));
        if (target != null)
        {
            Hopped?.Invoke();
            StartCoroutine(HopTo(target));
        }
    }

    // Converts a WASD direction into a flat world direction (optionally relative to the camera).
    Vector3 ToWorldDirection(Vector3 input)
    {
        if (!cameraRelative || cameraTransform == null) return input;

        Vector3 forward = cameraTransform.forward; forward.y = 0f;
        Vector3 right   = cameraTransform.right;   right.y = 0f;

        // Camera looking straight down: its forward flattens to nothing, so use its up instead.
        if (forward.sqrMagnitude < 0.001f) { forward = cameraTransform.up; forward.y = 0f; }

        forward.Normalize();
        right.Normalize();
        return (forward * input.z + right * input.x).normalized;
    }

    // Closest platform that lies roughly in the pressed direction.
    Platform FindPlatformInDirection(Vector3 dir)
    {
        Vector3 from = current != null ? current.TopCenter : transform.position;
        float minDot = Mathf.Cos(searchConeAngle * Mathf.Deg2Rad);

        Platform best = null;
        float bestDist = float.MaxValue;

        foreach (Platform p in Platform.All)
        {
            if (p == current) continue;

            Vector3 offset = p.TopCenter - from;
            offset.y = 0f; // compare on the ground plane only, so stack height doesn't matter
            float dist = offset.magnitude;
            if (dist < 0.01f || dist > maxHopDistance) continue;

            if (Vector3.Dot(offset / dist, dir) < minDot) continue; // not in that direction

            if (dist < bestDist)
            {
                bestDist = dist;
                best = p;
            }
        }
        return best;
    }

    Platform FindNearestPlatform()
    {
        Platform best = null;
        float bestDist = float.MaxValue;
        foreach (Platform p in Platform.All)
        {
            Vector3 offset = p.TopCenter - transform.position;
            offset.y = 0f;
            if (offset.sqrMagnitude < bestDist)
            {
                bestDist = offset.sqrMagnitude;
                best = p;
            }
        }
        return best;
    }

    IEnumerator HopTo(Platform target)
    {
        isHopping = true;

        Vector3 start = transform.position;
        Vector3 end = LandingPoint(target);
        float t = 0f;

        while (t < hopDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / hopDuration);

            Vector3 pos = Vector3.Lerp(start, end, k);
            pos.y += Mathf.Sin(k * Mathf.PI) * hopHeight; // arc
            transform.position = pos;

            yield return null;
        }

        // Final snap guarantees perfect centering, even if a frame overshot.
        current = target;
        SnapTo(target);
        isHopping = false;
    }

    void SnapTo(Platform p)
    {
        transform.position = LandingPoint(p);
    }

    // Top-center of the platform, raised by half the player's height so it sits on top.
    Vector3 LandingPoint(Platform p)
    {
        return p.TopCenter + Vector3.up * halfHeight;
    }

    float GetHalfHeight()
    {
        Collider col = GetComponent<Collider>();
        if (col != null) return col.bounds.extents.y;

        Renderer rend = GetComponent<Renderer>();
        if (rend != null) return rend.bounds.extents.y;

        return transform.lossyScale.y * 0.5f;
    }
}