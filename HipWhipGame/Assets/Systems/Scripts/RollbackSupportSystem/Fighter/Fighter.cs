using HipWhipGame;
using UnityEngine;

namespace RollbackSupport
{
    public enum FighterState { Idle, Walk, Jump, Attack, Block, Hitstun }

    public class Fighter : MonoBehaviour
    {
        public string fighterName;
        public FighterState State;
        public KinematicBody body = new KinematicBody();
        public MoveExecutor MoveExec;
        public DeterministicAnimator AnimatorSync;
        public MoveDatabase moves;

        public InputFrame LastInput;

        public void Initialize(Vector3 start)
        {
            body.Teleport(start);
            MoveExec.Bind(this);
            AnimatorSync.Bind(this);
        }

        public void SimulateFrame()
        {
            if (!MoveExec.IsExecuting)
            {
                ProcessMovement();
                HandleAttacks();
            }
            else
            {
                MoveExec.SimulateFrame();
            }

            transform.position = body.position;
        }

        void ProcessMovement()
        {
            if (LastInput.block)
            {
                State = FighterState.Block;
                return;
            }

            Vector3 move = Vector3.right * LastInput.horiz * 0.08f;
            body.position += move;

            if (LastInput.vert > 0 && body.grounded)
            {
                body.velocity.y = 0.25f;
                body.grounded = false;
                State = FighterState.Jump;
            }
        }

        void HandleAttacks()
        {
            if (LastInput.light) MoveExec.StartMove(moves.light);
            else if (LastInput.heavy) MoveExec.StartMove(moves.heavy);
        }

        public void TakeHit()
        {
            State = FighterState.Hitstun;
        }
    }
}