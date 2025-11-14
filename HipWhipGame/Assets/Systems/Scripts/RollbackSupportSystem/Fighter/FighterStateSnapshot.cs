using UnityEngine;

namespace RollbackSupport
{
    [System.Serializable]
    public struct FighterStateSnapshot
    {
        public Vector3 pos, vel;
        public InputFrame lastInput;
        public FighterState state;
        public int moveFrame;
        public string moveName;
        public float damagePercent;
        public float normalizedTime;
        public string animState;
        public int durationTimer;
        public int maxDurationTimer;
        public bool moveExecuted;

        public static FighterStateSnapshot From(FighterComponentManager f)
        {
            var animator = f.Animator;
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            return new FighterStateSnapshot
            {
                pos = f.FighterController.body.position,
                vel = f.FighterController.body.velocity,
                state = f.FighterStateMachine.CurrentStateType,
                lastInput = f.FighterController.LastInput,
                moveFrame = f.MoveExecutor.CurrentFrame,
                moveName = f.MoveExecutor.CurrentMoveName,
                damagePercent = f.FighterController.DamagePercent,
                normalizedTime = stateInfo.normalizedTime % 1f,
                animState = stateInfo.IsName(f.MoveExecutor.CurrentMoveName)
                    ? f.MoveExecutor.CurrentMoveName
                    : stateInfo.shortNameHash.ToString(),
                durationTimer = f.FighterStateMachine.DurationTimer,
                maxDurationTimer = f.FighterStateMachine.MaxDurationTimer,
                moveExecuted = f.MoveExecutor.IsExecuting
            };
}

        public void ApplyTo(FighterComponentManager f)
        {
            // 1. Restore body
            f.FighterController.body.position = pos;
            f.FighterController.body.velocity = vel;

            // 2. Restore inputs BEFORE logic
            f.FighterController.LastInput = lastInput;

            // 3. Restore state machine (state + timers)
            f.FighterStateMachine.SwitchState(state);
            f.FighterStateMachine.SetDurationTimer(durationTimer);
            f.FighterStateMachine.SetMaxDurationTimer(maxDurationTimer);
            f.FighterController.SetDamagePercent(damagePercent);

            // 4. Restore move executor START/STOP
            if (!moveExecuted)
            {
                f.MoveExecutor.ForceStop();
            }
            else
            {
                f.MoveExecutor.ForceStart(moveName, moveFrame);
            }

            // 5. Restore animation AFTER logical state
            var anim = f.Animator;

            if (moveExecuted && !string.IsNullOrEmpty(moveName))
            {
                anim.Play(moveName, 0, normalizedTime);
            }

            anim.Update(0f);
        }

    }

}