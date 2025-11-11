/*
File Name:    FighterStateMachine.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
using System.Collections.Generic;

namespace RollbackSupport
{
    [RequireComponent(typeof(FighterComponentManager))]
    public class FighterStateMachine : MonoBehaviour, IFighterComponentInjectable
    {
        private FighterBaseState currentState;
        private Dictionary<FighterState, FighterBaseState> stateMap;
        private List<FighterState> uninterruptedList;
        public bool IsInUninterruptedState => uninterruptedList.Contains(CurrentStateType);

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
                //{ FighterState.Blocking, new FighterBlockingState(FighterComponentManager) },
                //{ FighterState.BlockStun, new FighterBlockStunState(FighterComponentManager) },
                { FighterState.Hitstun, new FighterHitstunState(FighterComponentManager) },
                { FighterState.Attack, new FighterAttackingState(FighterComponentManager) },
                //{ FighterState.Sidestep, new FighterSidestepState(FighterComponentManager) },
                //{ FighterState.TryGrab, new FighterTryGrabState(FighterComponentManager) },
                //{ FighterState.Grabbing, new FighterGrabbing(FighterComponentManager) },
                //{ FighterState.BeingGrabbed, new FighterBeingGrabbed(FighterComponentManager) },
                // Add more as needed
            };

            uninterruptedList = new List<FighterState>
            {
                FighterState.Attack,
                FighterState.Sidestep,
                FighterState.TryGrab,
            };
        }

        void Start()
        {
            SwitchState(FighterState.Idle);
        }

        public void Step()
        {
            currentState?.OnUpdate();

            if (stateTimer > 0)
            {
                stateTimer--;
                if (stateTimer == 0)
                    OnTimerExpire();
            }
        }

        private void OnTimerExpire()
        {
            // determine transitions that end automatically
            if (CurrentStateType == FighterState.Attack ||
                CurrentStateType == FighterState.Hitstun ||
                CurrentStateType == FighterState.Block)
            {
                SwitchState(FighterState.Idle);
            }
        }

        public void SwitchState(FighterState newState, int duration = 0)
        {
            currentState?.OnExit();
            CurrentStateType = newState;
            currentState = stateMap[newState];
            stateTimer = duration;
            currentState.OnEnter();
        }

        public bool CanBlock() =>
            CurrentStateType == FighterState.Idle || CurrentStateType == FighterState.Walk;

        public bool CanStartMove() =>
            CurrentStateType == FighterState.Idle;
    }
}
