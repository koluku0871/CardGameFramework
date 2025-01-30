using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

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

    private DriveService service;
    private List<Google.Apis.Drive.v3.Data.File> files = new List<Google.Apis.Drive.v3.Data.File>();

    private bool isDownload = false;
    private long fileSize = 0;

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

        string path = ConstManager.DIRECTORY_FULL_PATH_TO_BUNDLES;
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

        path = ConstManager.DIRECTORY_FULL_PATH_TO_OPTION + "opt.json";
        if (!File.Exists(path))
        {
            OptionData optionData = new OptionData();
            optionData.name = "Player";
            StreamWriter streamWriter = new StreamWriter(path);
            streamWriter.WriteLine(JsonUtility.ToJson(optionData));
            streamWriter.Close();
        }

        StartCoroutine(DownloadFileList(() => {
            StartCoroutine(AssetBundleManager.Instance().ReadFileList(() => {
                AssetBundleManager.Instance().SetCardTextList();
                FadeManager.Instance().OnStart("HomeScene");
            }, (string str) => {
                m_textMeshPro.text += str;
                m_scrollRect.verticalNormalizedPosition = 0f;
            }));
        }));
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

    public IEnumerator DownloadFileList(Action action)
    {
        GoogleCredential credential;
        using (var stream = new FileStream(ConstManager.JSON_FILE_PATH_TO_GOOGLE_DRIVE, FileMode.Open, FileAccess.Read))
        {
            credential = GoogleCredential.FromStream(stream).CreateScoped(DriveService.ScopeConstants.Drive);
        }
        // Drive APIのサービスを作成
        service = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "Google Drive Sample",
        });

        var request = service.Files.List();
        request.Q = "'" + ConstManager.GOOGLE_DRIVE_FOLDER_ID + "' in parents";
        request.Fields = "nextPageToken, files(id, name, size, createdTime)";
        files = new List<Google.Apis.Drive.v3.Data.File>();

        while (files.Count == 0 || !string.IsNullOrEmpty(request.PageToken))
        {
            var result = request.ExecuteAsync();
            while (!result.IsCompleted)
            {
                yield return null;
            }

            files.AddRange(result.Result.Files);
            request.PageToken = result.Result.NextPageToken;
        }

        m_textMeshPro.text += "DownloadFileList : " + files.Count + "\n";
        yield return null;

        foreach (var file in files)
        {
            isDownload = true;

            if (File.Exists(ConstManager.DIRECTORY_FULL_PATH_TO_BUNDLES + file.Name))
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
            var fileStream = new FileStream(Path.Combine(ConstManager.DIRECTORY_FULL_PATH_TO_BUNDLES, file.Name), FileMode.Create, FileAccess.Write);
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
    }
}
