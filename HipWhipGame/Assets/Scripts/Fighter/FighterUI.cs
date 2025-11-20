/*
File Name:    FighterUI.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RollbackSupport
{
    public class FighterUI : MonoBehaviour
    {
        [Header("Hearts UI")]
        [SerializeField]
        private Image[] heartIcons;   // size = 3
        [SerializeField]
        private Sprite fullHeart;
        [SerializeField]
        private Sprite emptyHeart;

        [Header("Percentage UI")]
        [SerializeField]
        private TextMeshProUGUI percentText;

        [Header("Super Meter (5 Bars)")]
        [SerializeField]
        private List<Slider> superBars;  // Assign 5 sliders in Inspector

        // Smooth fill speed
        [SerializeField]
        private float fillSpeed = 10f;

        // Internal animation state
        private float[] currentValues = new float[5];

        [SerializeField]
        private GameObject specialUI;
        public GameObject SpecialUI => specialUI;

        private void Awake()
        {
            // Initialize values
            for (int i = 0; i < currentValues.Length; i++) 
            {
                currentValues[i] = 0f;
            }
        }

        private void Update()
        {
            // Smoothly animate the slider fill
            for (int i = 0; i < superBars.Count; i++)
            {
                if (superBars[i])
                {
                    superBars[i].value = Mathf.Lerp(
                        superBars[i].value,
                        currentValues[i],
                        Time.deltaTime * fillSpeed
                    );
                }
            }
        }

        public void UpdateHearts(int updatedHearts)
        {
            int hp = updatedHearts;

            for (int i = 0; i < heartIcons.Length; i++)
            {
                heartIcons[i].sprite = (i < hp) ? fullHeart : emptyHeart;
            }
        }

        public void UpdatePercentage(float percentage)
        {
            float pct = percentage;
            percentText.text = $"{pct:0}%";
        }

        public void UpdateMeter(float meter)
        {
            // For float meter 0.0 -> 5.0
            // Each bar is 1.0 segment
            for (int i = 0; i < superBars.Count; i++)
            {
                float segmentStart = i * 1f;
                float segmentEnd = (i + 1) * 1f;

                if (meter <= segmentStart)
                {
                    currentValues[i] = 0f; // this bar is empty
                }
                else if (meter >= segmentEnd)
                {
                    currentValues[i] = 1f; // this bar is full
                }
                else
                {
                    currentValues[i] = meter - segmentStart; // partial fill
                }
            }
        }
    }
}