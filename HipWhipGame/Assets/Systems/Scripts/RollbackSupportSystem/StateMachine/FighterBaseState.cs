/*
File Name:    FighterBaseState.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;

namespace RollbackSupport
{
    public abstract class FighterBaseState
    {
        protected FighterComponentManager fighterComponentManager;
        protected FighterStateMachine stateMachine;

        protected FighterBaseState(FighterComponentManager fighterComponentManager)
        {
            this.fighterComponentManager = fighterComponentManager;
            this.stateMachine = fighterComponentManager.FighterStateMachine;
        }

        /// <summary> Called when entering this state. </summary>
        public abstract void OnEnter();

        /// <summary> Called every frame while in this state. </summary>
        public abstract void OnUpdate();

        /// <summary> Called when exiting this state. </summary>
        public abstract void OnExit();
    }
}
