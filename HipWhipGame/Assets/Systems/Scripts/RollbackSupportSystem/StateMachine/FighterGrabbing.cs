/*
File Name:    FighterGrabbing.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;

namespace RollbackSupport
{
    public class FighterGrabbing : FighterBaseState
    {
        public FighterGrabbing(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

        public override void OnEnter(int duration = 0)
        {
            fighterComponentManager.FighterStateMachine.SetMaxDurationTimer(300);
            fighterComponentManager.FighterStateMachine.SetDurationTimer(300);
            fighterComponentManager.FighterGrabManager.Grab();
        }

        public override void OnUpdate()
        {
            fighterComponentManager.FighterStateMachine.DecreaseDurationTimer();
            fighterComponentManager.FighterGrabManager.UpdateGrab();
        }

        public override void OnExit()
        {

        }

        public override void OnUpdateAnimation()
        {
            UpdateGrabbingVisual();
        }

        private void UpdateGrabbingVisual()
        {
            float norm = 1f - (float)fighterComponentManager.FighterStateMachine.DurationTimer / Mathf.Max(1, fighterComponentManager.FighterStateMachine.MaxDurationTimer);
            fighterComponentManager.Animator.Play("Grabbing", 0, norm);
            fighterComponentManager.Animator.Update(0f);
        }
    }
}
