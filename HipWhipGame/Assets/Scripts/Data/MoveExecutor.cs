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
            Debug.Log($"Executing move: {move.moveName}");

            int totalFrames = move.startup + move.active + move.recovery;
            WaitForSeconds waitFrame = new WaitForSeconds(1f / 60f);

            // Disable root motion if we’re driving motion manually
            animator.applyRootMotion = !move.overrideRootMotion;

            _fsm.SetState(FighterState.Attacking, totalFrames / 60f);

            // Play animation at normalized time 0
            animator.Play(move.animation.name, 0, 0f);

            GameObject hb = null;

            // 
            // MAIN FRAME LOOP (fixed 60 Hz logic)
            // 
            for (int currentFrame = 0; currentFrame < totalFrames; currentFrame++)
            {
                float frameTimer = 0f;
                float frameDuration = 1f / 60f;

                //  Active-phase displacement only 
                if (move.overrideRootMotion && move.motionSegments != null)
                {
                    foreach (var seg in move.motionSegments)
                    {
                        if (currentFrame >= seg.frameStart && currentFrame <= seg.frameEnd)
                        {
                            // compute perframe target delta
                            Vector3 delta =
                                transform.forward * seg.forwardSpeed +
                                Vector3.up * seg.verticalSpeed;

                            // smooth interpolation inside this frame
                            // (renders smoothly even if camera updates per frame)
                            while (frameTimer < frameDuration)
                            {
                                transform.position += delta * (Time.deltaTime * 60f);
                                frameTimer += Time.deltaTime;
                                yield return null; // render frames between logic ticks
                            }

                            goto SkipFrameWait; // already yielded this frame
                        }
                    }
                }

                //  Spawn hitbox at start of active 
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

                //  Despawn hitbox at end of active
                if (currentFrame == move.startup + move.active && hb)
                    Destroy(hb);

                //  Standard wait for non-moving frames
                yield return waitFrame;

            SkipFrameWait:
                ; // empty label target
            }

            // Cleanup 
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
