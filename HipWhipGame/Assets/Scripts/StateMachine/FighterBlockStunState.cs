/*
File Name:    FighterBlockStunState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;

namespace RollbackSupport
{
    public class FighterBlockStunState : FighterBaseState
    {
        public FighterBlockStunState(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

        public override void OnEnter(int duration = 0)
        {
            fighterComponentManager.FighterStateMachine.SetMaxDurationTimer(duration);
            fighterComponentManager.FighterStateMachine.SetDurationTimer(duration);
        }

        public override void OnUpdate()
        {
            fighterComponentManager.FighterStateMachine.DecreaseDurationTimer();

            if (fighterComponentManager.FighterStateMachine.DurationTimer <= 0)
            {
                if (fighterComponentManager.FighterController.LastInput.block)
                {
                    fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Block);
                }
                else 
                {
                    fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Idle);
                }
            }
        }

        public override void OnExit()
        {

        }

        public override void OnUpdateAnimation()
        {
            UpdateBlockstunVisual();
        }

        private void UpdateBlockstunVisual()
        {
            float norm = 1f - (float)fighterComponentManager.FighterStateMachine.DurationTimer / Mathf.Max(1, fighterComponentManager.FighterStateMachine.MaxDurationTimer);

            fighterComponentManager.Animator.Play("BlockStun", 0, norm);
            fighterComponentManager.Animator.Update(0f);
        }
    }
}
