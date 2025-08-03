using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static CardOptionWindow;

public class PlayerFieldManager : MonoBehaviourPunCallbacks, IPunObservable
{

    [SerializeField]
    private PhotonView m_photonView = null;

    [SerializeField]
    private Button m_optionButton = null;
    [SerializeField]
    private Button m_markButton = null;


    [SerializeField]
    private Button m_countPlusButton = null;
    [SerializeField]
    private Button m_countMinusButton = null;


    [SerializeField]
    private Button m_lifePlusButton = null;
    [SerializeField]
    private Button m_lifeMinusButton = null;

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
    private Image m_field = null;
    [SerializeField]
    private Image m_trash = null;

    [SerializeField]
    private RectTransform m_markField = null;

    [SerializeField]
    private RectTransform m_cardField = null;

    [SerializeField]
    private RectTransform m_coreField = null;

    [SerializeField]
    private FieldCardManager m_fieldCardManager = null;

    private List<GameObject> m_countCoreList = new List<GameObject>();
    private List<GameObject> m_soulCoreList = new List<GameObject>();
    private List<GameObject> m_coreList = new List<GameObject>();

    private ExitGames.Client.Photon.Hashtable m_customRoomProperties = new ExitGames.Client.Photon.Hashtable();

    private static PlayerFieldManager instance = null;
    public static PlayerFieldManager Instance()
    {
        return instance;
    }

    private void Awake()
    {
        if (m_photonView.IsMine || BattleSceneManager.IsNoPlayerInstance(m_photonView)) instance = this;
    }

    private void Start()
    {
        transform.localScale = Vector3.one;
    }

    public override void OnEnable()
    {
        base.OnEnable();

        GameObject canvasPanel = GameObject.FindGameObjectWithTag("FieldPanel");
        transform.SetParent(canvasPanel.transform);

        if (m_fieldCardManager != null)
        {
            m_fieldCardManager.SetActiveToButton(m_photonView.IsMine || BattleSceneManager.IsNoPlayerInstance(m_photonView));
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

        if (SetActive(m_markButton, false))
        {
            m_markButton.onClick.AddListener(() => {
                GameObject mark = PhotonNetwork.Instantiate("Prefab/Battle/Mark", Vector3.zero, Quaternion.identity);
                mark.transform.SetParent(m_markField);
                mark.transform.localPosition = Vector3.zero;
                mark.transform.localScale = Vector3.one;
                mark.transform.localRotation = Quaternion.identity;
                mark.GetComponent<TouchManager>().SetAction(null, null, null, () => {
                    mark.SetActive(false);
                });
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
                m_coreList.Add(core);
            });
        }

        SetActive(m_lifeMinusButton, false);

        if (SetActive(m_soulPlusButton, IsMine))
        {
            m_soulPlusButton.onClick.AddListener(() => {
                GameObject core = PhotonNetwork.Instantiate("Prefab/Battle/SoulCore", Vector3.zero, Quaternion.identity);
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
                m_coreList.Add(core);
            }
        }

        for (int index = 0; index < soulCoreNum; index++)
        {
            GameObject core = PhotonNetwork.Instantiate("Prefab/Battle/SoulCore", Vector3.zero, Quaternion.identity);
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

            m_fieldCardManager.SetDeckDetail(OPTION_TYPE.DECK, jsonDeckDetail.cardDetailList);
            m_fieldCardManager.ShuffleCardDetailList(OPTION_TYPE.DECK);

            if (BattleSceneManager.m_type == "digimon")
            {
                m_fieldCardManager.SetDeckDetail(OPTION_TYPE.SUB, jsonDeckDetail.subCardDetailList);
                m_fieldCardManager.ShuffleCardDetailList(OPTION_TYPE.SUB);
            }

            m_fieldCardManager.SetDeckDetail(OPTION_TYPE.TOKEN, jsonDeckDetail.tokenCardDetailList);


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

    public RectTransform GetMarkField()
    {
        return m_markField;
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
                    FieldCardManager.Instance().SetCardToRest(image);
                    break;
            }
        }
    }

    public void OnClickPlusButton()
    {
        m_plusButton.onClick.Invoke();
    }

    public Image CreateCard(string name)
    {
        string[] list = name.Split('^');
        var cardId = list[1];
        GameObject card = PhotonNetwork.Instantiate("Prefab/Battle/Card", Vector3.zero, Quaternion.identity);
        card.transform.localScale = Vector3.one;
        Image cardImage = FieldCardManager.Instance().CreateCard(
            new DeckManager.CardDetail() { tag = list[0], cardId = cardId }, false, card.GetComponent<Image>(), m_cardField,
            null,
            (Image target, string targetTag, string targetCardId, bool isDoubleClick) => {
                if (isDoubleClick)
                {
                    CardOptionWindow.Instance().Open(target, CardOptionWindow.OPTION_TYPE.FIELD);
                }
                else
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
                    touchManager.IsOverlap = !touchManager.IsOverlap;
                }
            });
        cardImage.name = name;
        cardImage.gameObject.SetActive(true);
        return cardImage;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //データの送信
            if (m_fieldCardManager != null)
            {
                stream.SendNext(m_fieldCardManager.GetFieldCardManagerDataJson());
            }
        }
        else
        {
            //データの受信
            if (m_fieldCardManager != null)
            {
                m_fieldCardManager.SetFieldCardManagerDataJson((string)stream.ReceiveNext());
            }
        }
    }
}
