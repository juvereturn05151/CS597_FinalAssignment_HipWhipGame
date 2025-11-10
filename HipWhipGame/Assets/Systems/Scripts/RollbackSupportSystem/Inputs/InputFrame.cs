using UnityEngine;

namespace RollbackSupport
{
    [System.Serializable]
    public struct InputFrame
    {
        public int frame;
        public sbyte horiz;
        public sbyte vert;
        public bool light, heavy, grab, block;
    }

}