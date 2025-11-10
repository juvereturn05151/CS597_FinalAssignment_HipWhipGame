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

        public static FighterStateSnapshot From(Fighter f)
        {
            return new FighterStateSnapshot
            {
                pos = f.body.position,
                vel = f.body.velocity,
                state = f.State,
                moveFrame = f.MoveExec.CurrentFrame,
                moveName = f.MoveExec.CurrentMoveName
            };
        }

        public void ApplyTo(Fighter f)
        {
            f.body.position = pos;
            f.body.velocity = vel;
            f.State = state;
            f.MoveExec.RestoreMove(moveName, moveFrame);
        }
    }
}