using System.Collections.Generic;
using UnityEngine;

namespace RollbackSupport
{
    public class HitboxManager
    {
        public static readonly HitboxManager Instance = new HitboxManager();
        private readonly List<FighterComponentManager> fighters = new List<FighterComponentManager>();

        public void Register(FighterComponentManager f)
        {
            if (!fighters.Contains(f)) 
            {
                fighters.Add(f);
            } 
        }

        public void CheckHits()
        {
            for (int i = 0; i < fighters.Count; i++) 
            {
                for (int j = i + 1; j < fighters.Count; j++)
                {
                    var attacker = fighters[i];
                    var defender = fighters[j];

                    if (!attacker.MoveExecutor.IsExecuting)
                    {
                        continue;
                    }


                    var move = attacker.MoveExecutor.CurrentMove;
                    if (move == null)
                    {
                        continue;
                    }

                    foreach (var hitbox in attacker.MoveExecutor.ActiveHitboxes)
                    {
                        foreach (var hurt in defender.FighterController.Hurtboxes.ActiveBoxes)
                        {
                            if (CollisionBox.Overlaps(hitbox, attacker.transform, hurt, defender.transform))
                            {
                                Vector3 worldKnock = attacker.transform.TransformDirection(move.knockback);

                                // --- BLOCK or HIT ---
                                if (defender.FighterController.IsBlocking())
                                {
                                    defender.FighterController.TakeBlock(move, worldKnock);
                                }
                                else 
                                {
                                    defender.FighterController.TakeHit(move, worldKnock);
                                }

                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
