using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using static UnityEngine.Networking.UnityWebRequest;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField]
    private FadeManager m_fadeManager = null;

    [SerializeField]
    private AudioSourceManager m_audioManager = null;

    [SerializeField]
    private AssetBundleManager m_assetBundleManager = null;

    [SerializeField]
    private ScrollRect m_scrollRect = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI m_textMeshPro = null;

    [SerializeField]
    private List<Image> m_images = new List<Image>();

    private int imageIndex = 0;

    private List<Google.Apis.Drive.v3.Data.File> files = new List<Google.Apis.Drive.v3.Data.File>();

    public float timeOut = 0.1f;
    private float timeElapsed;

    void Start()
    {
        if (FadeManager.Instance() == null)
        {
            Instantiate(m_fadeManager);
        }

        if (AudioSourceManager.Instance() == null)
        {
            Instantiate(m_audioManager);
        }

        if (AssetBundleManager.Instance() == null)
        {
            Instantiate(m_assetBundleManager);
        }

        string path = ConstManager.DIRECTORY_FULL_PATH_TO_EXE;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        path = ConstManager.DIRECTORY_FULL_PATH_TO_BUNDLES;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        OptionData optionData = new OptionData();
        optionData.IsFileExists();
        if (!optionData.IsFileExists())
        {
            optionData.name = "Player";
            optionData.SaveTxt();
        }
        else
        {
            optionData.LoadTxt();
            optionData.SaveTxt();
        }

        path = ConstManager.DIRECTORY_FULL_PATH_TO_OPTION + "favoriteCardData.json";
        if (!File.Exists(path))
        {
            FavoriteData data = new FavoriteData();
            StreamWriter streamWriter = new StreamWriter(path);
            streamWriter.WriteLine(JsonUtility.ToJson(data));
            streamWriter.Close();
        }

        path = ConstManager.DIRECTORY_FULL_PATH_TO_OPTION + "DLlock.json";
        if (File.Exists(path))
        {
            StartCoroutine(AssetBundleManager.Instance().ReadFileList(() => {
                AssetBundleManager.Instance().SetCardTextList();
                FadeManager.Instance().OnStart("HomeScene");
            }, (string str) => {
                m_textMeshPro.text += str;
                m_scrollRect.verticalNormalizedPosition = 0f;
            }));
        }
        else
        {
            StartCoroutine(RequesFileList(
                optionData.apiPath,
                () => {
                    StartCoroutine(AssetBundleManager.Instance().ReadFileList(() => {
                        AssetBundleManager.Instance().SetCardTextList();
                        FadeManager.Instance().OnStart("HomeScene");
                    }, (string str) => {
                        m_textMeshPro.text += str;
                        m_scrollRect.verticalNormalizedPosition = 0f;
                    }));
                }));
        }
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed < timeOut)
        {
            return;
        }
        timeElapsed = 0.0f;

        if (imageIndex >= m_images.Count)
        {
            imageIndex = 0;
            foreach (var image in m_images)
            {
                image.gameObject.SetActive(false);
            }
        }
        m_images[imageIndex].gameObject.SetActive(true);
        imageIndex++;
    }


    public IEnumerator RequesFileList(string apiPath, Action action)
    {
        while (UnityEngine.Application.internetReachability == NetworkReachability.NotReachable)
        {
            m_textMeshPro.text += "Connect Network ..." + "\n";
            yield return new WaitForSeconds(1f);
        }

        UnityWebRequest req = UnityWebRequest.Get(apiPath);
        yield return req.SendWebRequest();

        if (req.result == Result.Success)
        {
            Debug.Log(req.downloadHandler.text);

            ApiResponseDataToFileList apiResponseData = JsonUtility.FromJson<ApiResponseDataToFileList>(req.downloadHandler.text);

            foreach (var fileData in apiResponseData.res)
            {
                m_textMeshPro.text += "UnityWebRequestPath : " + fileData.url + "\n";

                foreach (var data in fileData.list.Split(","))
                {
                    string directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_BUNDLES;
                    if (Path.GetExtension(data) == ".exe")
                    {
                        directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_EXE;
                    }

                    if (File.Exists(directoryPath + data))
                    {
                        continue;
                    }

                    m_textMeshPro.text += "DownloadFilePath : " + data + "\n";

                    UnityWebRequest fileReq = UnityWebRequest.Get(fileData.url + "/" + data);
                    yield return fileReq.SendWebRequest();

                    if (fileReq.result == Result.Success)
                    {
                        File.WriteAllBytes(directoryPath + data, fileReq.downloadHandler.data);
                    }
                }
            }

            action();
        }
        else
        {
            m_textMeshPro.text += "Is Completed Error " + apiPath + "\n";
            m_textMeshPro.text += req.error + "\n";
        }
    }
}

[System.Serializable]
public class ApiResponseDataToFileList
{
    public FileData[] res;
}

[System.Serializable]
public class FileData
{
    public string url;
    public string list;
}
