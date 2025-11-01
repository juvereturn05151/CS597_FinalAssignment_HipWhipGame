/*
File Name:    FighterController.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
using static HipWhipGame.Enums;

namespace HipWhipGame {
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(FighterStateMachine))]
    [RequireComponent(typeof(InputBuffer))]
    public class FighterController : MonoBehaviour
    {
        //public Team team = Team.TeamA;
        public FighterStats stats;
        public Animator animator;
        public MoveDatabase moves;

        CharacterController _cc;
        FighterStateMachine _fsm;
        InputBuffer _buffer;

        Vector3 _velocity;
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
            // --- Input (replace with Input System later) ---
            //if (Input.GetKeyDown(KeyCode.J)) _buffer.Push("Light");
            //if (Input.GetKeyDown(KeyCode.K)) _buffer.Push("Heavy");
            //if (Input.GetKeyDown(KeyCode.L)) _buffer.Push("Special");
            if (Input.GetKeyDown(KeyCode.Space)) 
            {
                
                _buffer.Push("ButtAttack");
            } 

            // --- State tick ---
            _fsm.Tick(Time.deltaTime);

            // --- Movement (very simple) ---
            _isGrounded = _cc.isGrounded;
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            if (_fsm.State == FighterState.Idle || _fsm.State == FighterState.Jump)
            {
                Vector3 input = new Vector3(h, 0, v).normalized;
                Vector3 move = input * stats.walkSpeed;
                _velocity.x = move.x;
                _velocity.z = move.z;

                // jump
                if (_buffer.Consume("Jump") && _isGrounded)
                {
                    _velocity.y = stats.jumpForce;
                    _fsm.SetState(FighterState.Jump);
                }
            }
            else
            {
                // lock movement while attacking/hitstun (customize)
                _velocity.x = 0f;
                _velocity.z = 0f;
            }

            // gravity
            if (_isGrounded && _velocity.y < 0) _velocity.y = -2f;
            _velocity.y -= stats.gravity * Time.deltaTime;
            _cc.Move(_velocity * Time.deltaTime);

            // Face movement direction (optional)
            Vector3 flatVel = new Vector3(_velocity.x, 0, _velocity.z);
            if (flatVel.sqrMagnitude > 0.001f) transform.forward = flatVel.normalized;

            // --- Attacks (data-driven) ---
            TryStartMove();
            _buffer.Prune();

            // Idle animation fallback
            if (_fsm.State == FighterState.Idle && animator && animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") == false)
            {
                animator.Play("Idle", 0, 0);
            }
        }

        void TryStartMove()
        {
            if (!_fsm.CanStartMove()) return;

            //if (_buffer.Consume("Light") && moves.light)
            //{
            //    GetComponent<MoveExecutor>().PlayMove(moves.light);
            //    return;
            //}
            //if (_buffer.Consume("Heavy") && moves.heavy)
            //{
            //    GetComponent<MoveExecutor>().PlayMove(moves.heavy);
            //    return;
            //}
            if (_buffer.Consume("ButtAttack") && moves.buttAttack)
            {
                //Debug.Log("ButtAttack input buffered");
                GetComponent<MoveExecutor>().PlayMove(moves.buttAttack);
                return;
            }
        }

        // Called by HitResolver when we are hit
        public void ApplyHitstun(float frames)
        {
            _fsm.EnterHitstun(frames / 60f * stats.hitstunScale);
        }

        public void ApplyKnockback(Vector3 worldKnock, float scale = 1f)
        {
            _velocity += worldKnock * (scale / Mathf.Max(0.01f, stats.weight));
        }
    }
}
