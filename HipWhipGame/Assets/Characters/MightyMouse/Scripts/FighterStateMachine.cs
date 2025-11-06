/*
File Name:    FighterStateMachine.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
using static HipWhipGame.Enums;

//TODO: Refactor state machine to use State Pattern later
namespace HipWhipGame
{
    [RequireComponent(typeof(FighterComponentManager))]
    public class FighterStateMachine : MonoBehaviour, IFighterComponentInjectable
    {
        private FighterComponentManager fighterComponentManager;
        private float stateTimer;
        public FighterState State { get; private set; } = FighterState.Idle;

        public void Inject(FighterComponentManager fighterComponentManager)
        {
            this.fighterComponentManager = fighterComponentManager;
        }

        public void SetState(FighterState newState, float lockTime = 0f)
        {
            State = newState;
            stateTimer = lockTime;
        }

        public void Tick(float dt)
        {
            switch (State)
            {
                case FighterState.Hitstun:
                    stateTimer -= dt;
                    if (stateTimer <= 0f) 
                    {
                        EndHitStun();
                    }
                    break;

                case FighterState.BlockStun:
                    stateTimer -= dt;

                    if (stateTimer <= 0f)
                    {
                        EndBlockStun();
                    }
                    break;
                    
                case FighterState.Attacking:
                    if (stateTimer > 0f)
                    {
                        stateTimer -= dt;
                        if (stateTimer <= 0f)
                        {
                            State = FighterState.Idle;
                        }
                    }
                    break;
            }
        }

        public void EnterHitstun(float duration)
        {
            SetState(FighterState.Hitstun, duration);
            if (fighterComponentManager.Animator is Animator animator) 
            {
                animator.SetBool("Block", false);
                animator.Play("HitStun", 0, 0f);
            }
        }

        private void EndHitStun() 
        {
            SetState(FighterState.Idle);
        }

        public void EnterBlockstun(float duration)
        {
            SetState(FighterState.BlockStun, duration);
            if (fighterComponentManager.Animator is Animator animator) 
            {
                animator.SetBool("BlockStun", true);
                animator.Play("BlockStun", 0, 0f);
            }
        }

        private void EndBlockStun() 
        {
            fighterComponentManager.Animator.SetBool("BlockStun", false);
            // choose next state based on input
            var controller = fighterComponentManager.FighterController;
            if (controller != null && controller.isBlocking)
            {
                SetState(FighterState.Blocking);
            }
            else
            {
                SetState(FighterState.Idle);
            }
        }

        public bool CanBlock()
        {
            return State == FighterState.Idle || State == FighterState.Moving;
        }

        public bool CanStartMove() 
        {
            return State == FighterState.Idle;
        }
    }
}