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
        private float grabbingTimer;
        public FighterGrabbing(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

        public override void OnEnter(int duration = 0)
        {
            fighterComponentManager.FighterGrabManager.Grab();
            grabbingTimer = 0f;
        }

        public override void OnUpdate()
        {
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
            grabbingTimer += 1f / 60f;

            // assume your Grabbing animation lasts about 1 second
            float clipLength = 1.0f;
            float norm = Mathf.Clamp01(grabbingTimer / clipLength);

            fighterComponentManager.Animator.Play("Grabbing", 0, norm);
        }
    }
}
