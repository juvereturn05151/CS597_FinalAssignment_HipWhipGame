using System.Collections.Generic;

namespace RollbackSupport
{
    public class InputBuffer
    {
        private Dictionary<int, InputFrame> _frames = new Dictionary<int, InputFrame>();

        public void SetAuthoritative(InputFrame frame) => _frames[frame.frame] = frame;

        public bool TryGet(int frame, out InputFrame f) => _frames.TryGetValue(frame, out f);

        public InputFrame Predict(int frame, InputFrame last)
        {
            last.frame = frame;
            last.light = last.heavy = last.grab = last.block = false;
            return last;
        }
    }
}