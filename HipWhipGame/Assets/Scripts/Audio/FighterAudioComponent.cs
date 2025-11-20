/*
File Name:    FighterAudioComponent.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 
DigiPen Institute of Technology. All rights reserved.
*/

using System.Collections.Generic;
using UnityEngine;

namespace RollbackSupport
{
    public class FighterAudioComponent : MonoBehaviour, IFighterComponentInjectable
    {
        private FighterComponentManager _fighterComponentManager;

        // --- Anti-spam tracking ---
        // Per-clip last played frame (prevents consecutive-frame spam)
        private readonly Dictionary<AudioClip, int> _lastPlayedClipFrame = new();

        // Per-frame dedupe (prevents multiple triggers on same frame)
        private int _lastSFXFrame = -999;
        private int _lastVoiceFrame = -999;

        [Header("General Movement")]
        public AudioClip jumpSFX;
        public AudioClip dashSFX;
        public AudioClip landSFX;

        [Header("Attacks")]
        public AudioClip lightAttackSFX;
        public AudioClip heavyAttackSFX;
        public AudioClip specialAttackSFX;
        public AudioClip grabSFX;

        [Header("On Hit / Block / KO")]
        public AudioClip hitConfirmSFX;
        public AudioClip blockSFX;
        public AudioClip KO_SFX;

        [Header("Voice Lines (Optional)")]
        public AudioClip attackVoice;
        public AudioClip grabVoice;
        public AudioClip hurtVoice;
        public AudioClip superVoice;


        public void Inject(FighterComponentManager fighterComponentManager)
        {
            _fighterComponentManager = fighterComponentManager;
        }

        private Vector3 GetAudioPosition()
        {
            return transform.position;
        }


        // Get deterministic frame from Rollback/Simulation
        private int CurrentFrame =>
            _fighterComponentManager.CurrentGameSimulation.FrameNumber;


        #region Generic Helpers (Now Anti-Spam + Rollback-safe)

        // --- SFX ---
        public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            if (clip == null || AudioSystem.Instance == null)
                return;

            int frame = CurrentFrame;

            // 1) Prevent SAME-FRAME duplicates
            if (frame == _lastSFXFrame)
                return;
            _lastSFXFrame = frame;

            // 2) Prevent SAME-CLIP on CONSECUTIVE frames
            if (_lastPlayedClipFrame.TryGetValue(clip, out int lastFrame))
            {
                if (lastFrame == frame - 1)
                    return;
            }

            _lastPlayedClipFrame[clip] = frame;

            AudioSystem.Instance.PlaySFX(clip, GetAudioPosition(), volume, pitch);
        }

        // --- VOICE ---
        public void PlayVoice(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            if (clip == null || AudioSystem.Instance == null)
                return;

            int frame = CurrentFrame;

            // 1) Prevent SAME-FRAME duplicates
            if (frame == _lastVoiceFrame)
                return;
            _lastVoiceFrame = frame;

            // 2) Prevent SAME CLIP on consecutive frames
            if (_lastPlayedClipFrame.TryGetValue(clip, out int lastFrame))
            {
                if (lastFrame == frame - 1)
                    return;
            }

            _lastPlayedClipFrame[clip] = frame;

            AudioSystem.Instance.PlayVoice(clip, GetAudioPosition(), volume, pitch);
        }

        #endregion


        #region High-Level API For Fighter Logic

        public void PlayJump() => PlaySFX(jumpSFX);
        public void PlayDash() => PlaySFX(dashSFX);
        public void PlayLand() => PlaySFX(landSFX);

        public void PlayLightAttack() => PlaySFX(lightAttackSFX);
        public void PlayHeavyAttack() => PlaySFX(heavyAttackSFX);
        public void PlaySpecialAttack() => PlaySFX(specialAttackSFX);
        public void PlayGrab() => PlaySFX(grabSFX);

        public void PlayHitConfirm() => PlaySFX(hitConfirmSFX);
        public void PlayBlock() => PlaySFX(blockSFX);
        public void PlayKO() => PlaySFX(KO_SFX);

        public void PlayAttackVoice() => PlayVoice(attackVoice);
        public void PlayGrabVoice() => PlayVoice(grabVoice);
        public void PlayHurtVoice() => PlayVoice(hurtVoice);
        public void PlaySuperVoice() => PlayVoice(superVoice);

        #endregion
    }
}
