/*
File Name:    FighterUI.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine.UI;
using TMPro;
using UnityEngine;

namespace RollbackSupport
{
    public class FighterUI : MonoBehaviour
    {
        [Header("Hearts UI")]
        public Image[] heartIcons;   // size = 3
        public Sprite fullHeart;
        public Sprite emptyHeart;

        [Header("Percentage UI")]
        public TextMeshProUGUI percentText;

        public void UpdateHearts(int updatedHearts)
        {
            int hp = updatedHearts;

            for (int i = 0; i < heartIcons.Length; i++)
            {
                heartIcons[i].sprite = (i < hp) ? fullHeart : emptyHeart;
            }
        }

        public void UpdatePercentage()
        {
            float pct = 55f;//fighter.FighterController.DamagePercent;
            percentText.text = $"{pct:0}%";
        }
    }
}