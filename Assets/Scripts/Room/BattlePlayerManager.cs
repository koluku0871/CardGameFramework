using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class BattlePlayerManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Text m_playerName1 = null;

    [SerializeField]
    private Text m_playerName2 = null;

    // ハッシュテーブルを宣言
    Hashtable roomHash = new Hashtable();

    public bool IsStandbyBattlePlayer()
    {
        return (m_playerName1.text != "" && m_playerName2.text != "");
    }

    public void SetRoomHash()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            roomHash["playerName1"] = "";
            roomHash["playerName2"] = "";
            roomHash["playerDeck1"] = "";
            roomHash["playerDeck2"] = "";
            PhotonNetwork.CurrentRoom.SetCustomProperties(roomHash);
        }
        else
        {
            foreach (var prop in PhotonNetwork.CurrentRoom.CustomProperties)
            {
                if (prop.Key.ToString() == "playerName1")
                {
                    roomHash["playerName1"] = prop.Value.ToString();
                    m_playerName1.text = roomHash["playerName1"].ToString();
                }
                else if (prop.Key.ToString() == "playerName2")
                {
                    roomHash["playerName2"] = prop.Value.ToString();
                    m_playerName2.text = roomHash["playerName2"].ToString();
                }
                else if (prop.Key.ToString() == "playerDeck1")
                {
                    roomHash["playerDeck1"] = prop.Value.ToString();
                }
                else if (prop.Key.ToString() == "playerDeck2")
                {
                    roomHash["playerDeck2"] = prop.Value.ToString();
                }
            }
        }
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

    public override void OnRoomPropertiesUpdate(Hashtable changedProps)
    {
        // 更新されたプレイヤーのカスタムプロパティのペアをコンソールに出力する
        foreach (var prop in changedProps)
        {
            if (prop.Key.ToString() == "playerName1")
            {
                roomHash["playerName1"] = prop.Value.ToString();
            }
            else if (prop.Key.ToString() == "playerName2")
            {
                roomHash["playerName2"] = prop.Value.ToString();
            }
            else if (prop.Key.ToString() == "playerDeck1")
            {
                roomHash["playerDeck1"] = prop.Value.ToString();
            }
            else if (prop.Key.ToString() == "playerDeck2")
            {
                roomHash["playerDeck2"] = prop.Value.ToString();
            }
        }

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
