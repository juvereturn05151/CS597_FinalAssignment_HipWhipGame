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
        private Vector3 localVel;

        // timers for deterministic animation phases
        private float walkAnimTimer;
        private float hitstunTimer;
        private float blockstunTimer;

        private float grabbingTimer;
        private float beingGrabbedTimer;

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
            if (!animator || fighter == null) return;

            var fsm = fighterComponentManager?.FighterStateMachine;
            var state = fsm?.CurrentStateType ?? FighterState.Idle;

            // --- Attack animation ---
            if (fighterComponentManager.MoveExecutor.IsExecuting)
            {
                PlayAttack();
            }
            else if (state == FighterState.Grabbing)
            {
                UpdateGrabbingVisual();
            }
            else if (state == FighterState.BeingGrabbed)
            {
                UpdateBeingGrabbedVisual();
            }
            else if (state == FighterState.BlockStun)
            {
                UpdateBlockstunVisual();
            }
            else if (state == FighterState.Block)
            {
                PlayBlockHold();
            }
            else if (state == FighterState.Hitstun)
            {
                UpdateHitstunVisual();
            }
            else
            {
                UpdateMovementBlend();
            }

            animator.Update(0f);
            lastFramePos = fighter.body.position;
        }

        // ------------------------------------------------------------
        // ATTACK
        // ------------------------------------------------------------
        private void PlayAttack()
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
        // WALK / IDLE
        // ------------------------------------------------------------
        private void UpdateMovementBlend()
        {
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
                walkAnimTimer += 1f / 60f;
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

        private void UpdateGrabbingVisual()
        {
            grabbingTimer += 1f / 60f;

            // assume your Grabbing animation lasts about 1 second
            float clipLength = 1.0f;
            float norm = Mathf.Clamp01(grabbingTimer / clipLength);

            animator.Play("Grabbing", 0, norm);
        }

        private void UpdateBeingGrabbedVisual()
        {
            beingGrabbedTimer += 1f / 60f;

            // assume your BeingGrabbed animation lasts about 1 second
            float clipLength = 1.0f;
            float norm = Mathf.Clamp01(beingGrabbedTimer / clipLength);

            animator.Play("BeingGrabbed", 0, norm);
        }
        public void ResetGrabTimers()
        {
            grabbingTimer = 0f;
            beingGrabbedTimer = 0f;
        }
    }
}
