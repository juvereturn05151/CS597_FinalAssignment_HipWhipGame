/*
File Name:    CollisionCapsule.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;

namespace RollbackSupport
{
    [System.Serializable]
    public struct CollisionCapsule
    {
        public Vector3 localCenter; // offset from fighter root
        public float radius;        // radius on XZ plane
        public float height;        // for gizmos/visualization only
        public bool enabled;

        /// <summary>
        /// World-space center of the capsule on XZ plane.
        /// Y is preserved from transformed point but usually ignored for push.
        /// </summary>
        public Vector3 GetWorldCenter(Transform t)
        {
            return t.TransformPoint(localCenter);
        }

#if UNITY_EDITOR
        public void DrawGizmos(Transform t, Color color)
        {
            if (!enabled) return;

            Vector3 center = GetWorldCenter(t);
            float h = Mathf.Max(0f, height - 2f * radius);
            float half = h * 0.5f;

            Gizmos.color = color;

            // Draw top and bottom spheres and a "cylinder" between them (cheap-ish visualization)
            Vector3 top = center + Vector3.up * half;
            Vector3 bottom = center - Vector3.up * half;

            Gizmos.DrawWireSphere(top, radius);
            Gizmos.DrawWireSphere(bottom, radius);
            Gizmos.DrawLine(top + Vector3.forward * radius, bottom + Vector3.forward * radius);
            Gizmos.DrawLine(top - Vector3.forward * radius, bottom - Vector3.forward * radius);
            Gizmos.DrawLine(top + Vector3.right * radius, bottom + Vector3.right * radius);
            Gizmos.DrawLine(top - Vector3.right * radius, bottom - Vector3.right * radius);
        }
#endif
    }
}
