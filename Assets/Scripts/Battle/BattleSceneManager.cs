﻿using Photon.Pun;
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
    private CardOptionWindow m_cardOptionWindow = null;

    private static BattleSceneManager instance = null;
    public static BattleSceneManager Instance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
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

    public static string m_playerName = "";
    public static string m_playerName1 = "";
    public static string m_playerName2 = "";
    public static string m_playerDeck1 = "";
    public static string m_playerDeck2 = "";

    private void Start()
    {
        AudioSourceManager.Instance().PlayOneShot((int)AudioSourceManager.BGM_NUM.BATTLE_1, true);

        PhotonNetwork.IsMessageQueueRunning = true;
        m_playerName = GetPlayerName(PhotonNetwork.LocalPlayer);

        foreach (var prop in PhotonNetwork.CurrentRoom.CustomProperties)
        {
            if (prop.Key.ToString() == "playerName1")
            {
                m_playerName1 = prop.Value.ToString();
            }
            else if (prop.Key.ToString() == "playerName2")
            {
                m_playerName2 = prop.Value.ToString();
            }
            else if (prop.Key.ToString() == "playerDeck1")
            {
                m_playerDeck1 = prop.Value.ToString();
            }
            else if (prop.Key.ToString() == "playerDeck2")
            {
                m_playerDeck2 = prop.Value.ToString();
            }
        }

        SetUi();
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

    public void SetUi()
    {
        IsPlayer = m_playerName == m_playerName1 || m_playerName == m_playerName2;

        m_playerName1Text.text = m_playerName1;
        m_playerName2Text.text = m_playerName2;

        if (m_playerName == m_playerName1)
        {
            m_playerName1Text.text += "- mine";
            Debug.LogError("m_playerName1 = " + m_playerName1);
            m_fieldPanel.localRotation = Quaternion.Euler(0, 0, 0);

            GameObject obj = PhotonNetwork.Instantiate("Prefab/Battle/PlayField", Vector3.zero, Quaternion.identity);
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

            GameObject obj = PhotonNetwork.Instantiate("Prefab/Battle/PlayField", Vector3.zero, Quaternion.Euler(0, 0, 180));
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

        if (PhotonNetwork.IsMasterClient)
        {
            GameObject coinObj = PhotonNetwork.Instantiate("Prefab/Battle/Coin", Vector3.zero, Quaternion.identity);
            System.Random random = new System.Random();
            coinObj.GetComponent<CoinManager>().SetIsOpen(Convert.ToBoolean(random.Next(0, 2)));
        }
    }

    public void OnClickToCloseButton()
    {
        PhotonNetwork.LeaveRoom();
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

    [PunRPC]
    public void MoveBattleScene()
    {
    }
}
