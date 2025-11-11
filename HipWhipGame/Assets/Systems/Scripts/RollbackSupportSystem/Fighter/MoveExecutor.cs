using HipWhipGame;
using System.Collections.Generic;
using UnityEngine;

namespace RollbackSupport
{
    public class MoveExecutor : MonoBehaviour
    {
        Fighter fighter;
        MoveData currentMove;
        int frame;
        bool executing;

        public bool IsExecuting => executing;
        public int CurrentFrame => frame;
        public string CurrentMoveName => currentMove ? currentMove.moveName : null;
        public List<CollisionBox> ActiveHitboxes { get; } = new List<CollisionBox>();

        public void Bind(Fighter f) => fighter = f;

        public void StartMove(MoveData move)
        {
            if (executing) return;
            currentMove = move;
            frame = 0;
            executing = true;
            fighter.State = FighterState.Attack;
        }

        public void SimulateFrame()
        {
            if (!executing) return;
            frame++;
            ActiveHitboxes.Clear();

            foreach (var h in currentMove.hitboxes)
            {
                if (frame >= h.start && frame < h.end)
                    ActiveHitboxes.Add(h.box);
            }

            if (frame >= currentMove.totalFrames)
            {
                executing = false;
                fighter.State = FighterState.Idle;
            }
        }

        public void RestoreMove(string name, int frameIndex)
        {
            if (string.IsNullOrEmpty(name)) { executing = false; return; }
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

                // Convert local box to world space
                Bounds b = box.ToWorld(transform);

                // Draw an outline cube for clarity
                Gizmos.DrawWireCube(b.center, b.size);

                // Slight transparency for fill
                Color fill = new Color(1f, 0f, 0f, 0.15f);
                Gizmos.color = fill;
                Gizmos.DrawCube(b.center, b.size);

                // Restore outline color
                Gizmos.color = Color.red;
            }
        }
#endif

    }
}