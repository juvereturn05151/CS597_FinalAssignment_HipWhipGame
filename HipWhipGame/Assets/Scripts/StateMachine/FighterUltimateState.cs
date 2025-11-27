/*
File Name:    FighterUltimateState.cs
*/

using UnityEngine;

namespace RollbackSupport
{
    public class FighterUltimateState : FighterBaseState
    {
        private MoveData move;
        private int freezeFrames;
        private int totalFrames;

        // This counter is critical for replay/rollback.
        private int frameCounter;

        /// <summary>
        /// Current internal frame counter for the ultimate sequence.
        /// Used by rollback/replay to restore exact timing.
        /// </summary>
        public int FrameCounter => frameCounter;

        /// <summary>
        /// Allows rollback/replay to restore the counter exactly.
        /// </summary>
        public void SetFrameCounter(int value)
        {
            frameCounter = Mathf.Max(0, value);
        }

        public FighterUltimateState(FighterComponentManager mgr) : base(mgr)
        {
        }

        public override void OnEnter(int duration = 0)
        {
            // Cache the move we are about to play.
            move = fighterComponentManager.MoveExecutor.CurrentMove;

            if (move == null)
            {
                Debug.LogWarning("FighterUltimateState.OnEnter: CurrentMove is null.");
                // Fail-safe: immediately return to idle if something is wrong.
                fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Idle);
                return;
            }

            freezeFrames = move.preSuperFreezeFrames;
            totalFrames = move.totalFrames;

            // Reset internal timer when entering the state the first time.
            frameCounter = 0;

            // Disable all movement.
            fighterComponentManager.FighterController.SetIsMovable(false);

            // Freeze move logic until freeze period is over.
            fighterComponentManager.MoveExecutor.SetFrozen(true);

            // Camera cinematic start.
            fighterComponentManager.FighterCameraController.StartUltimateZoom(move.preSuperFreezeFrames);

            // Start animation at frame 0.
            var animator = fighterComponentManager.Animator;
            animator.Play(move.animName, 0, 0f);
            animator.Update(0f);
        }

        public override void OnUpdate()
        {
            // Advance our internal timer.
            frameCounter++;

            // Run camera zooming every frame (including during freeze).
            fighterComponentManager.FighterCameraController.SimulateCameraFrame();

            // 1. Freeze Period
            if (frameCounter <= freezeFrames)
            {
                return;
            }

            // Unfreeze move logic on the first frame after freeze.
            if (frameCounter == freezeFrames + 1)
            {
                fighterComponentManager.MoveExecutor.SetFrozen(false);
            }

            // 2. Move Execution after freeze.
            fighterComponentManager.MoveExecutor.SimulateFrame();

            // 3. End of Ultimate when we've played all frames plus freeze.
            if (frameCounter >= totalFrames + freezeFrames)
            {
                fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Idle);
            }
        }

        public override void OnExit()
        {
            // Make sure move logic is not left frozen.
            fighterComponentManager.MoveExecutor.SetFrozen(false);

            // Restore movement.
            fighterComponentManager.FighterController.SetIsMovable(true);

            // Restore camera.
            fighterComponentManager.FighterCameraController.ResetUltimate();
        }

        public override void OnUpdateAnimation()
        {
            if (move == null)
            {
                move = fighterComponentManager.MoveExecutor.CurrentMove;
                if (move == null)
                    return;
            }

            float norm;

            if (move.animateDuringFreeze)
            {
                // Animation plays during freeze but DOES NOT affect startup.
                // We use frameCounter directly but normalize by totalFrames + freezeFrames.
                norm = (float)frameCounter / (totalFrames + freezeFrames);
            }
            else
            {
                // Animation is frozen during freeze, then starts at frameCounter == freezeFrames.
                norm = (float)(frameCounter - freezeFrames) / totalFrames;
            }

            norm = Mathf.Clamp01(norm);

            var animator = fighterComponentManager.Animator;
            animator.Play(move.animName, 0, norm);
            animator.Update(0f);
        }
    }
}
