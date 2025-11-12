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
        private int maxHitstunTimer;
        private int hitstunTimer;
        private float hitstunAnimTimer;
        public FighterHitstunState(FighterComponentManager fighterComponentManager) : base(fighterComponentManager) { }

        public override void OnEnter(int duration = 0)
        {
            maxHitstunTimer = duration;
            hitstunTimer = maxHitstunTimer;
            hitstunAnimTimer = 0;
        }

        public override void OnUpdate()
        {
            //Debug.Log("Hitstun Timer: " + hitstunTimer);

            hitstunTimer--;
            if (hitstunTimer <= 0)
            {
                fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Idle);
            }
        }

        public override void OnExit()
        {

        }

        public override void OnUpdateAnimation() 
        {
            UpdateHitstunVisual();
        }

        private void UpdateHitstunVisual()
        {
            hitstunAnimTimer += 1f / 60f;

            float clipLength = 1.0f - (1.0f / (float)maxHitstunTimer);
            float norm = Mathf.Clamp01(hitstunAnimTimer / clipLength);

            fighterComponentManager.Animator.Play("HitStun", 0, norm);
        }
    }
}
