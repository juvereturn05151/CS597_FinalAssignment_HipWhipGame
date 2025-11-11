/*
File Name:    FighterTryGrabState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

namespace RollbackSupport
{
    public class FighterGrabbing : FighterBaseState
    {
        public FighterGrabbing(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

        public override void OnEnter(int duration = 0)
        {
            fighterComponentManager.FighterGrabManager.Grab();
            fighterComponentManager.DeterministicAnimator.ResetGrabTimers();
        }

        public override void OnUpdate()
        {
            fighterComponentManager.FighterGrabManager.UpdateGrab();
        }

        public override void OnExit()
        {

        }

        public override void OnUpdateAnimation()
        {

        }
    }
}
