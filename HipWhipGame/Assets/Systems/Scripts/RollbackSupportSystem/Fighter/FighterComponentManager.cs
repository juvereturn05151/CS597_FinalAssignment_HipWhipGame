using UnityEngine;

namespace RollbackSupport
{
    public class FighterComponentManager : MonoBehaviour
    {
        [SerializeField]
        private Animator animator;
        public Animator Animator => animator;
        [SerializeField]
        private FighterStateMachine fighterStateMachine;
        public FighterStateMachine FighterStateMachine => fighterStateMachine;
        [SerializeField]
        private Camera cam;
        public Camera Cam => cam;
        [SerializeField]
        private InputBuffer inputBuffer;
        public InputBuffer InputBuffer => inputBuffer;
        [SerializeField]
        private MoveExecutor moveExecutor;
        public MoveExecutor MoveExecutor => moveExecutor;
        [SerializeField]
        private FighterGrabManager fighterGrabManager;
        public FighterGrabManager FighterGrabManager => fighterGrabManager;

        [SerializeField]
        private Fighter fighter;
        public Fighter Fighter => fighter;

        void Awake()
        {
            if (!animator) animator = GetComponentInChildren<Animator>();
            if (!fighterStateMachine) fighterStateMachine = GetComponent<FighterStateMachine>();
            if (!cam) cam = GetComponentInChildren<Camera>();
            if (!moveExecutor) moveExecutor = GetComponent<MoveExecutor>();
            if (!fighterGrabManager) fighterGrabManager = GetComponent<FighterGrabManager>();
            if (!fighter) fighter = GetComponent<Fighter>();

            foreach (var injectable in GetComponents<IFighterComponentInjectable>())
            {
                injectable.Inject(this);
            }
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