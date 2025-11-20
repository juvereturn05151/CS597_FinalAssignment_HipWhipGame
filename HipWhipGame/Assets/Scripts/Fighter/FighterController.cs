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
        Ultimate,
        Victory,
        Defeat
    }

    public class FighterController : MonoBehaviour, IFighterComponentInjectable
    {
        private FighterComponentManager fighterComponentManager;

        public int playerIndex;
        public string fighterName;
        public KinematicBody body = new KinematicBody();
        public MoveDatabase moves;
        [SerializeField]
        private FighterController opponent;
        public FighterController Opponent => opponent;
        public void SetOpponent(FighterController opponent) 
        {
            this.opponent = opponent;
        }

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

        [SerializeField]
        private GameObject pressingSpecialEffect;

        private float superMeter;

        public float SuperMeter
        {
            get => superMeter;
            private set => superMeter = Mathf.Clamp(value, 0f, 5f);
        }

        public void AddMeter(float amount)
        {
            SuperMeter += amount;
            fighterComponentManager.FighterUI.UpdateMeter(superMeter);
        }

        public bool SpendMeter(float amount)
        {
            if (superMeter < amount)
                return false;

            superMeter -= amount;
            fighterComponentManager.FighterUI.UpdateMeter(superMeter);
            return true;
        }

        public void SetMeter(float value)
        {
            superMeter = Mathf.Clamp(value, 0f, 5f);
            fighterComponentManager.FighterUI.UpdateMeter(superMeter);
        }

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
            fighterComponentManager.MoveExecutor.ForceStop();
            superMeter = 0.0f;
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
                    HandleSpecialButtonEffect();
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

            HandleSpecialButtonEffect();
            RotateToOpponent();
            transform.position = body.position;
        }

        private void HandleSpecialButtonEffect() 
        {
            fighterComponentManager.FighterUI.SpecialUI.SetActive(LastInput.special );
        }

        private void RotateToOpponent() 
        {
            if (fighterComponentManager.MoveExecutor.IsExecuting && fighterComponentManager.MoveExecutor.CurrentMove.isTrackingSidestep == false) 
            {
                return;
            }

            // Rotate toward target
            if (fighterComponentManager.FighterController.opponent)
            {
                Vector3 face = fighterComponentManager.FighterController.opponent.transform.position - fighterComponentManager.FighterController.transform.position;
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

            if (input.special && input.light && input.heavy)
            {
                if (SpendMeter(3f))
                {
                    fighterComponentManager.FighterAudioComponent.PlaySuperVoice();
                    fighterComponentManager.MoveExecutor.StartMove(moves.ultimateButt);
                    fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Ultimate);
                }
                return;
            }

            if (input.special && input.light)
            {
                if (SpendMeter(2f))
                {
                    fighterComponentManager.MoveExecutor.StartMove(moves.specialFart);
                    fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Attack);
                }
                return;
            }

            if (input.special && input.heavy)
            {
                if (SpendMeter(1f))
                {
                    fighterComponentManager.MoveExecutor.StartMove(moves.superButt);
                    fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Attack);
                }
                return;
            }



            if (LastInput.grab)
            {
                fighterComponentManager.MoveExecutor.StartMove(moves.grab);
                fighterComponentManager.FighterStateMachine.SwitchState(FighterState.TryGrab);
            }
            else if (LastInput.light && LastInput.heavy)
            {
                fighterComponentManager.MoveExecutor.StartMove(moves.spinButt);
                fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Attack);
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

            float knockbackGrowth = 1.0f;

            // calculate knockback using percentage
            Vector3 scaledKnock;

            if (move.isKnockbackAttack)
            {
                scaledKnock = worldKnock * (knockbackGrowth * (DamagePercent / 100f));
            }
            else 
            {
                scaledKnock = worldKnock;
            }

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
            Transform opponent = fighterComponentManager.FighterController.Opponent.transform;
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

        }

    }
}