using UnityEngine;
using UnityEngine.SceneManagement;

public class BootSceneLoader : MonoBehaviour
{
    private async void Start()
    {
        // Preload all characters
        await GamePreloader.Instance.PreloadAll();

        // Load game scene
        SceneManager.LoadSceneAsync("ControllerAssignment");
    }
}
