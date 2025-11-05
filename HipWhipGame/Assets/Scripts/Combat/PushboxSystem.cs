/*
File Name:    PushboxSystem.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;

namespace HipWhipGame
{
    public class PushboxSystem : MonoBehaviour
    {
        [Range(0.1f, 5f)] public float strength = 1f;

        void LateUpdate()
        {
            var players = GamePlayerManager.Instance.ActivePlayers;
            int count = players.Count;

            for (int i = 0; i < count; ++i)
            {
                var a = players[i].GetComponent<FighterController>();
                if (a == null || a.pushbox == null) continue;

                for (int j = i + 1; j < count; ++j)
                {
                    var b = players[j].GetComponent<FighterController>();
                    if (b == null || b.pushbox == null) continue;

                    PushResolver.Resolve(a, b, a.pushbox, b.pushbox, strength);
                }
            }
        }
    }
}
