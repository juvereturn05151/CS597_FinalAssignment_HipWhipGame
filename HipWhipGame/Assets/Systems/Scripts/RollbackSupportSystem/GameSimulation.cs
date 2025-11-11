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
        private FighterComponentManager fighter1;
        private FighterComponentManager fighter2;
        public RollbackManager rollback = new RollbackManager();
        public int FrameNumber { get; private set; }

        public void Initialize(FighterComponentManager fighter1, FighterComponentManager fighter2)
        {
            this.fighter1 = fighter1;
            this.fighter2 = fighter2;

            this.fighter1.FighterController.Initialize();
            this.fighter2.FighterController.Initialize();

            PhysicsWorld.Instance.Register(this.fighter1.FighterController.body);
            PhysicsWorld.Instance.Register(this.fighter2.FighterController.body);
            PushboxManager.Instance.Register(this.fighter1.FighterController);
            PushboxManager.Instance.Register(this.fighter2.FighterController);
            HitboxManager.Instance.Register(this.fighter1);
            HitboxManager.Instance.Register(this.fighter2);
        }

        public void Step()
        {
            FrameNumber++;
            fighter1.OnUpdate();
            fighter2.OnUpdate();

            PhysicsWorld.Instance.Step();
            PushboxManager.Instance.ResolvePush();
            HitboxManager.Instance.CheckHits();

            fighter1.DeterministicAnimator.ApplyVisuals();
            fighter2.DeterministicAnimator.ApplyVisuals();

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