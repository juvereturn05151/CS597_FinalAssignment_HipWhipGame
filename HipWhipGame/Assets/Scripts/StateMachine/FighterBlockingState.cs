/*
File Name:    FighterBlockingState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

namespace RollbackSupport
{
    public class FighterBlockingState : FighterBaseState
    {
        public FighterBlockingState(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

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
            PlayBlockHold();
        }

        private void PlayBlockHold()
        {
            fighterComponentManager.Animator.Play("HighBlock", 0, 0f);
        }
    }
}
