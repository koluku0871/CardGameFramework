using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private TMPro.TextMeshProUGUI m_playerName1 = null;

    [SerializeField]
    private TMPro.TextMeshProUGUI m_playerName2 = null;

    // ハッシュテーブルを宣言
    public Hashtable roomHash = new Hashtable();

    public int roomHashCount = 0;

    public bool IsStandbyBattlePlayer()
    {
        return (m_playerName1.text != "" && m_playerName2.text != "");
    }

    public void SetRoomHash()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable customRoomProperties = new ExitGames.Client.Photon.Hashtable();
            customRoomProperties.Add("Type", "bs");
            customRoomProperties.Add("Core", 3);
            customRoomProperties.Add("SoulCore", 1);
            customRoomProperties.Add("Life", 5);
            customRoomProperties.Add("Hand", 4);

            customRoomProperties.Add("playerName1", "");
            customRoomProperties.Add("playerName2", "");
            customRoomProperties.Add("playerDeck1", "");
            customRoomProperties.Add("playerDeck2", "");

            SetRoomHash(customRoomProperties);
        }
        else
        {
            roomHash = PhotonNetwork.CurrentRoom.CustomProperties;
        }

        if (roomHashCount < roomHash.Count)
        {
            roomHashCount = roomHash.Count;
        }
    }

    public void SetRoomHash(ExitGames.Client.Photon.Hashtable customRoomProperties)
    {
        roomHash = customRoomProperties;
        PhotonNetwork.CurrentRoom.SetCustomProperties(roomHash);
    }

    public void SetInBattlePlayer(string  playerName, string deckName)
    {
        if ((string)roomHash["playerName1"] == playerName
            || (string)roomHash["playerName2"] == playerName)
        {
            return;
        }

        if (!roomHash.ContainsKey("playerName1") || (string)roomHash["playerName1"] == "")
        {
            roomHash["playerName1"] = playerName;
            roomHash["playerDeck1"] = deckName;
        }
        else if (!roomHash.ContainsKey("playerName2") || (string)roomHash["playerName2"] == "")
        {
            roomHash["playerName2"] = playerName;
            roomHash["playerDeck2"] = deckName;
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(roomHash);

        if (roomHash.ContainsKey("playerName1"))
        {
            m_playerName1.text = roomHash["playerName1"].ToString();
        }

        if (roomHash.ContainsKey("playerName2"))
        {
            m_playerName2.text = roomHash["playerName2"].ToString();
        }
    }

    public void SetOutBattlePlayer(string playerName)
    {
        if ((string)roomHash["playerName1"] == playerName)
        {
            roomHash["playerName1"] = "";
            roomHash["playerDeck1"] = "";
        }

        if ((string)roomHash["playerName2"] == playerName)
        {
            roomHash["playerName2"] = "";
            roomHash["playerDeck2"] = "";
        }

        PhotonNetwork.CurrentRoom.SetCustomProperties(roomHash);

        if (roomHash.ContainsKey("playerName1"))
        {
            m_playerName1.text = roomHash["playerName1"].ToString();
        }

        if (roomHash.ContainsKey("playerName2"))
        {
            m_playerName2.text = roomHash["playerName2"].ToString();
        }
    }

    public void OnPropertiesUpdate(Hashtable changedProps)
    {
        roomHash = PhotonNetwork.CurrentRoom.CustomProperties;

        if (roomHash.ContainsKey("playerName1"))
        {
            m_playerName1.text = roomHash["playerName1"].ToString();
        }

        if (roomHash.ContainsKey("playerName2"))
        {
            m_playerName2.text = roomHash["playerName2"].ToString();
        }
    }
}
