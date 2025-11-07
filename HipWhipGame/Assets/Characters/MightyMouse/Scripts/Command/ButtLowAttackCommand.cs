using UnityEngine;

namespace HipWhipGame
{
    public class ButtLowAttackCommand : ICommand
    {
        public string Name => "ButtLowAttack";
        private FighterComponentManager fighterComponentManager;
        public ButtLowAttackCommand(FighterComponentManager fighterComponentManager)
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