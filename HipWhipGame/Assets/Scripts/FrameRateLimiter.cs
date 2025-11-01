/*
File Name:    FrameRateLimiter.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;


namespace HipWhipGame
{
    public class FrameRateLimiter : MonoBehaviour
    {
        void Awake()
        {
            // Turn off VSync so Application.targetFrameRate actually works
            QualitySettings.vSyncCount = 0;

            // Lock to 60 FPS
            Application.targetFrameRate = 60;

            Debug.Log($"Frame rate locked to {Application.targetFrameRate} FPS");
        }
    }
}