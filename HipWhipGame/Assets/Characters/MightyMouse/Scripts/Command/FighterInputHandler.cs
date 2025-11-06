using UnityEngine;

namespace HipWhipGame
{
    [RequireComponent(typeof(FighterComponentManager))]
    public class FighterInputHandler : MonoBehaviour, IFighterComponentInjectable
    {
        private FighterComponentManager fighterComponentManager;

        private MoveCommand moveCommand;
        private BlockCommand blockCommand;
        public void Inject(FighterComponentManager fighterComponentManager)
        {
            this.fighterComponentManager = fighterComponentManager;

            moveCommand = new MoveCommand(fighterComponentManager);
            blockCommand = new BlockCommand(fighterComponentManager);
        }

        public void OnMove(Vector2 updatedVector)
        {
            moveCommand.UpdateVectorInput(updatedVector);
        }

        public void HoldBlock() 
        {
            blockCommand.Execute();
        }

        public void ReleaseBlock() 
        {
            blockCommand.Release();
        }
    }
}