/*
File Name:    FighterHitstunState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;

namespace RollbackSupport
{
    public class FighterHitstunState : FighterBaseState
    {
        public FighterHitstunState(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

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
                fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Idle);
            }
        }

        public override void OnExit() { }

        public override void OnUpdateAnimation()
        {
            UpdateHitstunVisual();
        }

        private void UpdateHitstunVisual()
        {
            // Deterministic frame-based interpolation
            float norm = 1f - (float)fighterComponentManager.FighterStateMachine.DurationTimer / Mathf.Max(1, fighterComponentManager.FighterStateMachine.MaxDurationTimer);
            fighterComponentManager.Animator.Play("HitStun", 0, norm);
            fighterComponentManager.Animator.Update(0f);
        }
    }
}
