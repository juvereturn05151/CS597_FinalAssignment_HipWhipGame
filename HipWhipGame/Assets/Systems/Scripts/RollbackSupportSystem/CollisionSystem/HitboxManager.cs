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
                        // ---- FRAME ADVANTAGE CALC ----
                        int totalFrames = move.startup + move.active + move.recovery;
                        int currentFrame = attacker.MoveExecutor.CurrentFrame;
                        int remainingRecovery = Mathf.Max(0, totalFrames - currentFrame);

                        int advOnHit = move.hitstunFrames - remainingRecovery;
                        int advOnBlock = move.blockstunFrames - remainingRecovery;

                        Debug.Log(
                            $"[Frame Advantage] Move: {move.name} | " +
                            $"Hit: {(advOnHit >= 0 ? "+" : "")}{advOnHit} | " +
                            $"Block: {(advOnBlock >= 0 ? "+" : "")}{advOnBlock}"
                        );
                        // --------------------------------

                        Debug.Log($"Actual Object {defender.name} with {defender.transform.rotation}");
                        Debug.Log($"Global {defender.name} with {defender.Animator.transform.rotation}");
                        Debug.Log($"Local {defender.name} with {defender.Animator.transform.localRotation}");

                        Vector3 worldKnock = defender.transform.TransformDirection(move.knockback);

                        if (defender.FighterController.IsBlocking())
                        {
                            attacker.FighterController.AddMeter(move.meterGainOnBlock);
                            defender.FighterController.TakeBlock(move, -worldKnock);
                            defender.FighterController.AddMeter(move.meterGainWhenBlocked);
                        }
                        else 
                        {
                            Vector3 hitPos = GetHitPosition(hitbox, attacker.transform, hurt, defender.transform);
                            JuiceController.Instance.Play("Impact1", hitPos);
                            attacker.FighterController.AddMeter(move.meterGainOnHit);
                            defender.FighterController.TakeHit(move, -worldKnock);
                        }
                            

                        return;
                    }
                }
            }
        }

        public static Vector3 GetHitPosition(CollisionBox hit, Transform attackerT, CollisionBox hurt, Transform defenderT)
        {
            Bounds hb = hit.ToWorld(attackerT);
            Bounds hburt = hurt.ToWorld(defenderT);

            // midpoint between the two volumes
            return (hb.center + hburt.center) * 0.5f;
        }

    }
}
