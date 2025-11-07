/*
File Name:    FighterIdleState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
namespace HipWhipGame
{
    public class FighterSidestepState : FighterBaseState
    {
        public FighterSidestepState(FighterComponentManager mgr) : base(mgr) { }

        public override void OnEnter()
        {
            // optional animation flag
            fighterComponentManager.Animator?.Play("Sidestep", 0, 0); 
        }

        public override void OnUpdate(float dt) { }

        public override void OnExit() { }
    }

}