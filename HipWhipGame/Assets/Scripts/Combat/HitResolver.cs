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

            // Knockback direction: use attacker forward
            Vector3 dir = attacker.transform.forward.normalized;
            Vector3 worldKnock = new Vector3(move.knockback.x * dir.x,
                                             move.knockback.y,
                                             move.knockback.z * dir.z);
            defender.ApplyKnockback(worldKnock, 1f);

            // Pushback on attacker (recoil forward/back)
            if (move.pushbackOnHit > 0f)
            {
                attacker.ApplyKnockback(-dir * move.pushbackOnHit, 1f);
            }

            // Optional FX/SFX
            if (move.vfxPrefab) Object.Instantiate(move.vfxPrefab, hitboxTransform.position, Quaternion.identity);
            if (move.sfx) AudioSource.PlayClipAtPoint(move.sfx, hitboxTransform.position);
        }
    }
}