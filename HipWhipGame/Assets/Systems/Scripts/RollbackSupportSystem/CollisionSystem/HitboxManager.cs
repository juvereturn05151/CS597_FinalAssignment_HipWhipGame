using RollbackSupport;
using System.Collections.Generic;
using UnityEngine;

namespace RollbackSupport
{
    public class HitboxManager
    {
        public static readonly HitboxManager Instance = new HitboxManager();
        private readonly List<Fighter> fighters = new List<Fighter>();

        public void Register(Fighter f)
        {
            if (!fighters.Contains(f)) fighters.Add(f);
        }

        public void CheckHits()
        {
            for (int i = 0; i < fighters.Count; i++)
                for (int j = i + 1; j < fighters.Count; j++)
                {
                    var a = fighters[i];
                    var b = fighters[j];
                    foreach (var hb in a.MoveExec.ActiveHitboxes)
                        foreach (var hb2 in b.moves.hurtboxes)
                        {
                            if (CollisionBox.Overlaps(hb, a.transform, hb2, b.transform))
                                b.TakeHit();
                        }
                }
        }
    }
}