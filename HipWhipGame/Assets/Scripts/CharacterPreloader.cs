using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CharacterPreloader : MonoBehaviour
{
    public static CharacterPreloader Instance { get; private set; }

    [Tooltip("Addressable label that contains all characters")]
    public string characterLabel = "characters";

    // Stores loaded character prefabs
    private Dictionary<string, GameObject> characterPrefabs =
        new Dictionary<string, GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Preload all character prefabs using the addressable label.
    /// </summary>
    public async Task PreloadCharacters()
    {
        characterPrefabs.Clear();

        var handle = Addressables.LoadAssetsAsync<GameObject>(
            characterLabel,
            (prefab) =>
            {
                characterPrefabs[prefab.name] = prefab;
            }
        );

        await handle.Task;

        Debug.Log("✔ Preloaded " + characterPrefabs.Count + " characters.");
    }

    /// <summary>
    /// Spawn a preloaded character by name (fast, no load time).
    /// </summary>
    public static GameObject Spawn(string charName, Vector3 position, Quaternion rotation)
    {
        if (!Instance.characterPrefabs.ContainsKey(charName))
        {
            Debug.LogError("Character not preloaded: " + charName);
            return null;
        }

        return Instantiate(Instance.characterPrefabs[charName], position, rotation);
    }

    /// <summary>
    /// Get the prefab directly (e.g., for injection).
    /// </summary>
    public static GameObject GetPrefab(string charName)
    {
        if (!Instance.characterPrefabs.ContainsKey(charName))
            return null;

        return Instance.characterPrefabs[charName];
    }
}
