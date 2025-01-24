using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class RoomSceneManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private PhotonView m_photonView = null;

    [SerializeField]
    private Button m_inButton = null;

    [SerializeField]
    private Button m_outButton = null;

    [SerializeField]
    private Dropdown m_deckSelectDropdown = null;

    [SerializeField]
    private Text m_deckCountText = null;

    [SerializeField]
    private Button m_startButton = null;

    [SerializeField]
    private Button m_closeButton = null;

    [SerializeField]
    private GameObject m_content = null;

    [SerializeField]
    private GameObject m_frameObj = null;

    [SerializeField]
    private Text m_frameText = null;

    [SerializeField]
    private BattlePlayerManager m_battlePlayerManager = null;

    private string m_playerName = "";

    public List<string> m_deckList = new List<string>();

    private string m_deckName = "";

    private int m_count = 0;

    public void Awake() {
        m_inButton.interactable = false;
        m_outButton.interactable = false;
        m_startButton.interactable = false;
        m_closeButton.interactable = false;

        CheckDeckDirectory();

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnected();

        var directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_OPTION;
        string[] deckFiles = Directory.GetFiles(directoryPath, "*.json", SearchOption.AllDirectories);
        try
        {
            StreamReader sr = new StreamReader(deckFiles[0], Encoding.UTF8);
            OptionData optionData = JsonUtility.FromJson<OptionData>(sr.ReadToEnd());
            sr.Close();
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

        RoomOptions roomOptions = new RoomOptions()
        {
            IsOpen = true,
            MaxPlayers = 4,
        };

        PhotonNetwork.JoinOrCreateRoom("Room", roomOptions, TypedLobby.Default);
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

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);

        FadeManager.Instance().OnStart("HomeScene");
    }

    private void OnJoinOrCreateRoom()
    {
        var localPlayer = PhotonNetwork.LocalPlayer;
        m_playerName = localPlayer.ActorNumber + "P " + localPlayer.NickName;

        m_battlePlayerManager.SetRoomHash();

        m_inButton.interactable = true;
        m_outButton.interactable = true;
        if (PhotonNetwork.IsMasterClient)
        {
            m_startButton.interactable = true;
        }
        m_closeButton.interactable = true;

        UpdatePlayerList();
    }

    private void CheckDeckDirectory()
    {
        m_deckSelectDropdown.ClearOptions();
        m_deckSelectDropdown.options.Add(new Dropdown.OptionData()
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
                m_deckList.Add(sr.ReadToEnd());
                int start = deckFiles[index].LastIndexOf("/") + 1;
                int end = deckFiles[index].LastIndexOf(".json");
                int count = end - start;
                string deckFileName = deckFiles[index].Substring(start, count);
                m_deckSelectDropdown.options.Add(new Dropdown.OptionData()
                {
                    text = deckFileName
                });
                sr.Close();
            }
        }
    }

    public void OnValueChangedToDeckSelectDropdown()
    {
        var selectId = m_deckSelectDropdown.value - 1;
        if (0 < selectId || selectId < m_deckList.Count)
        {
            DeckManager.DeckDetail deckCardList = JsonUtility.FromJson<DeckManager.DeckDetail>(m_deckList[selectId]);
            m_deckName = m_deckSelectDropdown.options[selectId + 1].text;
            m_count = deckCardList.cardDetailList.Count;
            m_deckCountText.text = m_count + "枚";

            // 40枚以下のデッキを選んだ場合は設定を破棄
            if (m_count < ConstManager.DECK_CARD_MIN_COUNT)
            {
                OnClickToOutButton();
            }
        }
    }

    private void UpdatePlayerList()
    {
        foreach (Transform content in m_content.transform)
        {
            if (content.gameObject == m_frameObj)
            {
                continue;
            }

            Destroy(content.gameObject);
        }

        foreach (var player in PhotonNetwork.PlayerList)
        {
            m_frameText.text = player.ActorNumber + "P " + player.NickName;
            GameObject frameObj = Object.Instantiate(m_frameObj) as GameObject;
            frameObj.transform.SetParent(m_content.transform);
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

    public void OnClickToStartButton() {
        if (!m_battlePlayerManager.IsStandbyBattlePlayer())
        {
            return;
        }

        m_photonView.RPC(nameof(MoveBattleScene), RpcTarget.AllBuffered);
    }

    [PunRPC]
    public void MoveBattleScene()
    {        
        PhotonNetwork.IsMessageQueueRunning = false;

        FadeManager.Instance().OnStart("BattleScene");
    }

    public void OnClickToCloseButton() {
        PhotonNetwork.LeaveRoom();
    }
}
