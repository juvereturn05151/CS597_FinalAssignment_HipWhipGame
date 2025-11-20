/*
File Name:    PushboxManager.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using System.Collections.Generic;
using UnityEngine;

namespace RollbackSupport
{
    public class PushboxManager
    {
        public static readonly PushboxManager Instance = new PushboxManager();

        private readonly List<FighterComponentManager> fighters = new List<FighterComponentManager>();

        public void Register(FighterComponentManager f)
        {
            if (!fighters.Contains(f))
            {
                fighters.Add(f);
            }
        }

        public void Clear()
        {
            fighters.Clear();
        }

        public void ResolvePush()
        {
            if (fighters.Count < 2) return;

            for (int i = 0; i < fighters.Count; i++)
            {
                var a = fighters[i];
                for (int j = i + 1; j < fighters.Count; j++)
                {
                    var b = fighters[j];
                    if (TryResolvePair(a, b))
                    {
                        // you can break here or let it continue depending on how many overlaps you expect
                        // break;
                    }
                }
            }
        }

        private bool TryResolvePair(FighterComponentManager a, FighterComponentManager b)
        {
            var ac = a.FighterCollisionComponent.PushCapsule;
            var bc = b.FighterCollisionComponent.PushCapsule;

            if (!ac.enabled || !bc.enabled)
                return false;

            Vector3 centerA = ac.GetWorldCenter(a.transform);
            Vector3 centerB = bc.GetWorldCenter(b.transform);

            // work on XZ plane only
            Vector3 delta = centerA - centerB;
            delta.y = 0f;

            float distSq = delta.sqrMagnitude;
            if (distSq <= Mathf.Epsilon)
            {
                // centers are basically on top of each other; choose a deterministic direction
                delta = new Vector3(1f, 0f, 0f);
                distSq = 1f;
            }

            float radiusSum = ac.radius + bc.radius;
            float radiusSumSq = radiusSum * radiusSum;

            if (distSq >= radiusSumSq)
                return false; // no overlap

            float dist = Mathf.Sqrt(distSq);
            float penetration = radiusSum - dist;

            // normalized direction from B to A
            Vector3 dir = delta / dist;

            // split correction equally
            Vector3 correction = dir * (penetration * 0.5f);

            a.FighterController.body.position += correction;
            b.FighterController.body.position -= correction;

            a.FighterCollisionComponent.IsPushedThisFrame = true;
            b.FighterCollisionComponent.IsPushedThisFrame = true;

            return true;
        }
    }
}
