using UnityEditor;

#if UNITY_EDITOR

public class BuildBundles
{
    [MenuItem("Build/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string path = "Assets/AssetBundles/Windows";
        BuildPipeline.BuildAssetBundles(path,
            BuildAssetBundleOptions.ChunkBasedCompression,
            BuildTarget.StandaloneWindows64);
    }

    [MenuItem("Build/Build AssetBundles Switch")]
    static void BuildAllAssetBundlesSwitch()
    {
        string path = "Assets/AssetBundles/Switch";
        BuildPipeline.BuildAssetBundles(path,
            BuildAssetBundleOptions.ChunkBasedCompression,
            BuildTarget.Switch);
    }
}
#endif