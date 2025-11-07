using UnityEngine;

namespace HipWhipGame
{
    public class BlockCommand : ICommand
    {
        private FighterComponentManager fighterComponentManager;
        private MoveData moveData;
        public string Name => "Block";

        public BlockCommand(FighterComponentManager fighterComponentManager, MoveData moveData)
        {
            this.fighterComponentManager = fighterComponentManager;
            this.moveData = moveData;
        }

        public void Pressed()
        {
            fighterComponentManager.FighterController.SetIsBlocking(true);
        }

        public void Release()
        {
            fighterComponentManager.FighterController.SetIsBlocking(false);
        }

        public void UpdateVectorInput(Vector2 updatedVector)
        {
            
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