/*
File Name:    FighterTryGrabState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
using static HipWhipGame.Enums;

namespace HipWhipGame
{
    public class FighterTryGrabState : FighterBaseState
    {
        public FighterTryGrabState(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

        public override void OnEnter()
        {
            // Trigger attack animation is handled elsewhere
        }

        public override void OnUpdate(float dt)
        {
            // Optional: early cancel windows, hit confirms, etc.
        }

        public override void OnExit()
        {
            
        }
    }
}
