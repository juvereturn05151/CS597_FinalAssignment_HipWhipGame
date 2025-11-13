using UnityEngine;

namespace RollbackSupport
{
    [System.Serializable]
    public struct GameStateSnapshot
    {
        public int FrameNumber;
        public FighterStateSnapshot P1;
        public FighterStateSnapshot P2;
        public MatchState MatchCopy;

        public static GameStateSnapshot Capture(int frame, FighterComponentManager f1, FighterComponentManager f2, MatchState match)
        {
            var s = new GameStateSnapshot
            {
                FrameNumber = frame,
                P1 = FighterStateSnapshot.From(f1),
                P2 = FighterStateSnapshot.From(f2),
                MatchCopy = match.Clone()
            };

            return s;
        }


        public void Restore(FighterComponentManager f1, FighterComponentManager f2, MatchState matchState)
        {
            P1.ApplyTo(f1);
            P2.ApplyTo(f2);

            matchState.lives = (int[])MatchCopy.lives.Clone();
            matchState.isGameOver = MatchCopy.isGameOver;
            matchState.winnerIndex = MatchCopy.winnerIndex;
        }
    }
}