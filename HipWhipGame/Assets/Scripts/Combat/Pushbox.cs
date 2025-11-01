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
        public float radius = 0.4f;
        public float height = 1.8f;
        public float yCenter = 0.9f;

        CapsuleCollider _capsule;

        void Awake()
        {
            _capsule = GetComponent<CapsuleCollider>();
            _capsule.isTrigger = false; // solid for CharacterController overlap checks
            _capsule.center = new Vector3(0, yCenter, 0);
            _capsule.radius = radius;
            _capsule.height = height;
            gameObject.layer = LayerMask.NameToLayer("Default");
            if (!owner) owner = GetComponentInParent<FighterController>();
        }
    }
}