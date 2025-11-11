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
            fighterComponentManager.Animator.Play("Idle", 0, 0f);
        }

        public override void OnUpdate()
        {

        }

        public override void OnExit() 
        { 
        
        }

        public override void OnUpdateAnimation()
        {

        }
    }
}
