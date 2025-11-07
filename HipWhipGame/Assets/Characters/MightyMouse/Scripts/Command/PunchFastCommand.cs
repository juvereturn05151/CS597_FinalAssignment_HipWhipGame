using UnityEngine;

namespace HipWhipGame
{
    public class PunchFastCommand : ICommand
    {
        public string Name => "PunchFast";
        private FighterComponentManager fighterComponentManager;
        public PunchFastCommand(FighterComponentManager fighterComponentManager)
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