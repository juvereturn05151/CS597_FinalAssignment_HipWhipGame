/*
File Name:    SimulationDriver.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;

namespace RollbackSupport
{
    public class SimulationDriver : MonoBehaviour
    {
        public GameSimulation simulation;
        const float FRAME_DURATION = 1f / 60f;
        float accumulator;

        void Awake()
        {
            Application.targetFrameRate = 120;
            QualitySettings.vSyncCount = 0;
            simulation.Initialize();
        }

        void Update()
        {
            accumulator += Time.unscaledDeltaTime;
            while (accumulator >= FRAME_DURATION)
            {
                simulation.Step();
                accumulator -= FRAME_DURATION;
            }
        }
    }
}