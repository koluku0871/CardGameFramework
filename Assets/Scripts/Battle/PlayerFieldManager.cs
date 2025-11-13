using Photon.Pun;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFieldManager : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("共通")]

    [SerializeField]
    private PhotonView m_photonView = null;

    public RectTransform m_rectTransform = null;

    [SerializeField]
    private Button m_optionButton = null;
    [SerializeField]
    private Button m_coinButton = null;

    [SerializeField]
    private Button m_damageButton = null;

    [SerializeField]
    private Button m_soulPlusButton = null;
    [SerializeField]
    private Button m_soulMinusButton = null;

    [SerializeField]
    private Button m_plusButton = null;
    [SerializeField]
    private Button m_minusButton = null;

    [SerializeField]
    private Image m_count = null;
    [SerializeField]
    private Image m_life = null; 
    [SerializeField]
    private Image m_reserve = null;

    [SerializeField]
    private RectTransform m_cardField = null;

    [SerializeField]
    private RectTransform m_coreField = null;

    [SerializeField]
    private RectTransform m_coinField = null;

    [SerializeField]
    private FieldCardManager m_fieldCardManager = null;

    [Header("BS")]

    [SerializeField]
    private Button m_countPlusButton = null;
    [SerializeField]
    private Button m_countMinusButton = null;

    [SerializeField]
    private Button m_lifePlusButton = null;
    [SerializeField]
    private Button m_lifeMinusButton = null;

    [SerializeField]
    private Image m_field = null;
    [SerializeField]
    private Image m_trash = null;

    [Header("digimon")]
    [SerializeField]
    private Toggle m_moveSecurityToggle = null;

    public bool IsMoveSecurity
    {
        get
        {
            if (m_moveSecurityToggle == null)
            {
                return false;
            }

            return m_moveSecurityToggle.isOn;
        }
    }


    private List<GameObject> m_countCoreList = new List<GameObject>();
    private List<GameObject> m_soulCoreList = new List<GameObject>();
    private List<GameObject> m_coreList = new List<GameObject>();

    public List<string> m_logList = new List<string>();

    private ExitGames.Client.Photon.Hashtable m_customRoomProperties = new ExitGames.Client.Photon.Hashtable();

    private static PlayerFieldManager instance = null;
    public static PlayerFieldManager Instance()
    {
        return instance;
    }

    private void Awake()
    {
        if (m_photonView.IsMine || BattleSceneManager.Instance().IsNoPlayerInstance(m_photonView)) instance = this;
    }

    private void Start()
    {
        transform.localScale = Vector3.one;

        SetInitCore();
    }

    public override void OnEnable()
    {
        base.OnEnable();

        var targetPanel = BattleSceneManager.Instance().GetFieldPanelSub(BattleSceneManager.GetPlayerName(m_photonView));
        foreach (GameObject canvasPanel in GameObject.FindGameObjectsWithTag("FieldPanelSub"))
        {
            if (canvasPanel.transform == targetPanel)
            {
                transform.SetParent(canvasPanel.transform);
                transform.SetAsFirstSibling();
            }
        }

        if (m_fieldCardManager != null)
        {
            m_fieldCardManager.SetActiveToButton(m_photonView.IsMine || BattleSceneManager.Instance().IsNoPlayerInstance(m_photonView));
        }

        SetButton(m_photonView.IsMine);
    }

    private bool SetActive(Selectable obj, bool isActive)
    {
        bool isNotNull = obj != null && obj.gameObject != null;
        if (isNotNull)
        {
            obj.gameObject.SetActive(isActive);
        }
        return isNotNull;
    }

    public void SetButton(bool IsMine)
    {
        if (SetActive(m_optionButton, IsMine))
        {
            m_optionButton.onClick.AddListener(() => {
                CardOptionWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.STEP);
            });
        }

        if (SetActive(m_coinButton, IsMine))
        {
            m_coinButton.onClick.AddListener(() => {
                GameObject coinObj = PhotonNetwork.Instantiate("Prefab/Battle/Coin", Vector3.zero, Quaternion.identity);
                int index = new System.Random().Next(0, BattleSceneManager.Instance().m_playerIndexList.Count);
                coinObj.GetComponent<CoinManager>().SetIsOpen(
                    BattleSceneManager.Instance().m_playerStatusList[BattleSceneManager.Instance().m_playerIndexList[index]].m_playerName.Substring(0, 2)
                );
            });
        }

        SetActive(m_moveSecurityToggle, IsMine);

        if (SetActive(m_damageButton, IsMine))
        {
            m_damageButton.onClick.AddListener(() => {
                GameObject core = PhotonNetwork.Instantiate("Prefab/Battle/Core", Vector3.zero, Quaternion.identity);
                core.transform.SetParent(m_coinField);
                core.transform.localPosition = Vector3.zero;
                core.transform.localScale = Vector3.one;
                core.transform.localRotation = Quaternion.identity;
                core.GetComponent<TouchManager>().SetAction(null, null, null, () => {
                    core.SetActive(false);
                });
                core.GetComponent<TouchManager>().PhotonObjectType = ConstManager.PhotonObjectType.DAMAGE;
            });
        }

        if (SetActive(m_countPlusButton, IsMine) && m_count != null && m_countCoreList != null)
        {
            m_countPlusButton.onClick.AddListener(() => {
                GameObject core = PhotonNetwork.Instantiate("Prefab/Battle/Core", Vector3.zero, Quaternion.identity);
                core.transform.SetParent(m_coreField);
                float offset = 20;
                float sizeX = m_count.rectTransform.sizeDelta.x * m_count.rectTransform.localScale.x - offset;
                float sizeY = m_count.rectTransform.sizeDelta.y * m_count.rectTransform.localScale.y - offset + 5;
                float posX = m_count.rectTransform.position.x - ((int)(sizeX) / 2 - ((offset / 3 * 2) * (m_countCoreList.Count % 5)));
                float posY = m_count.rectTransform.position.y + ((int)(sizeY) / 2 - ((offset - 5) * (m_countCoreList.Count / 5)));
                core.transform.position = new Vector3(posX, posY, 0);
                core.GetComponent<TouchManager>().SetAction(null, null, null, () => {
                    int coreIndex = m_countCoreList.IndexOf(core);
                    if (coreIndex != -1)
                    {
                        m_countCoreList.RemoveAt(coreIndex);
                        core.SetActive(false);
                    }
                });
                core.GetComponent<TouchManager>().PhotonObjectType = ConstManager.PhotonObjectType.CORE;
                m_countCoreList.Add(core);
            });
        }

        if (SetActive(m_countMinusButton, IsMine) && m_countCoreList != null)
        {
            m_countMinusButton.onClick.AddListener(() => {
                if (m_countCoreList != null && m_countCoreList.Count > 0)
                {
                    GameObject core = m_countCoreList[m_countCoreList.Count - 1];
                    m_countCoreList.Remove(core);
                    core.SetActive(false);
                }
            });
        }

        if (SetActive(m_lifePlusButton, IsMine))
        {
            m_lifePlusButton.onClick.AddListener(() => {
                GameObject core = PhotonNetwork.Instantiate("Prefab/Battle/Core", Vector3.zero, Quaternion.identity);
                core.transform.SetParent(m_coreField);
                float offset = 10;
                float sizeX = m_life.rectTransform.sizeDelta.x * m_life.rectTransform.localScale.x - offset;
                float sizeY = m_life.rectTransform.sizeDelta.y * m_life.rectTransform.localScale.y - offset;
                core.transform.position = new Vector3(
                    m_life.rectTransform.position.x + (int)UnityEngine.Random.Range(-((int)sizeX / 2), (int)sizeX / 2),
                    m_life.rectTransform.position.y + (int)UnityEngine.Random.Range(-((int)sizeY / 2), (int)sizeY / 2),
                    0);
                core.GetComponent<TouchManager>().SetAction(null, null, null, () => {
                    int index = m_coreList.IndexOf(core);
                    if (index != -1)
                    {
                        m_coreList.RemoveAt(index);
                        core.SetActive(false);
                    }
                });
                core.GetComponent<TouchManager>().PhotonObjectType = ConstManager.PhotonObjectType.CORE;
                m_coreList.Add(core);
            });
        }

        SetActive(m_lifeMinusButton, false);

        if (SetActive(m_soulPlusButton, IsMine))
        {
            m_soulPlusButton.onClick.AddListener(() => {
                GameObject core = PhotonNetwork.Instantiate("Prefab/Battle/Core", Vector3.zero, Quaternion.identity);
                core.transform.SetParent(m_coreField);
                float offset = 10;
                float sizeX = m_reserve.rectTransform.sizeDelta.x * m_reserve.rectTransform.localScale.x - offset;
                float sizeY = m_reserve.rectTransform.sizeDelta.y * m_reserve.rectTransform.localScale.y - offset;
                core.transform.position = new Vector3(
                    m_reserve.rectTransform.position.x + (int)UnityEngine.Random.Range(-((int)sizeX / 2), (int)sizeX / 2),
                    m_reserve.rectTransform.position.y + (int)UnityEngine.Random.Range(-((int)sizeY / 2), (int)sizeY / 2),
                    0);
                core.GetComponent<TouchManager>().SetAction(null, null, null, () => {
                    int index = m_soulCoreList.IndexOf(core);
                    if (index != -1)
                    {
                        m_soulCoreList.RemoveAt(index);
                        core.SetActive(false);
                    }
                });
                core.GetComponent<TouchManager>().PhotonObjectType = ConstManager.PhotonObjectType.SOULCORE;
                m_soulCoreList.Add(core);
            });
        }

        if (SetActive(m_soulMinusButton, IsMine))
        {
            m_soulMinusButton.onClick.AddListener(() => {
                if (m_soulCoreList != null && m_soulCoreList.Count > 0)
                {
                    GameObject core = m_soulCoreList[m_soulCoreList.Count - 1];
                    m_soulCoreList.Remove(core);
                    core.SetActive(false);
                }
            });
        }

        if (SetActive(m_plusButton, IsMine))
        {
            m_plusButton.onClick.AddListener(() => {
                GameObject core = PhotonNetwork.Instantiate("Prefab/Battle/Core", Vector3.zero, Quaternion.identity);
                core.transform.SetParent(m_coreField);
                float offset = 10;
                float sizeX = m_reserve.rectTransform.sizeDelta.x * m_reserve.rectTransform.localScale.x - offset;
                float sizeY = m_reserve.rectTransform.sizeDelta.y * m_reserve.rectTransform.localScale.y - offset;
                core.transform.position = new Vector3(
                    m_reserve.rectTransform.position.x + (int)UnityEngine.Random.Range(-((int)sizeX / 2), (int)sizeX / 2),
                    m_reserve.rectTransform.position.y + (int)UnityEngine.Random.Range(-((int)sizeY / 2), (int)sizeY / 2),
                    0);
                core.GetComponent<TouchManager>().SetAction(null, null, null, () => {
                    int index = m_coreList.IndexOf(core);
                    if (index != -1)
                    {
                        m_coreList.RemoveAt(index);
                        core.SetActive(false);
                    }
                });
                core.GetComponent<TouchManager>().PhotonObjectType = ConstManager.PhotonObjectType.CORE;
                m_coreList.Add(core);
            });
        }

        if (SetActive(m_minusButton, IsMine))
        {
            m_minusButton.onClick.AddListener(() => {
                if (m_coreList != null && m_coreList.Count > 0)
                {
                    GameObject core = m_coreList[m_coreList.Count - 1];
                    m_coreList.Remove(core);
                    core.SetActive(false);
                }
            });
        }
    }

    public void SetInitCore()
    {
        int life = 0;
        int soulCoreNum = 0;
        int coreNum = 0;

        m_customRoomProperties = PhotonNetwork.CurrentRoom.CustomProperties;

        if (m_customRoomProperties["Type"].ToString() == "bs")
        {
            life = int.Parse(m_customRoomProperties["Life"].ToString());
            soulCoreNum = int.Parse(m_customRoomProperties["SoulCore"].ToString());
            coreNum = int.Parse(m_customRoomProperties["Core"].ToString());
        }

        if (m_life != null)
        {
            for (int index = 0; index < life; index++)
            {
                GameObject core = PhotonNetwork.Instantiate("Prefab/Battle/Core", Vector3.zero, Quaternion.identity);
                core.transform.SetParent(m_coreField);
                float offset = 20;
                float sizeX = m_life.rectTransform.sizeDelta.x * m_life.rectTransform.localScale.x - offset;
                float sizeY = m_life.rectTransform.sizeDelta.y * m_life.rectTransform.localScale.y - offset;
                float posX = m_life.rectTransform.position.x - (int)sizeX / 2 + (offset / 3 * 2) * index;
                float posY = m_life.rectTransform.position.y + ((offset / 2) + offset * (index / 5));
                core.transform.position = new Vector3(posX, posY, 0);
                core.GetComponent<TouchManager>().SetAction(null, null, null, () => {
                    int coreIndex = m_coreList.IndexOf(core);
                    if (coreIndex != -1)
                    {
                        m_coreList.RemoveAt(coreIndex);
                        core.SetActive(false);
                    }
                });
                core.GetComponent<TouchManager>().PhotonObjectType = ConstManager.PhotonObjectType.CORE;
                m_coreList.Add(core);
            }
        }

        for (int index = 0; index < soulCoreNum; index++)
        {
            GameObject core = PhotonNetwork.Instantiate("Prefab/Battle/Core", Vector3.zero, Quaternion.identity);
            core.transform.SetParent(m_coreField);
            float offset = 10;
            float sizeX = m_reserve.rectTransform.sizeDelta.x * m_reserve.rectTransform.localScale.x - offset;
            float sizeY = m_reserve.rectTransform.sizeDelta.y * m_reserve.rectTransform.localScale.y - offset;
            core.transform.position = new Vector3(
                m_reserve.rectTransform.position.x + (int)UnityEngine.Random.Range(-((int)sizeX / 2), (int)sizeX / 2),
                m_reserve.rectTransform.position.y + (int)UnityEngine.Random.Range(-((int)sizeY / 2), (int)sizeY / 2),
                0);
            core.GetComponent<TouchManager>().SetAction(null, null, null, () => {
                int coreIndex = m_soulCoreList.IndexOf(core);
                if (coreIndex != -1)
                {
                    m_soulCoreList.RemoveAt(coreIndex);
                    core.SetActive(false);
                }
            });
            core.GetComponent<TouchManager>().PhotonObjectType = ConstManager.PhotonObjectType.SOULCORE;
            m_soulCoreList.Add(core);
        }

        for (int index = 0; index < coreNum; index++)
        {
            GameObject core = PhotonNetwork.Instantiate("Prefab/Battle/Core", Vector3.zero, Quaternion.identity);
            core.transform.SetParent(m_coreField);
            float offset = 10;
            float sizeX = m_reserve.rectTransform.sizeDelta.x * m_reserve.rectTransform.localScale.x - offset;
            float sizeY = m_reserve.rectTransform.sizeDelta.y * m_reserve.rectTransform.localScale.y - offset;
            core.transform.position = new Vector3(
                m_reserve.rectTransform.position.x + (int)UnityEngine.Random.Range(-((int)sizeX / 2), (int)sizeX / 2),
                m_reserve.rectTransform.position.y + (int)UnityEngine.Random.Range(-((int)sizeY / 2), (int)sizeY / 2),
                0);
            core.GetComponent<TouchManager>().SetAction(null, null, null, () => {
                int coreIndex = m_coreList.IndexOf(core);
                if (coreIndex != -1)
                {
                    m_coreList.RemoveAt(coreIndex);
                    core.SetActive(false);
                }
            });
            core.GetComponent<TouchManager>().PhotonObjectType = ConstManager.PhotonObjectType.CORE;
            m_coreList.Add(core);
        }
    }

    public void SetCorePos(ConstManager.CorePosType src, ConstManager.CorePosType dst, int count = 0)
    {
        Vector2 dstPos = new Vector2();
        string dstType = "";
        float offset = 20;
        float sizeX = 0.0f;
        float sizeY = 0.0f;
        switch (dst)
        {
            case ConstManager.CorePosType.LIFE:
                dstType = "Life";
                sizeX = m_life.rectTransform.sizeDelta.x * m_life.rectTransform.localScale.x - offset;
                sizeY = m_life.rectTransform.sizeDelta.y * m_life.rectTransform.localScale.y - offset;
                dstPos = new Vector2(
                    m_life.rectTransform.position.x,
                    m_life.rectTransform.position.y);
                break;
            case ConstManager.CorePosType.RESERVE:
                dstType = "Reserve";
                sizeX = m_reserve.rectTransform.sizeDelta.x * m_reserve.rectTransform.localScale.x - offset;
                sizeY = m_reserve.rectTransform.sizeDelta.y * m_reserve.rectTransform.localScale.y - offset;
                dstPos = new Vector2(
                    m_reserve.rectTransform.position.x,
                    m_reserve.rectTransform.position.y);
                break;
            case ConstManager.CorePosType.FIELD:
                dstType = "Field";
                sizeX = m_field.rectTransform.sizeDelta.x * m_field.rectTransform.localScale.x - offset;
                sizeY = m_field.rectTransform.sizeDelta.y * m_field.rectTransform.localScale.y - offset;
                dstPos = new Vector2(
                    m_field.rectTransform.position.x,
                    m_field.rectTransform.position.y);
                break;
            case ConstManager.CorePosType.TRASH:
                dstType = "Trash";
                sizeX = m_trash.rectTransform.sizeDelta.x * m_trash.rectTransform.localScale.x - offset;
                sizeY = m_trash.rectTransform.sizeDelta.y * m_trash.rectTransform.localScale.y - offset;
                dstPos = new Vector2(
                    m_trash.rectTransform.position.x,
                    m_trash.rectTransform.position.y);
                break;
        }

        if (dstPos == Vector2.zero) return;

        string srcType = "";
        List<List<GameObject>> coreTypeList = new List<List<GameObject>>();
        switch (src)
        {
            case ConstManager.CorePosType.LIFE:
                srcType = "Life";
                coreTypeList.Add(m_coreList);
                coreTypeList.Add(m_soulCoreList);
                break;
            case ConstManager.CorePosType.RESERVE:
                srcType = "Reserve";
                coreTypeList.Add(m_coreList);
                coreTypeList.Add(m_soulCoreList);
                break;
            case ConstManager.CorePosType.FIELD:
                srcType = "Field";
                coreTypeList.Add(m_coreList);
                coreTypeList.Add(m_soulCoreList);
                break;
            case ConstManager.CorePosType.TRASH:
                srcType = "Trash";
                coreTypeList.Add(m_soulCoreList);
                coreTypeList.Add(m_coreList);
                break;
        }

        if (string.IsNullOrEmpty(srcType)) return;

        if (count <= 0)
        {
            count = m_soulCoreList.Count + m_coreList.Count;
        }

        foreach (var coreType in coreTypeList)
        {
            foreach (var core in coreType)
            {
                if (count <= 0)
                {
                    return;
                }

                TouchManager touchManager = core.GetComponent<TouchManager>();
                if (touchManager.EndTag != srcType)
                {
                    continue;
                }

                touchManager.EndTag = dstType;

                core.transform.position = new Vector2(
                        dstPos.x + (int)UnityEngine.Random.Range(-((int)sizeX / 2), (int)sizeX / 2),
                        dstPos.y + (int)UnityEngine.Random.Range(-((int)sizeY / 2), (int)sizeY / 2));

                count--;
            }
        }
    }

    public void SetDeckDetail(string deckDetailJson)
    {
        if (m_fieldCardManager != null)
        {
            DeckManager.DeckDetail jsonDeckDetail = JsonUtility.FromJson<DeckManager.DeckDetail>(deckDetailJson);

            m_fieldCardManager.SetDeckDetail(CardOptionWindow.OPTION_TYPE.DECK, jsonDeckDetail.cardDetailList);
            m_fieldCardManager.ShuffleCardDetailList(CardOptionWindow.OPTION_TYPE.DECK);

            if (BattleSceneManager.m_type != "bs")
            {
                m_fieldCardManager.SetDeckDetail(CardOptionWindow.OPTION_TYPE.SUB, jsonDeckDetail.subCardDetailList);
                m_fieldCardManager.ShuffleCardDetailList(CardOptionWindow.OPTION_TYPE.SUB);
            }

            m_fieldCardManager.SetDeckDetail(CardOptionWindow.OPTION_TYPE.TOKEN, jsonDeckDetail.tokenCardDetailList);


            m_fieldCardManager.InitSetting();
        }
    }

    public RectTransform GetCardField()
    {
        return m_cardField;
    }

    public RectTransform GetCoreField()
    {
        return m_coreField;
    }

    public void SetAllMyCardToRecovery()
    {
        if (!m_photonView.IsMine) return;

        foreach (Transform c in m_cardField)
        {
            TouchManager touchManager = c.GetComponent<TouchManager>();
            if (touchManager == null)
            {
                continue;
            }

            Image image = c.GetComponent<Image>();
            Debug.Log("z" + c.localRotation.eulerAngles.z);
            switch (c.localRotation.eulerAngles.z)
            {
                case 270:
                    FieldCardManager.Instance().SetCardToStand(image);
                    break;
                case 180:
                    FieldCardManager.Instance().SetCardToRRest(image);
                    break;
            }
        }
    }

    public void OnClickPlusButton()
    {
        m_plusButton.onClick.Invoke();
    }

    public Image CreateCard(string name, bool isOpen = true)
    {
        string[] list = name.Split('^');
        var cardId = list[1];
        GameObject card = PhotonNetwork.Instantiate("Prefab/Battle/Card", Vector3.zero, Quaternion.identity);
        card.transform.localScale = new Vector3(1, 1, 1);
        Image cardImage = FieldCardManager.Instance().CreateCard(
            new DeckManager.CardDetail() { tag = list[0], cardId = cardId }, false, card.GetComponent<Image>(), m_cardField,
            null,
            (Image target, string targetTag, string targetCardId, bool isDoubleClick) => {
                if (!isDoubleClick)
                {
                    CardOptionWindow.Instance().Open(
                        target,
                        CardOptionWindow.OPTION_TYPE.FIELD,
                        CardOptionWindow.OPTION_TYPE.FIELD_ROT);
                }
            },
            (Image target, string targetTag, string targetCardId, bool isDoubleClick) => {
                TouchManager touchManager = target.GetComponent<TouchManager>();
                if (touchManager != null)
                {
                    if (isDoubleClick)
                    {
                        touchManager.IsOverlap = !touchManager.IsOverlap;
                    }
                    else
                    {
                        touchManager.IsInner = !touchManager.IsInner;
                    }
                }
            },
            (Image target, string tag, string cardId) => {
                if (target.sprite != CardDetailManager.Instance().GetSleeveSprite())
                {
                    CardDetailManager.Instance().SetSprite(target.sprite);
                    CardDetailManager.Instance().SetCardDetail(tag, cardId);
                }
            });

        float size = float.Parse(PhotonNetwork.CurrentRoom.CustomProperties["CardSize"].ToString());
        cardImage.rectTransform.sizeDelta = new Vector2(
            cardImage.rectTransform.sizeDelta.x * size,
            cardImage.rectTransform.sizeDelta.y * size
        );

        cardImage.name = name;
        cardImage.gameObject.SetActive(true);
        TouchManager touchManager = card.GetComponent<TouchManager>();
        if (touchManager != null)
        {
            var sizeDelta = touchManager.m_scrollRectTransform.sizeDelta;
            touchManager.m_scrollRectTransform.sizeDelta = new Vector2 (sizeDelta.x, sizeDelta.y * size);
            touchManager.SetIsOpen(isOpen);
        }

        PlayerFieldManager.Instance().AddLogList(name + "を生成");

        return cardImage;
    }

    public void AddLogList(string log)
    {
        var logText = TimeUtil.GetUnixTime(DateTime.Now).ToString() + "," + BattleSceneManager.m_playerName + " --> " + log;
        Debug.Log(logText);
        m_logList.Add(logText);
    }

    public List<string> GetLogList()
    {
        return m_logList;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //データの送信
            if (m_fieldCardManager != null)
            {
                stream.SendNext(m_fieldCardManager.GetFieldCardManagerDataJson());
                stream.SendNext(string.Join("#", m_logList));
            }
        }
        else
        {
            //データの受信
            if (m_fieldCardManager != null)
            {
                m_fieldCardManager.SetFieldCardManagerDataJson((string)stream.ReceiveNext());
                m_logList.Clear();
                m_logList.AddRange(((string)stream.ReceiveNext()).Split("#").ToList());
            }
        }
    }
}
