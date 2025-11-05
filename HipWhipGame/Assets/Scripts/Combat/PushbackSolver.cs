/*
File Name:    PushResolver.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;

namespace HipWhipGame
{
    public static class PushResolver
    {
        /// <summary>
        /// Resolve overlap between two fighters' pushboxes using direct transform correction.
        /// </summary>
        public static void Resolve(FighterController a, FighterController b, Pushbox pushA, Pushbox pushB, float strength = 1f)
        {
            if (a == null || b == null || pushA == null || pushB == null)
                return;
            if (a == b)
                return;

            Vector3 aPos = pushA.WorldCenter;
            Vector3 bPos = pushB.WorldCenter;

            // Flat distance (XZ plane only)
            Vector3 delta = new Vector3(aPos.x - bPos.x, 0f, aPos.z - bPos.z);
            float dist = delta.magnitude;
            float minDist = pushA.Radius + pushB.Radius;

            // Overlapping?
            if (dist < minDist && dist > 0.0001f)
            {
                Vector3 n = delta / dist;
                float penetration = (minDist - dist);
                Vector3 push = n * (penetration * 0.5f * strength);

                // Direct transform correction (no CharacterController)
                a.transform.position += push;
                b.transform.position -= push;
            }
        }
    }
}
