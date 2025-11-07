/*
File Name:    FighterStateMachine.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
using System.Collections.Generic;
using static HipWhipGame.Enums;

namespace HipWhipGame
{
    [RequireComponent(typeof(FighterComponentManager))]
    public class FighterStateMachine : MonoBehaviour, IFighterComponentInjectable
    {
        private FighterBaseState currentState;
        private Dictionary<FighterState, FighterBaseState> stateMap;

        public FighterComponentManager FighterComponentManager { get; private set; }
        public FighterState CurrentStateType { get; private set; } = FighterState.Disabled;
        private float stateTimer;
        public float StateTimer => stateTimer;

        public void Inject(FighterComponentManager fighterComponentManager)
        {
            FighterComponentManager = fighterComponentManager;
        }

        void Awake()
        {
            // Create and register states
            stateMap = new Dictionary<FighterState, FighterBaseState>
            {
                { FighterState.Idle, new FighterIdleState(FighterComponentManager) },
                { FighterState.Blocking, new FighterBlockingState(FighterComponentManager) },
                { FighterState.BlockStun, new FighterBlockStunState(FighterComponentManager) },
                { FighterState.Hitstun, new FighterHitstunState(FighterComponentManager) },
                { FighterState.Attacking, new FighterAttackingState(FighterComponentManager) },
                { FighterState.Sidestep, new FighterSidestepState(FighterComponentManager) },
                // Add more as needed
            };
        }

        void Start()
        {
            SwitchState(FighterState.Idle);
        }

        public void CustomUpdate(float dt)
        {
            currentState?.OnUpdate(dt);
            if (stateTimer > 0)
            {
                stateTimer -= dt;

            }
            else if (stateTimer <= 0)
            {
                if (CurrentStateType == FighterState.Hitstun)
                {
                    SwitchState(FighterState.Idle);
                }
                else if (CurrentStateType == FighterState.BlockStun)
                {
                    if (FighterComponentManager.FighterController.IsBlocking)
                    {
                        SwitchState(FighterState.Blocking);
                    }
                    else
                    {
                        SwitchState(FighterState.Idle);
                    }
                }
                else if (CurrentStateType == FighterState.Attacking)
                {
                    SwitchState(FighterState.Idle);
                }

            }
        }

        public void SwitchState(FighterState newState, float duration = 0f)
        {
            currentState?.OnExit();
            CurrentStateType = newState;
            currentState = stateMap[newState];
            stateTimer = duration;
            currentState.OnEnter();
        }

        public bool CanBlock() =>
            CurrentStateType == FighterState.Idle || CurrentStateType == FighterState.Moving;

        public bool CanStartMove() =>
            CurrentStateType == FighterState.Idle;
    }
}
