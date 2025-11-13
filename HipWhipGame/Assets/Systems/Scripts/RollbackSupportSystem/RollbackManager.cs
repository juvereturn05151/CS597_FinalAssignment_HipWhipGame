using System.Collections.Generic;

namespace RollbackSupport
{
    public class RollbackManager
    {
        const int BUFFER_SIZE = 300;
        private readonly Queue<GameStateSnapshot> snapshots = new Queue<GameStateSnapshot>();

        public void Push(int frame, GameStateSnapshot snap)
        {
            if (snapshots.Count >= BUFFER_SIZE) 
            {
                snapshots.Dequeue();
            }

            snapshots.Enqueue(snap);
        }

        public bool TryGetSnapshot(int frame, out GameStateSnapshot snap)
        {
            foreach (var s in snapshots)
            {
                if (s.FrameNumber == frame)
                {
                    snap = s;
                    return true;
                }
            }
            snap = default;
            return false;
        }

        // Expose all snapshots for replay
        public IEnumerable<GameStateSnapshot> GetAllSnapshots()
        {
            return snapshots;
        }
    }

}