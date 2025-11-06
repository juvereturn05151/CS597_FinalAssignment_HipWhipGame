using UnityEngine;

namespace HipWhipGame
{
    public class MoveCommand : ICommand
    {
        private FighterComponentManager fighterComponentManager;
        public MoveCommand(FighterComponentManager fighterComponentManager) 
        { 
            this.fighterComponentManager = fighterComponentManager; 
        }
        public void Execute() 
        {
            
        }

        public void Release()
        {

        }

        public void UpdateVectorInput(Vector2 updatedVector)
        {
            fighterComponentManager.FighterController.UpdateMovementInput(updatedVector);
        }
    }
}