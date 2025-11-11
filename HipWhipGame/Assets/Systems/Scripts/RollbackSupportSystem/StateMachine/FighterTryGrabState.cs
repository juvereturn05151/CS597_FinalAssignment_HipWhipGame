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

        public override void OnEnter(int duration = 0)
        {

        }

        public override void OnUpdate()
        {

        }

        public override void OnExit()
        {
            
        }

        public override void OnUpdateAnimation()
        {
            fighterComponentManager.DeterministicAnimator.PerformMove();
        }
    }
}
