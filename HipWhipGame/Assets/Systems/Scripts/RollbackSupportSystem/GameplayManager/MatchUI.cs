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

    private void Update()
    {
        if (panel.activeSelf) 
        {
            if (Keyboard.current != null && Keyboard.current.enterKey.isPressed)
            {
                OnRestartButton();
            }

            foreach (var gamepad in Gamepad.all)
            {
                if (gamepad.startButton.wasPressedThisFrame)
                {
                    OnRestartButton();
                }
            }
        }
    }

    public void ShowGameOver(int winner)
    {
        panel.SetActive(true);
        winnerText.text = $"Player {winner + 1} Wins!";
    }

    public void Hide()
    {
        panel.SetActive(false);
    }

    public void OnRestartButton()
    {
        // Reload the scene or reset simulation
        //UnityEngine.SceneManagement.SceneManager.LoadScene(
        //    UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        //);
    }
}
