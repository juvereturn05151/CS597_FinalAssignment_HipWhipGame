/*
File Name:    FighterComponentManager.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;

namespace HipWhipGame 
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
        private FighterController fighterController;
        public FighterController FighterController => fighterController;
        [SerializeField]
        private Camera cam;
        public Camera Cam => cam;
        [SerializeField]
        private InputBuffer inputBuffer;
        public InputBuffer InputBuffer => inputBuffer;

        void Awake()
        {
            if (!animator) animator = GetComponentInChildren<Animator>();
            if (!fighterStateMachine) fighterStateMachine = GetComponent<FighterStateMachine>();
            if (!fighterController) fighterController = GetComponent<FighterController>();
            if (!inputBuffer) inputBuffer = GetComponent<InputBuffer>();
            if (!cam) cam = GetComponentInChildren<Camera>();

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