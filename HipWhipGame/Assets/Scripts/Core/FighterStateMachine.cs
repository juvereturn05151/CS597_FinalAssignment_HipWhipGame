/*
File Name:    FighterStateMachine.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
using static HipWhipGame.Enums;

namespace HipWhipGame
{
    public class FighterStateMachine : MonoBehaviour
    {
        public FighterState State { get; private set; } = FighterState.Idle;
        float _stateTimer;

        public void SetState(FighterState newState, float lockTime = 0f)
        {
            State = newState;
            _stateTimer = lockTime;
        }

        public bool CanStartMove() => State == FighterState.Idle || State == FighterState.Jump;

        public void Tick(float dt)
        {
            if (_stateTimer > 0f)
            {
                _stateTimer -= dt;
                if (_stateTimer <= 0f && (State == FighterState.Attacking || State == FighterState.Hitstun)) 
                {
                    State = FighterState.Idle;
                } 
            }
        }

        public void EnterHitstun(float duration)
        {
            SetState(FighterState.Hitstun, duration);
        }
    }

}