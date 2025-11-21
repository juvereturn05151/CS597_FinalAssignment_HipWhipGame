using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BundleLoader : MonoBehaviour
{
    private void Start() 
    {
#if UNITY_EDITOR_64 || UNITY_STANDALONE_WIN
        StartCoroutine(LoadAssetBundlePC());
        FadingUI.Instance.StartFadeIn();
        FadingUI.Instance.OnStopFading.AddListener(LoadControllerAssignmentScene);
#elif UNITY_SWITCH

#endif
    }

    public IEnumerator LoadAssetBundlePC()
    {
        string path = Path.Combine(Application.streamingAssetsPath + "/StandaloneWindows64", "stage");

        var bundleLoadRequest = AssetBundle.LoadFromFileAsync(path);
        yield return bundleLoadRequest;


    }

    private void LoadControllerAssignmentScene()
    {
        SceneManager.LoadScene("MenuScene");
    }


}
