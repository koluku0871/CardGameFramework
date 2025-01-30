using System.IO;
using System.Text;
using UnityEngine;

public class HomeSceneManager : MonoBehaviour
{
    [SerializeField]
    private TMPro.TMP_InputField m_nameInputField = null;

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
        StreamWriter streamWriter = new StreamWriter(directoryPath + "opt.json");
        streamWriter.WriteLine(JsonUtility.ToJson(optionData));
        streamWriter.Close();
    }
}
