/*
File Name:    FighterStats.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;

namespace HipWhipGame
{
    public class FighterStats : MonoBehaviour
    {
        [Header("Movement")]
        public float walkSpeed = 3.5f;
        public float jumpForce = 8f;
        public float gravity = 20f;

        [Header("Combat")]
        public float weight = 1f;   // scales knockback
        public float hitstunScale = 1f; // scales received hitstun
        public float pushStrength = 1f; // scales pushback applied to others
    }
}