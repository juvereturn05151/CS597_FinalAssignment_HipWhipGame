/*
File Name:    FighterHitstunState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
using static HipWhipGame.Enums;

namespace RollbackSupport
{
    public class FighterHitstunState : FighterBaseState
    {
        public FighterHitstunState(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

        public override void OnEnter()
        {
            //fighterComponentManager.Animator?.SetBool("Block", false);
            //fighterComponentManager.Animator?.Play("HitStun", 0, 0f);
            //fighterComponentManager.FighterController.SetIsInterrupted(true);

            var fighter = fighterComponentManager.Fighter;

            if (fighter.AnimatorSync)
                fighter.AnimatorSync.ResetHitstunTimer();
        
        }

        public override void OnUpdate()
        {
            // Wait for timer to expire
        }

        public override void OnExit()
        {
            // Transition back to idle handled by StateMachine
            //fighterComponentManager.FighterController.SetIsInterrupted(false);
        }
    }
}
