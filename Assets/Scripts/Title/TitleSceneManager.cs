using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
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

        path = ConstManager.DIRECTORY_FULL_PATH_TO_OPTION;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        path = ConstManager.DIRECTORY_FULL_PATH_TO_OPTION + "favoriteCardData.json";
        if (!File.Exists(path))
        {
            FavoriteData data = new FavoriteData();
            StreamWriter streamWriter = new StreamWriter(path);
            streamWriter.WriteLine(JsonUtility.ToJson(data));
            streamWriter.Close();
        }

        OptionData optionData = new OptionData();
        path = ConstManager.DIRECTORY_FULL_PATH_TO_OPTION + "opt.json";
        if (!File.Exists(path))
        {
            optionData.name = "Player";

            StreamWriter streamWriter = new StreamWriter(path);
            streamWriter.WriteLine(JsonUtility.ToJson(optionData));
            streamWriter.Close();
        }
        else
        {
            StreamReader streamReader = new StreamReader(path, Encoding.UTF8);
            optionData = JsonUtility.FromJson<OptionData>(streamReader.ReadToEnd());
            streamReader.Close();

            StreamWriter streamWriter = new StreamWriter(path);
            streamWriter.WriteLine(JsonUtility.ToJson(optionData));
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

            /*StartCoroutine(DownloadFileList(
                ConstManager.GOOGLE_DRIVE_FOLDER_ID_TO_EXE,
                ConstManager.DIRECTORY_FULL_PATH_TO_EXE,
                () => {
                    StartCoroutine(DownloadFileList(
                    ConstManager.GOOGLE_DRIVE_FOLDER_ID_TO_CARD,
                    ConstManager.DIRECTORY_FULL_PATH_TO_BUNDLES,
                    () => {
                        StartCoroutine(AssetBundleManager.Instance().ReadFileList(() => {
                            AssetBundleManager.Instance().SetCardTextList();
                            FadeManager.Instance().OnStart("HomeScene");
                        }, (string str) => {
                            m_textMeshPro.text += str;
                            m_scrollRect.verticalNormalizedPosition = 0f;
                        }));
                    }));
                }));*/
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

    private DriveService CreateDriveService()
    {
        GoogleCredential credential;
        using (var stream = new FileStream(ConstManager.JSON_FILE_PATH_TO_GOOGLE_DRIVE, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(DriveService.ScopeConstants.Drive);
        }
        // Drive APIのサービスを作成
        DriveService service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "Google Drive Sample",
        });
        return service;
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

    // TODO Googleドライブへのアクセスをやめたので破棄
    /*public IEnumerator DownloadFileList(string folderId, string dlPath, Action action)
    {
        DriveService service = CreateDriveService();

        var request = service.Files.List();
        request.Q = "'" + folderId + "' in parents";
        request.Fields = "nextPageToken, files(id, name, size, createdTime)";
        files = new List<Google.Apis.Drive.v3.Data.File>();

        while (files.Count == 0 || !string.IsNullOrEmpty(request.PageToken))
        {
            var result = request.ExecuteAsync();
            while (!result.IsCompleted)
            {
                yield return null;
            }

            if (result.IsCompletedSuccessfully)
            {
                files.AddRange(result.Result.Files);
                request.PageToken = result.Result.NextPageToken;
            }
            else
            {
                m_textMeshPro.text += "Is Completed Error " + folderId;
            }
        }

        m_textMeshPro.text += "DownloadFilePath : " + dlPath + "\n";
        m_textMeshPro.text += "DownloadFileList : " + files.Count + "\n";
        yield return null;

        foreach (var file in files)
        {
            isDownload = true;

            if (File.Exists(dlPath + file.Name))
            {
                isDownload = false;
                continue;
            }

            var str = "Name: " + file.Name + " ID: " + file.Id + " Size: " + file.Size + "byte CreatedTime: " + file.CreatedTimeDateTimeOffset;
            Debug.Log(str);
            m_textMeshPro.text += "DownloadFile : " + str + "\n";
            m_scrollRect.verticalNormalizedPosition = 0f;
            yield return null;

            fileSize = 0;
            if (file.Size.HasValue)
            {
                fileSize = file.Size.Value;
            }

            var serviceRequest = service.Files.Get(file.Id);
            var fileStream = new FileStream(Path.Combine(dlPath, file.Name), FileMode.Create, FileAccess.Write);
            serviceRequest.MediaDownloader.ProgressChanged += DownloadProgress;
            serviceRequest.Download(fileStream);
            fileStream.Close();

            while (isDownload)
            {
                yield return null;
            }
        }

        action();
    }

    public void DownloadProgress(IDownloadProgress dlP)
    {
        isDownload = dlP.Status != DownloadStatus.Completed;
        Debug.Log("Size:" + (dlP.BytesDownloaded / fileSize) * 100);
    }*/
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
