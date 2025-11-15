using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeManager : MonoBehaviour
{
    private static FadeManager instance = null;
    public static FadeManager Instance()
    {
        return instance;
    }

    public void Awake()
    {
        instance = this;
    }

    public void OnStart(string sceneName, bool isAsync = false)
    {
        KeyCodeManager.Instance().RemoveInputActionList();

        SceneManager.LoadScene(sceneName);
    }
}
