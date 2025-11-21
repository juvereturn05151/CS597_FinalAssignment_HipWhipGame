using UnityEngine;
using UnityEngine.SceneManagement;

public class BootSceneLoader : MonoBehaviour
{
    private async void Start()
    {
        // Preload all characters
        await CharacterPreloader.Instance.PreloadCharacters();

        // Load game scene
        SceneManager.LoadSceneAsync("ControllerAssignment");
    }
}
