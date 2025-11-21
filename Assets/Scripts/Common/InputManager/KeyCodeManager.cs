using System;
using System.Collections.Generic;
using UnityEngine;

public class KeyCodeManager : MonoBehaviour
{
    private static KeyCodeManager instance = null;
    public static KeyCodeManager Instance()
    {
        return instance;
    }

    public enum InputType
    {
        GET = 0,
        GET_DOWN,
        GET_UP,
        ALL_NUM,
    }

    [Serializable]
    public class InputAction
    {
        public InputType inputType = InputType.GET;
        public KeyCode keyCode = KeyCode.None;
        public string text = "";
        public Action action = null;
    }

    public List<InputAction> m_inputActionList = new List<InputAction>();

    [SerializeField]
    private GameObject m_helpWindow = null;

    [Header("キーコンフィグ元")]

    [SerializeField]
    private GameObject m_keyCodeArea = null;
    [SerializeField]
    private TMPro.TextMeshProUGUI m_countText = null;
    [SerializeField]
    private TMPro.TextMeshProUGUI m_text = null;
    [SerializeField]
    private TMPro.TMP_Dropdown m_inputTypeDropdown = null;
    [SerializeField]
    private TMPro.TMP_Dropdown m_keyCodeDropdown = null;

    [SerializeField]
    private List<GameObject> m_keyCodeAreaList = new List<GameObject>();

    public void OnEnable()
    {
        instance = this;

        RemoveInputActionList();
    }

    public void Update()
    {
        if (m_inputActionList.Count < 1)
        {
            return;
        }

        foreach (var item in m_inputActionList)
        {
            bool isAction = false;
            switch (item.inputType)
            {
                case InputType.GET_DOWN:
                    isAction = Input.GetKeyDown(item.keyCode);
                    break;
                case InputType.GET_UP:
                    isAction = Input.GetKeyUp(item.keyCode);
                    break;
                case InputType.GET:
                    isAction = Input.GetKey(item.keyCode);
                    break;
            }
            if (isAction)
            {
                item.action();
            }
        }
    }

    public void AddInputActionList(InputType inputType, KeyCode keyCode, string text, Action action)
    {
        m_inputActionList.Add(
            new InputAction()
            {
                inputType = inputType,
                keyCode = keyCode,
                text = text,
                action = action
            }
        );
    }

    public void RemoveInputActionList()
    {
        m_inputActionList.Clear();
        AddInputActionList(InputType.GET_DOWN, KeyCode.H, "ヘルプウィンドウの表示/非表示", ActiveToWindow);
        AddInputActionList(InputType.GET_DOWN, KeyCode.V, "音声認識開始/停止", SpeechManager.Instance().ChangeDictationRecognizer);
    }

    public void ActiveToWindow()
    {
        bool isActive = !m_helpWindow.activeInHierarchy;

        if (isActive)
        {
            foreach (var item in m_keyCodeAreaList)
            {
                Destroy(item.gameObject);
            }
            m_keyCodeAreaList.Clear();

            for (int i = 0; i < m_inputActionList.Count; i++)
            {
                m_countText.text = i.ToString();
                m_text.text = m_inputActionList[i].text;
                m_inputTypeDropdown.value = (int)(m_inputActionList[i].inputType);
                m_inputTypeDropdown.enabled = false;
                m_keyCodeDropdown.ClearOptions();
                m_keyCodeDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData()
                {
                    text = m_inputActionList[i].keyCode.ToString()
                });
                m_keyCodeDropdown.value = 1;
                m_keyCodeDropdown.enabled = false;

                var obj = Instantiate(m_keyCodeArea, new Vector3(0, 0, 0), Quaternion.identity);
                obj.transform.SetParent(m_keyCodeArea.transform.parent);
                obj.transform.localScale = Vector3.one;
                obj.SetActive(true);
                m_keyCodeAreaList.Add(obj);
            }
        }

        m_helpWindow.SetActive(isActive);
    }
}
