using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    private void Update()
    {
        // Detect keyboard join (Enter or Space)
        if (Keyboard.current != null && (Keyboard.current.enterKey.wasPressedThisFrame))
        {
            FadingUI.Instance.StartFadeIn();
            FadingUI.Instance.OnStopFading.AddListener(LoadNextScene);
        }

        // Detect gamepad join (Start button)
        foreach (var gamepad in Gamepad.all)
        {
            if (gamepad.startButton.wasPressedThisFrame)
            {
                FadingUI.Instance.StartFadeIn();
                FadingUI.Instance.OnStopFading.AddListener(LoadNextScene);
            }
        }
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene("ControllerAssignment");
    }
}
