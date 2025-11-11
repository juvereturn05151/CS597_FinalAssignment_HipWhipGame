/*
File Name:    DeterministicAnimator.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/


using UnityEngine;

namespace RollbackSupport
{
    [RequireComponent(typeof(FighterComponentManager))]
    public class DeterministicAnimator : MonoBehaviour, IFighterComponentInjectable
    {
        private FighterComponentManager fighterComponentManager;

        private Animator animator;
        private FighterController fighter;

        // cache for velocity
        private Vector3 lastFramePos;
        public Vector3 LastFramePos => lastFramePos;

        // timers for deterministic animation phases
        private float hitstunTimer;
        private float blockstunTimer;

        public void Inject(FighterComponentManager fighterComponentManager)
        {
            this.fighterComponentManager = fighterComponentManager;

            fighter = this.fighterComponentManager.FighterController;
            animator = this.fighterComponentManager.Animator;

            animator.updateMode = AnimatorUpdateMode.Normal;
            animator.speed = 0f;

            lastFramePos = fighter.body.position;
        }

        public void ApplyVisuals()
        {
            if (!animator || fighter == null) 
            {
                return;
            } 

            var fsm = fighterComponentManager?.FighterStateMachine;
            var state = fsm?.CurrentStateType ?? FighterState.Idle;

            fsm.UpdateAnimation();

            animator.Update(0f);
            lastFramePos = fighter.body.position;
        }

        // ------------------------------------------------------------
        // MOVE
        // ------------------------------------------------------------
        public void PerformMove()
        {
            var move = fighterComponentManager.MoveExecutor.CurrentMoveName;

            if (string.IsNullOrEmpty(move)) 
            {
                return;
            } 

            var data = fighter.moves.Get(move);
            if (data == null) return;

            float norm = (float)fighterComponentManager.MoveExecutor.CurrentFrame / data.totalFrames;
            animator.Play(move, 0, norm);
        }

        // ------------------------------------------------------------
        // HITSTUN
        // ------------------------------------------------------------
        private void UpdateHitstunVisual()
        {
            hitstunTimer += 1f / 60f;

            float clipLength = 1.0f;
            float norm = Mathf.Clamp01(hitstunTimer / clipLength);

            animator.Play("HitStun", 0, norm);
        }

        public void ResetHitstunTimer() => hitstunTimer = 0f;

        // ------------------------------------------------------------
        // BLOCK / BLOCKSTUN
        // ------------------------------------------------------------
        private void PlayBlockHold()
        {
            // constant pose (doesn't advance)
            animator.Play("HighBlock", 0, 0f);
            blockstunTimer = 0f;
        }

        private void UpdateBlockstunVisual()
        {
            blockstunTimer += 1f / 60f;

            // adjust to match your BlockStun clip duration
            float clipLength = 0.5f;
            float norm = Mathf.Clamp01(blockstunTimer / clipLength);

            animator.Play("BlockStun", 0, norm);
        }


        public void ResetBlockstunTimer() => blockstunTimer = 0f;

    }
}
