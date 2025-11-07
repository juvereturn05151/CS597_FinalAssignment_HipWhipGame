using UnityEngine;

namespace HipWhipGame
{
    public class MoveCommand : ICommand
    {
        private FighterComponentManager fighterComponentManager;
        private MoveData moveData;
        public string Name => "Move";

        public MoveCommand(FighterComponentManager fighterComponentManager, MoveData moveData) 
        { 
            this.fighterComponentManager = fighterComponentManager; 
            this.moveData = moveData;
        }

        public void Pressed() 
        {
            
        }

        public void Release()
        {

        }

        public void UpdateVectorInput(Vector2 updatedVector)
        {
            fighterComponentManager.FighterController.UpdateMovementInput(updatedVector);
        }

        public bool TryExecute()
        {
            return fighterComponentManager.InputBuffer.Consume(Name) && moveData;
        }

        public void TryStartMove()
        {
            fighterComponentManager.MoveExecutor.PlayMove(moveData);
        }
    }
}