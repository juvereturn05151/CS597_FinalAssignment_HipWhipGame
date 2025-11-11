using UnityEngine;

namespace RollbackSupport
{
    public enum FighterState { Idle, Walk, Jump, Attack, Block, BlockStun, Hitstun, TryGrab, Disabled, Sidestep }

    public class Fighter : MonoBehaviour, IFighterComponentInjectable
    {

        public int playerIndex;
        public string fighterName;
        public KinematicBody body = new KinematicBody();
        public MoveExecutor MoveExec;
        public DeterministicAnimator AnimatorSync;
        public MoveDatabase moves;
        public Transform lookAtTarget;
        public GameSimulation gameSimulation;
        public HurtboxComponent Hurtboxes = new HurtboxComponent();
        public CollisionBox pushbox = new CollisionBox
        {
            localCenter = new Vector3(0, 1.0f, 0),
            size = new Vector3(0.6f, 2.0f, 0.6f),
            enabled = true
        };

        public bool IsPushedThisFrame;

        private Vector3 hitVelocity;
        private int hitstunTimer;
        public bool InHitstun => hitstunTimer > 0;

        public InputFrame LastInput;

        public FighterComponentManager FighterComponentManager { get; private set; }

        public void Initialize(Vector3 start, GameSimulation gameSimulation)
        {
            MoveExec.Bind(this);
            AnimatorSync.Bind(this);
            Hurtboxes.AddBox(new Vector3(0, 1.0f, 0), new Vector3(0.6f, 2.0f, 0.6f));
            this.gameSimulation = gameSimulation;
        }

        public void Inject(FighterComponentManager fighterComponentManager)
        {
            FighterComponentManager = fighterComponentManager;
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
            else if (!MoveExec.IsExecuting)
            {
                HandleBlocking();
                HandleSidestep();
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
            if (input.sqrMagnitude > 1f)
                input.Normalize();

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



        private void HandleSidestep()
        {
            if (LastInput.sidestep < 0)
            {
                MoveExec.StartMove(moves.sideStepLeft);
                FighterComponentManager.FighterStateMachine.SwitchState(FighterState.Sidestep);
            }
            else if (LastInput.sidestep > 0)
            {
                MoveExec.StartMove(moves.sideStepRight);
                FighterComponentManager.FighterStateMachine.SwitchState(FighterState.Sidestep);
            }

            LastInput.sidestep = 0;
        }

        void HandleAttacks()
        {
            if (LastInput.light)
            {
                MoveExec.StartMove(moves.light);
                FighterComponentManager.FighterStateMachine.SwitchState(FighterState.Attack);
            }
            else if (LastInput.heavy) 
            {
                MoveExec.StartMove(moves.heavy);
                FighterComponentManager.FighterStateMachine.SwitchState(FighterState.Attack);
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
            FighterComponentManager.FighterStateMachine.SwitchState(FighterState.Hitstun, stunFrames);
        }

        public void ApplyRecoil(Vector3 recoil)
        {
            body.position += recoil;
        }

        private void SimulateHitstun()
        {
            // Apply knockback motion deterministically
            body.position += hitVelocity;

            hitstunTimer--;
            if (hitstunTimer <= 0)
            {
                hitVelocity = Vector3.zero;
                FighterComponentManager.FighterStateMachine.SwitchState(FighterState.Idle);
            }
        }

        private Vector3 blockPushVel;
        private int blockstunTimer;
        public bool isBlocking;
        public bool IsBlocking()
        {
            return FighterComponentManager.FighterStateMachine.CurrentStateType == FighterState.Block ||
                   FighterComponentManager.FighterStateMachine.CurrentStateType == FighterState.BlockStun;
        }
        public bool InBlockstun => blockstunTimer > 0;

        private void HandleBlocking()
        {
            if (LastInput.block && !InBlockstun && !InHitstun)
            {
                FighterComponentManager.FighterStateMachine.SwitchState(FighterState.Block);
            }
            else if (!LastInput.block)
            {
                FighterComponentManager.FighterStateMachine.SwitchState(FighterState.Idle);
            }
        }

        public void TakeBlock(MoveData move, Vector3 worldKnock)
        {
            if (move == null) return;

            Debug.Log($"[{fighterName}] Blocked {move.moveName}");

            blockstunTimer = move.blockstunFrames;
            //blockPushVel = worldKnock * 0.25f / blockstunTimer; // lighter knockback
            FighterComponentManager.FighterStateMachine.SwitchState(FighterState.BlockStun, blockstunTimer);
        }

        private void SimulateBlockstun()
        {
            body.position += blockPushVel;
            blockstunTimer--;

            if (blockstunTimer <= 0)
            {
                blockPushVel = Vector3.zero;
                isBlocking = false;
                FighterComponentManager.FighterStateMachine.SwitchState(FighterState.Idle);
            }
        }


#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            foreach (var hb in Hurtboxes.ActiveBoxes)
            {
                Bounds b = hb.ToWorld(transform);
                Gizmos.DrawWireCube(b.center, b.size);
            }

            if (!pushbox.enabled) return;
            Gizmos.color = Color.cyan;
            Bounds p = pushbox.ToWorld(transform);
            Gizmos.DrawWireCube(p.center, p.size);
        }
#endif
    }
}