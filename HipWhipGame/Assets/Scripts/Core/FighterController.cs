/*
File Name:    FighterController.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static HipWhipGame.Enums;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace HipWhipGame
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(FighterStateMachine))]
    [RequireComponent(typeof(InputBuffer))]
    public class FighterController : MonoBehaviour
    {
        public int playerIndex;

        public enum ControlType { Player, Dummy, AI }

        [Header("Control Settings")]
        public ControlType controlType = ControlType.Player;
        [Tooltip("If dummy, optionally face this target every frame.")]
        public Transform lookAtTarget;
        public Camera cam;
        public Pushbox pushbox;

        [Header("Core Data")]
        public FighterStats stats;
        public Animator animator;
        public MoveDatabase moves;

        CharacterController _cc;
        FighterStateMachine _fsm;
        InputBuffer _buffer;

        Vector2 movementInput;

        Vector3 _velocity;        // player-controlled movement + gravity
        Vector3 _externalForce;   // knockback, pushback, etc.
        bool _isGrounded;
        public bool isBlocking = false;

        void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _fsm = GetComponent<FighterStateMachine>();
            _buffer = GetComponent<InputBuffer>();
            if (!animator) animator = GetComponentInChildren<Animator>();
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
            _fsm.Tick(Time.deltaTime);

            // --- Ground check ---
            _isGrounded = _cc.isGrounded;



            if (_fsm.State == FighterState.BlockStun) 
            {
                _velocity = Vector3.zero;
                _externalForce = Vector3.zero;
                _cc.Move(Vector3.zero);
                return;
            }

            if (_fsm.State != FighterState.BlockStun) // don’t interrupt blockstun
            {
                if (isBlocking && _fsm.CanBlock())
                {
                    StartBlock();
                }
                else if (!isBlocking && _fsm.State == FighterState.Blocking)
                {
                    EndBlock();
                }
            }

            // Skip movement while blocking
            if (_fsm.State == FighterState.Blocking)
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
            if (_fsm.State == FighterState.Idle || _fsm.State == FighterState.Jump)
            {
                // Move relative to facing direction (look-at target)
                Vector3 input = new Vector3(h, 0, v).normalized;

                // Convert input to world direction based on facing
                Vector3 moveDir = (transform.forward * input.z) + (transform.right * input.x);
                Vector3 move = moveDir * stats.walkSpeed;

                _velocity.x = move.x;
                _velocity.z = move.z;
            }
            else
            {
                // Don’t overwrite existing knockback in hitstun or attack
                // Just let it decay naturally
                _velocity.x *= 0.9f;  // friction decay per frame
                _velocity.z *= 0.9f;
            }

            // --- Gravity ---
            if (_isGrounded && _velocity.y < 0) _velocity.y = -2f;
            _velocity.y -= stats.gravity * Time.deltaTime;

            // --- Combine internal + external forces ---
            Vector3 totalMove = _velocity + _externalForce;

            // --- Move character ---
            _cc.Move(totalMove * Time.deltaTime);

            // --- External force decay (smooth pushback slide) ---
            _externalForce = Vector3.Lerp(_externalForce, Vector3.zero, Time.deltaTime * 8f);

            // --- Animator blend ---
            Vector3 localVel = transform.InverseTransformDirection(new Vector3(_velocity.x, 0, _velocity.z));
            float maxSpeed = Mathf.Max(0.01f, stats.walkSpeed);
            animator.SetFloat("X", localVel.x / maxSpeed, 0.1f, Time.deltaTime);
            animator.SetFloat("Y", localVel.z / maxSpeed, 0.1f, Time.deltaTime);
            animator.SetBool("Move", h != 0 || v != 0);

            // --- Face movement direction (optional) ---
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
                Vector3 flatVel = new Vector3(_velocity.x + _externalForce.x, 0, _velocity.z + _externalForce.z);
                if (flatVel.sqrMagnitude > 0.001f)
                    transform.forward = flatVel.normalized;
            }

            // --- Attacks (data-driven) ---
            if (_fsm.State != FighterState.Hitstun) 
            {
                TryStartMove();

            }
            _buffer.Prune();


            // --- Idle fallback ---
            if (_fsm.State == FighterState.Idle && animator &&
                !animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") &&
                !animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            {
                animator.Play("Idle", 0, 0);
                animator.SetBool("Block", false);
            }
        }

        void StartBlock()
        {
            _fsm.SetState(FighterState.Blocking);
            _velocity = Vector3.zero;
            if (animator) 
            {
                animator.SetBool("Block", true);
            }
                
                
        }

        void EndBlock()
        {
            _fsm.SetState(FighterState.Idle);

            if (animator)
            {
                animator.SetBool("Block", false);
            }
        }

        void HandleBlockBehavior()
        {
            // Stay stationary
            _velocity = Vector3.zero;
            _externalForce = Vector3.zero;
            _cc.Move(Vector3.zero);

            // Optionally rotate toward target
            if (lookAtTarget)
            {
                Vector3 dir = lookAtTarget.position - transform.position;
                dir.y = 0f;
                if (dir.sqrMagnitude > 0.001f)
                    transform.forward = Vector3.Lerp(transform.forward, dir.normalized, 10f * Time.deltaTime);
            }
        }


        public void OnMove(Vector2 moveVector)
        {
            movementInput = moveVector;
        }

        public void HoldBlock() 
        {
            isBlocking = true;
        }

        public void ReleaseBlock()
        {
            isBlocking = false;
        }

        public void PerformPunchFast()
        {
            _buffer.Push("PunchFast");
        }

        public void PerformButtAttackHopKick() 
        {
            _buffer.Push("ButtAttackHopKick");
        }

        public void PerformButtAttackMidPoke()
        {
            _buffer.Push("ButtAttackMidPoke");
        }

        public void PerformButtLowAttack()
        {
            _buffer.Push("ButtLowAttack");
        }

        public void PerformButtTornado()
        {
            _buffer.Push("ButtTornado");
        }

        // 
        // DUMMY LOGIC
        // 
        void HandleDummyBehavior()
        {
            _fsm.Tick(Time.deltaTime);

            // Always stay idle unless hit
            if (_fsm.State == FighterState.Idle)
            {
                if (animator && !animator.GetCurrentAnimatorStateInfo(0).IsName("Idle")) 
                {
                    animator.Play("Idle", 0, 0f);
                    animator.SetBool("Block", false);
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
            if (_externalForce.sqrMagnitude > 0.0001f)
            {
                _cc.Move(_externalForce * Time.deltaTime);
                _externalForce = Vector3.Lerp(_externalForce, Vector3.zero, Time.deltaTime * 8f);
            }
        }

        // 
        // PLAYER ATTACK EXECUTION
        // 
        void TryStartMove()
        {
            if (!_fsm.CanStartMove()) return;

            if (_buffer.Consume("ButtAttackHopKick") && moves.buttAttackHopKick)
            {
                GetComponent<MoveExecutor>().PlayMove(moves.buttAttackHopKick);
                return;
            }

            if (_buffer.Consume("PunchFast") && moves.punchFast)
            {
                GetComponent<MoveExecutor>().PlayMove(moves.punchFast);
                return;
            }

            if (_buffer.Consume("ButtAttackMidPoke") && moves.buttAttackMidPoke) 
            {
                GetComponent<MoveExecutor>().PlayMove(moves.buttAttackMidPoke);
                return;
            }

            if (_buffer.Consume("ButtLowAttack") && moves.buttLowAttack)
            {
                GetComponent<MoveExecutor>().PlayMove(moves.buttLowAttack);
                return;
            }

            if (_buffer.Consume("ButtTornado") && moves.buttTornado)
            {
                GetComponent<MoveExecutor>().PlayMove(moves.buttTornado);
                return;
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
            

            if (_fsm.State == FighterState.Blocking)
            {
                int advantage = move.blockstunFrames - (move.recovery + remainingActive);

                Debug.Log(
                    $"[{name}] {move.moveName} HIT INFO\n" +
                    $"  Hit Frame: {hitFrame}\n" +
                    $"  Startup: {move.startup}\n" +
                    $"  Active: {move.active}\n" +
                    $"  Recovery: {move.recovery}\n" +
                    $"  Blockstun: {move.blockstunFrames}\n" +
                    $"  Frame Into Active: {frameIntoActive}\n" +
                    $"  Remaining Active: {remainingActive}\n" +
                    $"  Advantage on Block: {(advantage >= 0 ? "+" : "")}{advantage}\n" +
                    $"  Effective Duration (Recovery + Remaining Active): {move.recovery + remainingActive}"
                );
                ApplyBlockstun(move.blockstunFrames);
            }
            else
            {
                int advantage = move.hitstunFrames - (move.recovery + remainingActive);
                Debug.Log($"{move.moveName}:  is {(advantage >= 0 ? "+" : "")}{advantage} on hit.");
                animator.SetBool("Block", false);
                ApplyHitstun(move.hitstunFrames);
            }
        }

        public void ApplyBlockstun(float frames)
        {
            _fsm.EnterBlockstun(frames / 60f * stats.blockstunScale);
            _fsm.SetState(FighterState.BlockStun);
            animator.SetBool("BlockStun", true);
            if (animator) animator.Play("BlockStun", 0, 0f);
        }

        public void ApplyHitstun(float frames)
        {
            _fsm.EnterHitstun(frames / 60f * stats.hitstunScale);
            OnHitReaction();
        }

        public void ApplyKnockback(Vector3 worldKnock, float scale = 1f)
        {
           // Debug.Log($"{name} knocked back with {worldKnock}!");
            _externalForce += worldKnock * (scale / Mathf.Max(0.01f, stats.weight));
        }

        public void TakeDamage(float dmg)
        {
            Debug.Log($"{name} took {dmg} damage!");
        }

        public void OnHitReaction()
        {
            _fsm.SetState(FighterState.Hitstun);
            if (animator)
                animator.Play("HitStun", 0, 0f);
        }
    }
}
