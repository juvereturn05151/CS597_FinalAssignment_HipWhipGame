/*
File Name:    FighterIdleState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

namespace RollbackSupport
{
    public class FighterIdleState : FighterBaseState
    {
        private int idleFrame;

        public FighterIdleState(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

        public override void OnEnter(int duration = 0)
        {
            

            // Deterministically start at normalizedTime = 0
            if (stateMachine.PreviousState != null) 
            {
                if (stateMachine.PreviousState != this) 
                {
                    idleFrame = 0;
                }
            }

            fighterComponentManager.FighterController.SetIsMovable(true);
        }

        public override void OnUpdate()
        {
            // Increment by 1 fixed simulation frame
            idleFrame++;
        }

        public override void OnExit() { }

        public override void OnUpdateAnimation()
        {
            UpdateIdleVisual();
        }

        private void UpdateIdleVisual()
        {
            // Loop the animation using normalized [0,1)
            const float idleFPS = 120f;

            float norm = (idleFrame % idleFPS) / idleFPS;

            // Deterministic animation update
            fighterComponentManager.Animator.Play("Idle", 0, norm);
            fighterComponentManager.Animator.Update(0f);
        }
    }
}
