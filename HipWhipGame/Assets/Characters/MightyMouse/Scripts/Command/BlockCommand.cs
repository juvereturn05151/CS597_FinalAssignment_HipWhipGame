using UnityEngine;

namespace HipWhipGame
{
    public class BlockCommand
    {
        public string Name => "Block";
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