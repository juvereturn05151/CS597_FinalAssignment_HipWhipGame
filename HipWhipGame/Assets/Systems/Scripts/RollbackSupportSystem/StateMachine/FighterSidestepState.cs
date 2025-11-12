/*
File Name:    FighterSidestepState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

namespace RollbackSupport
{
    public class FighterSidestepState : FighterBaseState
    {
        public FighterSidestepState(FighterComponentManager mgr) : base(mgr) { }

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