using UnityEngine;

namespace RollbackSupport
{
    [System.Serializable]
    public struct GameStateSnapshot
    {
        public FighterStateSnapshot P1;
        public FighterStateSnapshot P2;
        public int FrameNumber;

        public static GameStateSnapshot Capture(int frame, FighterController f1, FighterController f2)
        {
            return new GameStateSnapshot
            {
                FrameNumber = frame,
                P1 = FighterStateSnapshot.From(f1),
                P2 = FighterStateSnapshot.From(f2)
            };
        }

        public void Restore(FighterController f1, FighterController f2)
        {
            P1.ApplyTo(f1);
            P2.ApplyTo(f2);
        }
    }
}