/*
File Name:    FighterTryGrabState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

namespace RollbackSupport
{
    public class FighterTryGrabState : FighterBaseState
    {
        public FighterTryGrabState(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

        public override void OnEnter()
        {
            // Trigger attack animation is handled elsewhere
        }

        public override void OnUpdate()
        {
            // Optional: early cancel windows, hit confirms, etc.
        }

        public override void OnExit()
        {
            
        }
    }
}
