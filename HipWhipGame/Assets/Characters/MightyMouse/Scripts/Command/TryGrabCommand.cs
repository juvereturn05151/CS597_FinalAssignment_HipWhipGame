using UnityEngine;

namespace HipWhipGame
{
    public class TryGrabCommand : ICommand
    {
        private FighterComponentManager fighterComponentManager;
        private MoveData moveData;
        public string Name => "TryGrab";

        public TryGrabCommand(FighterComponentManager fighterComponentManager, MoveData moveData)
        {
            this.fighterComponentManager = fighterComponentManager;
            this.moveData = moveData;
        }

        public void Pressed()
        {
            fighterComponentManager.InputBuffer.Push(Name);
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