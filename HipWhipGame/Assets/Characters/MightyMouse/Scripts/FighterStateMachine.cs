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
        float _blockstunTimer;

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
                case FighterState.BlockStun:
                    _blockstunTimer -= dt;
                    float framesLeft = _blockstunTimer * 60f;
                    Debug.Log($"Blockstun Timer: {_blockstunTimer:F3} sec ({framesLeft:F1} frames)");
                    if (_blockstunTimer <= 0f)
                    {
                        GetComponentInChildren<Animator>().SetBool("BlockStun", false);
                        // choose next state based on input
                        var controller = GetComponent<FighterController>();
                        if (controller != null && controller.isBlocking)
                            SetState(FighterState.Blocking);
                        else
                            SetState(FighterState.Idle);
                    }
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

        public void EnterBlockstun(float duration)
        {
            SetState(FighterState.BlockStun, duration);
            _blockstunTimer = Mathf.Abs(duration);
        }

        public bool CanBlock()
        {
            return State == FighterState.Idle || State == FighterState.Moving;
        }

    }

}