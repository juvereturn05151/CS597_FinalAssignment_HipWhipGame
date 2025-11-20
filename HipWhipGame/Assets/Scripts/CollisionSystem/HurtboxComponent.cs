using UnityEngine;
using System.Collections.Generic;

namespace RollbackSupport
{
    [System.Serializable]
    public class HurtboxComponent
    {
        public List<CollisionBox> boxes = new List<CollisionBox>();

        public void AddBox(Vector3 localCenter, Vector3 size)
        {
            boxes.Add(new CollisionBox { localCenter = localCenter, size = size, enabled = true });
        }

        public void EnableAll(bool value)
        {
            for (int i = 0; i < boxes.Count; i++)
            {
                var b = boxes[i];
                b.enabled = value;
                boxes[i] = b;
            }
        }

        public IEnumerable<CollisionBox> ActiveBoxes => boxes;
    }
}
