/*
File Name:    FighterAttackingState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
using static HipWhipGame.Enums;

namespace HipWhipGame
{
    public class FighterAttackingState : FighterBaseState
    {
        public FighterAttackingState(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

        public override void OnEnter()
        {
            Debug.Log("Entering Attacking State");
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
