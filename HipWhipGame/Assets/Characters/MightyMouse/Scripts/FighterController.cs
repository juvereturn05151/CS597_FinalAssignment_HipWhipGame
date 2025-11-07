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
        public MoveDatabase moves;

        private Vector2 movementInput;
        private Vector3 velocity;        // player-controlled movement + gravity
        private Vector3 externalForce;   // knockback, pushback, etc.
        private bool isGrounded;
        public bool isBlocking = false;

        public void SetIsBlocking(bool isBlocking) 
        {
            this.isBlocking = isBlocking;
        }

        public void Inject(FighterComponentManager fighterComponentManager)
        {
            this.fighterComponentManager = fighterComponentManager;
        }

        void Update()
        {
            // --- Skip input entirely for dummy ---
            if (controlType == ControlType.Dummy)
            {
                HandleDummyBehavior();
                return;
            }

            // --- State tick ---
            fighterComponentManager.FighterStateMachine.Tick(Time.deltaTime);

            // --- Ground check ---
            isGrounded = fighterComponentManager.CharacterController.isGrounded;

            if (fighterComponentManager.FighterStateMachine.State == FighterState.BlockStun) 
            {
                velocity = Vector3.zero;
                externalForce = Vector3.zero;
                fighterComponentManager.CharacterController.Move(Vector3.zero);
                return;
            }

            if (fighterComponentManager.FighterStateMachine.State != FighterState.BlockStun) // don’t interrupt blockstun
            {
                if (isBlocking && fighterComponentManager.FighterStateMachine.CanBlock())
                {
                    StartBlock();
                }
                else if (!isBlocking && fighterComponentManager.FighterStateMachine.State == FighterState.Blocking)
                {
                    EndBlock();
                }
            }

            // Skip movement while blocking
            if (fighterComponentManager.FighterStateMachine.State == FighterState.Blocking)
            {
                HandleBlockBehavior();
                return;
            }

            // --- Apply player input if controllable ---
            float h = 0f, v = 0f;
            if (controlType == ControlType.Player)
            {
                h = movementInput.x;
                v = movementInput.y;
            }

            // --- Base movement ---
            if (fighterComponentManager.FighterStateMachine.State == FighterState.Idle || fighterComponentManager.FighterStateMachine.State == FighterState.Jump)
            {
                // Move relative to facing direction (look-at target)
                Vector3 input = new Vector3(h, 0, v).normalized;

                // Convert input to world direction based on facing
                Vector3 moveDir = (transform.forward * input.z) + (transform.right * input.x);
                Vector3 move = moveDir * stats.walkSpeed;

                velocity.x = move.x;
                velocity.z = move.z;
            }
            else
            {
                // Don’t overwrite existing knockback in hitstun or attack
                // Just let it decay naturally
                velocity.x *= 0.9f;  // friction decay per frame
                velocity.z *= 0.9f;
            }

            // --- Gravity ---
            if (isGrounded && velocity.y < 0) velocity.y = -2f;
            velocity.y -= stats.gravity * Time.deltaTime;

            // --- Combine internal + external forces ---
            Vector3 totalMove = velocity + externalForce;

            // --- Move character ---
            fighterComponentManager.CharacterController.Move(totalMove * Time.deltaTime);

            // --- External force decay (smooth pushback slide) ---
            externalForce = Vector3.Lerp(externalForce, Vector3.zero, Time.deltaTime * 8f);

            // --- Animator blend ---
            Vector3 localVel = transform.InverseTransformDirection(new Vector3(velocity.x, 0, velocity.z));
            float maxSpeed = Mathf.Max(0.01f, stats.walkSpeed);
            fighterComponentManager.Animator.SetFloat("X", localVel.x / maxSpeed, 0.1f, Time.deltaTime);
            fighterComponentManager.Animator.SetFloat("Y", localVel.z / maxSpeed, 0.1f, Time.deltaTime);
            fighterComponentManager.Animator.SetBool("Move", h != 0 || v != 0);

            // --- Face movement direction
            if (lookAtTarget)
            {
                Vector3 dir = lookAtTarget.position - transform.position;
                dir.y = 0f; // keep rotation flat
                if (dir.sqrMagnitude > 0.001f)
                {
                    Quaternion targetRot = Quaternion.LookRotation(dir);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
                }
            }
            else
            {
                // fallback to velocity facing if no target assigned
                Vector3 flatVel = new Vector3(velocity.x + externalForce.x, 0, velocity.z + externalForce.z);
                if (flatVel.sqrMagnitude > 0.001f)
                    transform.forward = flatVel.normalized;
            }

            // --- Attacks (data-driven) ---
            if (fighterComponentManager.FighterStateMachine.State != FighterState.Hitstun) 
            {
                fighterComponentManager.FighterInputHandler.TryStartMove();
            }
            fighterComponentManager.InputBuffer.Prune();


            // --- Idle fallback ---
            if (fighterComponentManager.FighterStateMachine.State == FighterState.Idle && fighterComponentManager.Animator &&
                !fighterComponentManager.Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") &&
                !fighterComponentManager.Animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            {
                fighterComponentManager.Animator.Play("Idle", 0, 0);
                fighterComponentManager.Animator.SetBool("Block", false);
            }
        }

        void StartBlock()
        {
            fighterComponentManager.FighterStateMachine.SetState(FighterState.Blocking);
            velocity = Vector3.zero;
            if (fighterComponentManager.Animator) 
            {
                fighterComponentManager.Animator.SetBool("Block", true);
            }
        }

        void EndBlock()
        {
            fighterComponentManager.FighterStateMachine.SetState(FighterState.Idle);

            if (fighterComponentManager.Animator)
            {
                fighterComponentManager.Animator.SetBool("Block", false);
            }
        }

        void HandleBlockBehavior()
        {
            // Stay stationary
            velocity = Vector3.zero;
            externalForce = Vector3.zero;
            fighterComponentManager.CharacterController.Move(Vector3.zero);

            // Optionally rotate toward target
            if (lookAtTarget)
            {
                Vector3 dir = lookAtTarget.position - transform.position;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.001f) 
                {
                    transform.forward = Vector3.Lerp(transform.forward, dir.normalized, 10f * Time.deltaTime);
                }
            }
        }

        #region Command Bindings

        public void UpdateMovementInput(Vector2 moveVector) 
        {
            movementInput = moveVector;
        }

        #endregion

        // 
        // DUMMY LOGIC
        // 
        void HandleDummyBehavior()
        {
            fighterComponentManager.FighterStateMachine.Tick(Time.deltaTime);

            // Always stay idle unless hit
            if (fighterComponentManager.FighterStateMachine.State == FighterState.Idle)
            {
                if (fighterComponentManager.Animator && !fighterComponentManager.Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) 
                {
                    fighterComponentManager.Animator.Play("Idle", 0, 0f);
                    fighterComponentManager.Animator.SetBool("Block", false);
                }
            }

            // Optionally face the player
            if (lookAtTarget)
            {
                Vector3 dir = lookAtTarget.position - transform.position;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.001f)
                    transform.forward = Vector3.Lerp(transform.forward, dir.normalized, 10f * Time.deltaTime);
            }

            // Apply external knockback even for dummy
            if (externalForce.sqrMagnitude > 0.0001f)
            {
                fighterComponentManager.CharacterController.Move(externalForce * Time.deltaTime);
                externalForce = Vector3.Lerp(externalForce, Vector3.zero, Time.deltaTime * 8f);
            }
        }

        // 
        // HIT REACTIONS
        // 

        public void OnHit(MoveData move, int currentFrame)
        {
            int hitFrame = currentFrame; // the frame the attack landed
            int frameIntoActive = hitFrame - move.startup;
            int remainingActive = Mathf.Max(0, move.active - frameIntoActive - 1);
            

            if (fighterComponentManager.FighterStateMachine.State == FighterState.Blocking)
            {
                int advantage = move.blockstunFrames - (move.recovery + remainingActive);
                ApplyBlockstun(move.blockstunFrames);
            }
            else
            {
                int advantage = move.hitstunFrames - (move.recovery + remainingActive);
                Debug.Log($"{move.moveName}:  is {(advantage >= 0 ? "+" : "")}{advantage} on hit.");
                ApplyHitstun(move.hitstunFrames);
            }
        }

        public void ApplyBlockstun(float frames)
        {
            fighterComponentManager.FighterStateMachine.EnterBlockstun(frames / 60f * stats.blockstunScale);
        }

        public void ApplyHitstun(float frames)
        {
            fighterComponentManager.FighterStateMachine.EnterHitstun(frames / 60f * stats.hitstunScale);
        }

        public void ApplyKnockback(Vector3 worldKnock, float scale = 1f)
        {
           // Debug.Log($"{name} knocked back with {worldKnock}!");
            externalForce += worldKnock * (scale / Mathf.Max(0.01f, stats.weight));
        }

        public void TakeDamage(float dmg)
        {
            Debug.Log($"{name} took {dmg} damage!");
        }
    }
}
