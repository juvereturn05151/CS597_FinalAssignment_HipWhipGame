/*
File Name:    FighterBlockingState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
using static HipWhipGame.Enums;

namespace RollbackSupport
{
    public class FighterBlockingState : FighterBaseState
    {
        public FighterBlockingState(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

        public override void OnEnter(int duration = 0)
        {
            //fighterComponentManager.Animator?.SetBool("Block", true);
        }

        public override void OnUpdate()
        {
            // Remain blocking if input held
        }

        public override void OnExit()
        {
            //fighterComponentManager.Animator?.SetBool("Block", false);
        }

        public override void OnUpdateAnimation()
        {
            PlayBlockHold();
        }

        private void PlayBlockHold()
        {
            fighterComponentManager.Animator.Play("HighBlock", 0, 0f);
        }
    }
}
