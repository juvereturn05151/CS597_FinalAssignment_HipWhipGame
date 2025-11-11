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
        private float beingGrabbedTimer;
        public FighterBeingGrabbed(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

        public override void OnEnter(int duration = 0)
        {
            beingGrabbedTimer = 0.0f;
        }

        public override void OnUpdate()
        {

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
            beingGrabbedTimer += 1f / 60f;

            // assume your BeingGrabbed animation lasts about 1 second
            float clipLength = 1.0f;
            float norm = Mathf.Clamp01(beingGrabbedTimer / clipLength);

            fighterComponentManager.Animator.Play("BeingGrabbed", 0, norm);
        }
    }
}
