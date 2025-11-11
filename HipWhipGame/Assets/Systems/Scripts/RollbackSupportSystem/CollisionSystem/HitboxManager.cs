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
                    var attacker = fighters[i];
                    var defender = fighters[j];

                    // Skip if no move executing
                    if (!attacker.MoveExec.IsExecuting)
                        continue;

                    foreach (var hitbox in attacker.MoveExec.ActiveHitboxes)
                        foreach (var hurt in defender.Hurtboxes.ActiveBoxes)
                        {
                            if (CollisionBox.Overlaps(hitbox, attacker.transform, hurt, defender.transform))
                            {
                                defender.TakeHit();
                                return;
                            }
                        }
                }
        }

    }
}