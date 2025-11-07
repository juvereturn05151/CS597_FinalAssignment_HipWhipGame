using UnityEngine;

namespace HipWhipGame
{
    public class ButtAttackHopKickCommand : ICommand
    {
        public string Name => "ButtAttackHopKick";
        private FighterComponentManager fighterComponentManager;
        public ButtAttackHopKickCommand(FighterComponentManager fighterComponentManager)
        {
            this.fighterComponentManager = fighterComponentManager;
        }
        public void Execute()
        {
            fighterComponentManager.InputBuffer.Push(Name);
        }

        public void Release()
        {

        }

        public void UpdateVectorInput(Vector2 updatedVector)
        {
            
        }
    }
}