/*
File Name:    FrameMotion.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using System.Collections.Generic;
using UnityEngine;

namespace HipWhipGame 
{
    [System.Serializable]
    public class FrameMotion
    {
        [Tooltip("Start frame (inclusive)")]
        public int frameStart = 0;

        [Tooltip("End frame (inclusive)")]
        public int frameEnd = 0;

        [Tooltip("Forward speed (units per frame)")]
        public float forwardSpeed = 0f;

        [Tooltip("Vertical speed (units per frame)")]
        public float verticalSpeed = 0f;

        public float sideSpeed;
    }

}