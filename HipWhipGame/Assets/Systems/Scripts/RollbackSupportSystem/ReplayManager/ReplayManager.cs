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
        public bool IsReplaying => isReplaying;

        public void PrepareReplay(RollbackManager rollback)
        {
            // Copy all snapshots
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
            PlayerManager.Instance.SetInputManagersEnabled(false);
            StartCoroutine(PlayReplay());
        }

        private IEnumerator PlayReplay()
        {
            Debug.Log("Replaying...");

            foreach (var snap in replayFrames)
            {
                var p1 = snap.P1.pos;
                var p2 = snap.P2.pos;

                //Debug.Log(
                //    $"[Replay Dump] Frame {snap.FrameNumber} | " +
                //    $"P1=({p1.x:F2}, {p1.y:F2}, {p1.z:F2}) | " +
                //    $"P2=({p2.x:F2}, {p2.y:F2}, {p2.z:F2}) | " +
                //    $"Move={snap.P1.moveName} ({snap.P1.moveFrame}) / {snap.P2.moveName} ({snap.P2.moveFrame})"
                //);

                simulation.RestoreToSnapshot(snap);


                yield return new WaitForSeconds(playbackSpeed);
            }

            isReplaying = false;
            Debug.Log("Replay finished.");
        }
    }
}
