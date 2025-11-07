using UnityEngine;

namespace HipWhipGame
{
    public class ButtAttackHopKickCommand : ICommand
    {
        private FighterComponentManager fighterComponentManager;
        public ButtAttackHopKickCommand(FighterComponentManager fighterComponentManager)
        {
            this.fighterComponentManager = fighterComponentManager;
        }
        public void Execute()
        {
            fighterComponentManager.InputBuffer.Push("ButtAttackHopKick");
        }

        public void Release()
        {

        }

        public void UpdateVectorInput(Vector2 updatedVector)
        {
            
        }
    }
}