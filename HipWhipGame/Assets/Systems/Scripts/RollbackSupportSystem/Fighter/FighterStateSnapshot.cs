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

        public static FighterStateSnapshot From(FighterComponentManager f)
        {
            //Debug.Log($"Captured {f.name} at {f.transform.position}");
            return new FighterStateSnapshot
            {
                pos = f.FighterController.body.position,
                vel = f.FighterController.body.velocity,
                state = f.FighterStateMachine.CurrentStateType,
                lastInput = f.FighterController.LastInput,
                moveFrame = f.MoveExecutor.CurrentFrame,
                moveName = f.MoveExecutor.CurrentMoveName
            };
        }

        public void ApplyTo(FighterComponentManager f)
        {
            f.FighterController.body.position = pos;
            f.FighterController.body.velocity = vel;
            f.FighterController.LastInput = lastInput;
            //f.FighterStateMachine.SwitchState(state);
            //f.MoveExecutor.RestoreMove(moveName, moveFrame);
        }
    }
}