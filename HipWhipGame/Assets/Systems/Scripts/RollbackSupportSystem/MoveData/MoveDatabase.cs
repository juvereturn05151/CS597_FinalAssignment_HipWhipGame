using RollbackSupport;
using UnityEngine;

namespace RollbackSupport
{
    [CreateAssetMenu(menuName = "Fighter/MoveDatabase")]
    public class MoveDatabase : ScriptableObject
    {
        public MoveData light, heavy, grab, superButt, sideStepLeft, sideStepRight, combo_light, combo_light_ender;
        public MoveData specialFart;     
        public MoveData ultimateButt;

        public MoveData Get(string name)
        {
            if (light && light.moveName == name) return light;
            if (combo_light && combo_light.moveName == name) return combo_light;
            if (combo_light_ender && combo_light_ender.moveName == name) return combo_light_ender;
            if (heavy && heavy.moveName == name) return heavy;
            if (grab && grab.moveName == name) return grab;
            if (sideStepLeft && sideStepLeft.moveName == name) return sideStepLeft;
            if (sideStepRight && sideStepRight.moveName == name) return sideStepRight;
            if (superButt && superButt.moveName == name) return superButt;
            if (specialFart && specialFart.moveName == name) return specialFart;
            if (ultimateButt && ultimateButt.moveName == name) return ultimateButt;
            return null;
        }
    }
}