using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class HomeSceneManager : MonoBehaviour
{
    [SerializeField]
    private TMPro.TextMeshProUGUI m_nameText = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI m_versionText = null;

    [SerializeField]
    private TMPro.TMP_InputField m_nameInputField = null;

    [SerializeField]
    private TMPro.TMP_Dropdown m_typeDropdown = null;

    [SerializeField]
    private UnityEngine.UI.Slider m_homeBgmSlider = null;

    [SerializeField]
    private UnityEngine.UI.Slider m_battleBgmSlider = null;

    [SerializeField]
    private UnityEngine.UI.Slider m_deckBgmSlider = null;

    [SerializeField]
    private UnityEngine.UI.Slider m_seSlider = null;

    [SerializeField]
    private TMPro.TMP_Dropdown m_updateDropdown = null;

    [SerializeField]
    private UnityEngine.UI.Button m_updateButton = null;

    [Header("リソース再ダウンロード")]

    [SerializeField]
    private UnityEngine.UI.Toggle m_copyToggle = null;

    [SerializeField]
    private GameObject m_reDlWindowContent = null;

    [SerializeField]
    private Dictionary<string, UnityEngine.UI.Toggle> m_reDlWindowToggleList = new Dictionary<string, UnityEngine.UI.Toggle>();

    private void Start()
    {
        AudioSourceManager.Instance().PlayOneShot((int)AudioSourceManager.BGM_NUM.HOME_1, true);

        OptionData optionData = new OptionData();
        optionData.IsFileExists();
        optionData.LoadTxt();

        m_nameInputField.text = optionData.name;
        if (string.IsNullOrEmpty(m_nameInputField.text))
        {
            m_nameInputField.text = "Player";
        }

        m_typeDropdown.ClearOptions();
        int typeIndex = 0;
        foreach (var type in AssetBundleManager.Instance().AssetBundleBaseCardDataKeys)
        {
            m_typeDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData()
            {
                text = type
            });
            if (type == optionData.cardType)
            {
                m_typeDropdown.value = typeIndex;
                AssetBundleManager.Instance().CardType = type;
            }
            typeIndex++;
        }
        //m_packTypeDropdown.value = m_packTypeDropdown.options.Count - 1;
        //m_typeDropdown.value = -1;

        m_homeBgmSlider.value = optionData.homeBgmVolume;
        m_battleBgmSlider.value = optionData.battleBgmVolume;
        m_deckBgmSlider.value = optionData.deckBgmVolume;
        m_seSlider.value = optionData.seVolume;

        m_updateDropdown.ClearOptions();
        int index = 0;
        int selectIndex = 0;
        string[] fs = Directory.GetFiles(ConstManager.DIRECTORY_FULL_PATH_TO_EXE, "CardGameFrameworkSetup_*", SearchOption.TopDirectoryOnly);
        foreach (string file in fs)
        {
            string key = Path.GetFileNameWithoutExtension(file);
            string update = key.Split('_')[1];
            m_updateDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData()
            {
                text = update
            });

            if (update == UnityEngine.Application.version)
            {
                selectIndex = index;
            }

            index++;
        }
        //m_packTypeDropdown.value = m_packTypeDropdown.options.Count - 1;
        
        m_updateDropdown.value = selectIndex;

        m_updateButton.onClick.AddListener(() => {
            string fileName = ConstManager.DIRECTORY_FULL_PATH_TO_EXE + "CardGameFrameworkSetup_" + m_updateDropdown.options[m_updateDropdown.value].text + ".exe";
#if UNITY_EDITOR
            UnityEngine.Debug.Log(fileName);
            return;
#endif
#pragma warning disable CS0162 // 到達できないコードが検出されました
            Process process = new Process();
#pragma warning restore CS0162 // 到達できないコードが検出されました
            process.StartInfo.FileName = fileName;
            process.Start();

            UnityEngine.Application.Quit();
        });

        m_nameText.text = m_nameInputField.text;

        m_versionText.text = "v " + UnityEngine.Application.version;

        foreach (var fileData in AssetBundleManager.Instance().apiResponseData.res)
        {
            foreach (var data in fileData.list.Split(","))
            {
                UnityEngine.UI.Toggle toggle = Instantiate<UnityEngine.UI.Toggle>(m_copyToggle, m_copyToggle.transform.position, Quaternion.identity);
                toggle.transform.SetParent(m_reDlWindowContent.transform);
                toggle.GetComponent<RectTransform>().localScale = Vector3.one;
                toggle.transform.Find("Text (TMP)").GetComponent<TMPro.TextMeshProUGUI>().text = data;
                toggle.gameObject.SetActive(true);
                m_reDlWindowToggleList.Add(fileData.url + data, toggle);
            }
        }
    }

    public void OnClickToRoomButton() {
        FadeManager.Instance().OnStart("RoomScene");
    }

    public void OnClickToDeckButton() {
        //SceneManager.LoadScene("DeckScene");
        FadeManager.Instance().OnStart("DeckScene");
    }

    public void OnClickToRestartButton()
    {
#if UNITY_EDITOR
#elif UNITY_STANDALONE_WIN
        System.Diagnostics.Process.Start(Application.dataPath.Replace("_Data", ".exe"));
        UnityEngine.Application.Quit();
#endif
    }

    public void OnClickToCloseButton() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE_WIN
        UnityEngine.Application.Quit();
#endif
    }

    private Coroutine reDlCoroutine = null;
    public void OnClickToReDlButton()
    {
        if (reDlCoroutine != null) return; 

        reDlCoroutine = StartCoroutine(ReDlCoroutine());
    }

    public IEnumerator ReDlCoroutine()
    {
        foreach (var reDlWindowToggle in m_reDlWindowToggleList)
        {
            if (!reDlWindowToggle.Value.isOn)
            {
                continue;
            }

            var data = reDlWindowToggle.Value.transform.Find("Text (TMP)").GetComponent<TMPro.TextMeshProUGUI>().text;
            var directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_BUNDLES;
            if (Path.GetExtension(data) == ".exe")
            {
                directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_EXE;
            }

            using var fileStream = File.OpenWrite(directoryPath + data);
            UnityWebRequest req = UnityWebRequest.Get(reDlWindowToggle.Key);
            yield return req.WriteToStreamAsync(fileStream);

            while (!req.isDone)
            {
                yield return new WaitForSeconds(1f);
            }
        }

        OnClickToRestartButton();
    }

    public void OnClickToOptionCloseButton()
    {
        if (string.IsNullOrEmpty(m_nameInputField.text))
        {
            return;
        }

        OptionData optionData = new OptionData();
        optionData.IsFileExists();
        optionData.LoadTxt();

        optionData.name = m_nameInputField.text;
        m_nameText.text = m_nameInputField.text;
        optionData.cardType = m_typeDropdown.options[m_typeDropdown.value].text;
        AssetBundleManager.Instance().CardType = optionData.cardType;

        optionData.homeBgmVolume = m_homeBgmSlider.value;
        optionData.battleBgmVolume = m_battleBgmSlider.value;
        optionData.deckBgmVolume = m_deckBgmSlider.value;
        optionData.seVolume = m_seSlider.value;

        optionData.SaveTxt();
    }
}
