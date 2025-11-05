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
            defender.ApplyHitstun(move.hitstunFrames);

            // Correct knockback calculation
            Vector3 worldKnock = attacker.transform.TransformDirection(move.knockback);
            defender.ApplyKnockback(worldKnock, 1f);

            // Pushback on attacker (recoil)
            if (move.pushbackOnHit > 0f)
            {
                Vector3 recoilDir = -attacker.transform.forward * move.pushbackOnHit;
                attacker.ApplyKnockback(recoilDir, 1f);
            }

            float advantage = move.hitstunFrames - move.recovery;
            Debug.Log($"{move.moveName}: {attacker.name} is {(advantage >= 0 ? "+" : "")}{advantage} on hit.");

            // FX/SFX
            if (move.vfxPrefab)
                Object.Instantiate(move.vfxPrefab, hitboxTransform.position, Quaternion.identity);

            if (move.sfx)
                AudioSource.PlayClipAtPoint(move.sfx, hitboxTransform.position);
        }
    }
}