/*
File Name:    PushbackSolver.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/


using UnityEngine;

namespace HipWhipGame
{
    public class PushbackSolver : MonoBehaviour
    {
        public float resolveStrength = 2f;
        public float resolveIterations = 2f;

    void LateUpdate()
    {
        var pushboxes = FindObjectsOfType<Pushbox>();
        for (int i = 0; i < pushboxes.Length; ++i)
        for (int j = i + 1; j < pushboxes.Length; ++j)
        {
            var a = pushboxes[i];
            var b = pushboxes[j];
            if (!a || !b || a.owner == b.owner) continue;

            // Very rough capsule-overlap -> treat as spheres on ground plane
            Vector3 aPos = a.transform.position + Vector3.up * a.yCenter;
            Vector3 bPos = b.transform.position + Vector3.up * b.yCenter;

            Vector3 delta = new Vector3(aPos.x - bPos.x, 0f, aPos.z - bPos.z);
            float minDist = a.radius + b.radius;
            float d = delta.magnitude;
            if (d < minDist && d > 0.0001f)
            {
                Vector3 n = delta / d;
                float pen = (minDist - d);
                Vector3 move = n * (pen * 0.5f * resolveStrength);
                a.owner.transform.position += move;
                b.owner.transform.position -= move;
            }
        }
    }
    }
}