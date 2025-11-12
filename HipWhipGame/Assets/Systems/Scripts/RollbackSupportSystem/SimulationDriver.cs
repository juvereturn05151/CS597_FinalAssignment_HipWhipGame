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
        public ReplayManager replayManager;
        const float FRAME_DURATION = 1f / 60f;
        float accumulator;
        bool isRunning = true;

        void Awake()
        {
            Application.targetFrameRate = 120;
            QualitySettings.vSyncCount = 0;
        }

        void Update()
        {
            // Start replay manually (for testing)
            if (Input.GetKeyDown(KeyCode.R))
            {
                isRunning = false;
                replayManager.PrepareReplay(simulation.rollback);
                replayManager.StartReplay();
            }

            if (!isRunning) return;

            accumulator += Time.unscaledDeltaTime;
            while (accumulator >= FRAME_DURATION)
            {
                simulation.Step();
                accumulator -= FRAME_DURATION;
            }
        }
    }
}
