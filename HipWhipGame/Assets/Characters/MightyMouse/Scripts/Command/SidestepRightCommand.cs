using UnityEngine;

namespace HipWhipGame
{
    public class SidestepRightCommand : ICommand
    {
        private FighterComponentManager fighterComponentManager;
        private MoveData moveData;
        public string Name => "SidestepRight";

        public SidestepRightCommand(FighterComponentManager fighterComponentManager, MoveData moveData)
        {
            this.fighterComponentManager = fighterComponentManager;
            this.moveData = moveData;
        }

        public void Pressed()
        {
            if (!fighterComponentManager.InputBuffer.Has(Name))
            {
                fighterComponentManager.InputBuffer.Push(Name);
            }

        }

        public void Release()
        {

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