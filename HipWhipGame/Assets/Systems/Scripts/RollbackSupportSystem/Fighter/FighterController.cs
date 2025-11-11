using UnityEngine;

namespace RollbackSupport
{
    public enum FighterState { Idle, Walk, Jump, Attack, Block, BlockStun, Hitstun, TryGrab, Disabled, Sidestep,     // startup frames of the grab
        Grabbing,    // holding the opponent
        BeingGrabbed,
    }

    public class FighterController : MonoBehaviour, IFighterComponentInjectable
    {
        private FighterComponentManager fighterComponentManager;

        public int playerIndex;
        public string fighterName;
        public KinematicBody body = new KinematicBody();
        public MoveDatabase moves;
        public Transform lookAtTarget;

        private Vector3 hitVelocity;
        private int hitstunTimer;
        public bool InHitstun => hitstunTimer > 0;

        private Vector3 blockPushVel;
        private int blockstunTimer;
        public bool isBlocking;
        public bool InBlockstun => blockstunTimer > 0;

        public InputFrame LastInput;

        private bool isMovable = true;

        public bool IsMovable
        {
            get => isMovable;
            private set => isMovable = value;
        }

        public void SetIsMovable(bool canMove)
        {
            isMovable = canMove;
        }

        public void Inject(FighterComponentManager fighterComponentManager)
        {
            this.fighterComponentManager = fighterComponentManager;
        }

        public void SimulateFrame()
        {
            if (InHitstun)
            {
                SimulateHitstun();
            }
            else if (InBlockstun)
            {
                SimulateBlockstun();
            }
            else if (!IsMovable)
            {
                // skip movement and input handling if frozen
                fighterComponentManager.MoveExecutor.SimulateFrame();
            }
            else if (!fighterComponentManager.MoveExecutor.IsExecuting)
            {
                HandleBlocking();
                HandleSidestep();
                ProcessMovement();
                HandleAttacks();
            }
            else
            {
                fighterComponentManager.MoveExecutor.SimulateFrame();
            }

            transform.position = body.position;
        }

        void ProcessMovement()
        {
            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

            if (lookAtTarget)
            {
                Vector3 dir = lookAtTarget.position - transform.position;
                dir.y = 0;
                if (dir.sqrMagnitude > 0.0001f) 
                {
                    forward = dir.normalized;
                }

                right = Quaternion.Euler(0, 90f, 0) * forward;
            }

            // Compute input direction
            Vector3 input = new Vector3(LastInput.horiz, 0f, LastInput.vert);
            if (input.sqrMagnitude > 1f)
                input.Normalize();

            Vector3 moveDir = (forward * input.z + right * input.x).normalized;

            // Apply horizontal movement (fixed per frame, deterministic)
            const float movePerFrame = 0.08f;
            body.position += moveDir * movePerFrame;

            // Rotate toward target
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

            // Apply visual transform from rollback body
            transform.position = body.position;
        }



        private void HandleSidestep()
        {
            if (LastInput.sidestep < 0)
            {
                fighterComponentManager.MoveExecutor.StartMove(moves.sideStepLeft);
                fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Sidestep);
            }
            else if (LastInput.sidestep > 0)
            {
                fighterComponentManager.MoveExecutor.StartMove(moves.sideStepRight);
                fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Sidestep);
            }

            LastInput.sidestep = 0;
        }

        void HandleAttacks()
        {
            if (LastInput.grab)
            {
                fighterComponentManager.MoveExecutor.StartMove(moves.grab);
                fighterComponentManager.FighterStateMachine.SwitchState(FighterState.TryGrab);
            }
            else if(LastInput.light)
            {
                fighterComponentManager.MoveExecutor.StartMove(moves.light);
                fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Attack);
            }
            else if (LastInput.heavy) 
            {
                fighterComponentManager.MoveExecutor.StartMove(moves.heavy);
                fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Attack);
            } 
        }


        public void TakeHit(MoveData move, Vector3 worldKnock)
        {
            if (move == null) return;

            Debug.Log($"[{fighterName}] Took hit from [{move.moveName}]!");

            // Apply hit reaction
            ApplyHitReaction(move.hitstunFrames, worldKnock);
        }

        public void ApplyHitReaction(int stunFrames, Vector3 knockback)
        {
            hitstunTimer = stunFrames;
            hitVelocity = knockback / stunFrames; // consistent knockback per frame
            fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Hitstun, stunFrames);
        }

        private void SimulateHitstun()
        {
            // Apply knockback motion deterministically
            body.position += hitVelocity;

            hitstunTimer--;
            if (hitstunTimer <= 0)
            {
                hitVelocity = Vector3.zero;
                fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Idle);
            }
        }


        public bool IsBlocking()
        {
            return fighterComponentManager.FighterStateMachine.CurrentStateType == FighterState.Block ||
                   fighterComponentManager.FighterStateMachine.CurrentStateType == FighterState.BlockStun;
        }


        private void HandleBlocking()
        {
            if (LastInput.block && !InBlockstun && !InHitstun)
            {
                fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Block);
            }
            else if (!LastInput.block)
            {
                fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Idle);
            }
        }

        public void TakeBlock(MoveData move, Vector3 worldKnock)
        {
            if (move == null) return;

            Debug.Log($"[{fighterName}] Blocked {move.moveName}");

            blockstunTimer = move.blockstunFrames;
            fighterComponentManager.FighterStateMachine.SwitchState(FighterState.BlockStun, blockstunTimer);
        }

        private void SimulateBlockstun()
        {
            body.position += blockPushVel;
            blockstunTimer--;

            if (blockstunTimer <= 0)
            {
                blockPushVel = Vector3.zero;
                isBlocking = false;
                fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Idle);
            }
        }



        /// <summary>
        /// Locks or unlocks the fighter's ability to move and perform actions.
        /// </summary>

    }
}