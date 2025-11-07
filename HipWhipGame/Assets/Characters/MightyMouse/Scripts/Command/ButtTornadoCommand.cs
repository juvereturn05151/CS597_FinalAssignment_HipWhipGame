using UnityEngine;

namespace HipWhipGame
{
    public class ButtTornadoCommand : ICommand
    {
        public string Name => "ButtTornado";
        private FighterComponentManager fighterComponentManager;
        public ButtTornadoCommand(FighterComponentManager fighterComponentManager)
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