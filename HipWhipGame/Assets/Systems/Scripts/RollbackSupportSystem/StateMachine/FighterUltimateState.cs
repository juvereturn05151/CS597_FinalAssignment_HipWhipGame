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
        private int frameCounter;

        public FighterUltimateState(FighterComponentManager mgr) : base(mgr) { }

        public override void OnEnter(int duration = 0)
        {
            move = fighterComponentManager.MoveExecutor.CurrentMove;
            freezeFrames = move.preSuperFreezeFrames;
            totalFrames = move.totalFrames;

            frameCounter = 0;

            // Disable all movement
            fighterComponentManager.FighterController.SetIsMovable(false);

            // Camera cinematic start
            fighterComponentManager.FighterCameraController.StartUltimateZoom(move.preSuperFreezeFrames);

            // Start animation at frame 0
            fighterComponentManager.Animator.Play(move.animName, 0, 0f);
            fighterComponentManager.Animator.Update(0f);
        }

        public override void OnUpdate()
        {
            frameCounter++;

            // Run camera zooming
            fighterComponentManager.FighterCameraController.SimulateCameraFrame();

            // 1. Freeze Period
            if (frameCounter <= freezeFrames)
            {
                return;
            }

            // 2. Move Execution after freeze
            fighterComponentManager.MoveExecutor.SimulateFrame();

            // 3. End of Ultimate
            if (frameCounter >= totalFrames + freezeFrames)
            {
                fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Idle);
            }
        }

        public override void OnExit()
        {
            // restore movement
            fighterComponentManager.FighterController.SetIsMovable(true);

            // restore camera
            fighterComponentManager.FighterCameraController.ResetUltimate();
        }

        public override void OnUpdateAnimation()
        {
            float norm = (float)(frameCounter - freezeFrames) / totalFrames;
            norm = Mathf.Clamp01(norm);

            fighterComponentManager.Animator.Play(move.animName, 0, norm);
            fighterComponentManager.Animator.Update(0f);
        }
    }
}
