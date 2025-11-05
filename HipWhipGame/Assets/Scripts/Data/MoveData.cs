/*
File Name:    MoveData.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using System.Collections.Generic;
using UnityEngine;
using static HipWhipGame.Enums;

namespace HipWhipGame
{
    [CreateAssetMenu(fileName = "MoveData", menuName = "FightingGame/MoveData")]
    public class MoveData : ScriptableObject
    {
        [Header("Identity")]
        public string moveName;
        public AnimationClip animation;

        [Header("Frame Data (at 60 FPS)")]
        public int startup = 5;
        public int active = 3;
        public int recovery = 12;

        [Header("Hit Properties")]
        public int damage = 10;
        public int hitstunFrames = 15;
        public int blockstunFrames;
        public Vector3 knockback = new Vector3(2f, 0.5f, 0f); // world-space or local-forward scaled
        public GuardType guardType = GuardType.Block;
        public float pushbackOnHit = 1.5f;

        [Header("Cancel Rules")]
        public bool canCancelOnHit = false;
        public bool canCancelOnBlock = false;
        public MoveData[] cancelInto; // optional chain

        [Header("Hitbox")]
        public GameObject hitboxPrefab;  // prefab with Hitbox component + collider(isTrigger)
        public Vector3 hitboxLocalPos;
        public Vector3 hitboxLocalScale = Vector3.one;
        public float hitboxLifetimeFrames = 3f;

        [Header("FX/SFX")]
        public AudioClip sfx;
        public GameObject vfxPrefab;

        [Header("Movement During Animation (Frame-Range Based)")]
        public bool overrideRootMotion = true;
        public List<FrameMotion> motionSegments = new List<FrameMotion>();

        [HideInInspector] public float plusOnHit;
        [HideInInspector] public float plusOnBlock;

        public float TotalAnimTimeSec => animation ? animation.length : (startup + active + recovery) / 60f;
    }
}