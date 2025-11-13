/*
File Name:    MatchUI.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using UnityEngine;
using TMPro;

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

    public void OnRestartButton()
    {
        // Reload the scene or reset simulation
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }
}
