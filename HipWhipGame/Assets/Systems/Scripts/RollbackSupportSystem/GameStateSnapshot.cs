using UnityEngine;

namespace RollbackSupport
{
    public class GameStateSnapshot : MonoBehaviour
    {
        public FighterStateSnapshot P1;
        public FighterStateSnapshot P2;
        public int FrameNumber;

        public static GameStateSnapshot Capture(int frame, Fighter f1, Fighter f2)
        {
            return new GameStateSnapshot
            {
                FrameNumber = frame,
                P1 = FighterStateSnapshot.From(f1),
                P2 = FighterStateSnapshot.From(f2)
            };
        }

        public void Restore(Fighter f1, Fighter f2)
        {
            P1.ApplyTo(f1);
            P2.ApplyTo(f2);
        }
    }
}