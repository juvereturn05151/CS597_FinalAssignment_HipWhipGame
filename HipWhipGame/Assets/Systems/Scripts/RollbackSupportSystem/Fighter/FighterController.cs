/*
File Name:    FighterController.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
using UnityEngine.EventSystems;

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


        public void SetDamagePercent(float damagePercent) 
        {
            this.damagePercent = damagePercent;
        }

        public float DamagePercent
        {
            get => damagePercent;
            private set => damagePercent = value;
        }

        bool isSideStepLeft;

        public float sidestepAngleSpeed = 6f;

        public void Inject(FighterComponentManager fighterComponentManager)
        {
            this.fighterComponentManager = fighterComponentManager;
        }

        public void ResetStateForRespawn()
        {
            LastInput.sidestep = 0;
            LastInput.light = false;
            LastInput.heavy = false;
            LastInput.grab = false;
            LastInput.block = false;
            LastInput.horiz = 0f;
            LastInput.vert = 0f;
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
                // SIDESTEP OVERRIDES NORMAL MOVEMENT
                if (fighterComponentManager.FighterStateMachine.CurrentStateType == FighterState.Sidestep)
                {
                    SimulateSidestep();
                }
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
            if (fighterComponentManager.FighterStateMachine.CurrentStateType == FighterState.Sidestep)
            {
                return;
            }

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
                isSideStepLeft = true;
            }
            else if (LastInput.sidestep > 0)
            {
                fighterComponentManager.MoveExecutor.StartMove(moves.sideStepRight);
                fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Sidestep);
                isSideStepLeft = false;
            }
            LastInput.sidestep = 0;
        }

        void HandleAttacks()
        {
            var input = fighterComponentManager.FighterController.LastInput;
            var moves = fighterComponentManager.FighterController.moves;

            // ============================================
            // SPECIAL MOVE SYSTEM
            // ============================================

            // 1. SPECIAL + LIGHT  -> THROW
            if (input.special && input.light)
            {
                fighterComponentManager.MoveExecutor.StartMove(moves.grab);
                fighterComponentManager.FighterStateMachine.SwitchState(FighterState.TryGrab);
                return;
            }

            // 2. SPECIAL + HEAVY -> FART (big hitstun)
            if (input.special && input.heavy)
            {
                fighterComponentManager.MoveExecutor.StartMove(moves.specialFart);
                fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Attack);
                return;
            }

            // 3. SPECIAL + SUPER BUTT  -> ULTRA SUPER BUTT
            if (input.special && input.superButt)
            {
                fighterComponentManager.MoveExecutor.StartMove(moves.ultimateButt);
                fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Attack);
                return;
            }

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
            else if (LastInput.superButt)
            {
                fighterComponentManager.MoveExecutor.StartMove(moves.superButt);
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

            float knockbackGrowth = 1.0f;

            // calculate knockback using percentage
            Vector3 scaledKnock =
                worldKnock * (knockbackGrowth * (DamagePercent / 100f));

            Debug.Log($"scaledKnock: {scaledKnock}");

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

        private void SimulateSidestep()
        {
            // The opponent we orbit around
            Transform opponent = fighterComponentManager.FighterController.lookAtTarget;
            if (!opponent)
                return;

            Vector3 center = opponent.position;
            Vector3 pos = body.position;

            // vector from opponent-> self
            Vector3 offset = pos - center;
            offset.y = 0f;

            // direction of sidestep (left or right)
            float angle;
            if (isSideStepLeft)
            {
                angle = sidestepAngleSpeed;
            }
            else 
            {
                angle = -sidestepAngleSpeed;
            }

                

            // rotate around opponent on the XZ plane
            offset = Quaternion.Euler(0f, angle, 0f) * offset;

            // update position
            body.position = center + offset;

            // keep grounded Y
            body.position = new Vector3(body.position.x, pos.y, body.position.z);

            // consume input (we only use it once per frame)
            
        }

    }
}