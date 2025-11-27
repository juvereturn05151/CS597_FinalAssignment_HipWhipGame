using UnityEngine;

namespace RollbackSupport
{
    [System.Serializable]
    public struct FighterStateSnapshot
    {
        public Vector3 pos, vel;
        public InputFrame lastInput;
        public FighterState state;
        public float meter;
        public int moveFrame;
        public string moveName;
        public float damagePercent;
        public float normalizedTime;
        public string animState;
        public int durationTimer;
        public int maxDurationTimer;
        public bool moveExecuted;

        // New: track ultimate-specific timing for deterministic replay.
        public int ultimateFrameCounter;

        public static FighterStateSnapshot From(FighterComponentManager f)
        {
            var animator = f.Animator;
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            var moveExecutor = f.MoveExecutor;
            var currentMove = moveExecutor.CurrentMove;

            // Determine a good animation state name to store.
            string storedAnimName;

            if (currentMove != null &&
                !string.IsNullOrEmpty(currentMove.animName) &&
                stateInfo.IsName(currentMove.animName))
            {
                storedAnimName = currentMove.animName;
            }
            else if (!string.IsNullOrEmpty(moveExecutor.CurrentMoveName) &&
                     stateInfo.IsName(moveExecutor.CurrentMoveName))
            {
                storedAnimName = moveExecutor.CurrentMoveName;
            }
            else
            {
                // Fallback: store the hash as string.
                storedAnimName = stateInfo.shortNameHash.ToString();
            }

            // Capture ultimate state's internal frame counter if we're currently in Ultimate.
            int capturedUltimateCounter = 0;
            if (f.FighterStateMachine.CurrentStateType == FighterState.Ultimate &&
                f.FighterStateMachine.CurrentState is FighterUltimateState ultimateState)
            {
                capturedUltimateCounter = ultimateState.FrameCounter;
            }

            return new FighterStateSnapshot
            {
                pos = f.FighterController.body.position,
                vel = f.FighterController.body.velocity,
                state = f.FighterStateMachine.CurrentStateType,
                lastInput = f.FighterController.LastInput,
                meter = f.FighterController.SuperMeter,
                moveFrame = moveExecutor.CurrentFrame,
                moveName = moveExecutor.CurrentMoveName,
                damagePercent = f.FighterController.DamagePercent,
                normalizedTime = stateInfo.normalizedTime % 1f,
                animState = storedAnimName,
                durationTimer = f.FighterStateMachine.DurationTimer,
                maxDurationTimer = f.FighterStateMachine.MaxDurationTimer,
                moveExecuted = moveExecutor.IsExecuting,
                ultimateFrameCounter = capturedUltimateCounter
            };
        }

        public void ApplyTo(FighterComponentManager f)
        {
            // 1. Restore body.
            f.FighterController.body.position = pos;
            f.FighterController.body.velocity = vel;

            // 2. Restore inputs BEFORE logic.
            f.FighterController.LastInput = lastInput;

            // 3. Restore meters and damage.
            f.FighterController.SetMeter(meter);
            f.FighterController.SetDamagePercent(damagePercent);

            var moveExecutor = f.MoveExecutor;

            // 4. Restore move executor START/STOP + frame.
            if (!moveExecuted)
            {
                moveExecutor.ForceStop();
            }
            else
            {
                moveExecutor.ForceStart(moveName, moveFrame);
            }

            // 5. Switch state machine AFTER move executor is configured,
            //    so states that depend on CurrentMove see correct data.
            f.FighterStateMachine.SwitchState(state);

            // Restore timers.
            f.FighterStateMachine.SetDurationTimer(durationTimer);
            f.FighterStateMachine.SetMaxDurationTimer(maxDurationTimer);

            // 6. If we're in Ultimate, restore its internal frame counter
            //    so that animation + freeze behavior remain deterministic.
            if (state == FighterState.Ultimate &&
                f.FighterStateMachine.CurrentState is FighterUltimateState ultimateState)
            {
                ultimateState.SetFrameCounter(ultimateFrameCounter);
            }

            // 7. Restore animation AFTER logical state + move executor.
            var anim = f.Animator;

            if (moveExecuted && !string.IsNullOrEmpty(moveName))
            {
                var currentMove = moveExecutor.CurrentMove;

                // Prefer the actual animation name from MoveData if available.
                string animNameToUse = currentMove != null && !string.IsNullOrEmpty(currentMove.animName)
                    ? currentMove.animName
                    : animState;

                if (string.IsNullOrEmpty(animNameToUse))
                {
                    // Last fallback: use moveName.
                    animNameToUse = moveName;
                }

                anim.Play(animNameToUse, 0, normalizedTime);
            }

            anim.Update(0f);
        }
    }
}
