using UnityEngine;

namespace HipWhipGame
{
    public class ButtAttackHopKickCommand : ICommand
    {
        private FighterComponentManager fighterComponentManager;
        public string Name => "ButtAttackHopKick";
        public MoveData moveData;
        
        public ButtAttackHopKickCommand(FighterComponentManager fighterComponentManager, MoveData moveData)
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