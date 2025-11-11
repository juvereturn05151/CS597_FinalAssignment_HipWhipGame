/*
File Name:    FighterTryGrabState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
using static HipWhipGame.Enums;

namespace RollbackSupport
{
    public class FighterGrabbing : FighterBaseState
    {
        public FighterGrabbing(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

        public override void OnEnter()
        {
            fighterComponentManager.FighterGrabManager.Grab();
            fighterComponentManager.FighterController.AnimatorSync.ResetGrabTimers();
        }

        public override void OnUpdate()
        {
            fighterComponentManager.FighterGrabManager.UpdateGrab();
        }

        public override void OnExit()
        {

        }
    }
}
