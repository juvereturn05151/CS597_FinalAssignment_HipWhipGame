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

        private Vector3 lastFramePos;
        public Vector3 LastFramePos => lastFramePos;

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
        // BLOCK / BLOCKSTUN
        // ------------------------------------------------------------


        

    }
}
