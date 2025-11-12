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
        private int blockstunTimer;
        private float blockstunTimerAnim;

        public FighterBlockStunState(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

        public override void OnEnter(int duration = 0)
        {
            blockstunTimer = duration;
        }

        public override void OnUpdate()
        {
            blockstunTimer--;

            if (blockstunTimer <= 0)
            {
                fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Idle);
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
            blockstunTimerAnim += 1f / 60f;

            // adjust to match your BlockStun clip duration
            float clipLength = 0.5f;
            float norm = Mathf.Clamp01(blockstunTimer / clipLength);

            fighterComponentManager.Animator.Play("BlockStun", 0, norm);
        }
    }
}
