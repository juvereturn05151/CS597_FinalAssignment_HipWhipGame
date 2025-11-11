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
                        break;
                    }
                }
            }
        }

        private bool TryResolvePair(FighterComponentManager a, FighterComponentManager b)
        {
            if (!a.FighterCollisionComponent.Pushbox.enabled || !b.FighterCollisionComponent.Pushbox.enabled) 
            {
                return false;
            }
                
            Bounds A = a.FighterCollisionComponent.Pushbox.ToWorld(a.transform);
            Bounds B = b.FighterCollisionComponent.Pushbox.ToWorld(b.transform);

            if (!A.Intersects(B))
                return false;

            // Compute overlap
            Vector3 overlap = Vector3.zero;
            overlap.x = (A.extents.x + B.extents.x) - Mathf.Abs(A.center.x - B.center.x);
            overlap.z = (A.extents.z + B.extents.z) - Mathf.Abs(A.center.z - B.center.z);

            // Ignore Y overlap (we don't push vertically)
            if (overlap.x <= 0f && overlap.z <= 0f)
                return false;

            // Decide push direction (X dominant for 2.5D, otherwise both)
            Vector3 dir = (A.center - B.center).normalized;
            if (Mathf.Abs(dir.x) >= Mathf.Abs(dir.z))
                dir = new Vector3(Mathf.Sign(dir.x), 0, 0);
            else
                dir = new Vector3(0, 0, Mathf.Sign(dir.z));

            Vector3 correction = new Vector3(overlap.x * dir.x * 0.5f, 0, overlap.z * dir.z * 0.5f);

            // Apply push equally
            a.FighterController.body.position += correction;
            b.FighterController.body.position -= correction;

            a.FighterCollisionComponent.IsPushedThisFrame = b.FighterCollisionComponent.IsPushedThisFrame = true;
            return true;
        }
    }
}
