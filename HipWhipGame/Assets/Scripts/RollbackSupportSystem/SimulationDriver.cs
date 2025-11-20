/*
File Name:    SimulationDriver.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
using UnityEngine.InputSystem;

namespace RollbackSupport
{
    public class SimulationDriver : MonoBehaviour
    {
        public GameSimulation simulation;
        public ReplayManager replayManager;
        public MatchUI matchUI;

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
            if (!isRunning && !replayManager.IsReplaying) 
            {
                isRunning = true;
                simulation.SetRoundOver(false);
                matchUI.ReplayPanel.SetActive(false);
                matchUI.GroupPlayerUI.SetActive(true);
            }

            if (!isRunning)
                return;

            accumulator += Time.unscaledDeltaTime;
            while (accumulator >= FRAME_DURATION)
            {
                simulation.Step();
                accumulator -= FRAME_DURATION;
            }

            // NOT deterministic — UI logic
            if (simulation.matchState.isGameOver)
            {
                matchUI.ShowGameOver(simulation.matchState.winnerIndex);

                if (Keyboard.current != null && Keyboard.current.enterKey.isPressed)
                {
                    matchUI.Hide();
                    simulation.Reset();
                }

                foreach (var gamepad in Gamepad.all)
                {
                    if (gamepad.startButton.wasPressedThisFrame)
                    {
                        matchUI.Hide();
                        simulation.Reset();
                    }
                }
            } 
            else if (simulation.IsRoundOver()) 
            {
                isRunning = false;
                simulation.ResetForReplay();
                replayManager.PrepareReplay(simulation.rollback);
                replayManager.StartReplay();
                matchUI.ReplayPanel.SetActive(true);
                matchUI.GroupPlayerUI.SetActive(false);
            }
        }
    }
}
