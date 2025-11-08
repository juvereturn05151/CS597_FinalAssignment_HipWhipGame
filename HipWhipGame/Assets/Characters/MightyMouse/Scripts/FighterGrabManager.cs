using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace HipWhipGame
{
    [RequireComponent(typeof(FighterComponentManager))]
    public class FighterGrabManager : MonoBehaviour, IFighterComponentInjectable
    {
        private FighterComponentManager fighterComponentManager;

        private FighterComponentManager grabbedOpponent;

        private MoveData grabData;

        private float currentGrabTime;

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

            grabbedOpponent.FighterStateMachine.SwitchState(Enums.FighterState.BeingGrabbed);

            currentGrabTime = grabData.grabDuration;
        }

        public void UpdateGrab() 
        {
            if (grabbedOpponent != null) 
            {
                currentGrabTime -= Time.deltaTime;
                if (currentGrabTime <= 0f) 
                {
                    ReleaseGrab();
                }
            }
        }

        public void ReleaseGrab() 
        {
            if (grabbedOpponent != null) 
            {
                fighterComponentManager.FighterController.SetIsMovable(true);
                fighterComponentManager.FighterStateMachine.SwitchState(Enums.FighterState.Idle);
                grabbedOpponent.FighterController.SetIsMovable(true);
                grabbedOpponent.FighterController.ApplyHitstun(grabData.hitstunFrames);
                Vector3 worldKnock = fighterComponentManager.transform.TransformDirection(grabData.knockback);

                grabbedOpponent.FighterController.ApplyKnockback(worldKnock, 1f);
                grabbedOpponent = null;
            }
        }
    }
}