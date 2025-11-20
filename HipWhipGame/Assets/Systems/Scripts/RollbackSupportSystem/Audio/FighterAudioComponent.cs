/*
File Name:    FighterAudioComponent.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;

namespace RollbackSupport
{
    public class FighterAudioComponent : MonoBehaviour, IFighterComponentInjectable
    {
        private FighterComponentManager _fighterComponentManager;

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

        #region Generic Helpers

        public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            if (clip == null || AudioSystem.Instance == null)
                return;

            AudioSystem.Instance.PlaySFX(clip, GetAudioPosition(), volume, pitch);
        }

        public void PlayVoice(AudioClip clip, float volume = 1f, float pitch = 1f)
        {
            if (clip == null || AudioSystem.Instance == null)
                return;

            AudioSystem.Instance.PlayVoice(clip, GetAudioPosition(), volume, pitch);
        }

        #endregion

        #region High-Level API For Fighter Logic

        public void PlayJump()
        {
            PlaySFX(jumpSFX);
        }

        public void PlayDash()
        {
            PlaySFX(dashSFX);
        }

        public void PlayLand()
        {
            PlaySFX(landSFX);
        }

        public void PlayLightAttack()
        {
            PlaySFX(lightAttackSFX);
        }

        public void PlayHeavyAttack()
        {
            PlaySFX(heavyAttackSFX);
        }

        public void PlaySpecialAttack()
        {
            PlaySFX(specialAttackSFX);
        }

        public void PlayGrab()
        {
            PlaySFX(grabSFX);
        }

        public void PlayHitConfirm()
        {
            PlaySFX(hitConfirmSFX);
        }

        public void PlayBlock()
        {
            PlaySFX(blockSFX);
        }

        public void PlayKO()
        {
            PlaySFX(KO_SFX);
        }

        public void PlayAttackVoice()
        {
            PlayVoice(attackVoice);
        }

        public void PlayGrabVoice()
        {
            PlayVoice(grabVoice);
        }

        public void PlayHurtVoice()
        {
            PlayVoice(hurtVoice);
        }

        public void PlaySuperVoice()
        {
            PlayVoice(superVoice);
        }

        #endregion
    }
}
