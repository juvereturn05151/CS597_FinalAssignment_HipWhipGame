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
        float _hitstunTimer;

        public void SetState(FighterState newState, float lockTime = 0f)
        {
            State = newState;
            _stateTimer = lockTime;
        }

        public bool CanStartMove() => State == FighterState.Idle;

        public void Tick(float dt)
        {
            switch (State)
            {
                case FighterState.Hitstun:
                    _hitstunTimer -= dt;
                    if (_hitstunTimer <= 0f)
                        SetState(FighterState.Idle);
                    break;
                case FighterState.Attacking:
                    if (_stateTimer > 0f)
                    {
                        _stateTimer -= dt;
                        if (_stateTimer <= 0f)
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
            _hitstunTimer = duration;
        }
    }

}