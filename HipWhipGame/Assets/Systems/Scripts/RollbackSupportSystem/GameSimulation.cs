/*
File Name:    GameSimulation.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
using HipWhipGame;

namespace RollbackSupport
{
    public class GameSimulation : MonoBehaviour
    {
        public Fighter fighter1;
        public Fighter fighter2;
        public RollbackManager rollback = new RollbackManager();
        public int FrameNumber { get; private set; }

        public void Initialize()
        {
            fighter1.Initialize(new Vector3(-2, 0, 0), this);
            fighter2.Initialize(new Vector3(2, 0, 0), this);

            PhysicsWorld.Instance.Register(fighter1.body);
            PhysicsWorld.Instance.Register(fighter2.body);
            PushboxManager.Instance.Register(fighter1);
            PushboxManager.Instance.Register(fighter2);
            HitboxManager.Instance.Register(fighter1);
            HitboxManager.Instance.Register(fighter2);
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