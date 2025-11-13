/*
File Name:    MatchUI.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using RollbackSupport;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.HID.HID;

public class MatchUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI winnerText;

    public void ShowGameOver(int winner)
    {
        panel.SetActive(true);
        winnerText.text = $"Player {winner + 1} Wins!";
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}
