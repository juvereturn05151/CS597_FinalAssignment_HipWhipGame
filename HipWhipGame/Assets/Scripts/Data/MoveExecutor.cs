/*
File Name:    MoveExecutor.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using System.Collections;
using UnityEngine;
using static HipWhipGame.Enums;

namespace HipWhipGame
{
    [RequireComponent(typeof(FighterStateMachine))]
    public class MoveExecutor : MonoBehaviour
    {
        public Animator animator;
        FighterStateMachine _fsm;
        FighterController _fc;

        void Awake()
        {
            _fsm = GetComponent<FighterStateMachine>();
            _fc = GetComponent<FighterController>();
            if (!animator) animator = GetComponentInChildren<Animator>();
        }

        public void PlayMove(MoveData move)
        {
            if (move == null || animator == null) return;
            StopAllCoroutines();
            StartCoroutine(DoMove(move));
        }

        IEnumerator DoMove(MoveData move)
        {
            int totalFrames = move.startup + move.active + move.recovery;
            const float frameDuration = 1f / 60f;
            WaitForSeconds waitFrame = new WaitForSeconds(frameDuration);

            // Disable root motion if we’re driving motion manually
            animator.applyRootMotion = !move.overrideRootMotion;

            _fsm.SetState(FighterState.Attacking, totalFrames / 60f);
            animator.Play(move.animation.name, 0, 0f);

            GameObject hb = null;

            //
            // MAIN FRAME LOOP (deterministic 60 Hz logic)
            //
            for (int currentFrame = 0; currentFrame < totalFrames; currentFrame++)
            {
                bool movedThisFrame = false;

                // --- Movement During Active Frames ---
                if (move.overrideRootMotion && move.motionSegments != null)
                {
                    foreach (var seg in move.motionSegments)
                    {
                        if (currentFrame >= seg.frameStart && currentFrame <= seg.frameEnd)
                        {
                            movedThisFrame = true;
                            Vector3 delta =
                                transform.forward * seg.forwardSpeed +
                                Vector3.up * seg.verticalSpeed;

                            // smooth interpolation for this single frame
                            float elapsed = 0f;
                            while (elapsed < frameDuration)
                            {
                                transform.position += delta * (Time.deltaTime * 60f);
                                elapsed += Time.deltaTime;
                                yield return null;
                            }
                            break; // once we interpolated, no more segments this frame
                        }
                    }
                }

                // --- Hitbox Spawn ---
                if (currentFrame == move.startup)
                {
                    Debug.Log($"Move {move.moveName} entering active frames.");

                    if (move.hitboxPrefab)
                    {
                        hb = Instantiate(move.hitboxPrefab, transform);
                        hb.transform.localPosition = move.hitboxLocalPos;
                        hb.transform.localScale = move.hitboxLocalScale;

                        var hitbox = hb.GetComponent<Hitbox>();
                        if (hitbox)
                        {
                            hitbox.Init(owner: _fc, move);
                            hitbox.Activate();
                            hitbox.SetLifetimeFrames(
                                (int)Mathf.Max(1,
                                    move.hitboxLifetimeFrames > 0
                                        ? move.hitboxLifetimeFrames
                                        : move.active));
                        }
                    }
                }

                // --- Hitbox Cleanup ---
                if (currentFrame == move.startup + move.active && hb)
                {
                    Destroy(hb);
                }

                // --- Yield once per logic step (if not already yielded during interpolation) ---
                if (!movedThisFrame)
                    yield return waitFrame;
            }

            // Cleanup at end
            if (hb) Destroy(hb);

            if (_fsm.State == FighterState.Attacking)
                _fsm.SetState(FighterState.Idle);
        }

        IEnumerator WaitFrames(int frames)
        {
            float t = frames / 60f;
            while (t > 0f)
            {
                t -= Time.deltaTime;
                yield return null;
            }
        }
    }
}
