using RollbackSupport;
using UnityEngine;

namespace RollbackSupport
{
    [CreateAssetMenu(menuName = "Fighter/MoveDatabase")]
    public class MoveDatabase : ScriptableObject
    {
        public MoveData light, heavy, grab, sideStepLeft, sideStepRight;

        public MoveData Get(string name)
        {
            if (light && light.moveName == name) return light;
            if (heavy && heavy.moveName == name) return heavy;
            if (grab && grab.moveName == name) return grab;
            if (sideStepLeft && sideStepLeft.moveName == name) return sideStepLeft;
            if (sideStepRight && sideStepRight.moveName == name) return sideStepRight;

            return null;
        }
    }
}