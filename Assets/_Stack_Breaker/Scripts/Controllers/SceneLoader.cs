using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class SceneLoader : MonoBehaviour {

    private static string targetScene = string.Empty;

    private void Start()
    {
        ViewManager.Instance.OnLoadingSceneDone(SceneManager.GetActiveScene().name);
        StartCoroutine(CRRunningText());
    }

    public void LoadScene(float delay)
    {
        StartCoroutine(LoadingScene(delay));
    }


    private IEnumerator CRRunningText()
    {
        int temp = 0;
        while (true)
        {
            temp++;
            if (temp == 1)
                ViewManager.Instance.LoadingViewController.SetLoadingText("LOADING.");
            else if (temp == 2)
                ViewManager.Instance.LoadingViewController.SetLoadingText("LOADING..");
            else
            {
                ViewManager.Instance.LoadingViewController.SetLoadingText("LOADING...");
                temp = 0;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator LoadingScene(float delay)
    {
        yield return new WaitForSeconds(delay);
        AsyncOperation asyn = SceneManager.LoadSceneAsync(targetScene);
        while (!asyn.isDone)
        {
            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// Set target scene.
    /// </summary>
    /// <param name="sceneName"></param>
    public static void SetTargetScene(string sceneName)
    {
        targetScene = sceneName;
    }
}
