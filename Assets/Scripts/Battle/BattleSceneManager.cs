using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class BattleSceneManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private RectTransform m_fieldPanel = null;

    [SerializeField]
    private Text m_playerName1Text = null;

    [SerializeField]
    private Text m_playerName2Text = null;

    [SerializeField]
    private ScrollRect m_scrollRect = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI m_logText = null;

    [SerializeField]
    private CardOptionWindow m_cardOptionWindow = null;

    public CoinManager m_coinManager = null;

    [SerializeField]
    private TMPro.TMP_Dropdown m_optionDropdown = null;

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
    public static string m_playerName = "";
    public static string m_playerName1 = "";
    public static string m_playerName2 = "";
    public static string m_playerDeck1 = "";
    public static string m_playerDeck2 = "";

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
                case "playerName1":
                    m_playerName1 = value;
                    break;
                case "playerName2":
                    m_playerName2 = value;
                    break;
                case "playerDeck1":
                    m_playerDeck1 = value;
                    break;
                case "playerDeck2":
                    m_playerDeck2 = value;
                    break;
            }
        }

        SetUi();
    }

    public List<PlayerFieldManager> m_fieldManagerList = new List<PlayerFieldManager>();
    SortedDictionary<long, string> m_sortLogList = new SortedDictionary<long, string>();

    public void Update()
    {
        if (m_fieldManagerList.Count < 2)
        {
            GameObject[] playFieldList = GameObject.FindGameObjectsWithTag("PlayField");
            foreach (var playFieldObj in playFieldList)
            {
                PlayerFieldManager fieldManager = playFieldObj.GetComponent<PlayerFieldManager>();
                if (fieldManager == null || m_fieldManagerList.Contains(fieldManager))
                {
                    continue;
                }
                m_fieldManagerList.Add(fieldManager);
                break;
            }
        }
        else
        {
            bool isAdd = false;
            foreach (var fieldManager in m_fieldManagerList)
            {
                foreach (var log in fieldManager.GetLogList())
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

        FadeManager.Instance().OnStart("HomeScene");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
    }

    public void SetUi()
    {
        IsPlayer = m_playerName == m_playerName1 || m_playerName == m_playerName2;

        m_playerName1Text.text = m_playerName1;
        m_playerName2Text.text = m_playerName2;

        if (PhotonNetwork.IsMasterClient)
        {
            GameObject coinObj = PhotonNetwork.Instantiate("Prefab/Battle/Coin", Vector3.zero, Quaternion.identity);
            m_coinManager = coinObj.GetComponent<CoinManager>();
            m_coinManager.SetIsOpen(Convert.ToBoolean(new System.Random().Next(0, 2)));

            if (m_type == "digimon")
            {
                GameObject costManagerObj = PhotonNetwork.InstantiateRoomObject("Prefab/Battle/CostManager", Vector3.zero, Quaternion.identity);
            }
        }

        if (m_playerName == m_playerName1)
        {
            m_playerName1Text.text += "- mine";
            Debug.LogError("m_playerName1 = " + m_playerName1);
            m_fieldPanel.localRotation = Quaternion.Euler(0, 0, 0);

            GameObject obj = null;
            try
            {
                obj = PhotonNetwork.Instantiate("Prefab/Battle/PlayField/PlayField_" + m_type, Vector3.zero, Quaternion.identity);
            }
            catch
            {
                obj = PhotonNetwork.Instantiate("Prefab/Battle/PlayField/PlayField", Vector3.zero, Quaternion.identity);
            }
            
            PlayerFieldManager playerFieldManager = obj.GetComponent<PlayerFieldManager>();
            obj.SetActive(true);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;

            playerFieldManager.SetInitCore();

            var directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_DECK;
            string[] deckFiles = Directory.GetFiles(directoryPath, "*.json", SearchOption.AllDirectories);
            for (int index = 0; index < deckFiles.Length; index++)
            {
                int start = deckFiles[index].LastIndexOf("/") + 1;
                int end = deckFiles[index].LastIndexOf(".json");
                int count = end - start;
                string deckFileName = deckFiles[index].Substring(start, count);

                if (m_playerDeck1 != deckFileName)
                {
                    continue;
                }

                StreamReader sr = new StreamReader(deckFiles[index], Encoding.UTF8);
                playerFieldManager.SetDeckDetail(sr.ReadToEnd());
                sr.Close();
                break;
            }

            PhotonNetwork.Instantiate("Prefab/Battle/CardListWindow", Vector3.zero, Quaternion.identity);

            m_cardOptionWindow.transform.SetAsLastSibling();
        }

        if (m_playerName == m_playerName2)
        {
            m_playerName2Text.text += "- mine";
            Debug.LogError("m_playerName2 = " + m_playerName2);
            m_fieldPanel.localRotation = Quaternion.Euler(0, 0, 180);

            GameObject obj = null;
            try
            {
                obj = PhotonNetwork.Instantiate("Prefab/Battle/PlayField/PlayField_" + m_type, Vector3.zero, Quaternion.Euler(0, 0, 180));
            }
            catch
            {
                obj = PhotonNetwork.Instantiate("Prefab/Battle/PlayField/PlayField", Vector3.zero, Quaternion.Euler(0, 0, 180));
            }

            PlayerFieldManager playerFieldManager = obj.GetComponent<PlayerFieldManager>();
            obj.SetActive(true);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.Euler(0, 0, 180);
            obj.transform.localScale = Vector3.one;

            playerFieldManager.SetInitCore();

            var directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_DECK;
            string[] deckFiles = Directory.GetFiles(directoryPath, "*.json", SearchOption.AllDirectories);
            for (int index = 0; index < deckFiles.Length; index++)
            {
                int start = deckFiles[index].LastIndexOf("/") + 1;
                int end = deckFiles[index].LastIndexOf(".json");
                int count = end - start;
                string deckFileName = deckFiles[index].Substring(start, count);

                if (m_playerDeck2 != deckFileName)
                {
                    continue;
                }

                StreamReader sr = new StreamReader(deckFiles[index], Encoding.UTF8);
                playerFieldManager.SetDeckDetail(sr.ReadToEnd());
                sr.Close();
                break;
            }

            PhotonNetwork.Instantiate("Prefab/Battle/CardListWindow", Vector3.zero, Quaternion.identity);

            m_cardOptionWindow.transform.SetAsLastSibling();
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

    public static bool IsNoPlayerInstance(PhotonView photonView)
    {
        string s =  GetPlayerName(photonView);
        return !IsPlayer && IsPlayerName1(s);
    }

    public static bool IsPlayerName1(string playerName)
    {
        return playerName == m_playerName1;
    }
}
