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

        private int grabTimer; // counts frames deterministically

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
                grabbedOpponent = fighterComponentManager.Fighter.lookAtTarget.GetComponent<FighterComponentManager>();
            }

            fighterComponentManager.Fighter.SetIsMovable(false);
            grabbedOpponent.Fighter.SetIsMovable(false);

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
                fighterComponentManager.Fighter.SetIsMovable(true);
                fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Idle);
                grabbedOpponent.Fighter.SetIsMovable(true);
                grabbedOpponent.Fighter.TakeHit(grabData, grabData.knockback);
                grabbedOpponent = null;
            }
        }
    }
}