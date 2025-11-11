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
            fighterComponentManager.FighterController.AnimatorSync.ResetGrabTimers();
        }

        public override void OnUpdate()
        {

        }

        public override void OnExit()
        {

        }
    }
}
