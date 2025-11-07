/*
File Name:    FighterController.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
using static HipWhipGame.Enums;

namespace HipWhipGame
{
    [RequireComponent(typeof(FighterComponentManager))]
    public class FighterController : MonoBehaviour, IFighterComponentInjectable
    {
        private FighterComponentManager fighterComponentManager;

        public int playerIndex;
        public enum ControlType { Player, Dummy, AI }

        [Header("Control Settings")]
        public ControlType controlType = ControlType.Player;
        [Tooltip("If dummy, optionally face this target every frame.")]
        public Transform lookAtTarget;
        public Pushbox pushbox;

        [Header("Core Data")]
        public FighterStats stats;

        private Vector2 movementInput;
        private Vector3 velocity;        // player-controlled movement + gravity
        private Vector3 externalForce;   // knockback, pushback, etc.
        private bool isGrounded;
        private bool isBlocking = false;

        public bool IsBlocking => isBlocking;
        public void SetIsBlocking(bool value) => isBlocking = value;

        public void Inject(FighterComponentManager fighterComponentManager)
        {
            this.fighterComponentManager = fighterComponentManager;
        }

        void Update()
        {
            // Skip entirely for dummy AI
            if (controlType == ControlType.Dummy)
            {
                HandleDummyBehavior();
                return;
            }

            // FSM update
            fighterComponentManager.FighterStateMachine?.CustomUpdate(Time.deltaTime);

            // Ground check
            isGrounded = fighterComponentManager.CharacterController.isGrounded;

            // Handle movement (player only)
            if (controlType == ControlType.Player)
            {
                HandleMovement();
            }

            // Attack logic
            if (fighterComponentManager.FighterStateMachine.CurrentStateType != FighterState.Hitstun)
            {
                fighterComponentManager.FighterInputHandler.TryStartMove();
            }

            // Input buffer cleanup
            fighterComponentManager.InputBuffer.Prune();

            // Facing and idle
            UpdateFacing();
            EnsureIdleAnimation();
        }

        // ============================================================
        // MOVEMENT
        // ============================================================

        private void HandleMovement()
        {
            FighterStateMachine fsm = fighterComponentManager.FighterStateMachine;

            // Stop movement only for block states, not for hitstun
            if (fsm.CurrentStateType == FighterState.Blocking ||
                fsm.CurrentStateType == FighterState.BlockStun)
            {
                ApplyStationaryBehavior();
                return;
            }

            // Directional input
            float h = movementInput.x;
            float v = movementInput.y;

            Vector3 input = new Vector3(h, 0, v).normalized;
            Vector3 moveDir = (transform.forward * input.z) + (transform.right * input.x);
            Vector3 move = moveDir * stats.walkSpeed;

            velocity.x = move.x;
            velocity.z = move.z;

            // Gravity
            if (isGrounded && velocity.y < 0) velocity.y = -2f;
            velocity.y -= stats.gravity * Time.deltaTime;

            // Combine movement and forces
            Vector3 totalMove = velocity + externalForce;
            fighterComponentManager.CharacterController.Move(totalMove * Time.deltaTime);

            // Smoothly decay external forces
            externalForce = Vector3.Lerp(externalForce, Vector3.zero, Time.deltaTime * 8f);

            // Animation blend
            UpdateMovementAnimation();
        }

        private void ApplyStationaryBehavior()
        {
            velocity = Vector3.zero;
            externalForce = Vector3.zero;
            fighterComponentManager.CharacterController.Move(Vector3.zero);
            // Do not clear externalForce here (knockback uses it)
        }

        private void UpdateMovementAnimation()
        {
            if (fighterComponentManager.Animator == null) return;

            Vector3 localVel = transform.InverseTransformDirection(new Vector3(velocity.x, 0, velocity.z));
            float maxSpeed = Mathf.Max(0.01f, stats.walkSpeed);
            fighterComponentManager.Animator.SetFloat("X", localVel.x / maxSpeed, 0.1f, Time.deltaTime);
            fighterComponentManager.Animator.SetFloat("Y", localVel.z / maxSpeed, 0.1f, Time.deltaTime);
            fighterComponentManager.Animator.SetBool("Move", movementInput.sqrMagnitude > 0.01f);
        }

        // ============================================================
        // FACING DIRECTION
        // ============================================================

        private void UpdateFacing()
        {
            if (lookAtTarget)
            {
                Vector3 dir = lookAtTarget.position - transform.position;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
                }
            }
            else
            {
                Vector3 flatVel = new Vector3(velocity.x + externalForce.x, 0, velocity.z + externalForce.z);
                if (flatVel.sqrMagnitude > 0.001f)
                    transform.forward = flatVel.normalized;
            }
        }

        // ============================================================
        // IDLE MANAGEMENT
        // ============================================================

        private void EnsureIdleAnimation()
        {
            var animator = fighterComponentManager.Animator;
            if (!animator) return;

            if (fighterComponentManager.FighterStateMachine.CurrentStateType == FighterState.Idle &&
                !animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") &&
                !animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            {
                animator.Play("Idle", 0, 0);
                animator.SetBool("Block", false);
            }
        }

        // ============================================================
        // INPUT FROM PLAYER
        // ============================================================

        public void UpdateMovementInput(Vector2 moveVector)
        {
            movementInput = moveVector;
        }

        // ============================================================
        // DUMMY LOGIC
        // ============================================================

        private void HandleDummyBehavior()
        {
            fighterComponentManager.FighterStateMachine?.CustomUpdate(Time.deltaTime);

            var fsm = fighterComponentManager.FighterStateMachine;
            if (fsm.CurrentStateType == FighterState.Idle)
            {
                var animator = fighterComponentManager.Animator;
                if (animator && !animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                {
                    animator.Play("Idle", 0, 0);
                    animator.SetBool("Block", false);
                }
            }

            if (lookAtTarget)
            {
                Vector3 dir = lookAtTarget.position - transform.position;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.001f)
                    transform.forward = Vector3.Lerp(transform.forward, dir.normalized, 10f * Time.deltaTime);
            }

            if (externalForce.sqrMagnitude > 0.0001f)
            {
                fighterComponentManager.CharacterController.Move(externalForce * Time.deltaTime);
                externalForce = Vector3.Lerp(externalForce, Vector3.zero, Time.deltaTime * 8f);
            }
        }

        // ============================================================
        // HIT REACTIONS
        // ============================================================

        public void OnHit(MoveData move, int currentFrame)
        {
            int hitFrame = currentFrame;
            int frameIntoActive = hitFrame - move.startup;
            int remainingActive = Mathf.Max(0, move.active - frameIntoActive - 1);

            bool blocked = (fighterComponentManager.FighterStateMachine.CurrentStateType == FighterState.Blocking);

            if (blocked)
            {
                ApplyBlockstun(move.blockstunFrames);

                // Debug: Frame advantage on block
                int frameAdvantage = Mathf.RoundToInt(move.recovery - move.blockstunFrames);
                Debug.Log($"[FrameData] {name} (attacker) vs {fighterComponentManager.FighterController.name} (defender): " +
                          $"{move.moveName} is {(frameAdvantage >= 0 ? "+" : "")}{-frameAdvantage} on block " +
                          $"(Attacker recovery: {move.recovery}f, Defender blockstun: {move.blockstunFrames}f)");
            }
            else
            {
                ApplyHitstun(move.hitstunFrames);

                // Debug: Frame advantage on hit
                int frameAdvantage = Mathf.RoundToInt(move.recovery - move.hitstunFrames);
                Debug.Log($"[FrameData] {name} (attacker) vs {fighterComponentManager.FighterController.name} (defender): " +
                          $"{move.moveName} is {(frameAdvantage >= 0 ? "+" : "")}{-frameAdvantage} on hit " +
                          $"(Attacker recovery: {move.recovery}f, Defender hitstun: {move.hitstunFrames}f)");
            }
        }

        public void ApplyBlockstun(float frames)
        {
            fighterComponentManager.FighterStateMachine.SwitchState(
                FighterState.BlockStun,
                frames / 60f * stats.blockstunScale
            );
        }

        public void ApplyHitstun(float frames)
        {
            fighterComponentManager.FighterStateMachine.SwitchState(
                FighterState.Hitstun,
                frames / 60f * stats.hitstunScale
            );
        }

        public void ApplyKnockback(Vector3 worldKnock, float scale = 1f)
        {
            Vector3 appliedKnockback = worldKnock * (scale / Mathf.Max(0.01f, stats.weight));
            externalForce += appliedKnockback;
            
        }

        public void TakeDamage(float dmg)
        {
            Debug.Log(name + " took " + dmg + " damage!");
        }
    }
}
