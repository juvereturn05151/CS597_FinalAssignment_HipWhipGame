using HipWhipGame;
using UnityEngine;

namespace RollbackSupport
{
    public enum FighterState { Idle, Walk, Jump, Attack, Block, Hitstun }

    public class Fighter : MonoBehaviour
    {
        public int playerIndex;
        public string fighterName;
        public FighterState State;
        public KinematicBody body = new KinematicBody();
        public MoveExecutor MoveExec;
        public DeterministicAnimator AnimatorSync;
        public MoveDatabase moves;
        public Transform lookAtTarget;
        public GameSimulation gameSimulation;

        public InputFrame LastInput;

        public void Initialize(Vector3 start, GameSimulation gameSimulation)
        {
            MoveExec.Bind(this);
            AnimatorSync.Bind(this);
            this.gameSimulation = gameSimulation;
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
            // 1. Blocking logic
            if (LastInput.block)
            {
                State = FighterState.Block;
                return;
            }

            // 2. Determine facing direction
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

            if (lookAtTarget)
            {
                Vector3 dir = lookAtTarget.position - transform.position;
                dir.y = 0;
                if (dir.sqrMagnitude > 0.0001f)
                    forward = dir.normalized;

                right = Quaternion.Euler(0, 90f, 0) * forward;
            }

            // 3. Compute input direction
            Vector3 input = new Vector3(LastInput.horiz, 0f, LastInput.vert);
            //if (input.sqrMagnitude > 1f)
            //    input.Normalize();

            Vector3 moveDir = (forward * input.z + right * input.x).normalized;

            // 4. Apply horizontal movement (fixed per frame, deterministic)
            const float movePerFrame = 0.08f;
            body.position += moveDir * movePerFrame;

            // 8. Rotate toward target
            if (lookAtTarget)
            {
                Vector3 face = lookAtTarget.position - transform.position;
                face.y = 0f;
                if (face.sqrMagnitude > 0.0001f)
                    transform.rotation = Quaternion.LookRotation(face);
            }
            else if (moveDir.sqrMagnitude > 0.001f)
            {
                transform.forward = moveDir;
            }

            // 9. Apply visual transform from rollback body
            transform.position = body.position;
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