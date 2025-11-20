/*
File Name:    UIAudioComponent.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;

namespace RollbackSupport
{
    public class UIAudioComponent : MonoBehaviour
    {
        [Header("UI Sounds")]
        public AudioClip hoverSFX;
        public AudioClip clickSFX;
        public AudioClip cancelSFX;
        public AudioClip confirmSFX;
        public AudioClip navigateSFX;

        public void PlayHover()
        {
            AudioSystem.Instance.PlayUI(hoverSFX);
        }

        public void PlayClick()
        {
            AudioSystem.Instance.PlayUI(clickSFX);
        }

        public void PlayCancel()
        {
            AudioSystem.Instance.PlayUI(cancelSFX);
        }

        public void PlayConfirm()
        {
            AudioSystem.Instance.PlayUI(confirmSFX);
        }

        public void PlayNavigate()
        {
            AudioSystem.Instance.PlayUI(navigateSFX);
        }
    }
}
