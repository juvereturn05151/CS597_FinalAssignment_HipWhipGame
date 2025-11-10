using UnityEngine;

namespace RollbackSupport
{
    [CreateAssetMenu(menuName = "Fighter/MoveData")]
    public class MoveData : ScriptableObject
    {
        public string moveName;
        public string animName;
        public int totalFrames;
        public FrameHitbox[] hitboxes;
    }

    [System.Serializable]
    public struct FrameHitbox
    {
        public int start, end;
        public CollisionBox box;
    }
}