using UnityEngine;

namespace HipWhipGame
{
    public class BlockCommand
    {
        private FighterComponentManager fighterComponentManager;
        public BlockCommand(FighterComponentManager fighterComponentManager)
        {
            this.fighterComponentManager = fighterComponentManager;
        }
        public void Execute()
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
    }
}