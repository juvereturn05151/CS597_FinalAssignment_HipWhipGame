/*
File Name:    FighterAttackingState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using HipWhipGame;
using UnityEngine;

namespace RollbackSupport
{
    public class FighterAttackingState : FighterBaseState
    {
        public FighterAttackingState(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

        public override void OnEnter(int duration = 0)
        {
           
        }

        public override void OnUpdate()
        {
            
        }

        public override void OnExit()
        {
            
        }

        public override void OnUpdateAnimation()
        {
            fighterComponentManager.DeterministicAnimator.PerformMove();
        }
    }
}
