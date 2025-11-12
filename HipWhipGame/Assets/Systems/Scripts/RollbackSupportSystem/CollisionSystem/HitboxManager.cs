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
                    var a = fighters[i];
                    var b = fighters[j];

                    // Check a attacking b
                    CheckCollisionPair(a, b);
                    // Check b attacking a
                    CheckCollisionPair(b, a);
                }
            }

        }

        private void CheckCollisionPair(FighterComponentManager attacker, FighterComponentManager defender)
        {
            if (!attacker.MoveExecutor.IsExecuting)
                return;

            var move = attacker.MoveExecutor.CurrentMove;
            if (move == null)
                return;

            foreach (var hitbox in attacker.MoveExecutor.ActiveHitboxes)
            {
                foreach (var hurt in defender.FighterCollisionComponent.Hurtboxes.ActiveBoxes)
                {
                    if (CollisionBox.Overlaps(hitbox, attacker.transform, hurt, defender.transform))
                    {
                        Debug.Log($"Hit detected! Attacker: {attacker.name}, Defender: {defender.name}, Move: {move.moveName}");

                        Vector3 worldKnock = attacker.transform.TransformDirection(move.knockback);

                        if (defender.FighterController.IsBlocking())
                            defender.FighterController.TakeBlock(move, worldKnock);
                        else
                            defender.FighterController.TakeHit(move, worldKnock);

                        return;
                    }
                }
            }
        }

    }
}
