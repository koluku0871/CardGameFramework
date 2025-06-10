using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;

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
    private TMPro.TMP_Dropdown m_updateDropdown = null;

    [SerializeField]
    private UnityEngine.UI.Button m_updateButton = null;

    private void Start()
    {
        AudioSourceManager.Instance().PlayOneShot((int)AudioSourceManager.BGM_NUM.HOME_1, true);

        var directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_OPTION;

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string[] optFiles = Directory.GetFiles(directoryPath, "opt.json", SearchOption.AllDirectories);
        StreamReader sr = new StreamReader(optFiles[0], Encoding.UTF8);
        OptionData optionData = JsonUtility.FromJson<OptionData>(sr.ReadToEnd());
        sr.Close();
        m_nameInputField.text = optionData.name;
        if (string.IsNullOrEmpty(m_nameInputField.text))
        {
            m_nameInputField.text = "Player";
        }

        m_typeDropdown.ClearOptions();
        foreach (var type in AssetBundleManager.Instance().AssetBundleBaseCardDataKeys)
        {
            m_typeDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData()
            {
                text = type
            });
        }
        //m_packTypeDropdown.value = m_packTypeDropdown.options.Count - 1;
        m_typeDropdown.value = -1;

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

            if (update == Application.version)
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
            Process process = new Process();
            process.StartInfo.FileName = fileName;
            process.Start();

            Application.Quit();
        });

        m_nameText.text = m_nameInputField.text;

        m_versionText.text = "v " + Application.version;
    }

    public void OnClickToRoomButton() {
        FadeManager.Instance().OnStart("RoomScene");
    }

    public void OnClickToDeckButton() {
        //SceneManager.LoadScene("DeckScene");
        FadeManager.Instance().OnStart("DeckScene");
    }

    public void OnClickToCloseButton() {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #elif UNITY_STANDALONE
            UnityEngine.Application.Quit();
        #endif
    }

    public void OnClickToOptionCloseButton()
    {
        if (string.IsNullOrEmpty(m_nameInputField.text))
        {
            return;
        }

        var directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_OPTION;

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        OptionData optionData = new OptionData();
        optionData.name = m_nameInputField.text;
        m_nameText.text = m_nameInputField.text;
        optionData.cardType = m_typeDropdown.options[m_typeDropdown.value].text;
        StreamWriter streamWriter = new StreamWriter(directoryPath + "opt.json");
        streamWriter.WriteLine(JsonUtility.ToJson(optionData));
        streamWriter.Close();
    }
}
