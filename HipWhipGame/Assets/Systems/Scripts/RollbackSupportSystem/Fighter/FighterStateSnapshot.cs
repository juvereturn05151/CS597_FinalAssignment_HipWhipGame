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
        public float normalizedTime;
        public string animState;
        public int durationTimer;
        public int maxDurationTimer;

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
                normalizedTime = stateInfo.normalizedTime % 1f,
                animState = stateInfo.IsName(f.MoveExecutor.CurrentMoveName)
                    ? f.MoveExecutor.CurrentMoveName
                    : stateInfo.shortNameHash.ToString(),
                durationTimer = f.FighterStateMachine.DurationTimer,
                maxDurationTimer = f.FighterStateMachine.MaxDurationTimer
            };
}

        public void ApplyTo(FighterComponentManager f)
        {
            f.FighterController.body.position = pos;
            f.FighterController.body.velocity = vel;
            f.FighterController.LastInput = lastInput;
            f.FighterStateMachine.SwitchState(state);
            f.FighterStateMachine.SetDurationTimer(durationTimer);
            f.FighterStateMachine.SetMaxDurationTimer(maxDurationTimer);
            // Restore animation
            var anim = f.Animator;
            if (!string.IsNullOrEmpty(moveName))
            {
                anim.Play(moveName, 0, normalizedTime);
                anim.Update(0f);
            }

            // Restore move executor logical progress
            f.MoveExecutor.RestoreMove(moveName, moveFrame);
        }
    }

}