/*
File Name:    FighterIdleState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
using static HipWhipGame.Enums;

namespace RollbackSupport
{
    public class FighterIdleState : FighterBaseState
    {
        public FighterIdleState(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

        public override void OnEnter(int duration = 0)
        {
            //Debug.Log("Fighter is in Idle State");
            //fighterComponentManager.Animator?.Play("Idle", 0, 0);
            //fighterComponentManager.Animator?.SetBool("Block", false);
            fighterComponentManager.Animator.Play("Idle", 0, 0f);
        }

        public override void OnUpdate()
        {
            // Transition example
            //if (fighterComponentManager.FighterController.IsBlocking && stateMachine.CanBlock())
            //{
            //    stateMachine.SwitchState(FighterState.Blocking);
            //}
        }

        public override void OnExit() { }

        public override void OnUpdateAnimation()
        {

        }
    }
}
