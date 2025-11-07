using UnityEngine;

namespace HipWhipGame
{
    public class ButtTornadoCommand : ICommand
    {
        private FighterComponentManager fighterComponentManager;
        public ButtTornadoCommand(FighterComponentManager fighterComponentManager)
        {
            this.fighterComponentManager = fighterComponentManager;
        }
        public void Execute()
        {
            fighterComponentManager.InputBuffer.Push("ButtTornado");
        }

        public void Release()
        {

        }

        public void UpdateVectorInput(Vector2 updatedVector)
        {
            
        }
    }
}