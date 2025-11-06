/*
File Name:    Damageable.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/


using UnityEngine;

namespace HipWhipGame
{
    public class Damageable : MonoBehaviour
    {
        public int maxHP = 100;
        public int hp;

        void Awake() { hp = maxHP; }

        public void ApplyDamage(int amount)
        {
            hp = Mathf.Max(0, hp - amount);
            // TODO: KO logic when hp==0
        }
    }
}