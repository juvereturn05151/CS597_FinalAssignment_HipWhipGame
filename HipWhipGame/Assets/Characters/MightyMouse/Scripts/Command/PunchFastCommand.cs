using UnityEngine;

namespace HipWhipGame
{
    public class PunchFastCommand : ICommand
    {
        private FighterComponentManager fighterComponentManager;
        public PunchFastCommand(FighterComponentManager fighterComponentManager)
        {
            this.fighterComponentManager = fighterComponentManager;
        }
        public void Execute()
        {
            fighterComponentManager.InputBuffer.Push("PunchFast");
        }

        public void Release()
        {

        }

        public void UpdateVectorInput(Vector2 updatedVector)
        {
            
        }
    }
}