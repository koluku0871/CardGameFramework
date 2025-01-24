using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static DeckManager;

public class HomeSceneManager : MonoBehaviour
{
    [SerializeField]
    private TMPro.TMP_InputField m_nameInputField = null;

    [SerializeField]
    private FadeManager m_fadeManager = null;

    [SerializeField]
    private GameObject m_audioManager = null;

    private void Start()
    {
        if (FadeManager.Instance() == null)
        {
            Instantiate(m_fadeManager);
        }

        if (AudioSourceManager.Instance() == null)
        {
            Instantiate(m_audioManager);
        }

        AudioSourceManager.Instance().PlayOneShot((int)AudioSourceManager.BGM_NUM.HOME_1, true);

        var directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_OPTION;

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        string[] deckFiles = Directory.GetFiles(directoryPath, "*.json", SearchOption.AllDirectories);
        try
        {
            StreamReader sr = new StreamReader(deckFiles[0], Encoding.UTF8);
            OptionData optionData = JsonUtility.FromJson<OptionData>(sr.ReadToEnd());
            sr.Close();
            m_nameInputField.text = optionData.name;
            if (string.IsNullOrEmpty(m_nameInputField.text))
            {
                m_nameInputField.text = "Player";
            }
        }
        catch
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
