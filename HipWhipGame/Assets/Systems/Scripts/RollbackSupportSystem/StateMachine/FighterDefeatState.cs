/*
File Name:    FighterDefeatState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

namespace RollbackSupport
{
    public class FighterDefeatState : FighterBaseState
    {
        private int defeatFrame;

        public FighterDefeatState(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

        public override void OnEnter(int duration = 0)
        {
            // Deterministically start at normalizedTime = 0
            if (stateMachine.PreviousState != null)
            {
                if (stateMachine.PreviousState != this)
                {
                    defeatFrame = 0;
                }
            }

            fighterComponentManager.FighterController.SetIsMovable(false);
        }

        public override void OnUpdate()
        {
            // Increment by 1 fixed simulation frame
            defeatFrame++;
        }

        public override void OnExit() { }

        public override void OnUpdateAnimation()
        {
            UpdateDefeatVisual();
        }

        private void UpdateDefeatVisual()
        {
            // Loop the animation using normalized [0,1)
            const float defeatFPS = 120f;

            float norm = (defeatFrame % defeatFPS) / defeatFPS;

            // Deterministic animation update
            fighterComponentManager.Animator.Play("Defeat", 0, norm);
            fighterComponentManager.Animator.Update(0f);
        }
    }
}
