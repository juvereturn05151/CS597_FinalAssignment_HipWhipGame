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
        //[SerializeField]
        //private FighterController fighterController;
        //public FighterController FighterController => fighterController;
        [SerializeField]
        private Camera cam;
        public Camera Cam => cam;
        [SerializeField]
        private InputBuffer inputBuffer;
        public InputBuffer InputBuffer => inputBuffer;
        [SerializeField]
        private MoveExecutor moveExecutor;
        public MoveExecutor MoveExecutor => moveExecutor;
        //[SerializeField]
        //private FighterInputHandler fighterInputHandler;
        //public FighterInputHandler FighterInputHandler => fighterInputHandler;
        //[SerializeField]
        //private FighterGrabManager fighterGrabManager;
        //public FighterGrabManager FighterGrabManager => fighterGrabManager;

        [SerializeField]
        private Fighter fighter;
        public Fighter Fighter => fighter;

        void Awake()
        {
            if (!animator) animator = GetComponentInChildren<Animator>();
            if (!fighterStateMachine) fighterStateMachine = GetComponent<FighterStateMachine>();
            //if (!fighterController) fighterController = GetComponent<FighterController>();
            //if (!inputBuffer) inputBuffer = GetComponent<InputBuffer>();
            if (!cam) cam = GetComponentInChildren<Camera>();
            if (!moveExecutor) moveExecutor = GetComponent<MoveExecutor>();
            //if (!fighterInputHandler) fighterInputHandler = GetComponent<FighterInputHandler>();
            //if (!fighterGrabManager) fighterGrabManager = GetComponent<FighterGrabManager>();

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