/*
File Name:    FighterBlockStunState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
using static HipWhipGame.Enums;

namespace RollbackSupport
{
    public class FighterBlockStunState : FighterBaseState
    {
        public FighterBlockStunState(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

        public override void OnEnter()
        {
            //fighterComponentManager.Animator?.SetBool("BlockStun", true);
            //fighterComponentManager.Animator?.Play("BlockStun", 0, 0);
        }

        public override void OnUpdate()
        {
            //Debug.Log("Fighter is in Block Stun State: "+ fighterComponentManager.FighterStateMachine.StateTimer);
            // Timer handled by state machine
        }

        public override void OnExit()
        {
            //fighterComponentManager.Animator?.SetBool("BlockStun", false);

        }
    }
}
