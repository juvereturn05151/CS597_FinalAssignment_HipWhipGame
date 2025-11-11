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

                    // Skip if attacker has no active move
                    if (!attacker.MoveExec.IsExecuting)
                        continue;

                    var move = attacker.MoveExec.CurrentMove;
                    if (move == null)
                        continue;

                    foreach (var hitbox in attacker.MoveExec.ActiveHitboxes)
                        foreach (var hurt in defender.Hurtboxes.ActiveBoxes)
                        {
                            if (CollisionBox.Overlaps(hitbox, attacker.transform, hurt, defender.transform))
                            {
                                // --- Deterministic hit reaction ---
                                Vector3 worldKnock = attacker.transform.TransformDirection(move.knockback);
                                defender.TakeHit(move, worldKnock);

                                // Optional recoil for attacker (small pushback)
                                attacker.ApplyRecoil(-attacker.transform.forward * Mathf.Abs(move.knockback.x) * 0.25f);

                                // Optional VFX/SFX triggers
                                if (move.vfxPrefab)
                                    Object.Instantiate(move.vfxPrefab, hitbox.ToWorld(attacker.transform).center, attacker.transform.rotation);
                                if (move.sfx)
                                    AudioSource.PlayClipAtPoint(move.sfx, hitbox.ToWorld(attacker.transform).center);

                                // Stop after first valid collision
                                return;
                            }
                        }
                }
        }
    }
}
