/*
File Name:    FighterIdleState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
namespace RollbackSupport
{
    public class FighterSidestepState : FighterBaseState
    {
        public FighterSidestepState(FighterComponentManager mgr) : base(mgr) { }

        public override void OnEnter()
        {
            // optional animation flag
            //fighterComponentManager.Animator?.Play("Sidestep", 0, 0); 
        }

        public override void OnUpdate() { }

        public override void OnExit() { }
    }

}