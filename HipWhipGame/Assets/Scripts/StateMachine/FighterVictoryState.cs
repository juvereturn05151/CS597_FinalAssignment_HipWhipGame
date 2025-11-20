/*
File Name:    FighterVictoryState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

namespace RollbackSupport
{
    public class FighterVictoryState : FighterBaseState
    {
        private int victoryFrame;

        public FighterVictoryState(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

        public override void OnEnter(int duration = 0)
        {
            // Deterministically start at normalizedTime = 0
            if (stateMachine.PreviousState != null)
            {
                if (stateMachine.PreviousState != this)
                {
                    victoryFrame = 0;
                }
            }

            fighterComponentManager.FighterController.SetIsMovable(false);
        }

        public override void OnUpdate()
        {
            // Increment by 1 fixed simulation frame
            victoryFrame++;
        }

        public override void OnExit() { }

        public override void OnUpdateAnimation()
        {
            UpdateVictoryVisual();
        }

        private void UpdateVictoryVisual()
        {
            // Loop the animation using normalized [0,1)
            const float victoryFPS = 180f;

            float norm = (victoryFrame % victoryFPS) / victoryFPS;

            // Deterministic animation update
            fighterComponentManager.Animator.Play("Victory", 0, norm);
            fighterComponentManager.Animator.Update(0f);
        }
    }
}