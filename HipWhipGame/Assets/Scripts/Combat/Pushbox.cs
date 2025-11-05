/*
File Name:    Pushbox.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;

namespace HipWhipGame
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class Pushbox : MonoBehaviour
    {
        public FighterController owner;
        public float pushStrength = 1f;

        private CapsuleCollider _capsule;

        public Vector3 WorldCenter => transform.TransformPoint(_capsule.center);
        public float Radius => _capsule.radius;

        void Awake()
        {
            _capsule = GetComponent<CapsuleCollider>();
            _capsule.isTrigger = true;
            gameObject.layer = LayerMask.NameToLayer("Pushbox");

            if (!owner)
                owner = GetComponentInParent<FighterController>();
        }

        void OnDrawGizmosSelected()
        {
            if (!_capsule)
                _capsule = GetComponent<CapsuleCollider>();

            Gizmos.color = Color.yellow;

            // Capsule data
            Vector3 center = transform.TransformPoint(_capsule.center);
            float radius = _capsule.radius;
            float height = Mathf.Max(_capsule.height, radius * 2f);

            // Local up direction (based on collider direction)
            Vector3 upDir = Vector3.up;
            if (_capsule.direction == 0) upDir = Vector3.right;      // X-axis
            else if (_capsule.direction == 1) upDir = Vector3.up;    // Y-axis (default)
            else if (_capsule.direction == 2) upDir = Vector3.forward; // Z-axis

            upDir = transform.TransformDirection(upDir);

            // Compute world-space positions of the top and bottom spheres
            float halfHeight = (height * 0.5f) - radius;
            Vector3 top = center + upDir * halfHeight;
            Vector3 bottom = center - upDir * halfHeight;

            // Draw top and bottom spheres
            Gizmos.DrawWireSphere(top, radius);
            Gizmos.DrawWireSphere(bottom, radius);

            // Draw lines connecting spheres (approximate capsule body)
            Vector3 right = transform.right * radius;
            Vector3 forward = transform.forward * radius;

            Gizmos.DrawLine(top + right, bottom + right);
            Gizmos.DrawLine(top - right, bottom - right);
            Gizmos.DrawLine(top + forward, bottom + forward);
            Gizmos.DrawLine(top - forward, bottom - forward);
        }

    }
}
