using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField]
    private FadeManager m_fadeManager = null;

    [SerializeField]
    private AudioSourceManager m_audioManager = null;

    private DriveService service;
    private List<Google.Apis.Drive.v3.Data.File> files = new List<Google.Apis.Drive.v3.Data.File>();

    private int fileId = 0;
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

        if (!Directory.Exists(ConstManager.DIRECTORY_FULL_PATH_TO_BUNDLES))
        {
            Directory.CreateDirectory(ConstManager.DIRECTORY_FULL_PATH_TO_BUNDLES);
        }

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
        do
        {
            var result = request.Execute();
            files.AddRange(result.Files);
            request.PageToken = result.NextPageToken;
        }
        while (!string.IsNullOrEmpty(request.PageToken));
        fileId = 0;
    }

    private void Update()
    {
        timeElapsed += Time.deltaTime;
        if (timeElapsed < timeOut)
        {
            return;
        }
        timeElapsed = 0.0f;

        if (fileId >= files.Count)
        {
            StartCoroutine("ReadFileList");
            FadeManager.Instance().OnStart("HomeScene");
            return;
        }

        DownloadFileList();
    }

    public IEnumerable ReadFileList()
    {
        string[] fs = System.IO.Directory.GetFiles(ConstManager.DIRECTORY_FULL_PATH_TO_BUNDLES, "*_spriteatlas.assetbundle", System.IO.SearchOption.TopDirectoryOnly);
        foreach (string file in fs)
        {
            Debug.Log("folderPath : " + file);
            AssetBundle asset_bundle = AssetBundle.LoadFromFile(file);
            var request = asset_bundle.LoadAllAssetsAsync();
            request.completed += (operation) =>
            {
                if (request.asset.GetType() == typeof(UnityEngine.U2D.SpriteAtlas))
                {
                    var atlas = (UnityEngine.U2D.SpriteAtlas)request.asset;
                    Debug.Log("spriteCount : " + atlas.spriteCount);
                }
            };

            while (!request.isDone)
            {
                yield return 0;
            }
        }
    }

    public void DownloadFileList()
    {
        if (isDownload)
        {
            return;
        }

        isDownload = true;

        var file = files[fileId];
        fileId++;

        if (File.Exists(ConstManager.DIRECTORY_FULL_PATH_TO_BUNDLES + file.Name))
        {
            isDownload = false;
            return;
        }

        Debug.Log("Name: " + file.Name + " ID: " + file.Id + " Size: " + file.Size + "byte CreatedTime: " + file.CreatedTimeDateTimeOffset);
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
    }

    public void DownloadProgress(IDownloadProgress dlP)
    {
        isDownload = dlP.Status != DownloadStatus.Completed;
        Debug.Log("Size:" + (dlP.BytesDownloaded / fileSize) * 100);
    }
}
