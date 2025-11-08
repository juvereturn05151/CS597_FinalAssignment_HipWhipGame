/*
File Name:    FighterTryGrabState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
using static HipWhipGame.Enums;

namespace HipWhipGame
{
    public class FighterGrabbing : FighterBaseState
    {
        public FighterGrabbing(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

        public override void OnEnter()
        {
            fighterComponentManager.Animator?.SetBool("Grabbing", true);
            fighterComponentManager.FighterGrabManager.Grab();
        }

        public override void OnUpdate(float dt)
        {
            fighterComponentManager.FighterGrabManager.UpdateGrab();
        }

        public override void OnExit()
        {
            fighterComponentManager.Animator?.SetBool("Grabbing", false);
        }
    }
}
