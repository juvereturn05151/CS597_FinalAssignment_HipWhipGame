using System.Collections.Generic;
using UnityEngine;

namespace HipWhipGame
{
    [RequireComponent(typeof(FighterComponentManager))]
    public class FighterInputHandler : MonoBehaviour, IFighterComponentInjectable
    {
        [SerializeField] private MoveDatabase moves;

        private FighterComponentManager fighterComponentManager;

        // Individual references (optional, for direct access)
        private MoveCommand moveCommand;
        private BlockCommand blockCommand;

        // Unified command list
        private readonly List<ICommand> _moveCommands = new();

        public void Inject(FighterComponentManager fighterComponentManager)
        {
            this.fighterComponentManager = fighterComponentManager;

            // Initialize movement & block
            moveCommand = new MoveCommand(fighterComponentManager, null);
            blockCommand = new BlockCommand(fighterComponentManager, null);

            // Create and register all move commands
            _moveCommands.Add(new PunchFastCommand(fighterComponentManager, moves.punchFast));
            _moveCommands.Add(new ButtAttackHopKickCommand(fighterComponentManager, moves.buttAttackHopKick));
            _moveCommands.Add(new ButtAttackMidPokeCommand(fighterComponentManager, moves.buttAttackMidPoke));
            _moveCommands.Add(new ButtLowAttackCommand(fighterComponentManager, moves.buttLowAttack));
            _moveCommands.Add(new ButtTornadoCommand(fighterComponentManager, moves.buttTornado));
            _moveCommands.Add(new SidestepLeftCommand(fighterComponentManager, moves.sideStepLeft));
            _moveCommands.Add(new SidestepRightCommand(fighterComponentManager, moves.sideStepRight));
        }

        public void OnMove(Vector2 updatedVector)
        {
            moveCommand.UpdateVectorInput(updatedVector);
        }

        public void OnRightStick(Vector2 updatedVector)
        {
            if (updatedVector.x >= 0.1f) 
            {
                TryPressCommand<SidestepRightCommand>();
            }
            else if (updatedVector.x <= -0.1f)
            {
                TryPressCommand<SidestepLeftCommand>();
            }
        }

        public void HoldBlock() => blockCommand.Pressed();
        public void ReleaseBlock() => blockCommand.Release();

        // These can directly call Pressed() if needed
        public void PerformPunchFast() => TryPressCommand<PunchFastCommand>();
        public void PerformButtAttackHopKick() => TryPressCommand<ButtAttackHopKickCommand>();
        public void PerformButtAttackMidPoke() => TryPressCommand<ButtAttackMidPokeCommand>();
        public void PerformButtLowAttack() => TryPressCommand<ButtLowAttackCommand>();
        public void PerformButtTornado() => TryPressCommand<ButtTornadoCommand>();

        private void TryPressCommand<T>() where T : ICommand
        {
            foreach (var cmd in _moveCommands)
            {
                if (cmd is T)
                {
                    cmd.Pressed();
                    break;
                }
            }
        }

        public void TryStartMove()
        {
            if (!fighterComponentManager.FighterStateMachine.CanStartMove())
                return;

            // Loop through all move commands
            foreach (var command in _moveCommands)
            {
                // Only execute the first valid one
                if ((command).TryExecute())
                {
                    (command).TryStartMove();
                    return;
                }
            }
        }
    }
}