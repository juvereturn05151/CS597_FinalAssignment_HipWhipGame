using UnityEngine;

namespace HipWhipGame
{
    [RequireComponent(typeof(FighterComponentManager))]
    public class FighterInputHandler : MonoBehaviour, IFighterComponentInjectable
    {
        [SerializeField]
        private MoveDatabase moves;

        private FighterComponentManager fighterComponentManager;

        private MoveCommand moveCommand;
        private BlockCommand blockCommand;
        private PunchFastCommand punchFastCommand;
        private ButtAttackHopKickCommand buttAttackHopKickCommand;
        private ButtAttackMidPokeCommand buttAttackMidPokeCommand;
        private ButtLowAttackCommand buttLowAttackCommand;
        private ButtTornadoCommand buttTornadoCommand;

        public void Inject(FighterComponentManager fighterComponentManager)
        {
            this.fighterComponentManager = fighterComponentManager;

            moveCommand = new MoveCommand(fighterComponentManager);
            blockCommand = new BlockCommand(fighterComponentManager);
            punchFastCommand = new PunchFastCommand(fighterComponentManager);
            buttAttackHopKickCommand = new ButtAttackHopKickCommand(fighterComponentManager);
            buttAttackMidPokeCommand = new ButtAttackMidPokeCommand(fighterComponentManager);
            buttLowAttackCommand = new ButtLowAttackCommand(fighterComponentManager);
            buttTornadoCommand = new ButtTornadoCommand(fighterComponentManager);
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

        public void PerformPunchFast() 
        {
            punchFastCommand.Execute();
        }

        public void PerformButtAttackHopKick()
        {
            buttAttackHopKickCommand.Execute();
        }

        public void PerformButtAttackMidPoke()
        {
            buttAttackMidPokeCommand.Execute();
        }

        public void PerformButtLowAttack()
        {
            buttLowAttackCommand.Execute();
        }

        public void PerformButtTornado()
        {
            buttTornadoCommand.Execute();
        }
    }
}