using RollbackSupport;
using UnityEngine;

namespace RollbackSupport
{
    [CreateAssetMenu(menuName = "Fighter/MoveDatabase")]
    public class MoveDatabase : ScriptableObject
    {
        public MoveData light, heavy, grab;
        public CollisionBox[] hurtboxes;

        public MoveData Get(string name)
        {
            if (light && light.moveName == name) return light;
            if (heavy && heavy.moveName == name) return heavy;
            if (grab && grab.moveName == name) return grab;
            return null;
        }
    }
}