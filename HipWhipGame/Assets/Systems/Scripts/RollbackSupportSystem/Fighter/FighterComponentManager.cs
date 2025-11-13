/*
File Name:    FighterComponentManager.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/


using UnityEngine;

namespace RollbackSupport
{
    public class FighterComponentManager : MonoBehaviour
    {
        [SerializeField]
        private FighterController fighterController;
        public FighterController FighterController => fighterController;
        [SerializeField]
        private FighterCollisionComponent fighterCollisionComponent;
        public FighterCollisionComponent FighterCollisionComponent => fighterCollisionComponent;
        [SerializeField]
        private FighterStateMachine fighterStateMachine;
        public FighterStateMachine FighterStateMachine => fighterStateMachine;
        [SerializeField]
        private MoveExecutor moveExecutor;
        public MoveExecutor MoveExecutor => moveExecutor;
        [SerializeField]
        private Animator animator;
        public Animator Animator => animator;
        [SerializeField]
        private DeterministicAnimator deterministicAnimator;
        public DeterministicAnimator DeterministicAnimator => deterministicAnimator;
        [SerializeField]
        private Camera cam;
        public Camera Cam => cam;
        [SerializeField]
        private FighterGrabManager fighterGrabManager;
        public FighterGrabManager FighterGrabManager => fighterGrabManager;
        private Vector3 spawnPoint;
        public void SetSpawnPoint(Vector3 spawnPoint) 
        {
            this.spawnPoint = spawnPoint;
        }

        private void Awake()
        {
            if (!fighterController) fighterController = GetComponent<FighterController>();
            if (!fighterCollisionComponent) fighterCollisionComponent = GetComponent<FighterCollisionComponent>(); 
            if (!fighterStateMachine) fighterStateMachine = GetComponent<FighterStateMachine>();
            if (!moveExecutor) moveExecutor = GetComponent<MoveExecutor>();
            if (!animator) animator = GetComponentInChildren<Animator>();
            if (!deterministicAnimator) deterministicAnimator = GetComponent<DeterministicAnimator>();
            if (!cam) cam = GetComponentInChildren<Camera>();
            if (!fighterGrabManager) fighterGrabManager = GetComponent<FighterGrabManager>();

            foreach (var injectable in GetComponents<IFighterComponentInjectable>())
            {
                injectable.Inject(this);
            }
        }

        public void OnUpdate()
        {
            fighterController.SimulateFrame();
            fighterStateMachine.Step();
        }

        public void ResetStateForRespawn()
        {
            fighterController.body.velocity = Vector3.zero;
            fighterController.body.position = spawnPoint;
            fighterStateMachine.SwitchState(FighterState.Idle);
        }

        public void ResetForReplay() 
        {
            fighterController.body.velocity = Vector3.zero;
            fighterStateMachine.SwitchState(FighterState.Idle);
        }

        public T Require<T>() where T : Component
        {
            var comp = GetComponent<T>();
            if (!comp)
                Debug.LogError($"[{name}] Missing required component: {typeof(T).Name}");
            return comp;
        }
    }
}