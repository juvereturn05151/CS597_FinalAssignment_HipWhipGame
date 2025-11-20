/*
File Name:    MatchUI.cs
Author(s):    Ju-ve Chankasemporn
Copyright:    (c) 2025 DigiPen Institute of Technology. All rights reserved.
*/

using RollbackSupport;
using TMPro;
using UnityEngine;

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
    [SerializeField]
    private FighterUI fighterUI1;
    public FighterUI FighterUI1 => fighterUI1;
    [SerializeField]
    private FighterUI fighterUI2;
    public FighterUI FighterUI2 => fighterUI2;

    [SerializeField]
    private GameObject groupPlayerUI;
    public GameObject GroupPlayerUI => groupPlayerUI;

    [SerializeField]
    private TextMeshProUGUI p1Status;

    [SerializeField]
    private TextMeshProUGUI p2Status;

    public void ShowGameOver(int winner)
    {
        gameOverPanel.SetActive(true);

        if (winner == 0)
        {
            p1Status.text = "Winner!";
            p2Status.text = "Loser!";
        }
        else 
        {
            p1Status.text = "Loser!";
            p2Status.text = "Winner!";
        }
    }

    public void Hide()
    {
        gameOverPanel.SetActive(false);
    }


}
