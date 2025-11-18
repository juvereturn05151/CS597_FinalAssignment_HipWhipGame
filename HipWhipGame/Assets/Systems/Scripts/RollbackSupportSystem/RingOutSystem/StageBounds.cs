/*
File Name:    StageBounds.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace RollbackSupport
{
    [System.Serializable]
    public struct StageBounds
    {
        public Vector3 min;
        public Vector3 max;

        public bool IsOutside(Vector3 pos)
        {
            return pos.x < min.x || pos.x > max.x ||
                   pos.y < min.y || pos.y > max.y ||
                   pos.z < min.z || pos.z > max.z;
        }

        public bool IsFalling(Vector3 pos) 
        {
            return pos.y < (min.y - 5.0f); 
        }
    }
}
