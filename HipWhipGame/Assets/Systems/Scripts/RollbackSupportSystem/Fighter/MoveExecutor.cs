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
    }
}