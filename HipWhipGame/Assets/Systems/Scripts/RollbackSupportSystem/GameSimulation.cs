/*
File Name:    GameSimulation.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;

namespace RollbackSupport
{
    public class GameSimulation : MonoBehaviour
    {
        private FighterController fighter1;
        private FighterController fighter2;
        public RollbackManager rollback = new RollbackManager();
        public int FrameNumber { get; private set; }

        public void Initialize(FighterController fighter1, FighterController fighter2)
        {
            this.fighter1 = fighter1;
            this.fighter2 = fighter2;

            this.fighter1.Initialize();
            this.fighter2.Initialize();

            PhysicsWorld.Instance.Register(this.fighter1.body);
            PhysicsWorld.Instance.Register(this.fighter2.body);
            PushboxManager.Instance.Register(this.fighter1);
            PushboxManager.Instance.Register(this.fighter2);
            HitboxManager.Instance.Register(this.fighter1);
            HitboxManager.Instance.Register(this.fighter2);
        }

        public void Step()
        {
            FrameNumber++;
            fighter1.SimulateFrame();
            fighter1.FighterComponentManager.FighterStateMachine.Step();
            fighter2.SimulateFrame();
            fighter2.FighterComponentManager.FighterStateMachine.Step();

            PhysicsWorld.Instance.Step();
            PushboxManager.Instance.ResolvePush();
            HitboxManager.Instance.CheckHits();

            fighter1.AnimatorSync.ApplyVisuals();
            fighter2.AnimatorSync.ApplyVisuals();

            rollback.Push(FrameNumber, GameStateSnapshot.Capture(FrameNumber, fighter1, fighter2));
        }

        public void RollbackTo(int frame)
        {
            if (rollback.TryGetSnapshot(frame, out var snap))
            {
                snap.Restore(fighter1, fighter2);
                FrameNumber = frame;
            }
        }
    }
}