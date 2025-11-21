using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BundleLoader : MonoBehaviour
{
    private AssetBundle stageBundle;

    public IEnumerator Start()
    {
        string path = Path.Combine(Application.streamingAssetsPath + "/StandaloneWindows64", "stage");

        var bundleLoadRequest = AssetBundle.LoadFromFileAsync(path);
        yield return bundleLoadRequest;

        stageBundle = bundleLoadRequest.assetBundle;

        SceneManager.LoadScene("ControllerAssignment");
    }
}
