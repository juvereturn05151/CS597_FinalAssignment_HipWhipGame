/*
File Name:    FighterController.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

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

        public void SetHitVelocity(Vector3 velocity) 
        {
            hitVelocity = velocity;
        }

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

        private float damagePercent;
        public float DamagePercent
        {
            get => damagePercent;
            private set => damagePercent = value;
        }

        public void Inject(FighterComponentManager fighterComponentManager)
        {
            this.fighterComponentManager = fighterComponentManager;
        }

        public void ResetStateForRespawn()
        {
            damagePercent = 0.0f;
            fighterComponentManager.FighterUI.UpdatePercentage(damagePercent);
        }

        public void SimulateFrame()
        {
            if (fighterComponentManager.FighterStateMachine.CurrentStateType == FighterState.Hitstun)
            {
                SimulateHitstun();
            }
            else if (fighterComponentManager.FighterStateMachine.CurrentStateType == FighterState.BlockStun)
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

                if (!LastInput.block) 
                {
                    ProcessMovement();
                    HandleAttacks();
                }
            }
            else
            {
                fighterComponentManager.MoveExecutor.SimulateFrame();
            }

            RotateToOpponent();
            transform.position = body.position;
        }

        private void RotateToOpponent() 
        {
            // Rotate toward target
            if (fighterComponentManager.FighterController.lookAtTarget)
            {
                Vector3 face = fighterComponentManager.FighterController.lookAtTarget.position - fighterComponentManager.FighterController.transform.position;
                face.y = 0f;
                if (face.sqrMagnitude > 0.0001f) 
                {
                    fighterComponentManager.transform.rotation = Quaternion.LookRotation(face);
                }
            }
        }

        private void ProcessMovement()
        {
            Vector3 input = new Vector3(LastInput.horiz, 0f, LastInput.vert);
            if (input.sqrMagnitude > 0f)
            {
                fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Walk);
            }
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
            transform.position = body.position;
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
            if (move == null) 
            {
                return;
            }

            if (fighterComponentManager.FighterStateMachine.CurrentStateType == FighterState.Grabbing) 
            {
                return;
            }

            damagePercent += move.damage;   // add to % like Smash
            damagePercent = Mathf.Clamp(DamagePercent, 0, 999);
            fighterComponentManager.FighterUI.UpdatePercentage(damagePercent);

            Vector3 knockbackGrowth = new Vector3(1.0f, 1.0f, 1.0f);

            // calculate knockback using percentage
            Vector3 scaledKnock =
                worldKnock + knockbackGrowth * (DamagePercent / 100f);

            hitVelocity = scaledKnock / move.hitstunFrames;

            fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Hitstun, move.hitstunFrames);
        }

        public void SimulateHitstun()
        {
            body.position += hitVelocity;
        }

        public bool IsBlocking()
        {
            return fighterComponentManager.FighterStateMachine.CurrentStateType == FighterState.Block ||
                   fighterComponentManager.FighterStateMachine.CurrentStateType == FighterState.BlockStun;
        }

        private void HandleBlocking()
        {
            if (LastInput.block && fighterComponentManager.FighterStateMachine.CurrentStateType != FighterState.BlockStun
                && fighterComponentManager.FighterStateMachine.CurrentStateType != FighterState.Hitstun)
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
            if (move == null) 
            {
                return;
            } 

            fighterComponentManager.FighterStateMachine.SwitchState(FighterState.BlockStun, move.blockstunFrames);
        }

        private void SimulateBlockstun()
        {

        }
    }
}