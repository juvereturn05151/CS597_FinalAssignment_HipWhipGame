/*
File Name:    FighterIdleState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
using UnityEngine.EventSystems;
using static HipWhipGame.Enums;

namespace RollbackSupport
{
    public class FighterWalkState : FighterBaseState
    {
        private Vector3 localVel;
        private float walkAnimTimer;
        public FighterWalkState(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

        public override void OnEnter(int duration = 0)
        {

        }

        public override void OnUpdate()
        {
            ProcessMovement();
        }

        public override void OnExit() 
        {
        
        }

        public override void OnUpdateAnimation()
        {
            UpdateMovementBlend();
        }

        private void ProcessMovement()
        {
            Vector3 forward = fighterComponentManager.transform.forward;
            Vector3 right = fighterComponentManager.transform.right;

            if (fighterComponentManager.FighterController.lookAtTarget)
            {
                Vector3 dir = fighterComponentManager.FighterController.lookAtTarget.position - fighterComponentManager.FighterController.transform.position;
                dir.y = 0;
                if (dir.sqrMagnitude > 0.0001f)
                {
                    forward = dir.normalized;
                }

                right = Quaternion.Euler(0, 90f, 0) * forward;
            }

            Vector3 input = new Vector3(fighterComponentManager.FighterController.LastInput.horiz, 0f, fighterComponentManager.FighterController.LastInput.vert);
            if (input.sqrMagnitude > 1f)
            {
                input.Normalize();
            }

            Vector3 moveDir = (forward * input.z + right * input.x).normalized;

            // Apply horizontal movement (fixed per frame, deterministic)
            const float movePerFrame = 0.08f;
            fighterComponentManager.FighterController.body.position += moveDir * movePerFrame;

            // Apply visual transform from rollback body
            fighterComponentManager.transform.position = fighterComponentManager.FighterController.body.position;
        }

        private void UpdateMovementBlend()
        {
            Vector3 delta = fighterComponentManager.FighterController.body.position - fighterComponentManager.DeterministicAnimator.LastFramePos;
            localVel = fighterComponentManager.FighterController.transform.InverseTransformDirection(new Vector3(delta.x, 0f, delta.z));

            const float movePerFrame = 0.08f;
            float maxSpeed = Mathf.Max(0.0001f, movePerFrame);

            float x = Mathf.Clamp(localVel.x / maxSpeed, -1f, 1f);
            float y = Mathf.Clamp(localVel.z / maxSpeed, -1f, 1f);
            bool moving = (Mathf.Abs(x) + Mathf.Abs(y)) > 0.01f;

            fighterComponentManager.Animator.SetFloat("X", x);
            fighterComponentManager.Animator.SetFloat("Y", y);
            fighterComponentManager.Animator.SetBool("Move", moving);

            if (moving)
            {
                walkAnimTimer += 1f / 60f;
                float walkCycleLength = 1f;
                float normTime = (walkAnimTimer % walkCycleLength) / walkCycleLength;
                fighterComponentManager.Animator.Play("Walk", 0, normTime);
            }
            else
            {
                walkAnimTimer = 0f;
                fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Idle);
            }
        }
    }
}
