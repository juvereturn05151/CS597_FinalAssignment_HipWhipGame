using UnityEngine;

namespace HipWhipGame
{
    public class ButtAttackMidPokeCommand : ICommand
    {
        private FighterComponentManager fighterComponentManager;
        public ButtAttackMidPokeCommand(FighterComponentManager fighterComponentManager)
        {
            this.fighterComponentManager = fighterComponentManager;
        }
        public void Execute()
        {
            fighterComponentManager.InputBuffer.Push("ButtAttackMidPoke");
        }

        public void Release()
        {

        }

        public void UpdateVectorInput(Vector2 updatedVector)
        {
            
        }
    }
}