using UnityEngine;

namespace RollbackSupport
{
    public class DeterministicAnimator : MonoBehaviour
    {
        public Animator animator;
        Fighter fighter;

        public void Bind(Fighter f)
        {
            fighter = f;
            if (!animator) animator = GetComponentInChildren<Animator>();
            animator.updateMode = AnimatorUpdateMode.Normal;
            animator.speed = 0f;
        }

        public void ApplyVisuals()
        {
            if (!animator) return;

            // Simple example: sync attack animation by frame
            if (fighter.MoveExec.IsExecuting)
            {
                var move = fighter.MoveExec.CurrentMoveName;
                float norm = (float)fighter.MoveExec.CurrentFrame / fighter.moves.Get(move).totalFrames;
                animator.Play(move, 0, norm);
            }

            animator.Update(0f);
        }
    }
}