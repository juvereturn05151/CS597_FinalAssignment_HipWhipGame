/*
File Name:    HitResolver.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;

namespace HipWhipGame
{
    public static class HitResolver
    {
        public static void Resolve(FighterController attacker, FighterController defender, MoveData move, Transform hitboxTransform)
        {
            if (attacker == null || defender == null || move == null) return;

            // Damage
            var dmg = defender.GetComponent<Damageable>();
            if (dmg) dmg.ApplyDamage(move.damage);

            // Hitstun
            defender.OnHit(move, attacker.GetComponent<MoveExecutor>().CurrentFrame);

            // Knockback
            Vector3 worldKnock = attacker.transform.TransformDirection(move.knockback);
            Debug.Log("[HitResolver] Knockback Applied: " + worldKnock + " | Magnitude: " + worldKnock.magnitude + " | Move: " + move.moveName);

            defender.ApplyKnockback(worldKnock, 1f);

            // Pushback on attacker (recoil)
            if (move.pushbackOnHit > 0f)
            {
                Vector3 recoilDir = -attacker.transform.forward * move.pushbackOnHit;
                Debug.Log("[HitResolver] Recoil Applied: " + recoilDir + " | Magnitude: " + recoilDir.magnitude);

                attacker.ApplyKnockback(recoilDir, 1f);
            }

            // FX/SFX
            if (move.vfxPrefab)
                Object.Instantiate(move.vfxPrefab, hitboxTransform.position, Quaternion.identity);

            if (move.sfx)
                AudioSource.PlayClipAtPoint(move.sfx, hitboxTransform.position);
        }
    }
}
