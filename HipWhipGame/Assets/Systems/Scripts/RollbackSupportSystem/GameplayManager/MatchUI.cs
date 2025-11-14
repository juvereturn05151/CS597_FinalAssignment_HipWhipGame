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
    [SerializeField]
    private GameObject gameOverPanel;
    public GameObject GameOverPanel => gameOverPanel;
    [SerializeField]
    private TextMeshProUGUI winnerText;
    [SerializeField]
    private GameObject replayPanel;
    public GameObject ReplayPanel => replayPanel;

    public void ShowGameOver(int winner)
    {
        gameOverPanel.SetActive(true);
        winnerText.text = $"Player {winner + 1} Wins!";
    }

    public void Hide()
    {
        gameOverPanel.SetActive(false);
    }


}
