/*
File Name:    Hurtbox.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;

namespace HipWhipGame
{
    [RequireComponent(typeof(Collider))]
    public class Hurtbox : MonoBehaviour
    {
        public FighterController owner;

        void Awake()
        {
            var col = GetComponent<Collider>();
            col.isTrigger = true;
            gameObject.layer = LayerMask.NameToLayer("Hurtbox");
        }

        void OnDrawGizmos()
        {
            Collider col = GetComponent<Collider>();
            if (col == null)
                return;

            Gizmos.color = Color.green;

            // Save old matrix
            Matrix4x4 oldMatrix = Gizmos.matrix;

            // Apply the object's local transform (position + rotation + scale)
            Gizmos.matrix = transform.localToWorldMatrix;

            // Draw cube centered at the collider's local center
            // Using local coordinates (so rotation now applies)
            if (col is BoxCollider box)
            {
                Gizmos.DrawWireCube(box.center, box.size);
            }
            else
            {
                // fallback for other collider types
                Gizmos.DrawWireCube(Vector3.zero, col.bounds.size);
            }

            // Restore the original matrix
            Gizmos.matrix = oldMatrix;
        }
    }
}
