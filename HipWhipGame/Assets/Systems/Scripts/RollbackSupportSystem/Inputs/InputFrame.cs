using UnityEngine;

namespace RollbackSupport
{
    [System.Serializable]
    public struct InputFrame
    {
        public int frame;
        public float horiz;
        public float vert;
        public float sidestep;
        public bool light, heavy, grab, block;
    }

}