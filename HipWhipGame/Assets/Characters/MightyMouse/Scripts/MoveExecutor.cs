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
    [RequireComponent(typeof(FighterComponentManager))]
    public class MoveExecutor : MonoBehaviour, IFighterComponentInjectable
    {
        private FighterComponentManager fighterComponentManager;

        private int currentFrame;
        public int CurrentFrame => currentFrame;

        public void Inject(FighterComponentManager fighterComponentManager)
        {
            this.fighterComponentManager = fighterComponentManager;
        }

        public void PlayMove(MoveData move)
        {
            if (move == null || fighterComponentManager.Animator == null) 
            {
                return;
            } 

            StopAllCoroutines();
            StartCoroutine(DoMove(move));
        }

        IEnumerator DoMove(MoveData move)
        {
            int totalFrames = move.startup + move.active + move.recovery;
            const float frameDuration = 1f / 60f;
            WaitForSeconds waitFrame = new WaitForSeconds(frameDuration);

            fighterComponentManager.Animator.applyRootMotion = !move.overrideRootMotion;
            fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Attacking, totalFrames / 60f);
            fighterComponentManager.Animator.Play(move.animation.name, 0, 0f);

            GameObject hb = null;

            for (currentFrame = 0; currentFrame < totalFrames; currentFrame++)
            {
                // Interrupt check
                if (fighterComponentManager.FighterStateMachine.CurrentStateType != FighterState.Attacking)
                {
                    // Move was interrupted: cleanup & exit immediately
                    if (hb) Destroy(hb);
                    yield break;
                }

                bool movedThisFrame = false;

                // Manual motion 
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

                            float elapsed = 0f;
                            while (elapsed < frameDuration)
                            {
                                // Check for interruption during interpolation too
                                if (fighterComponentManager.FighterStateMachine.CurrentStateType != FighterState.Attacking)
                                {
                                    if (hb) Destroy(hb);
                                    yield break;
                                }

                                transform.position += delta * (Time.deltaTime * 60f);
                                elapsed += Time.deltaTime;
                                yield return null;
                            }
                            break;
                        }
                    }
                }

                // Hitbox spawn 
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
                            hitbox.Init(owner: fighterComponentManager.FighterController, move);
                            hitbox.Activate();
                            hitbox.SetLifetimeFrames(
                                (int)Mathf.Max(1,
                                    move.hitboxLifetimeFrames > 0
                                        ? move.hitboxLifetimeFrames
                                        : move.active));
                        }
                    }
                }

                // Hitbox cleanup
                if (currentFrame == move.startup + move.active && hb)
                {
                    Destroy(hb);
                }

                // Wait one frame
                if (!movedThisFrame) 
                {
                    yield return waitFrame;
                }
            }

            // Final cleanup 
            if (hb) Destroy(hb);

            if (fighterComponentManager.FighterStateMachine.CurrentStateType == FighterState.Attacking) 
            {
                fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Idle);
            }
        }
    }
}
