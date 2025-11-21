using System;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;

public class SpeechManager : MonoBehaviour
{
    [SerializeField]
    private Image m_speechImage = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI m_speechText = null;

    public DictationRecognizer m_dictationRecognizer;

    public Action<string> m_action = null;

    private static SpeechManager instance = null;
    public static SpeechManager Instance()
    {
        return instance;
    }

    public void OnEnable()
    {
        instance = this;
    }

    private void Start()
    {
        m_dictationRecognizer = new DictationRecognizer();
        m_dictationRecognizer.DictationResult += DictationResult;
        m_dictationRecognizer.DictationError += DictationError;
        m_dictationRecognizer.DictationComplete += DictationComplete;
    }

    private void OnDestroy()
    {
        m_dictationRecognizer.Stop();
        m_dictationRecognizer.Dispose();
    }

    private void DictationResult(string text, ConfidenceLevel confidence)
    {
        Debug.Log($"DictationResultÅF {text}");
        m_speechText.text = text;
        if (m_action != null)
        {
            m_action(text);
        }
    }

    private void DictationError(string error, int hresult)
    {
        Debug.LogError($"DictationErrorÅF{error}, {hresult}");
        m_speechText.text = error + " : " + hresult;
    }

    private void DictationComplete(DictationCompletionCause cause)
    {
        Debug.Log($"DictationComplete");
        m_speechText.text = "";
        m_speechImage.gameObject.SetActive(false);
        m_dictationRecognizer.Stop();
    }

    public void ChangeDictationRecognizer()
    {
        if (m_dictationRecognizer.Status == SpeechSystemStatus.Running)
        {
            m_speechImage.gameObject.SetActive(false);
            m_dictationRecognizer.Stop();
        }
        else
        {
            m_speechImage.gameObject.SetActive(true);
            m_dictationRecognizer.Start();
        }
    }

    public void AddAction(Action<string> action)
    {
        m_action = action;
    }

    public void RemoveAction()
    {
        m_action = null;
    }
}
