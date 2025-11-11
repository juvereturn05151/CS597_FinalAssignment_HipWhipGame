using HipWhipGame;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace RollbackSupport
{
    [RequireComponent(typeof(FighterComponentManager))]
    public class FighterGrabManager : MonoBehaviour, IFighterComponentInjectable
    {
        private FighterComponentManager fighterComponentManager;

        private FighterComponentManager grabbedOpponent;

        private MoveData grabData;

        private int grabTimer;

        public bool IsGrabbing => grabTimer > 0;

        public void Inject(FighterComponentManager fighterComponentManager)
        {
            this.fighterComponentManager = fighterComponentManager;
        }

        public void SetUpGrabData(MoveData grabData) 
        {
            this.grabData = grabData;
        }

        public void Grab()
        {
            if (grabbedOpponent == null) 
            {
                grabbedOpponent = fighterComponentManager.FighterController.lookAtTarget.GetComponent<FighterComponentManager>();
            }

            fighterComponentManager.FighterController.SetIsMovable(false);
            grabbedOpponent.FighterController.SetIsMovable(false);

            grabbedOpponent.FighterStateMachine.SwitchState(FighterState.BeingGrabbed);

            grabTimer = grabData.grabDuration;
        }

        public void UpdateGrab() 
        {
            if (grabTimer <= 0)
                return;

            grabTimer--;

            if (grabTimer <= 0)
            {
                ReleaseGrab();
            }
        }

        public void ReleaseGrab() 
        {
            if (grabbedOpponent != null) 
            {
                fighterComponentManager.FighterController.SetIsMovable(true);
                fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Idle);
                grabbedOpponent.FighterController.SetIsMovable(true);
                grabbedOpponent.FighterController.TakeHit(grabData, grabData.knockback);
                grabbedOpponent = null;
            }
        }
    }
}