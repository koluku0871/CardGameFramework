
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TitleSceneManager : MonoBehaviour
{
    [SerializeField]
    private AudioSourceManager m_audioManager = null;

    [SerializeField]
    private AssetBundleManager m_assetBundleManager = null;

    [SerializeField]
    private MouseManager m_mouseManager = null;

    [SerializeField]
    private ScrollRect m_scrollRect = null;

    [SerializeField]
    public TMPro.TextMeshProUGUI m_textMeshPro = null;

    [SerializeField]
    private List<Image> m_images = new List<Image>();

    private int imageIndex = 0;

    public float timeOut = 0.1f;
    private float timeElapsed;

    private Process _process;
    private static ApiResponseDataToFileList apiResponseData = null;

    private static TitleSceneManager instance = null;
    public static TitleSceneManager Instance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Application.targetFrameRate = 120;

        if (AudioSourceManager.Instance() == null)
        {
            Instantiate(m_audioManager);
        }

        if (MouseManager.Instance() == null)
        {
            Instantiate(m_mouseManager);
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

        path = ConstManager.DIRECTORY_FULL_PATH_TO_LOG;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        path = ConstManager.DIRECTORY_FULL_PATH_TO_HELP;
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        path = ConstManager.DIRECTORY_FULL_PATH_TO_SAMPLEDECK;
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
        AssetBundleManager.Instance().CardType = optionData.cardType;

        path = ConstManager.DIRECTORY_FULL_PATH_TO_OPTION + "favoriteCardData.json";
        if (!File.Exists(path))
        {
            FavoriteData data = new FavoriteData();
            StreamWriter streamWriter = new StreamWriter(path);
            streamWriter.WriteLine(JsonUtility.ToJson(data));
            streamWriter.Close();
        }

        path = ConstManager.DIRECTORY_PATH + "/ResLoad.exe";
        if (File.Exists(path))
        {
            _process = new Process();

            // プロセスを起動するときに使用する値のセットを指定
            _process.StartInfo = new ProcessStartInfo
            {
                FileName = path,
                UseShellExecute = false,
                WorkingDirectory = ConstManager.DIRECTORY_PATH,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                CreateNoWindow = true,
            };

            _process.OutputDataReceived += OnStandardOut;

            _process.EnableRaisingEvents = true;
            _process.Exited += DisposeProcess;

            _process.Start();
            _process.BeginOutputReadLine();
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
            StartCoroutine(RequesFileList(() => {
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

    private static void OnStandardOut(object sender, DataReceivedEventArgs e)
    {
        if (string.IsNullOrEmpty(e.Data))
        {
            return;
        }

        try
        {
            apiResponseData = JsonUtility.FromJson<ApiResponseDataToFileList>(e.Data);
        }
        catch
        {
            TitleSceneManager.Instance().m_textMeshPro.text += "DownloadFilePath : " + e.Data + "\n";
        }
        UnityEngine.Debug.Log(e.Data);
    }

    private void DisposeProcess(object sender, EventArgs e)
    {
        if (_process == null || _process.HasExited) return;

        _process.StandardInput.Close();
        _process.CloseMainWindow();
        _process.Dispose();
        _process = null;
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


    public IEnumerator RequesFileList(Action action)
    {
        string directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_BUNDLES;
        while (UnityEngine.Application.internetReachability == NetworkReachability.NotReachable)
        {
            m_textMeshPro.text += "Connect Network ..." + "\n";
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitUntil(() => apiResponseData != null);
        AssetBundleManager.Instance().apiResponseData = apiResponseData;

        List<string> pathList = new List<string>();
        foreach (var fileData in apiResponseData.res)
        {
            m_textMeshPro.text += "UnityWebRequestPath : " + fileData.url + "\n";

            foreach (var data in fileData.list.Split(","))
            {
                if (string.IsNullOrEmpty(data))
                {
                    continue;
                }

                directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_BUNDLES;
                if (Path.GetExtension(data) == ".exe")
                {
                    directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_EXE;
                }
                else if(Path.GetExtension(data) == ".help")
                {
                    directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_HELP;
                }
                else if (Path.GetExtension(data) == ".sample")
                {
                    directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_SAMPLEDECK;
                }

                    string key = Path.GetFileNameWithoutExtension(directoryPath + data);
                pathList.Add(key);

                if (File.Exists(directoryPath + data))
                {
                    continue;
                }

                m_textMeshPro.text += "DownloadFilePath : " + data + "\n";
                UnityEngine.Debug.Log(fileData.url + data);

                using var fileStream = File.OpenWrite(directoryPath + data);
                UnityWebRequest req = UnityWebRequest.Get(fileData.url + data);
                yield return req.WriteToStreamAsync(fileStream);

                while (!req.isDone)
                {
                    yield return new WaitForSeconds(1f);
                }
            }
        }

        directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_BUNDLES;
        string[] assetbundleFiles = Directory.GetFiles(directoryPath, "*.assetbundle", SearchOption.AllDirectories);

        for (int index = 0; index < assetbundleFiles.Length; index++)
        {
            string key = Path.GetFileNameWithoutExtension(assetbundleFiles[index]);
            if (pathList.Contains(key))
            {
                continue;
            }
            m_textMeshPro.text += "DeleteFilePath : " + assetbundleFiles[index] + "\n";
            UnityEngine.Debug.Log(assetbundleFiles[index]);
            File.Delete(assetbundleFiles[index]);
        }

        directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_EXE;
        string[] exeFiles = Directory.GetFiles(directoryPath, "*.exe", SearchOption.AllDirectories);

        for (int index = 0; index < exeFiles.Length; index++)
        {
            string key = Path.GetFileNameWithoutExtension(exeFiles[index]);
            if (pathList.Contains(key))
            {
                continue;
            }
            m_textMeshPro.text += "DeleteFilePath : " + exeFiles[index] + "\n";
            UnityEngine.Debug.Log(exeFiles[index]);
            File.Delete(exeFiles[index]);
        }

        action();
    }
}


