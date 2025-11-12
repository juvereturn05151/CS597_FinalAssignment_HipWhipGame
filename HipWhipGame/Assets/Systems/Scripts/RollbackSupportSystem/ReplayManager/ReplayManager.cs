/*
File Name:    ReplayManager.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RollbackSupport
{
    public class ReplayManager : MonoBehaviour
    {
        [Header("Dependencies")]
        public GameSimulation simulation;
        public float playbackSpeed = 1f / 60f; // 60fps

        private List<GameStateSnapshot> replayFrames;
        private bool isReplaying;

        public void PrepareReplay(RollbackManager rollback)
        {
            // copy all snapshots
            replayFrames = new List<GameStateSnapshot>(rollback.GetAllSnapshots());
            Debug.Log($"Prepared {replayFrames.Count} replay frames");
        }

        public void StartReplay()
        {
            if (replayFrames == null || replayFrames.Count == 0)
            {
                Debug.LogWarning("No replay data to play.");
                return;
            }

            isReplaying = true;
            StartCoroutine(PlayReplay());
        }

        private IEnumerator PlayReplay()
        {
            Debug.Log("Replaying...");

            foreach (var snap in replayFrames)
            {
                snap.Restore(simulation.GetFighter1(), simulation.GetFighter2());
                yield return new WaitForSeconds(playbackSpeed);
            }

            isReplaying = false;
            Debug.Log("Replay finished.");
        }

        public bool IsReplaying() => isReplaying;
    }
}
