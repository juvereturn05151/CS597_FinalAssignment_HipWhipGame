/*
File Name:    FighterBlockStunState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
using static HipWhipGame.Enums;

namespace HipWhipGame
{
    public class FighterBlockStunState : FighterBaseState
    {
        public FighterBlockStunState(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

        public override void OnEnter()
        {
            fighterComponentManager.Animator?.SetBool("BlockStun", true);
            fighterComponentManager.Animator?.Play("BlockStun", 0, 0);
        }

        public override void OnUpdate(float dt)
        {
            // Timer handled by state machine
        }

        public override void OnExit()
        {
            fighterComponentManager.Animator?.SetBool("BlockStun", false);
            if (fighterComponentManager.FighterController.IsBlocking)
                stateMachine.SwitchState(FighterState.Blocking);
        }
    }
}
