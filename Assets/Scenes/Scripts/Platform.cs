using System.Collections.Generic;
using UnityEngine;

// Put this on the ROOT object of each platform (e.g. the parent of a stack of cubes).
// It works out the exact center of the platform's top surface from all of its colliders,
// so the player always lands perfectly centered no matter how the platform is built.
public class Platform : MonoBehaviour
{
    // Registry of every active platform, so the player doesn't need FindObjectsOfType every hop.
    public static readonly List<Platform> All = new List<Platform>();

    void OnEnable()  { All.Add(this); }
    void OnDisable() { All.Remove(this); }

    // Center of the top face, in world space.
    public Vector3 TopCenter
    {
        get
        {
            Collider[] cols = GetComponentsInChildren<Collider>();
            if (cols.Length == 0) return transform.position;

            Bounds b = cols[0].bounds;
            for (int i = 1; i < cols.Length; i++) b.Encapsulate(cols[i].bounds);

            return new Vector3(b.center.x, b.max.y, b.center.z);
        }
    }

#if UNITY_EDITOR
    // Shows a small yellow sphere where the player will land (Scene view only).
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(TopCenter, 0.1f);
    }
#endif
}