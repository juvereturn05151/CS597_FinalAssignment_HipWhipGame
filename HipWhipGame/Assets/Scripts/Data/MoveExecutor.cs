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
            print($"Executing move: {move.name}");

            _fsm.SetState(FighterState.Attacking, (move.startup + move.active + move.recovery) / 60f);

            // play anim (no animator links)
            animator.Play(move.animation.name, 0, 0f);

            // Startup
            yield return WaitFrames(move.startup);

            // Active — spawn hitbox
            GameObject hb = null;
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
                    hitbox.SetLifetimeFrames((int)Mathf.Max(1, move.hitboxLifetimeFrames > 0 ? move.hitboxLifetimeFrames : move.active));
                }
            }

            // Active wait
            yield return WaitFrames(move.active);

            // Despawn hitbox if still present
            if (hb) Destroy(hb);

            // Recovery
            yield return WaitFrames(move.recovery);

            // Back to idle if not interrupted
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