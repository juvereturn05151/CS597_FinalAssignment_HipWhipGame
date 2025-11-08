/*
File Name:    MoveExecutor.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using System.Collections;
using Unity.VisualScripting;
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
                return;

            StopAllCoroutines();
            StartCoroutine(DoMove(move));
        }

        IEnumerator DoMove(MoveData move)
        {
            int totalFrames = move.startup + move.active + move.recovery;
            const float frameDuration = 1f / 60f;
            WaitForSeconds waitFrame = new WaitForSeconds(frameDuration);

            fighterComponentManager.Animator.applyRootMotion = !move.overrideRootMotion;

            fighterComponentManager.FighterStateMachine.SwitchState(move.state,totalFrames / 60f);

            fighterComponentManager.Animator.Play(move.animation.name, 0, 0f);

            GameObject hb = null;

            for (currentFrame = 0; currentFrame < totalFrames; currentFrame++)
            {
                var state = fighterComponentManager.FighterStateMachine.CurrentStateType;

                // Allow sidestep to continue even though it's not "Attacking"
                if (fighterComponentManager.FighterController.IsInterrupted)
                {
                    if (hb) Destroy(hb);
                    yield break;
                }

                bool movedThisFrame = false;

                // --- Manual motion ---
                if (move.overrideRootMotion && move.motionSegments != null)
                {
                    foreach (var seg in move.motionSegments)
                    {
                        if (currentFrame >= seg.frameStart && currentFrame <= seg.frameEnd)
                        {
                            movedThisFrame = true;

                            Vector3 delta =
                                transform.forward * seg.forwardSpeed +
                                transform.right * seg.sideSpeed +
                                Vector3.up * seg.verticalSpeed;

                            float elapsed = 0f;
                            while (elapsed < frameDuration)
                            {
                                // Stop interpolation if move interrupted
                                var s = fighterComponentManager.FighterStateMachine.CurrentStateType;
                                if (fighterComponentManager.FighterController.IsInterrupted)
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

                // --- Hitbox spawn ---
                if (currentFrame == move.startup)
                {
                    // --- Hitbox spawn only for attacking---
                    if (move.state == FighterState.Attacking)
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
                                hitbox.SetLifetimeFrames((int)Mathf.Max(1, move.hitboxLifetimeFrames > 0 ? move.hitboxLifetimeFrames : move.active));
                            }
                        }
                    } 
                    else if (move.state == FighterState.TryGrab) 
                    {
                        if (!fighterComponentManager.FighterController.lookAtTarget)
                            yield break;

                        Transform target = fighterComponentManager.FighterController.lookAtTarget;
                        Vector3 fighterPos = fighterComponentManager.FighterController.transform.position;
                        Vector3 targetPos = target.position;

                        // Distance check
                        float distance = Vector3.Distance(fighterPos, targetPos);
                        bool inRange = distance <= move.grabRange;

                        // Direction check (optional: ensure target is in front)
                        Vector3 toTarget = (targetPos - fighterPos).normalized;
                        float dot = Vector3.Dot(fighterComponentManager.FighterController.transform.forward, toTarget);
                        bool inFront = dot > 0.3f; // Adjust threshold for strictness

                        if (inRange && inFront)
                        {
                            fighterComponentManager.FighterGrabManager.SetUpGrabData(move);
                            fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Grabbing);
                            yield break;
                        }
                    }
                }

                // --- Hitbox cleanup ---
                if (currentFrame == move.startup + move.active && hb)
                {
                    Destroy(hb);
                }

                // --- Wait one frame ---
                if (!movedThisFrame)
                {
                    yield return waitFrame;
                }
            }

            // --- Cleanup ---
            if (hb) Destroy(hb);

            // --- Return to Idle when move finishes ---
            fighterComponentManager.FighterStateMachine.SwitchState(FighterState.Idle);
        }
    }
}
