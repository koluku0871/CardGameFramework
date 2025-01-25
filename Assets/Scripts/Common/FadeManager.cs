using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    [SerializeField]
    Fade fade = null;

    private static FadeManager instance = null;
    public static FadeManager Instance()
    {
        return instance;
    }

    public void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void OnStart(string sceneName, bool isAsync = false)
    {
        if (isAsync)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false;

        fade.FadeIn(0.5f, () =>
        {
            asyncLoad.allowSceneActivation = true;
        });
        
        while (!asyncLoad.isDone)
        {
            Debug.Log(asyncLoad.progress * 100f + "%ì«Ç›çûÇ›äÆóπ");
            yield return null;
        }

        fade.FadeOut(0.5f);
    }
}
