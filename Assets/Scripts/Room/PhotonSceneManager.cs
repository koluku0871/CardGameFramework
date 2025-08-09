using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static DeckManager;

public class PhotonSceneManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private RoomAnimManager roomAnimManager = null;

    [SerializeField]
    private Button m_closeButton = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI m_roomTitleText = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI m_messageText = null;

    [SerializeField]
    private GameObject m_roomContent = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI m_roomNameText = null;

    [SerializeField]
    private Button m_roomButton = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI m_roomButtonText = null;

    [SerializeField]
    private GameObject m_playerNameContent = null;

    [SerializeField]
    private Image m_masterImage = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI m_frameText = null;

    [SerializeField]
    private Button m_inButton = null;

    [SerializeField]
    private Button m_outButton = null;

    [SerializeField]
    private TMPro.TMP_Dropdown m_deckSelectDropdown = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI m_cardCountText = null;

    [SerializeField]
    private Button m_startButton = null;

    [SerializeField]
    private GameObject m_detailAreaContents = null;

    [SerializeField]
    private GameObject m_detailObj = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI m_detailTitleText = null;

    [SerializeField]
    private TMPro.TMP_InputField m_detailTextInputField = null;

    [SerializeField]
    private Toggle m_detailToggle = null;

    [SerializeField]
    private BattlePlayerManager m_battlePlayerManager = null;

    private string m_playerName = "";

    public List<string> m_deckList = new List<string>();

    private string m_deckName = "";

    private int m_count = 0;

    private Dictionary<string, RoomInfo> m_cachedRoomList = new Dictionary<string, RoomInfo>();

    public void Awake()
    {
        m_inButton.interactable = false;
        m_outButton.interactable = false;
        m_startButton.interactable = false;
        m_closeButton.interactable = false;

        CheckDeckDirectory();

        // マルチプレイに参加していない場合は参加する
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.AutomaticallySyncScene = true;

            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void Update()
    {
        string clientState = "NetworkState : " + PhotonNetwork.NetworkClientState.ToString();
        if (m_messageText.text != clientState)
        {
            m_messageText.gameObject.SetActive(false);
            m_messageText.text = clientState;
            m_messageText.gameObject.SetActive(true);
        }
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnected();

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        try
        {
            OptionData optionData = new OptionData();
            optionData.LoadTxt();

            PhotonNetwork.NickName = optionData.name;
            if (string.IsNullOrEmpty(PhotonNetwork.NickName))
            {
                PhotonNetwork.NickName = "Player";
            }
        }
        catch
        {
            PhotonNetwork.NickName = "Player";
        }

        m_closeButton.interactable = true;

        m_roomNameText.text = PhotonNetwork.NickName + "の部屋";
    }

    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

        OnJoinOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        OnJoinOrCreateRoom();
    }

    private void OnJoinOrCreateRoom()
    {
        var localPlayer = PhotonNetwork.LocalPlayer;
        m_playerName = localPlayer.ActorNumber + "P " + localPlayer.NickName;

        m_battlePlayerManager.SetRoomHash();

        m_inButton.interactable = true;
        m_outButton.interactable = true;

        roomAnimManager.SetActiveToRoomContents(true);

        UpdatePlayerList();

        SetCustomRoomProperties();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);

        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);

        UpdatePlayerList();
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        base.OnRoomPropertiesUpdate(propertiesThatChanged);

        m_battlePlayerManager.OnPropertiesUpdate(propertiesThatChanged);

        SetCustomRoomProperties();
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        m_closeButton.interactable = false;

        roomAnimManager.SetActiveToRoomContents(false);
    }

    public override void OnLeftLobby()
    {
        base.OnLeftLobby();

        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);

        FadeManager.Instance().OnStart("HomeScene");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (m_cachedRoomList.ContainsKey(info.Name))
                {
                    m_cachedRoomList.Remove(info.Name);
                }
                continue;
            }

            if (m_cachedRoomList.ContainsKey(info.Name))
            {
                m_cachedRoomList[info.Name] = info;
            }
            else
            {
                m_cachedRoomList.Add(info.Name, info);
            }
        }

        foreach (Transform content in m_roomContent.transform)
        {
            if (content.gameObject == m_roomButton.gameObject)
            {
                continue;
            }

            Destroy(content.gameObject);
        }

        foreach (RoomInfo info in m_cachedRoomList.Values)
        {
            m_roomButtonText.text = info.Name + " : " + (byte)info.PlayerCount + "/" + info.MaxPlayers;
            Button roomButtonObj = UnityEngine.Object.Instantiate(m_roomButton);
            roomButtonObj.name = info.masterClientId.ToString();
            roomButtonObj.transform.SetParent(m_roomContent.transform);
            roomButtonObj.transform.localScale = Vector3.one;
            roomButtonObj.transform.localPosition = new Vector3(
                roomButtonObj.transform.localPosition.x,
                roomButtonObj.transform.localPosition.y,
                0
            );
            roomButtonObj.gameObject.SetActive(true);
            roomButtonObj.onClick.AddListener(() =>
            {
                m_roomTitleText.gameObject.SetActive(false);
                m_roomTitleText.text = info.Name;
                m_roomTitleText.gameObject.SetActive(true);

                PhotonNetwork.JoinRoom(info.Name);
            });
        }
    }

    private void CheckDeckDirectory()
    {
        m_deckSelectDropdown.ClearOptions();
        m_deckSelectDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData()
        {
            text = "no select"
        });
        var directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_DECK;
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
            Debug.Log("directoryPath:" + directoryPath);
        }
        else
        {
            m_deckList.Clear();
            string[] deckFiles = Directory.GetFiles(directoryPath, "*.json", SearchOption.AllDirectories);
            for (int index = 0; index < deckFiles.Length; index++)
            {
                StreamReader sr = new StreamReader(deckFiles[index], Encoding.UTF8);
                var deckStr = sr.ReadToEnd();
                try
                {
                    DeckDetail deckCardList = JsonUtility.FromJson<DeckDetail>(deckStr);
                    if (deckCardList.GetDeckType() == AssetBundleManager.Instance().CardType)
                    {
                        m_deckList.Add(deckStr);
                        int start = deckFiles[index].LastIndexOf("/") + 1;
                        int end = deckFiles[index].LastIndexOf(".json");
                        int count = end - start;
                        string deckFileName = deckFiles[index].Substring(start, count);
                        m_deckSelectDropdown.options.Add(new TMPro.TMP_Dropdown.OptionData()
                        {
                            text = deckFileName
                        });
                    }
                }
                catch { }

                sr.Close();
            }
        }
    }

    public void OnClickToRoomCreateButton()
    {
        m_roomTitleText.gameObject.SetActive(false);
        m_roomTitleText.text = m_roomNameText.text;
        m_roomTitleText.gameObject.SetActive(true);

        RoomOptions roomOptions = new RoomOptions()
        {
            IsOpen = true,
            MaxPlayers = 4
        };
        PhotonNetwork.JoinOrCreateRoom(m_roomNameText.text, roomOptions, TypedLobby.Default);
    }

    public void OnClickToRoomOutButton()
    {
        m_roomTitleText.text = "";

        PhotonNetwork.LeaveRoom();
    }

    public void OnClickToUpButton()
    {
        ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable();
        string str = "";

        foreach (Transform content in m_detailAreaContents.transform)
        {
            if (!content.gameObject.activeInHierarchy)
            {
                continue;
            }

            string key = "";
            object value = null;
            foreach (Transform children in content.transform)
            {
                if (!children.gameObject.activeInHierarchy)
                {
                    continue;
                }

                switch (children.gameObject.name)
                {
                    case "Text (TMP)":
                        var text = children.GetComponent<TMPro.TextMeshProUGUI>();
                        if (text != null)
                        {
                            key = text.text;
                        }
                        break;
                    case "InputField (TMP)":
                        var inputField = children.GetComponent<TMPro.TMP_InputField>();
                        if (inputField != null)
                        {
                            if (inputField.contentType == TMPro.TMP_InputField.ContentType.IntegerNumber)
                            {
                                value = int.Parse(inputField.text);
                            }
                            else
                            {
                                value = inputField.text;
                            }
                        }
                        break;
                    case "Toggle":
                        var toggle = children.GetComponent<Toggle>();
                        if (toggle != null)
                        {
                            value = toggle.isOn;
                        }
                        break;
                }
            }

            if (string.IsNullOrEmpty(key) || value == null)
            {
                continue;
            }

            customRoomProperties.Add(key, value);
            str += "[key = " + key.ToString() + ", value = " + value.ToString() + "],";
        }

        customRoomProperties.Add("playerName1", m_battlePlayerManager.roomHash["playerName1"]);
        customRoomProperties.Add("playerName2", m_battlePlayerManager.roomHash["playerName2"]);
        customRoomProperties.Add("playerDeck1", m_battlePlayerManager.roomHash["playerDeck1"]);
        customRoomProperties.Add("playerDeck2", m_battlePlayerManager.roomHash["playerDeck2"]);

        if (m_battlePlayerManager.roomHashCount <= customRoomProperties.Count)
        {
            Debug.Log(str);
            m_battlePlayerManager.SetRoomHash(customRoomProperties);
        }
    }

    private void UpdatePlayerList()
    {
        m_startButton.interactable = PhotonNetwork.IsMasterClient;

        foreach (Transform content in m_playerNameContent.transform)
        {
            if (content.gameObject == m_frameText.gameObject)
            {
                continue;
            }

            Destroy(content.gameObject);
        }

        foreach (var player in PhotonNetwork.PlayerList)
        {
            m_masterImage.gameObject.SetActive(player.IsMasterClient);
            m_frameText.text = player.ActorNumber + "P " + player.NickName;
            GameObject frameObj = UnityEngine.Object.Instantiate(m_frameText.gameObject) as GameObject;
            frameObj.transform.SetParent(m_playerNameContent.transform);
            frameObj.transform.localScale = Vector3.one;
            frameObj.transform.localPosition = new Vector3(
                frameObj.transform.localPosition.x,
                frameObj.transform.localPosition.y,
                0
            );
            frameObj.SetActive(true);
        }
    }

    public void OnClickToInButton()
    {
        if (m_count < ConstManager.DECK_CARD_MIN_COUNT)
        {
            return;
        }

        m_battlePlayerManager.SetInBattlePlayer(m_playerName, m_deckName);
    }

    public void OnClickToOutButton()
    {
        m_battlePlayerManager.SetOutBattlePlayer(m_playerName);
    }

    public void OnValueChangedToDeckSelectDropdown()
    {
        var selectId = m_deckSelectDropdown.value - 1;
        if (0 < selectId || selectId < m_deckList.Count)
        {
            DeckManager.DeckDetail deckCardList = JsonUtility.FromJson<DeckManager.DeckDetail>(m_deckList[selectId]);
            m_deckName = m_deckSelectDropdown.options[selectId + 1].text;
            m_count = deckCardList.cardDetailList.Count;
            m_cardCountText.text = m_count + "枚";

            // 40枚以下のデッキを選んだ場合は設定を破棄
            if (m_count < ConstManager.DECK_CARD_MIN_COUNT)
            {
                OnClickToOutButton();
            }
        }
    }

    public void OnClickToStartButton()
    {
        if (!m_battlePlayerManager.IsStandbyBattlePlayer())
        {
            return;
        }

        //m_photonView.RPC(nameof(MoveBattleScene), RpcTarget.AllBuffered);

        PhotonNetwork.CurrentRoom.IsOpen = false;
        m_battlePlayerManager.SetRoomHash();
        PhotonNetwork.LoadLevel("BattleScene");
    }

    public void SetCustomRoomProperties()
    {
        foreach (Transform content in m_detailAreaContents.transform)
        {
            if (content.gameObject == m_detailObj.gameObject)
            {
                continue;
            }
            Destroy(content.gameObject);
        }

        ArrayList keys = new ArrayList(m_battlePlayerManager.roomHash.Keys);
        keys.Sort();
        foreach (var key in keys)
        {
            var propertie = m_battlePlayerManager.roomHash[key];
            if (key.ToString() == "playerName1"
                || key.ToString() == "playerName2"
                || key.ToString() == "playerDeck1"
                || key.ToString() == "playerDeck2")
            {
                continue;
            }

            m_detailTitleText.text = key.ToString();

            if (propertie.GetType() == typeof(string))
            {
                m_detailToggle.gameObject.SetActive(false);

                m_detailTextInputField.gameObject.SetActive(true);
                m_detailTextInputField.contentType = TMPro.TMP_InputField.ContentType.Standard;
                m_detailTextInputField.text = propertie.ToString();
            }
            else if (propertie.GetType() == typeof(int))
            {
                m_detailToggle.gameObject.SetActive(false);

                m_detailTextInputField.gameObject.SetActive(true);
                m_detailTextInputField.contentType = TMPro.TMP_InputField.ContentType.IntegerNumber;
                m_detailTextInputField.text = propertie.ToString();
            }
            else if (propertie.GetType() == typeof(bool))
            {
                m_detailToggle.gameObject.SetActive(true);
                m_detailToggle.isOn = bool.Parse(propertie.ToString());

                m_detailTextInputField.gameObject.SetActive(false);
                m_detailTextInputField.text = "";
            }

            

            GameObject detailObj = UnityEngine.Object.Instantiate(m_detailObj) as GameObject;
            detailObj.transform.SetParent(m_detailAreaContents.transform);
            detailObj.transform.localScale = Vector3.one;
            detailObj.transform.localPosition = new Vector3(
                detailObj.transform.localPosition.x,
                detailObj.transform.localPosition.y,
                0
            );
            detailObj.SetActive(true);
        }
    }

    /*[PunRPC]
    public void MoveBattleScene()
    {
        PhotonNetwork.IsMessageQueueRunning = false;

        m_battlePlayerManager.SetRoomHash();

        FadeManager.Instance().OnStart("BattleScene", false);
    }*/

    public void OnClickToCloseButton()
    {
        PhotonNetwork.LeaveLobby();
    }
}
