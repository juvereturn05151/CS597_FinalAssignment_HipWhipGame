using UnityEngine;

namespace RollbackSupport
{
    [System.Serializable]
    public struct GameStateSnapshot
    {
        public FighterStateSnapshot P1;
        public FighterStateSnapshot P2;
        public int FrameNumber;

        public static GameStateSnapshot Capture(int frame, FighterComponentManager f1, FighterComponentManager f2)
        {
            var s = new GameStateSnapshot
            {
                FrameNumber = frame,
                P1 = FighterStateSnapshot.From(f1),
                P2 = FighterStateSnapshot.From(f2)
            };

            //Debug.Log($"Captured Frame {s.FrameNumber}: P1={s.P1.pos}, P2={s.P2.pos}");

            return s;
        }


        public void Restore(FighterComponentManager f1, FighterComponentManager f2)
        {
            P1.ApplyTo(f1);
            P2.ApplyTo(f2);
            //Debug.Log($"Restored Frame {FrameNumber}: P1={f1.transform.position}, P2={f2.transform.position}");
        }
    }
}