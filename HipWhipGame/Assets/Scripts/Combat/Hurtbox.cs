/*
File Name:    Hurtbox.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;

namespace HipWhipGame
{
    [RequireComponent(typeof(Collider))]
    public class Hurtbox : MonoBehaviour
    {
        public FighterController owner;

        void Awake()
        {
            var col = GetComponent<Collider>();
            col.isTrigger = true;
            gameObject.layer = LayerMask.NameToLayer("Hurtbox");
        }
    }
}