/*
File Name:    FighterTryGrabState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

namespace RollbackSupport
{
    public class FighterBeingGrabbed : FighterBaseState
    {
        public FighterBeingGrabbed(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

        public override void OnEnter(int duration = 0)
        {
            fighterComponentManager.DeterministicAnimator.ResetGrabTimers();
        }

        public override void OnUpdate()
        {

        }

        public override void OnExit()
        {

        }

        public override void OnUpdateAnimation()
        {

        }
    }
}
