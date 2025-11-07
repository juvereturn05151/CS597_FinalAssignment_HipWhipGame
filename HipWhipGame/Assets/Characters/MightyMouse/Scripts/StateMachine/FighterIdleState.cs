/*
File Name:    FighterIdleState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
using static HipWhipGame.Enums;

namespace HipWhipGame
{
    public class FighterIdleState : FighterBaseState
    {
        public FighterIdleState(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

        public override void OnEnter()
        {
            fighterComponentManager.Animator?.Play("Idle", 0, 0);
            fighterComponentManager.Animator?.SetBool("Block", false);
        }

        public override void OnUpdate(float dt)
        {
            // Transition example
            if (fighterComponentManager.FighterController.IsBlocking && stateMachine.CanBlock())
            {
                stateMachine.SwitchState(FighterState.Blocking);
            }
        }

        public override void OnExit() { }
    }
}
