using UnityEngine;

namespace RollbackSupport
{
    [System.Serializable]
    public struct CollisionBox
    {
        public Vector3 localCenter;
        public Vector3 size;
        public bool enabled;

        public Bounds ToWorld(Transform t)
        {
            Vector3 c = t.TransformPoint(localCenter);
            return new Bounds(c, size);
        }

        public static bool Overlaps(CollisionBox a, Transform ta, CollisionBox b, Transform tb)
        {
            if (!a.enabled || !b.enabled) return false;
            return a.ToWorld(ta).Intersects(b.ToWorld(tb));
        }
    }
}