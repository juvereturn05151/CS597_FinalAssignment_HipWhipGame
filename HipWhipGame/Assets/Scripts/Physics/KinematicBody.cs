using UnityEngine;

namespace RollbackSupport
{
    [System.Serializable]
    public class KinematicBody
    {
        public string debugName;
        public Vector3 position;
        public Vector3 velocity;
        public bool grounded;
        public bool useGravity = true;
        public bool isKinematic = false;

        public void Teleport(Vector3 pos)
        {
            position = pos;
            velocity = Vector3.zero;
        }
    }
}