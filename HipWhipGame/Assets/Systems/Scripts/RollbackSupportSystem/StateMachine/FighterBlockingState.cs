/*
File Name:    FighterBlockingState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using static HipWhipGame.Enums;

namespace RollbackSupport
{
    public class FighterBlockingState : FighterBaseState
    {
        public FighterBlockingState(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

        public override void OnEnter()
        {
            //fighterComponentManager.Animator?.SetBool("Block", true);
        }

        public override void OnUpdate()
        {
            // Remain blocking if input held
            //if (!fighterComponentManager.FighterController.IsBlocking)
            //{
            //    stateMachine.SwitchState(FighterState.Idle);
            //}
        }

        public override void OnExit()
        {
            //fighterComponentManager.Animator?.SetBool("Block", false);
        }
    }
}
