/*
File Name:    MoveExecutor.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using System.Collections;
using UnityEngine;
using static HipWhipGame.Enums;
using static UnityEngine.Rendering.DebugUI;

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
            Debug.Log($"Executing move: {move.moveName}");

            int totalFrames = move.startup + move.active + move.recovery;

            // ensure deterministic frame timing (60 FPS)
            WaitForSeconds waitFrame = new WaitForSeconds(1f / 60f);

            // disable root motion if using custom movement
            animator.applyRootMotion = !move.overrideRootMotion;

            _fsm.SetState(FighterState.Attacking, totalFrames / 60f);

            // play animation directly (no animator links)
            animator.Play(move.animation.name, 0, 0f);

            GameObject hb = null;

            // MAIN FRAME LOOP
            for (int currentFrame = 0; currentFrame < totalFrames; currentFrame++)
            {
                // FRAME-RANGE MOVEMENT  (data-driven)
                if (move.overrideRootMotion && move.motionSegments != null)
                {
                    foreach (var seg in move.motionSegments)
                    {
                        if (currentFrame >= seg.frameStart && currentFrame <= seg.frameEnd)
                        {
                            Vector3 delta =
                                transform.forward * seg.forwardSpeed +
                                Vector3.up * seg.verticalSpeed;
                            transform.position += delta;
                        }
                    }
                }

                // HITBOX SPAWN (start of active)
                if (currentFrame == move.startup)
                {
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

                // HITBOX DESPAWN (end of active)
                if (currentFrame == move.startup + move.active && hb)
                    Destroy(hb);

                yield return waitFrame;
            }

            // ensure cleanup
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