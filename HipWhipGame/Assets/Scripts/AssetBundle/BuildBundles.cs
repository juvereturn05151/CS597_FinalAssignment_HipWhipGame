using UnityEditor;

#if UNITY_EDITOR

public class BuildBundles
{
    [MenuItem("Build/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string path = "Assets/StreamingAssets/StandaloneWindows64";
        BuildPipeline.BuildAssetBundles(path,
            BuildAssetBundleOptions.ChunkBasedCompression,
            BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Build/Build AssetBundles Switch")]
    static void BuildAllAssetBundlesSwitch()
    {
        string path = "Assets/StreamingAssets/Switch";
        BuildPipeline.BuildAssetBundles(path,
            BuildAssetBundleOptions.ChunkBasedCompression,
            BuildTarget.Switch);
    }
}
#endif