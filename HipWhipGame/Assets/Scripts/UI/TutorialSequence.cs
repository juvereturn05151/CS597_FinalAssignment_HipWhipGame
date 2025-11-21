using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TutorialSequence : MonoBehaviour
{
    [SerializeField] private List<GameObject> pages;
    [SerializeField] private string nextSceneName = "PlaygroundRollback";

    private int currentPage = 0;
    private bool active = false;

    public void StartTutorial()
    {
        if (pages.Count == 0)
            return;

        active = true;
        currentPage = 0;

        // Enable first page
        pages[currentPage].SetActive(true);
    }

    private void Update()
    {
        if (!active) return;


        if (Keyboard.current != null && (Keyboard.current.enterKey.wasPressedThisFrame))
        {
            NextPage();
        }

        // Detect gamepad join (Start button)
        foreach (var gamepad in Gamepad.all)
        {
            if (gamepad.startButton.wasPressedThisFrame)
            {
                NextPage();
            }
        }
    }

    private void NextPage()
    {
        // Hide current page
        pages[currentPage].SetActive(false);

        currentPage++;

        // If no more pages -> load scene
        if (currentPage >= pages.Count)
        {
            FadingUI.Instance.StartFadeIn();
            FadingUI.Instance.OnStopFading.AddListener(LoadNextScene);
            SceneManager.LoadScene(nextSceneName);
            return;
        }

        // Show next page
        pages[currentPage].SetActive(true);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
