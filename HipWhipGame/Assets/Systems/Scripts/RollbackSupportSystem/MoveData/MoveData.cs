using System.Collections.Generic;
using UnityEngine;

namespace RollbackSupport
{
    [CreateAssetMenu(menuName = "Fighter/MoveData")]
    public class MoveData : ScriptableObject
    {
        [Header("Basic Info")]
        public string moveName;
        public string animName;

        [Header("Frame Data")]
        [Tooltip("Number of startup frames before the hitbox becomes active.")]
        public int startup = 5;

        [Tooltip("Number of active frames where the attack can hit.")]
        public int active = 3;

        [Tooltip("Number of recovery frames after the attack ends.")]
        public int recovery = 12;

        [Tooltip("Total number of frames for this move (auto-calculated if 0).")]
        public int totalFrames => startup + active + recovery;

        [Header("Hit Properties")]
        [Tooltip("Number of frames the opponent is stunned on hit.")]
        public int hitstunFrames = 15;

        [Tooltip("Number of frames the opponent is stunned when blocking.")]
        public int blockstunFrames = 10;

        public bool isKnockbackAttack = false;
        [Tooltip("Knockback direction and magnitude applied on hit.")]
        public Vector3 knockback = new Vector3(2f, 0.5f, 0f);

        public bool isTrackingSidestep = true;

        public float damage = 1.0f;

        [Header("Cancel Rules")]
        [Tooltip("Can this move be canceled into another move on hit?")]
        public bool canCancelOnHit = false;

        [Tooltip("Can this move be canceled into another move on block?")]
        public bool canCancelOnBlock = false;

        public List<MoveCancelRule> cancelRules = new List<MoveCancelRule>();

        [Header("Hitboxes")]
        [Tooltip("Per-frame hitboxes active during this move.")]
        public FrameHitbox[] hitboxes;

        [Header("FX / SFX")]
        [Tooltip("Sound effect played when the move starts.")]
        public AudioClip sfx;

        [Tooltip("Visual effect spawned when the move hits or activates.")]
        public GameObject vfxPrefab;
        public string vfxName;

        [Header("Movement During Animation (Frame-Range Based)")]
        [Tooltip("If true, disables Unity's root motion and applies manual motion instead.")]
        public bool overrideRootMotion = true;

        [Tooltip("Per-frame movement segments (used to simulate dashes or lunges).")]
        public List<FrameMotion> motionSegments = new List<FrameMotion>();

        [HideInInspector] public float plusOnHit;
        [HideInInspector] public float plusOnBlock;

        [Header("Grab Settings")]
        [Tooltip("Is this move a grab instead of a hit?")]
        public bool isGrab;

        [Tooltip("Effective range of the grab (distance check).")]
        public float grabRange = 1.2f;

        [Tooltip("Duration (seconds) the grab holds the opponent.")]
        public int grabDuration = 5;

        [Header("Ultimate Settings")]
        public bool isUltimate;
        public bool animateDuringFreeze = false;
        public int preSuperFreezeFrames = 20;

        [Header("Meter Gain Settings")]
        public float meterConsumption = 0f;
        public float meterGainOnHit = 0.2f;
        public float meterGainOnBlock = 0.1f;
        public float meterGainWhenBlocked = 0.3f;

        private void OnValidate()
        {
            // Auto-fill totalFrames if not manually specified

            // Auto-compute frame advantage when recoveries are defined
            plusOnHit = hitstunFrames - recovery;
            plusOnBlock = blockstunFrames - recovery;
        }

        public bool CanCancelInto(bool isOpponentBlocking, MoveData targetMove, int frame, InputFrame input)
        {
            if (!canCancelOnHit)
                return false;

            if (isOpponentBlocking && !canCancelOnBlock)
            {
                return false;
            }

            if (frame < startup || frame > recovery)
                return false;

            foreach (var rule in cancelRules)
            {
                if (rule.nextMove != targetMove)
                    continue;

                switch (rule.requiredInput)
                {
                    case CancelInputType.Light:
                        if (input.light) return true;
                        break;

                    case CancelInputType.Heavy:
                        if (input.heavy) return true;
                        break;

                    case CancelInputType.SpecialPlusLight:
                        if (input.special && input.light) return true;
                        break;

                    case CancelInputType.SpecialPlusHeavy:
                        if (input.special && input.heavy) return true;
                        break;
                }
            }

            return false;
        }


    }

    [System.Serializable]
    public struct FrameHitbox
    {
        public int start;
        public int end;
        public CollisionBox box;
    }

    [System.Serializable]
    public class FrameMotion
    {
        [Tooltip("Start frame (inclusive)")]
        public int frameStart = 0;

        [Tooltip("End frame (inclusive)")]
        public int frameEnd = 0;

        [Tooltip("Forward speed (units per frame)")]
        public float forwardSpeed = 0f;

        [Tooltip("Vertical speed (units per frame)")]
        public float verticalSpeed = 0f;

        public float sideSpeed;
    }

    public enum CancelInputType { None, Light, Heavy, Grab, SpecialPlusLight, SpecialPlusHeavy, SpecialPlusLightPlusHeavy }

    [System.Serializable]
    public class MoveCancelRule
    {
        public MoveData nextMove;
        public CancelInputType requiredInput;
    }
}
