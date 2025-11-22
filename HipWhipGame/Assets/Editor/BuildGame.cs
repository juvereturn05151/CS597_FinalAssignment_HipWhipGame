using UnityEditor;
using UnityEngine;

public static class BuildGame
{
    [MenuItem("Build/Build Game on Windows")]
    public static void BuildWindows()
    {
        string buildPath = "Builds/Windows/HipWhipGame.exe";

        BuildPlayerOptions options = new BuildPlayerOptions()
        {
            scenes = new[] {
                "Assets/Scenes/BootScene.unity",
                "Assets/Scenes/MenuScene.unity",
                "Assets/Scenes/ControllerAssignment.unity"
            },
            locationPathName = buildPath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None
        };

        var report = BuildPipeline.BuildPlayer(options);

        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            throw new System.Exception("Build failed! Check the console.");
        }

        Debug.Log("Build Successful!");
    }

    [MenuItem("Build/Build Game on Switch")]
    public static void BuildSwitch()
    {
        string buildPath = "Builds/Switch/HipWhipGame.nsp";

        BuildPlayerOptions options = new BuildPlayerOptions()
        {
            scenes = new[] {
                "Assets/Scenes/BootScene.unity",
                "Assets/Scenes/MenuScene.unity",
                "Assets/Scenes/ControllerAssignment.unity"
            },
            locationPathName = buildPath,
            target = BuildTarget.Switch,
            options = BuildOptions.None
        };

        var report = BuildPipeline.BuildPlayer(options);

        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
        {
            throw new System.Exception("Build failed! Check the console.");
        }

        Debug.Log("Build Successful!");
    }
}
