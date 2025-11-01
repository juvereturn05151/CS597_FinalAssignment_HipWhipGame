/*
File Name:    FighterController.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
using static HipWhipGame.Enums;

namespace HipWhipGame
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(FighterStateMachine))]
    [RequireComponent(typeof(InputBuffer))]
    public class FighterController : MonoBehaviour
    {
        public enum ControlType { Player, Dummy, AI }

        [Header("Control Settings")]
        public ControlType controlType = ControlType.Player;
        [Tooltip("If dummy, optionally face this target every frame.")]
        public Transform lookAtTarget;

        [Header("Core Data")]
        public FighterStats stats;
        public Animator animator;
        public MoveDatabase moves;

        CharacterController _cc;
        FighterStateMachine _fsm;
        InputBuffer _buffer;

        Vector3 _velocity;        // player-controlled movement + gravity
        Vector3 _externalForce;   // knockback, pushback, etc.
        bool _isGrounded;

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

            // --- Input (temporary / legacy system) ---
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _buffer.Push("ButtAttack");
            }

            // --- State tick ---
            _fsm.Tick(Time.deltaTime);

            // --- Ground check ---
            _isGrounded = _cc.isGrounded;

            // --- Apply player input if controllable ---
            float h = 0f, v = 0f;
            if (controlType == ControlType.Player)
            {
                h = Input.GetAxisRaw("Horizontal");
                v = Input.GetAxisRaw("Vertical");
            }

            // --- Base movement ---
            if (_fsm.State == FighterState.Idle || _fsm.State == FighterState.Jump)
            {
                Vector3 input = new Vector3(h, 0, v).normalized;
                Vector3 move = input * stats.walkSpeed;
                _velocity.x = move.x;
                _velocity.z = move.z;

                // Jump
                if (_buffer.Consume("Jump") && _isGrounded)
                {
                    _velocity.y = stats.jumpForce;
                    _fsm.SetState(FighterState.Jump);
                }
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
            Vector3 flatVel = new Vector3(_velocity.x + _externalForce.x, 0, _velocity.z + _externalForce.z);
            if (flatVel.sqrMagnitude > 0.001f)
                transform.forward = flatVel.normalized;

            // --- Attacks (data-driven) ---
            TryStartMove();
            _buffer.Prune();

            // --- Idle fallback ---
            if (_fsm.State == FighterState.Idle && animator &&
                !animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") &&
                !animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
            {
                animator.Play("Idle", 0, 0);
            }
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
                    animator.Play("Idle", 0, 0f);
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

            if (_buffer.Consume("ButtAttack") && moves.buttAttack)
            {
                GetComponent<MoveExecutor>().PlayMove(moves.buttAttack);
                return;
            }
        }

        // 
        // HIT REACTIONS
        // 
        public void ApplyHitstun(float frames)
        {
            _fsm.EnterHitstun(frames / 60f * stats.hitstunScale);
        }

        public void ApplyKnockback(Vector3 worldKnock, float scale = 1f)
        {
            Debug.Log($"{name} knocked back with {worldKnock}!");
            _externalForce += worldKnock * (scale / Mathf.Max(0.01f, stats.weight));
        }

        public void TakeDamage(float dmg)
        {
            Debug.Log($"{name} took {dmg} damage!");
        }

        public void OnHitReaction(MoveData move)
        {
            _fsm.SetState(FighterState.Hitstun);
            if (animator)
                animator.Play("HitReact", 0, 0f);
        }
    }
}
