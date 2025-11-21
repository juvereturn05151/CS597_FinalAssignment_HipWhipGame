using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class GamePreloader : MonoBehaviour
{
    public static GamePreloader Instance { get; private set; }

    [Header("Addressable Labels to Preload")]
    public List<string> labelsToPreload = new List<string>
    {
        "characters",
        "sfx",
        "ui",
        "audio",
    };

    // Cache for ANY asset type
    private Dictionary<string, Object> loadedAssets =
        new Dictionary<string, Object>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Preload all asset labels listed in labelsToPreload
    /// </summary>
    public async Task PreloadAll()
    {
        loadedAssets.Clear();

        Debug.Log("▶ Starting preload...");

        foreach (var label in labelsToPreload)
            await PreloadLabel(label);

        Debug.Log("✔ Preload Complete: " + loadedAssets.Count + " assets loaded.");
    }

    /// <summary>
    /// Preload all assets associated with a single label.
    /// </summary>
    private async Task PreloadLabel(string label)
    {
        Debug.Log("⏳ Loading label: " + label);

        var handle = Addressables.LoadAssetsAsync<Object>(
            label,
            (asset) =>
            {
                if (!loadedAssets.ContainsKey(asset.name))
                    loadedAssets[asset.name] = asset;
            }
        );

        await handle.Task;

        Debug.Log($"   ✔ Loaded {handle.Result.Count} items from '{label}'");
    }

    /// <summary>
    /// Spawn prefab-type assets quickly
    /// </summary>
    public static GameObject Spawn(string name, Vector3 pos, Quaternion rot)
    {
        if (!Instance.loadedAssets.TryGetValue(name, out var obj))
        {
            Debug.LogError("Asset not preloaded: " + name);
            return null;
        }

        if (obj is GameObject prefab)
            return Instantiate(prefab, pos, rot);

        Debug.LogError(name + " is not a GameObject!");
        return null;
    }

    /// <summary>
    /// Get ANY asset type from cache
    /// </summary>
    public static T Get<T>(string name) where T : Object
    {
        if (Instance.loadedAssets.TryGetValue(name, out var obj))
            return obj as T;

        return null;
    }
}
