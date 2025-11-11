using UnityEngine;

namespace RollbackSupport
{
    [System.Serializable]
    public struct FighterStateSnapshot
    {
        public Vector3 pos, vel;
        public FighterState state;
        public int moveFrame;
        public string moveName;

        public static FighterStateSnapshot From(FighterController f)
        {
            return new FighterStateSnapshot
            {
                pos = f.body.position,
                vel = f.body.velocity,
                state = f.FighterComponentManager.FighterStateMachine.CurrentStateType,
                moveFrame = f.MoveExec.CurrentFrame,
                moveName = f.MoveExec.CurrentMoveName
            };
        }

        public void ApplyTo(FighterController f)
        {
            f.body.position = pos;
            f.body.velocity = vel;
            f.FighterComponentManager.FighterStateMachine.SwitchState(state);
            f.MoveExec.RestoreMove(moveName, moveFrame);
        }
    }
}