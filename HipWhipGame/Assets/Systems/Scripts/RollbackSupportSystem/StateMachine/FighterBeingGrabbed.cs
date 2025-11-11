/*
File Name:    FighterTryGrabState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
using static HipWhipGame.Enums;

namespace RollbackSupport
{
    public class FighterBeingGrabbed : FighterBaseState
    {
        public FighterBeingGrabbed(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

        public override void OnEnter()
        {
            //fighterComponentManager.Animator?.SetBool("BeingGrabbed", true);
            fighterComponentManager.Fighter.AnimatorSync.ResetGrabTimers();
        }

        public override void OnUpdate()
        {
            // Optional: early cancel windows, hit confirms, etc.
        }

        public override void OnExit()
        {
            //fighterComponentManager.Animator?.SetBool("BeingGrabbed", false);
        }
    }
}
