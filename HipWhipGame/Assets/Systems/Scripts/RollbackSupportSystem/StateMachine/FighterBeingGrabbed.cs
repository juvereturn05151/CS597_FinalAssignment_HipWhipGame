/*
File Name:    FighterBeingGrabbed.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;

namespace RollbackSupport
{
    public class FighterBeingGrabbed : FighterBaseState
    {
        public FighterBeingGrabbed(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

        public override void OnEnter(int duration = 0)
        {
            fighterComponentManager.FighterStateMachine.SetMaxDurationTimer(300);
            fighterComponentManager.FighterStateMachine.SetDurationTimer(300);
        }

        public override void OnUpdate()
        {
            fighterComponentManager.FighterStateMachine.DecreaseDurationTimer();
        }

        public override void OnExit()
        {

        }

        public override void OnUpdateAnimation()
        {
            UpdateBeingGrabbedVisual();
        }

        private void UpdateBeingGrabbedVisual()
        {
            float norm = 1f - (float)fighterComponentManager.FighterStateMachine.DurationTimer / Mathf.Max(1, fighterComponentManager.FighterStateMachine.MaxDurationTimer);
            fighterComponentManager.Animator.Play("BeingGrabbed", 0, norm);
            fighterComponentManager.Animator.Update(0f);
        }
    }
}
