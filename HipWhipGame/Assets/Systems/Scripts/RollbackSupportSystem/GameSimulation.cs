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

        [SerializeField]
        private MatchUI matchUI;

        public RollbackManager rollback = new RollbackManager();
        public MatchState matchState = new MatchState();
        public int FrameNumber { get; private set; }

        private bool roundOver = false;
        public bool IsRoundOver() => roundOver;


        public void Initialize(FighterComponentManager fighter1, FighterComponentManager fighter2)
        {
            this.fighter1 = fighter1;
            this.fighter2 = fighter2;

            matchState.Initialize(3);
            matchUI.FighterUI1.UpdateHearts(matchState.lives[0]);
            matchUI.FighterUI2.UpdateHearts(matchState.lives[1]);

            PhysicsWorld.Instance.Register(this.fighter1.FighterController.body);
            PhysicsWorld.Instance.Register(this.fighter2.FighterController.body);
            PushboxManager.Instance.Register(this.fighter1);
            PushboxManager.Instance.Register(this.fighter2);
            HitboxManager.Instance.Register(this.fighter1);
            HitboxManager.Instance.Register(this.fighter2);
        }

        public void Reset()
        {
            matchState.Initialize(3);
            matchUI.FighterUI1.UpdateHearts(matchState.lives[0]);
            matchUI.FighterUI2.UpdateHearts(matchState.lives[1]);

            fighter1.ResetStateForRespawn();
            fighter2.ResetStateForRespawn();
        }

        public void Step()
        {
            FrameNumber++;

            fighter1.OnUpdate();
            fighter2.OnUpdate();

            PhysicsWorld.Instance.Step();
            PushboxManager.Instance.ResolvePush();
            HitboxManager.Instance.CheckHits();

            CheckFalls();

            fighter1.DeterministicAnimator.ApplyVisuals();
            fighter2.DeterministicAnimator.ApplyVisuals();

            rollback.Push(FrameNumber, GameStateSnapshot.Capture(FrameNumber, fighter1, fighter2, matchState));
        }

        private void CheckFalls()
        {
            if (matchState.isGameOver)
            {
                return;
            }

            if (PhysicsWorld.Instance.GetStageBounds().IsOutside(fighter1.FighterController.body.position)) 
            {
                OnPlayerFall(0);
            }
            else if (PhysicsWorld.Instance.GetStageBounds().IsOutside(fighter2.FighterController.body.position)) 
            {
                OnPlayerFall(1);
            }
        }

        private void OnPlayerFall(int playerIndex)
        {
            if (matchState.isGameOver)
                return;

            matchState.lives[playerIndex]--;

            if (playerIndex == 0)
            {
                matchUI.FighterUI1.UpdateHearts(matchState.lives[0]);
            }
            else 
            {
                matchUI.FighterUI2.UpdateHearts(matchState.lives[1]);
            }

            if (matchState.lives[playerIndex] <= 0)
            {
                matchState.isGameOver = true;
                matchState.winnerIndex = (playerIndex == 0 ? 1 : 0);
                return;
            }

            // Respawn logic
            SetRoundOver(true);
        }

        public void SetRoundOver(bool setter) 
        {
            roundOver = setter;
            
            if (!roundOver) 
            {
                fighter1.ResetStateForRespawn();
                fighter2.ResetStateForRespawn();
            }
        }

        public void ResetForReplay() 
        {
            fighter1.ResetForReplay();
            fighter2.ResetForReplay();
        }

        public void RestoreToSnapshot(GameStateSnapshot snap)
        {
            // Reset fighters
            snap.Restore(fighter1, fighter2, matchState);

            fighter1.OnUpdate();
            fighter2.OnUpdate();

            PhysicsWorld.Instance.Step();
            PushboxManager.Instance.ResolvePush();
            HitboxManager.Instance.CheckHits();

            // Update visuals
            fighter1.DeterministicAnimator.ApplyVisuals();
            fighter2.DeterministicAnimator.ApplyVisuals();

            // Reset frame number
            FrameNumber = snap.FrameNumber;
        }

        public void RollbackTo(int frame)
        {
            if (rollback.TryGetSnapshot(frame, out var snap))
            {
                RestoreToSnapshot(snap);
            }
        }
    }
}