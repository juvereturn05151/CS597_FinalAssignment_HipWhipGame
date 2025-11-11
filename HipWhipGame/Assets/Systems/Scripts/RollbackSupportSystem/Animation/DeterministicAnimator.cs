using UnityEngine;

namespace RollbackSupport
{
    public class DeterministicAnimator : MonoBehaviour
    {
        public Animator animator;
        private Fighter fighter;

        // cache for local velocity
        private Vector3 lastFramePos;
        private Vector3 localVel;

        public void Bind(Fighter f)
        {
            fighter = f;
            if (!animator) animator = GetComponentInChildren<Animator>();

            animator.updateMode = AnimatorUpdateMode.Normal;
            animator.speed = 0f;

            lastFramePos = fighter.body.position;
        }

        public void ApplyVisuals()
        {
            if (!animator || fighter == null) return;

            // update movement blend only when not attacking
            if (!fighter.MoveExec.IsExecuting)
            {
                UpdateMovementBlend();
            }
            else
            {
                // attack playback based on frame
                var move = fighter.MoveExec.CurrentMoveName;
                float norm = (float)fighter.MoveExec.CurrentFrame / fighter.moves.Get(move).totalFrames;
                animator.Play(move, 0, norm);
            }

            animator.Update(0f);
            lastFramePos = fighter.body.position;
        }

        private float walkAnimTimer;

        private void UpdateMovementBlend()
        {
            // compute local velocity based on rollback body positions
            Vector3 delta = fighter.body.position - lastFramePos;
            localVel = fighter.transform.InverseTransformDirection(new Vector3(delta.x, 0f, delta.z));

            const float movePerFrame = 0.08f;
            float maxSpeed = Mathf.Max(0.0001f, movePerFrame);

            float x = Mathf.Clamp(localVel.x / maxSpeed, -1f, 1f);
            float y = Mathf.Clamp(localVel.z / maxSpeed, -1f, 1f);
            bool moving = (Mathf.Abs(x) + Mathf.Abs(y)) > 0.01f;

            animator.SetFloat("X", x);
            animator.SetFloat("Y", y);
            animator.SetBool("Move", moving);

            if (moving)
            {
                // advance visual playback deterministically (1 frame = 1/60s)
                walkAnimTimer += 1f / 60f;

                // assuming your walk animation is about 1 second long
                float walkCycleLength = 1f;
                float normTime = (walkAnimTimer % walkCycleLength) / walkCycleLength;

                animator.Play("Walk", 0, normTime);
            }
            else
            {
                walkAnimTimer = 0f;
                animator.Play("Idle", 0, 0f);
            }
        }
    }
}
