using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static System.Runtime.CompilerServices.RuntimeHelpers;

public class BattleSceneManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private ToggleGroup m_viewToggleGroup = null;

    [SerializeField]
    private ScrollRect m_scrollRect = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI m_logText = null;

    [SerializeField]
    private CardOptionWindow m_cardOptionWindow = null;

    [SerializeField]
    private TMPro.TMP_Dropdown m_optionDropdown = null;

    [SerializeField]
    private Button m_notebooklmButtonToDigimon = null;
    [SerializeField]
    private Button m_notebooklmButtonToBs = null;
    [SerializeField]
    private Button m_notebooklmButtonToDm = null;
    [SerializeField]
    private Button m_notebooklmButtonToYugiou = null;
    [SerializeField]
    private Button m_notebooklmButtonToHolo = null;
    [SerializeField]
    private Button m_notebooklmButtonToPoke = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI m_onKeyText = null;

    public List<string> m_logList = new List<string>();

    private static BattleSceneManager instance = null;
    public static BattleSceneManager Instance()
    {
        return instance;
    }

    private static bool m_isPlayer = false;
    public static bool IsPlayer
    {
        private set
        {
            m_isPlayer = value;
        }
        get
        {
            return m_isPlayer;
        }
    }

    public static string m_type = "";
    public bool isSolo = false;
    public static string m_playerName = "";

    [Serializable]
    public class PlayerStatus
    {
        public string m_playerName = "";
        public string m_playerDeck = "";
        public TMPro.TextMeshProUGUI m_playerNameText = null;
        public Canvas m_canvas = null;
        public Transform m_fieldPanelSub = null;
        public PlayerFieldManager m_playerFieldManager = null;
        public Button m_discodeButton = null;

        public void SetPlayerName(string playerName)
        {
            m_playerName = playerName;
            m_playerNameText.text = playerName;
            var child = m_discodeButton.transform.GetChild(0);
            if (child != null)
            {
                child.GetComponent<TMPro.TextMeshProUGUI>().text = playerName;
            }
        }

        public Transform GetFieldPanelSub(string playerName)
        {
            if (m_playerName != playerName)
            {
                return null;
            }

            return m_fieldPanelSub;
        }

        public PlayerFieldManager GetPlayerFieldManager(string playerName)
        {
            if (m_playerName != playerName)
            {
                return null;
            }

            return m_playerFieldManager;
        }

        public bool IsNoPlayer()
        {
            return string.IsNullOrEmpty(m_playerName);
        }

        public bool IsNoPlayerFieldManager()
        {
            return m_playerFieldManager == null;
        }
    }

    public List<PlayerStatus> m_playerStatusList = new List<PlayerStatus>();
    public List<int> m_playerIndexList = new List<int>();
    public bool m_isPlayerStatusComplete = false;

    public void Awake()
    {
        instance = this;

        m_logList = new List<string>();

        AudioSourceManager.Instance().PlayOneShot((int)AudioSourceManager.BGM_NUM.BATTLE_1, true);

        PhotonNetwork.IsMessageQueueRunning = true;
    }

    public void Start()
    {
        m_playerName = GetPlayerName(PhotonNetwork.LocalPlayer);

        foreach (var prop in PhotonNetwork.CurrentRoom.CustomProperties)
        {
            string key = prop.Key.ToString();
            string value = prop.Value.ToString();
            switch (key)
            {
                case "Type":
                    m_type = value;
                    break;
                case "Solo":
                    isSolo = (value == "1");
                    break;
                case "playerName1":
                    if (!string.IsNullOrEmpty(value))
                    {
                        m_playerIndexList.Add(0);
                    }
                    m_playerStatusList[0].SetPlayerName(value);
                    break;
                case "playerName2":
                    if (!string.IsNullOrEmpty(value))
                    {
                        m_playerIndexList.Add(1);
                    }
                    m_playerStatusList[1].SetPlayerName(value);
                    break;
                case "playerName3":
                    if (!string.IsNullOrEmpty(value))
                    {
                        m_playerIndexList.Add(2);
                    }
                    m_playerStatusList[2].SetPlayerName(value);
                    break;
                case "playerName4":
                    if (!string.IsNullOrEmpty(value))
                    {
                        m_playerIndexList.Add(3);
                    }
                    m_playerStatusList[3].SetPlayerName(value);
                    break;
                case "playerDeck1":
                    m_playerStatusList[0].m_playerDeck = value;
                    break;
                case "playerDeck2":
                    m_playerStatusList[1].m_playerDeck = value;
                    break;
                case "playerDeck3":
                    m_playerStatusList[2].m_playerDeck = value;
                    break;
                case "playerDeck4":
                    m_playerStatusList[3].m_playerDeck = value;
                    break;
            }
        }

        for (int i = 0; i < 4; i++)
        {
            if (m_playerStatusList[i].m_playerName == m_playerName)
            {
                m_playerStatusList[i].m_playerNameText.text += "- mine";
                Debug.LogError("m_playerName = " + m_playerName);
            }
        }

        SpeechManager.Instance().RemoveAction();
        SpeechManager.Instance().AddAction((text) => {
            if (text.Contains("専攻はもらった"))
            {
                GameObject coinObj = PhotonNetwork.Instantiate("Prefab/Battle/Coin", Vector3.zero, Quaternion.identity);
                coinObj.GetComponent<CoinManager>().m_playerFieldManager = GetPlayerFieldManager(GetPlayerName(PhotonNetwork.LocalPlayer));
                coinObj.GetComponent<CoinManager>().SetIsOpen(m_playerName.Substring(0, 2));
            }
        });

        SetUi();
    }

    SortedDictionary<long, string> m_sortLogList = new SortedDictionary<long, string>();

    public void Update()
    {
        if (!m_isPlayerStatusComplete)
        {
            GameObject[] playFieldList = GameObject.FindGameObjectsWithTag("PlayField");
            foreach (var playFieldObj in playFieldList)
            {
                PlayerFieldManager fieldManager = playFieldObj.GetComponent<PlayerFieldManager>();
                if (fieldManager == null)
                {
                    continue;
                }

                foreach (var playerStatu in m_playerStatusList)
                {
                    if (playerStatu.IsNoPlayer())
                    {
                        continue;
                    }
                    if (!playerStatu.IsNoPlayerFieldManager())
                    {
                        continue;
                    }
                    if (fieldManager.transform.parent != playerStatu.m_fieldPanelSub)
                    {
                        continue;
                    }
                    playerStatu.m_playerFieldManager = fieldManager;
                    break;
                }
            }

            bool isPlayerStatusComplete = true;
            foreach (var playerStatu in m_playerStatusList)
            {
                if (playerStatu.IsNoPlayer())
                {
                    continue;
                }
                if (!playerStatu.IsNoPlayerFieldManager())
                {
                    continue;
                }
                isPlayerStatusComplete = false;
                break;
            }
            m_isPlayerStatusComplete = isPlayerStatusComplete;
            if (m_isPlayerStatusComplete)
            {
                SetFieldPanelSubSize(true);
                CostManager.Instance().SetAsLastSiblingToPlayerName(m_playerName);
            }
        }
        else
        {
            
        }

        if (Input.anyKey)
        {
            string str = "";
            foreach (KeyCode kcode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(kcode))
                {
                    str += kcode.ToString();
                }
            }
            if (m_onKeyText.text != str)
            {
            m_onKeyText.text = str;
            }
        }
        else if (!string.IsNullOrEmpty(m_onKeyText.text))
        {
            m_onKeyText.text = "";
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        PhotonNetwork.LeaveLobby();
    }
    public override void OnLeftLobby()
    {
        base.OnLeftLobby();

        PhotonNetwork.Disconnect();

        SpeechManager.Instance().RemoveAction();

        FadeManager.Instance().OnStart("HomeScene");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
    }

    public void SetUi()
    {
        if (m_notebooklmButtonToDigimon != null)
        {
            m_notebooklmButtonToDigimon.onClick.AddListener (() => {
                OnClickToNotebooklmButton("digimon");
            });
        }

        if (m_notebooklmButtonToBs != null)
        {
            m_notebooklmButtonToBs.onClick.AddListener(() => {
                OnClickToNotebooklmButton("bs");
            });
        }

        if (m_notebooklmButtonToHolo != null)
        {
            m_notebooklmButtonToHolo.onClick.AddListener(() => {
                OnClickToNotebooklmButton("hololive");
            });
        }

        if (m_notebooklmButtonToYugiou != null)
        {
            m_notebooklmButtonToYugiou.onClick.AddListener(() => {
                OnClickToNotebooklmButton("yugioh");
            });
        }

        if (m_notebooklmButtonToDm != null)
        {
            m_notebooklmButtonToDm.onClick.AddListener(() => {
                OnClickToNotebooklmButton("dm");
            });
        }

        if (m_notebooklmButtonToPoke != null)
        {
            m_notebooklmButtonToPoke.onClick.AddListener(() => {
                OnClickToNotebooklmButton("pokemon");
            });
        }
        

        IsPlayer = false;
        for (int i = 0; i < m_playerStatusList.Count; i++)
        {
            if (m_playerStatusList[i].m_playerName != m_playerName)
            {
                continue;
            }
            IsPlayer = true;
        }

        PlayerFieldManager myPlayerFieldManager = null;

        foreach (var playerStatu in m_playerStatusList)
        {
            if (m_playerName == playerStatu.m_playerName
                || (!IsPlayer && GetPlayerName(PhotonNetwork.MasterClient) == playerStatu.m_playerName)
                || (isSolo && !string.IsNullOrEmpty(playerStatu.m_playerName)))
            {
                playerStatu.m_fieldPanelSub.localRotation = Quaternion.Euler(0, 0, 0);

                if (m_playerName == playerStatu.m_playerName
                    || isSolo)
                {
                    GameObject obj = null;
                    try
                    {
                        obj = PhotonNetwork.Instantiate("Prefab/Battle/PlayField/PlayField_" + m_type, Vector3.zero, Quaternion.identity);
                    }
                    catch
                    {
                        obj = PhotonNetwork.Instantiate("Prefab/Battle/PlayField/PlayField", Vector3.zero, Quaternion.identity);
                    }

                    if (isSolo)
                    {
                        obj.transform.SetParent(playerStatu.m_fieldPanelSub);
                    }

                    PlayerFieldManager playerFieldManager = obj.GetComponent<PlayerFieldManager>();
                    obj.SetActive(true);
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localRotation = Quaternion.identity;
                    obj.transform.localScale = Vector3.one;

                    if (m_playerName == playerStatu.m_playerName)
                    {
                        myPlayerFieldManager = playerFieldManager;
                        m_cardOptionWindow.m_myPlayerFieldManager = myPlayerFieldManager;
                        m_cardOptionWindow.m_myFieldCardManager = myPlayerFieldManager.m_fieldCardManager;
                    }

                    var directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_DECK;
                    string[] deckFiles = Directory.GetFiles(directoryPath, "*.json", SearchOption.AllDirectories);
                    for (int index = 0; index < deckFiles.Length; index++)
                    {
                        int start = deckFiles[index].LastIndexOf("/") + 1;
                        int end = deckFiles[index].LastIndexOf(".json");
                        int count = end - start;
                        string deckFileName = deckFiles[index].Substring(start, count);

                        if (playerStatu.m_playerDeck != deckFileName)
                        {
                            continue;
                        }

                        StreamReader sr = new StreamReader(deckFiles[index], Encoding.UTF8);
                        playerFieldManager.SetDeckDetail(sr.ReadToEnd());
                        sr.Close();
                        break;
                    }

                    GameObject cardListWindowObj = PhotonNetwork.Instantiate("Prefab/Battle/CardListWindow", Vector3.zero, Quaternion.identity);

                    m_cardOptionWindow.transform.SetAsLastSibling();
                }
            }
            else
            {
                playerStatu.m_fieldPanelSub.localRotation = Quaternion.Euler(0, 0, 180);
            }
        }

        if (PhotonNetwork.IsMasterClient)
        {
            GameObject coinObj = PhotonNetwork.Instantiate("Prefab/Battle/Coin", Vector3.zero, Quaternion.identity);
            coinObj.GetComponent<CoinManager>().m_playerFieldManager = myPlayerFieldManager;

            string coin = (string)PhotonNetwork.CurrentRoom.CustomProperties["Coin"];

            if (string.IsNullOrEmpty(coin))
            {
                int index = new System.Random().Next(0, m_playerIndexList.Count);
                coinObj.GetComponent<CoinManager>().SetIsOpen(m_playerStatusList[m_playerIndexList[index]].m_playerName.Substring(0, 2));
            }
            else
            {
                coinObj.GetComponent<CoinManager>().SetIsOpen(coin);
            }

            if (m_type == "digimon")
            {
                GameObject costManagerObj = PhotonNetwork.InstantiateRoomObject("Prefab/Battle/CostManager", Vector3.zero, Quaternion.identity);
                costManagerObj.GetComponent<CostManager>().m_playerFieldManager = myPlayerFieldManager;
            }
        }

        if (!IsPlayer)
        {
            m_cardOptionWindow.Close();
        }

        m_optionDropdown.options.Clear();
        foreach (var type in Enum.GetValues(typeof(CardOptionWindow.OPTION_TYPE)))
        {
            string name = Enum.GetName(typeof(CardOptionWindow.OPTION_TYPE), type);

            m_optionDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData()
            {
                text = name
            });
        }
        m_optionDropdown.onValueChanged.AddListener((_) => {
            string name = m_optionDropdown.options[m_optionDropdown.value].text;
            foreach (CardOptionWindow.OPTION_TYPE type in Enum.GetValues(typeof(CardOptionWindow.OPTION_TYPE)))
            {
                if (name != Enum.GetName(typeof(CardOptionWindow.OPTION_TYPE), type))
                {
                    continue;
                }
                CardOptionWindow.Instance().innerListFromType = type;
            }
        });
    }

    public bool UpdateLog()
    {
        if (!m_isPlayerStatusComplete) return false;

        bool isAdd = false;
        foreach (var playerStatu in m_playerStatusList)
        {
            if (playerStatu.m_playerFieldManager == null)
            {
                continue;
            }

            foreach (var log in playerStatu.m_playerFieldManager.GetLogList())
            {
                if (string.IsNullOrEmpty(log))
                {
                    continue;
                }

                var item = log.Split(',');
                var key = long.Parse(item[0]);
                if (m_sortLogList.ContainsKey(key))
                {
                    continue;
                }
                isAdd = true;
                m_sortLogList.Add(key, item[1]);
            }
        }

        if (isAdd)
        {
            Debug.Log(string.Join("\n", m_sortLogList.Values));
            m_logText.text = string.Join("\n", m_sortLogList.Values);
            m_scrollRect.verticalNormalizedPosition = 0f;
        }

        return isAdd;
    }

    public void OnClickToCloseButton()
    {
        PhotonNetwork.Disconnect();

        FadeManager.Instance().OnStart("HomeScene");
    }

    public static string GetPlayerName(PhotonView photonView)
    {
        return photonView.Owner.ActorNumber + "P " + photonView.Owner.NickName;
    }

    public static string GetPlayerName(Player player)
    {
        return player.ActorNumber + "P " + player.NickName;
    }

    public bool IsNoPlayerInstance(PhotonView photonView)
    {
        string playerName =  GetPlayerName(photonView);
        return !IsPlayer && GetPlayerName(PhotonNetwork.MasterClient) == playerName;
    }

    public Transform GetFieldPanelSub(string playerName)
    {
        foreach (var playerStatu in m_playerStatusList)
        {
            var fieldPanelSub = playerStatu.GetFieldPanelSub(playerName);
            if (fieldPanelSub != null)
            {
                return fieldPanelSub;
            }
        }
        return null;
    }

    public PlayerFieldManager GetPlayerFieldManager(string playerName)
    {
        foreach (var playerStatu in m_playerStatusList)
        {
            var playerFieldManager = playerStatu.GetPlayerFieldManager(playerName);
            if (playerFieldManager != null)
            {
                return playerFieldManager;
            }
        }
        return null;
    }

    public void SetFieldPanelSubRot(int index)
    {
        float sW = Screen.width;
        float w = 744;
        float h = 600;

        List<float> xList = new List<float>();
        List<float> yList = new List<float>();
        List<float> discodeYList = new List<float>();
        xList.Add(-(w / 4));
        yList.Add(0);
        discodeYList.Add(-75);

        xList.Add((w / 4));
        yList.Add(0);
        discodeYList.Add(-75);

        xList.Add(-(w / 4));
        yList.Add(-(h / 4) - 2);
        discodeYList.Add(-(h / 4) - 2 - 75);

        xList.Add((w / 4));
        yList.Add(-(h / 4) - 2);
        discodeYList.Add(-(h / 4) - 2 - 75);

        for (int i = 0; i < m_playerStatusList.Count; i++)
        {
            if (m_playerStatusList[i].IsNoPlayer())
            {
                m_playerStatusList[i].m_discodeButton.gameObject.SetActive(false);
                continue;
            }

            if (index == i)
            {
                m_playerStatusList[i].m_fieldPanelSub.localRotation = Quaternion.Euler(0, 0, 0);
                m_playerStatusList[i].m_playerFieldManager.m_rectTransform.localScale = Vector3.one;
                m_playerStatusList[i].m_playerFieldManager.m_rectTransform.localPosition = Vector3.zero;
                m_playerStatusList[i].m_discodeButton.gameObject.SetActive(false);
            }
            else
            {
                m_playerStatusList[i].m_fieldPanelSub.localRotation = Quaternion.Euler(0, 0, 180);
                m_playerStatusList[i].m_playerFieldManager.m_rectTransform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                m_playerStatusList[i].m_playerFieldManager.m_rectTransform.localPosition = new Vector3(xList[i], yList[i], 0);
                //m_playerStatusList[i].m_fieldPanelSub.localPosition = new Vector3(xList[i], yList[i], 0);
                m_playerStatusList[i].m_discodeButton.gameObject.SetActive(true);
                m_playerStatusList[i].m_discodeButton.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                m_playerStatusList[i].m_discodeButton.transform.localPosition = new Vector3(xList[i], discodeYList[i], 0);
            }
        }
    }

    public void SetFieldPanelSubRot(Transform fieldPanelSub)
    {
        if (fieldPanelSub.localRotation != Quaternion.Euler(0, 0, 0))
        {
            fieldPanelSub.localRotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            fieldPanelSub.localRotation = Quaternion.Euler(0, 0, 180);
        }
    }

    public void SetFieldPanelSubSize(bool state)
    {
        if (!state)
        {
            return;
        }

        if (!m_isPlayerStatusComplete)
        {
            return;
        }

        float scale = 1;
        UnityEngine.UI.Toggle activeToggle = m_viewToggleGroup.ActiveToggles().First();
        switch (activeToggle.name)
        {
            case "ToggleViewS":
                scale = 1;
                break;
            case "ToggleViewM":
                scale = 0.5f;
                break;
            case "ToggleViewDiscode":
                scale = 0.5f;
                break;
        }


        foreach (var playerStatu in m_playerStatusList)
        {
            playerStatu.m_discodeButton.gameObject.SetActive(false);
            if (playerStatu.IsNoPlayer())
            {
                continue;
            }

            if (activeToggle.name == "ToggleViewDiscode")
            {
                playerStatu.m_fieldPanelSub.localRotation = Quaternion.Euler(0, 0, 180);
            }
        }

        float sW = Screen.width;
        float w = 744;
        float h = 600;

        if (scale == 1)
        {
            foreach (var playerStatu in m_playerStatusList)
            {
                if (playerStatu.IsNoPlayer())
                {
                    continue;
                }
                playerStatu.m_playerFieldManager.m_rectTransform.localScale = Vector3.one;

                var localPos = playerStatu.m_playerFieldManager.m_rectTransform.localPosition;
                playerStatu.m_playerFieldManager.m_rectTransform.localPosition = Vector3.zero;
                //playerStatu.m_fieldPanelSub.localPosition = new Vector3((sW / 2) - (w / 2), 0, 0);
            }
            return;
        }

        List<string> upNameList = new List<string>();
        List<string> downNameList = new List<string>();
        foreach (var playerStatu in m_playerStatusList)
        {
            if (playerStatu.IsNoPlayer())
            {
                continue;
            }

            if (playerStatu.m_fieldPanelSub.localRotation != Quaternion.Euler(0, 0, 0))
            {
                upNameList.Add(playerStatu.m_playerName);
            }
            else
            {
                downNameList.Add(playerStatu.m_playerName);
            }
        }

        List<float> xList = new List<float>();
        List<float> yList = new List<float>();
        List<float> discodeYList = new List<float>();
        xList.Add(-(w / 4));
        yList.Add(0);
        discodeYList.Add(-75);

        xList.Add((w / 4));
        yList.Add(0);
        discodeYList.Add(-75);

        xList.Add(-(w / 4));
        yList.Add(-(h / 4) - 2);
        discodeYList.Add(-(h / 4) - 2 - 75);

        xList.Add((w / 4));
        yList.Add(-(h / 4) - 2);
        discodeYList.Add(-(h / 4) - 2 - 75);

        for (int i = 0; i < m_playerStatusList.Count; i++)
        {
            var playerStatu = m_playerStatusList[i];

            if (playerStatu.IsNoPlayer())
            {
                continue;
            }

            if (downNameList.IndexOf(playerStatu.m_playerName) != -1)
            {
                if (downNameList.Count < 2)
                {
                    playerStatu.m_playerFieldManager.m_rectTransform.localScale = Vector3.one;
                    playerStatu.m_playerFieldManager.m_rectTransform.localPosition = Vector3.zero;
                    //playerStatu.m_fieldPanelSub.localPosition = new Vector3((sW / 2) - (w / 2), 0, 0);
                    continue;
                }
                int index = downNameList.IndexOf(playerStatu.m_playerName);

                playerStatu.m_playerFieldManager.m_rectTransform.localScale = new Vector3(scale, scale, scale);
                playerStatu.m_playerFieldManager.m_rectTransform.localPosition = new Vector3(xList[index], yList[index], 0);
                //playerStatu.m_fieldPanelSub.localPosition = new Vector3(xList[index], -yList[index], 0);
            }
            else if (upNameList.IndexOf(playerStatu.m_playerName) != -1)
            {
                if (upNameList.Count < 2)
                {
                    playerStatu.m_playerFieldManager.m_rectTransform.localScale = Vector3.one;
                    playerStatu.m_playerFieldManager.m_rectTransform.localPosition = Vector3.zero;
                    //playerStatu.m_fieldPanelSub.localPosition = new Vector3((sW / 2) - (w / 2), 0, 0);
                    continue;
                }
                int index = upNameList.IndexOf(playerStatu.m_playerName);
                if (activeToggle.name == "ToggleViewDiscode")
                {
                    index = i;
                }

                playerStatu.m_playerFieldManager.m_rectTransform.localScale = new Vector3(scale, scale, scale);
                playerStatu.m_playerFieldManager.m_rectTransform.localPosition = new Vector3(xList[index], yList[index], 0);
                //playerStatu.m_fieldPanelSub.localPosition = new Vector3(xList[index], yList[index], 0);

                if (activeToggle.name == "ToggleViewDiscode")
                {
                    playerStatu.m_discodeButton.gameObject.SetActive(true);
                    playerStatu.m_discodeButton.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                    playerStatu.m_discodeButton.transform.localPosition = new Vector3(xList[index], discodeYList[index], 0);
                    //playerStatu.m_discodeButton.transform.localPosition = new Vector3(xList[index] + (w / 2), yList[index] + (h / 4), 0);
                }
            }
        }
    }

    public void OnClickToNotebooklmButton(string type)
    {
        switch(type)
        {
            case "digimon":
                Application.OpenURL("https://notebooklm.google.com/notebook/03a42ad4-541b-459e-9e64-ebf2ac041674");
                break;
            case "yugioh":
                Application.OpenURL("https://notebooklm.google.com/notebook/1e048254-d7c8-4282-8855-105957b164a6");
                break;
            case "hololive":
                Application.OpenURL("https://notebooklm.google.com/notebook/bd32400f-eaf9-4693-a807-091a78b23ca6");
                break;
            case "dm":
                Application.OpenURL("https://notebooklm.google.com/notebook/a5a43a78-5412-4e01-bc3c-e7aa7d88da68");
                break;
            case "bs":
                Application.OpenURL("https://notebooklm.google.com/notebook/47a9949f-bccb-4712-9a4c-0a1ef3f6c3fe");
                break;
            case "pokemon":
                Application.OpenURL("https://notebooklm.google.com/notebook/6549ba90-d014-4f5b-b9b0-3eb76430ec6b");
                break;
        }
    }
}
