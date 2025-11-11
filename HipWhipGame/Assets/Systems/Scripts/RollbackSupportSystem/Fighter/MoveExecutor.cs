using HipWhipGame;
using System.Collections.Generic;
using UnityEngine;

namespace RollbackSupport
{
    public class MoveExecutor : MonoBehaviour, IFighterComponentInjectable
    {
        public FighterComponentManager FighterComponentManager { get; private set; }
        private Fighter fighter;
        private MoveData currentMove;
        public MoveData CurrentMove => currentMove;
        private int frame;
        private bool executing;

        public bool IsExecuting => executing;
        public int CurrentFrame => frame;
        public string CurrentMoveName => currentMove ? currentMove.moveName : null;
        public List<CollisionBox> ActiveHitboxes { get; } = new List<CollisionBox>();

        // One shot triggers (ensures SFX/VFX fire exactly once per simulation)
        private bool sfxPlayed;
        private bool vfxSpawned;

        public void Bind(Fighter f) => fighter = f;

        public void Inject(FighterComponentManager fighterComponentManager)
        {
            FighterComponentManager = fighterComponentManager;
        }

        public void StartMove(MoveData move)
        {
            if (executing || move == null)
                return;

            currentMove = move;
            frame = 0;
            executing = true;

            sfxPlayed = false;
            vfxSpawned = false;
        }

        public void SimulateFrame()
        {
            if (!executing || currentMove == null)
                return;

            frame++;
            ActiveHitboxes.Clear();

            // Startup, Active, Recovery
            int activeStart = currentMove.startup;
            int activeEnd = currentMove.startup + currentMove.active;

            // Hitbox activation (deterministic frame by frame)
            foreach (var h in currentMove.hitboxes)
            {
                if (frame >= h.start && frame < h.end)
                    ActiveHitboxes.Add(h.box);
            }

            // Motion override
            if (currentMove.overrideRootMotion && currentMove.motionSegments != null)
            {
                foreach (var seg in currentMove.motionSegments)
                {
                    if (frame >= seg.frameStart && frame <= seg.frameEnd)
                    {
                        Vector3 move = Vector3.zero;
                        move += fighter.transform.forward * seg.forwardSpeed;
                        move += fighter.transform.right * seg.sideSpeed;
                        move += fighter.transform.up * seg.verticalSpeed;

                        // deterministic movement (no deltaTime)
                        fighter.body.position += move;
                    }
                }
            }

            // SFX trigger (on first startup frame)
            if (!sfxPlayed && frame == 1 && currentMove.sfx)
            {
                AudioSource.PlayClipAtPoint(currentMove.sfx, fighter.body.position);
                sfxPlayed = true;
            }

            // 5VFX trigger (when move becomes active)
            if (!vfxSpawned && frame == activeStart && currentMove.vfxPrefab)
            {
                Object.Instantiate(
                    currentMove.vfxPrefab,
                    fighter.body.position + fighter.transform.forward * 0.5f,
                    fighter.transform.rotation
                );
                vfxSpawned = true;
            }

            // Grab detection
            if (currentMove.isGrab && frame == activeStart)
            {
                TryGrabOpponent();
            }

            // End of move
            if (frame >= currentMove.totalFrames)
            {
                executing = false;
                fighter.FighterComponentManager?.FighterStateMachine.SwitchState(FighterState.Idle);
            }
        }

        private void TryGrabOpponent()
        {
            var opponent = FighterComponentManager.Fighter;
            float dist = Vector3.Distance(fighter.body.position, FighterComponentManager.Fighter.body.position);
            if (dist <= currentMove.grabRange)
            {
                Debug.Log($"[{fighter.fighterName}] grabbed [{FighterComponentManager.Fighter.fighterName}]!");
                //opponent.TakeGrab(currentMove);
                
            }
        }

        public void RestoreMove(string name, int frameIndex)
        {
            if (string.IsNullOrEmpty(name))
            {
                executing = false;
                currentMove = null;
                return;
            }

            if (currentMove && currentMove.moveName == name)
            {
                frame = frameIndex;
                executing = true;
            }
        }

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            if (ActiveHitboxes == null || ActiveHitboxes.Count == 0)
                return;

            Gizmos.color = Color.red;

            foreach (var box in ActiveHitboxes)
            {
                if (!box.enabled) continue;
                Bounds b = box.ToWorld(transform);

                // Outline
                Gizmos.DrawWireCube(b.center, b.size);

                // Transparent fill
                Color fill = new Color(1f, 0f, 0f, 0.15f);
                Gizmos.color = fill;
                Gizmos.DrawCube(b.center, b.size);

                // Reset color
                Gizmos.color = Color.red;
            }
        }
#endif
    }
}
