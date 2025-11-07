using UnityEngine;

namespace HipWhipGame
{
    public class ButtLowAttackCommand : ICommand
    {
        private FighterComponentManager fighterComponentManager;
        public ButtLowAttackCommand(FighterComponentManager fighterComponentManager)
        {
            this.fighterComponentManager = fighterComponentManager;
        }
        public void Execute()
        {
            fighterComponentManager.InputBuffer.Push("ButtLowAttack");
        }

        public void Release()
        {

        }

        public void UpdateVectorInput(Vector2 updatedVector)
        {
            
        }
    }
}