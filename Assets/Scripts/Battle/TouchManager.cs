using Photon.Pun;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchManager : MonoBehaviourPunCallbacks, IBeginDragHandler, IDragHandler, IEndDragHandler, IPunObservable
{
    [Header("共通")]
    
    [SerializeField]
    private List<Sprite> m_sprites = new List<Sprite>();

    public ConstManager.PhotonObjectType m_photonObjectType = ConstManager.PhotonObjectType.NONE;

    public ConstManager.PhotonObjectType PhotonObjectType
    {
        set
        {
            m_photonObjectType = value;
            if (m_image == null) m_image = gameObject.GetComponent<Image>();
            if (m_image == null) m_image = gameObject.AddComponent<Image>();
            if (m_photonObjectType != ConstManager.PhotonObjectType.CARD)
            {
                var sprite = m_sprites[(int)m_photonObjectType];
                if (sprite != null) m_image.sprite = sprite;
            }

            if (m_inputFields != null)
            {
                m_inputFields.gameObject.SetActive(m_photonObjectType == ConstManager.PhotonObjectType.DAMAGE);
            }
            if (m_photonView == null) m_photonView = gameObject.GetComponent<PhotonView>();
            if (m_photonView == null) m_photonView = gameObject.AddComponent<PhotonView>();
            if (m_TextMeshPro != null)
            {
                m_TextMeshPro.gameObject.SetActive(m_photonObjectType == ConstManager.PhotonObjectType.DAMAGE && !m_photonView.IsMine);
            }
        }
        get
        {
            return m_photonObjectType;
        }
    }

    private bool isDrag = false;

    [Header("カード")]

    [SerializeField]
    private Text m_openText = null;

    [SerializeField]
    private Image m_soulImage = null;

    [SerializeField]
    private Image m_isOverlapImage = null;

    [SerializeField]
    private Image m_isInnerImage = null;

    [SerializeField]
    private ScrollRect m_scrollRect = null;

    [SerializeField]
    private Transform m_content = null;

    [SerializeField]
    private Image m_contentInnerImage = null;

    [Header("ダメージ")]

    [SerializeField]
    private TMPro.TMP_InputField m_inputFields = null;
    [SerializeField]
    private TMPro.TextMeshProUGUI m_TextMeshPro = null;

    private PhotonView m_photonView = null;
    private List<BoxCollider2D> m_boxColliderList = new List<BoxCollider2D>();
    private Image m_image = null;
    private EventTrigger m_eventTrigger = null;

    private System.Action pointerEnterAction = null;
    private System.Action leftClickAction = null;
    private System.Action rightClickAction = null;
    private System.Action middleClickAction = null;


    private string m_endTagStr = "";
    private GameObject m_endObj = null;
    public string EndTag
    {
        set
        {
            m_endTagStr = value;
        }
        get
        {
            return m_endTagStr;
        }
    }

    private bool m_isOpen = false;
    public bool IsOpen
    {
        private set
        {
            m_isOpen = value;
            if (m_isOpen)
            {
                m_openText.text = "表";
            }
            else
            {
                m_openText.text = "裏";
            }

            if (m_photonView.IsMine) return;

            m_image = gameObject.GetComponent<Image>();
            if (m_image == null) m_image = gameObject.AddComponent<Image>();

            if (m_isOpen)
            {
                string[] list = transform.name.Split('^');
                if (list.Length < 2) return;
                m_image.sprite = CardDetailManager.Instance().GetCardSprite(new DeckManager.CardDetail() { tag = list[0], cardId = list[1] });
                
                m_eventTrigger = gameObject.GetComponent<EventTrigger>();
                if (m_eventTrigger == null) m_eventTrigger = gameObject.AddComponent<EventTrigger>();

                // マウスオーバー
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener((_) => {
                    if (CardDetailManager.Instance().isLock)
                    {
                        return;
                    }
                    CardDetailManager.Instance().SetSprite(m_image.sprite);
                    CardDetailManager.Instance().SetCardDetail(list[0], list[1]);
                });
                m_eventTrigger.triggers.Add(entry);
            }
            else
            {
                m_image.sprite = CardDetailManager.Instance().GetSleeveSprite();
                pointerEnterAction = null;
            }
        }
        get
        {
            return m_isOpen;
        }
    }

    private bool m_isSoul = false;
    public bool IsSoul
    {
        private set
        {
            if (m_isOpen)
            {
                m_isSoul = value;
                m_soulImage.gameObject.SetActive(m_isSoul);
            }
        }
        get
        {
            return m_isSoul;
        }
    }


    private bool m_isAwake = false;
    public bool IsAwake
    {
        private set
        {
            m_isAwake = value;

            m_image = gameObject.GetComponent<Image>();
            if (m_image == null) m_image = gameObject.AddComponent<Image>();

            if (m_isOpen)
            {
                string[] list = transform.name.Split('^');
                if (list.Length < 2) return;
                string cardId = list[1];
                if (m_isAwake)
                {
                    cardId = cardId + "_b";
                }
                else if (!m_isAwake && cardId.LastIndexOf("_b") != -1)
                {
                    cardId = cardId.Substring(0, cardId.LastIndexOf("_b"));
                }

                try
                {
                    m_image.sprite = CardDetailManager.Instance().GetCardSprite(new DeckManager.CardDetail() { tag = list[0], cardId = cardId });
                }
                catch
                {
                    cardId = list[1];
                    m_image.sprite = CardDetailManager.Instance().GetCardSprite(new DeckManager.CardDetail() { tag = list[0], cardId = cardId });
                }

                m_eventTrigger = gameObject.GetComponent<EventTrigger>();
                if (m_eventTrigger == null) m_eventTrigger = gameObject.AddComponent<EventTrigger>();
                 
                // マウスオーバー
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener((_) => {
                    if (CardDetailManager.Instance().isLock)
                    {
                        return;
                    }
                    CardDetailManager.Instance().SetSprite(m_image.sprite);
                    CardDetailManager.Instance().SetCardDetail(list[0], cardId);
                });
                m_eventTrigger.triggers.Add(entry);
            }
        }
        get
        {
            return m_isAwake;
        }
    }

    private HashSet<GameObject> m_overlapObjectList = new HashSet<GameObject>();

    private List<Vector2> m_overlapPosList = new List<Vector2>();

    private bool m_isOverlap = false;
    public bool IsOverlap
    {
        set
        {
            m_isOverlap = value;
            m_isOverlapImage.gameObject.SetActive(value);
            if (!value)
            {
                m_overlapObjectList.Clear();
            }
        }
        get
        {
            return m_isOverlap;
        }
    }

    public void AddOoverlapObjectList(GameObject overlapObject)
    {
        if (IsOverlap)
        {
            return;
        }

        m_overlapObjectList.Add(overlapObject);
    }
    public void RemoveOoverlapObjectList(GameObject overlapObject)
    {
        if (m_overlapObjectList.Contains(overlapObject))
        {
            m_overlapObjectList.Remove(overlapObject);
        }
    }

    public List<DeckManager.CardDetail> m_innerCardDetailList = new List<DeckManager.CardDetail>();
    private string m_innerCardDetailListStr = "";
    private List<Image> m_cardImageList = new List<Image>();

    private bool m_isInner = false;
    public bool IsInner
    {
        set
        {
            m_isInner = value;
            m_scrollRect.gameObject.SetActive(!value);
            m_isInnerImage.gameObject.SetActive(value);

            if (!value)
            {
                SetInnerCardDetailList(string.Join("#", m_innerCardDetailList.Select(x => x.ToString())));
            }
        }
        get
        {
            return m_isInner;
        }
    }

    private void SetInnerCardDetailList(string innerCardDetailListStr)
    {
        if (m_innerCardDetailListStr == innerCardDetailListStr) return;

        m_innerCardDetailListStr = innerCardDetailListStr;
        m_innerCardDetailList.Clear();
        if (!string.IsNullOrEmpty(innerCardDetailListStr))
        {
            foreach (string str in m_innerCardDetailListStr.Split("#"))
            {
                var split = str.Split("^");
                if (split.Count() != 2)
                {
                    continue;
                }
                m_innerCardDetailList.Add(new DeckManager.CardDetail() { tag = split[0], cardId = split[1] });
            }
        }

        foreach (Transform c in m_content)
        {
            if (c == m_contentInnerImage.transform)
            {
                continue;
            }
            Destroy(c.gameObject);
        }

        m_cardImageList.Clear();
        foreach (var cardDetail in m_innerCardDetailList)
        {
            var card = FieldCardManager.Instance().CreateCard(cardDetail, true, m_contentInnerImage, m_content,
                (Image target, string tag, string cardId, bool isDoubleClick) => {
                    if (!m_photonView.IsMine) return;
                    int index = m_cardImageList.IndexOf(target);
                    if (index != -1)
                    {
                        AudioSourceManager.Instance().PlayOneShot(0);
                        var cardDetail = m_innerCardDetailList[index - 1];
                        m_innerCardDetailList[index - 1] = m_innerCardDetailList[index];
                        m_innerCardDetailList[index] = cardDetail;
                        SetInnerCardDetailList(string.Join("#", m_innerCardDetailList.Select(x => x.ToString())));
                    }
                },
                (Image target, string tag, string cardId, bool isDoubleClick) => {
                    if (!m_photonView.IsMine) return;
                    int index = m_cardImageList.IndexOf(target);
                    if (index != -1)
                    {
                        AudioSourceManager.Instance().PlayOneShot(0);
                        if (index + 1 < m_innerCardDetailList.Count)
                        {
                            var cardDetail = m_innerCardDetailList[index + 1];
                            m_innerCardDetailList[index + 1] = m_innerCardDetailList[index];
                            m_innerCardDetailList[index] = cardDetail;
                        }
                        else
                        {
                            m_innerCardDetailList.RemoveAt(index);
                            m_innerCardDetailList.Add(new DeckManager.CardDetail() { tag = "", cardId = "" });
                        }
                        
                        SetInnerCardDetailList(string.Join("#", m_innerCardDetailList.Select(x => x.ToString())));
                    }
                },
                (Image target, string tag, string cardId, bool isDoubleClick) => {
                    if (!m_photonView.IsMine) return;
                    int index = m_cardImageList.IndexOf(target);
                    if (index != -1)
                    {
                        PlayerFieldManager.Instance().CreateCard(target.name, true);
                        AudioSourceManager.Instance().PlayOneShot(0);
                        m_innerCardDetailList.RemoveAt(index);
                        SetInnerCardDetailList(string.Join("#", m_innerCardDetailList.Select(x => x.ToString())));
                    }
                }
            );
            card.gameObject.name = cardDetail.ToString();
            m_cardImageList.Add(card);
        }
    }

    public void Awake()
    {
        m_photonView = gameObject.GetComponent<PhotonView>();
        if (m_photonView == null) m_photonView = gameObject.AddComponent<PhotonView>();

        List<GameObject> playFieldList = new List<GameObject>();
        playFieldList.AddRange(GameObject.FindGameObjectsWithTag("PlayField"));
        foreach (var playField in playFieldList)
        {
            PlayerFieldManager playerFieldManager = playField.GetComponent<PlayerFieldManager>();
            if (playerFieldManager == null || playerFieldManager.photonView.Owner != m_photonView.Owner)
            {
                continue;
            }

            switch (m_photonObjectType)
            {
                case ConstManager.PhotonObjectType.CARD:
                    transform.SetParent(playerFieldManager.GetCardField());
                    break;
                case ConstManager.PhotonObjectType.CORE:
                case ConstManager.PhotonObjectType.SOULCORE:
                case ConstManager.PhotonObjectType.DAMAGE:
                    transform.SetParent(playerFieldManager.GetCoreField());
                    break;
                case ConstManager.PhotonObjectType.MARK:
                    transform.SetParent(playerFieldManager.GetMarkField());
                    break;
            }
        }

        BoxCollider2D[] boxColliderList = gameObject.GetComponents<BoxCollider2D>();
        if (boxColliderList.Length < 1)
        {
            BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
            m_boxColliderList.Add(boxCollider);
        }
        else
        {
            m_boxColliderList.AddRange(boxColliderList);
        }

        m_image = gameObject.GetComponent<Image>();
        if (m_image == null) m_image = gameObject.AddComponent<Image>();

        m_eventTrigger = gameObject.GetComponent<EventTrigger>();
        if (m_eventTrigger == null) m_eventTrigger = gameObject.AddComponent<EventTrigger>();

        m_eventTrigger.triggers = new List<EventTrigger.Entry>();
        // マウスオーバー
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((_) => {
            if (pointerEnterAction != null) pointerEnterAction();
        });
        m_eventTrigger.triggers.Add(entry);

        foreach (var boxCollider in m_boxColliderList)
        {
            boxCollider.enabled = m_photonView.IsMine;
        }
        if (!m_photonView.IsMine) return;

        // マウスクリック
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((pointerEventData) => {
            bool isPointerEvent = pointerEventData is PointerEventData;
            if (!isPointerEvent)
            {
                return;
            }

            this.transform.SetAsLastSibling();

            switch ((pointerEventData as PointerEventData).pointerId)
            {
                case -1:
                    Debug.Log("Left Click");
                    if (leftClickAction != null) leftClickAction();
                    break;
                case -2:
                    Debug.Log("Right Click");
                    if (rightClickAction != null) rightClickAction();
                    break;
                case -3:
                    Debug.Log("Middle Click");
                    if (middleClickAction != null) middleClickAction();
                    break;
            }
        });
        m_eventTrigger.triggers.Add(entry);
    }

    private void Start()
    {
        transform.localScale = Vector3.one;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        PhotonNetwork.Destroy(m_photonView);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!m_photonView.IsMine) return;

        isDrag = true;

        m_overlapPosList.Clear();

        if (IsOverlap)
        {
            foreach (var overlapObject in m_overlapObjectList)
            {
                // X
                float x = 0;
                float y = 0;
                if (transform.position.x > overlapObject.transform.position.x)
                {
                    x = -(transform.position.x - overlapObject.transform.position.x);
                }
                else if (transform.position.x < overlapObject.transform.position.x)
                {
                    x =  overlapObject.transform.position.x - transform.position.x;
                }

                if (transform.position.y > overlapObject.transform.position.y)
                {
                    y = -(transform.position.y - overlapObject.transform.position.y);
                }
                else if (transform.position.y < overlapObject.transform.position.y)
                {
                    y = overlapObject.transform.position.y - transform.position.y;
                }

                m_overlapPosList.Add(new Vector2(x, y));
            }
        }

        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData data)
	{
        if (!m_photonView.IsMine) return;

        transform.position = data.position;

        if (IsOverlap)
        {
            int i = 0;
            foreach (var overlapObject in m_overlapObjectList)
            {
                overlapObject.transform.position = data.position + m_overlapPosList[i];
                i++;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!m_photonView.IsMine) return;

        m_overlapPosList.Clear();

        string[] list = m_image.name.Split('^');

        switch (EndTag)
        {
            case "Deck":
                // ドラッグで戻すとデッキの上に戻る
                if (m_photonObjectType != ConstManager.PhotonObjectType.CARD) return;
                foreach (var cardDetail in m_innerCardDetailList)
                {
                    FieldCardManager.Instance().AddDstFromSrc(CardOptionWindow.OPTION_TYPE.FIELD, CardOptionWindow.Instance().innerListFromType, true, m_image, cardDetail.tag, cardDetail.cardId);
                }
                FieldCardManager.Instance().AddDstFromSrc(CardOptionWindow.OPTION_TYPE.FIELD, CardOptionWindow.OPTION_TYPE.DECK, true, m_image, list[0], list[1]);
                AudioSourceManager.Instance().PlayOneShot(0);
                break;
            case "Trash":
                if (m_photonObjectType != ConstManager.PhotonObjectType.CARD) return;
                foreach (var cardDetail in m_innerCardDetailList)
                {
                    FieldCardManager.Instance().AddDstFromSrc(CardOptionWindow.OPTION_TYPE.FIELD, CardOptionWindow.Instance().innerListFromType, true, m_image, cardDetail.tag, cardDetail.cardId);
                }
                FieldCardManager.Instance().AddDstFromSrc(CardOptionWindow.OPTION_TYPE.FIELD, CardOptionWindow.OPTION_TYPE.TRASH, m_image, list[0], list[1]);
                AudioSourceManager.Instance().PlayOneShot(0);
                break;
            case "Exclusion":
                if (m_photonObjectType != ConstManager.PhotonObjectType.CARD) return;
                foreach (var cardDetail in m_innerCardDetailList)
                {
                    FieldCardManager.Instance().AddDstFromSrc(CardOptionWindow.OPTION_TYPE.FIELD, CardOptionWindow.Instance().innerListFromType, true, m_image, cardDetail.tag, cardDetail.cardId);
                }
                FieldCardManager.Instance().AddDstFromSrc(CardOptionWindow.OPTION_TYPE.FIELD, CardOptionWindow.OPTION_TYPE.EXCLUSION, m_image, list[0], list[1]);
                AudioSourceManager.Instance().PlayOneShot(0);
                break;
            case "Hand":
                if (m_photonObjectType != ConstManager.PhotonObjectType.CARD) return;
                foreach (var cardDetail in m_innerCardDetailList)
                {
                    FieldCardManager.Instance().AddDstFromSrc(CardOptionWindow.OPTION_TYPE.FIELD, CardOptionWindow.Instance().innerListFromType, true, m_image, cardDetail.tag, cardDetail.cardId);
                }
                FieldCardManager.Instance().AddDstFromSrc(CardOptionWindow.OPTION_TYPE.FIELD, CardOptionWindow.OPTION_TYPE.HAND, m_image, list[0], list[1]);
                AudioSourceManager.Instance().PlayOneShot(0);
                break;
            case "AtHand":
                if (m_photonObjectType != ConstManager.PhotonObjectType.CARD) return;
                foreach (var cardDetail in m_innerCardDetailList)
                {
                    FieldCardManager.Instance().AddDstFromSrc(CardOptionWindow.OPTION_TYPE.FIELD, CardOptionWindow.Instance().innerListFromType, true, m_image, cardDetail.tag, cardDetail.cardId);
                }
                FieldCardManager.Instance().AddDstFromSrc(CardOptionWindow.OPTION_TYPE.FIELD, CardOptionWindow.OPTION_TYPE.AT_HAND, m_image, list[0], list[1]);
                AudioSourceManager.Instance().PlayOneShot(0);
                break;
            case "Damage":
                if (m_photonObjectType != ConstManager.PhotonObjectType.CARD) return;
                foreach (var cardDetail in m_innerCardDetailList)
                {
                    FieldCardManager.Instance().AddDstFromSrc(CardOptionWindow.OPTION_TYPE.FIELD, CardOptionWindow.Instance().innerListFromType, true, m_image, cardDetail.tag, cardDetail.cardId);
                }
                FieldCardManager.Instance().AddDstFromSrc(CardOptionWindow.OPTION_TYPE.FIELD, CardOptionWindow.OPTION_TYPE.DAMAGE, true, m_image, list[0], list[1]);
                AudioSourceManager.Instance().PlayOneShot(0);
                break;
            case "Sub":
                if (m_photonObjectType != ConstManager.PhotonObjectType.CARD) return;
                foreach (var cardDetail in m_innerCardDetailList)
                {
                    FieldCardManager.Instance().AddDstFromSrc(CardOptionWindow.OPTION_TYPE.FIELD, CardOptionWindow.Instance().innerListFromType, true, m_image, cardDetail.tag, cardDetail.cardId);
                }
                FieldCardManager.Instance().AddDstFromSrc(CardOptionWindow.OPTION_TYPE.FIELD, CardOptionWindow.OPTION_TYPE.SUB, true, m_image, list[0], list[1]);
                AudioSourceManager.Instance().PlayOneShot(0);
                break;
            case "Card":
                if (m_photonObjectType != ConstManager.PhotonObjectType.CARD) return;
                if (m_endObj != null)
                {
                    RemoveOoverlapObjectList(m_endObj);
                    m_endObj.GetComponent<TouchManager>().m_innerCardDetailList.AddRange(m_innerCardDetailList);
                    m_endObj.GetComponent<TouchManager>().m_innerCardDetailList.Add(new DeckManager.CardDetail() { tag = list[0], cardId = list[1] });
                    FieldCardManager.Instance().RemoveCardImage(CardOptionWindow.OPTION_TYPE.FIELD, m_image);
                    AudioSourceManager.Instance().PlayOneShot(0);
                }
                break;
        }

        isDrag = false;
    }

    /// <summary>
    /// オブジェクトが触れている間
    /// </summary>
    /// <param name="coll"></param>
    void OnTriggerStay2D(Collider2D coll)
    {
        if (!m_photonView.IsMine) return;
        if (!isDrag) return;
        if (EndTag == coll.gameObject.tag) return;
        if (coll.gameObject.tag == "Core") return;
        if (coll.gameObject.tag == "Card")
        {
            if (coll.GetComponent<TouchManager>().IsInner)
            {
                m_endObj = coll.gameObject;
                EndTag = coll.gameObject.tag;
            }
            return;
        }

        EndTag = coll.gameObject.tag;
    }

    /// <summary>
    /// オブジェクトが離れた
    /// </summary>
    /// <param name="coll"></param>
    private void OnTriggerExit2D(Collider2D coll)
    {
        //if (!m_photonView.IsMine) return;
        if (coll.gameObject.tag != "Card" && coll.gameObject.tag != "Core") return;

        Debug.Log("OnTriggerExit2D : " + coll.gameObject.name);

        RemoveOoverlapObjectList(coll.gameObject);
    }

    /// <summary>
    /// オブジェクトが重なった
    /// </summary>
    /// <param name="coll"></param>
    private void OnTriggerEnter2D(Collider2D coll)
    {
        //if (!m_photonView.IsMine) return;
        if (!isDrag) return;

        if (coll.gameObject.tag != "Card" && coll.gameObject.tag != "Core") return;

        if (coll.gameObject.tag == "Card" && coll.GetComponent<TouchManager>().IsInner) return;

        Debug.Log("OnTriggerEnter2D : " + coll.gameObject.name);

        AddOoverlapObjectList(coll.gameObject);
    }

    public Image GetImage()
    {
        return m_image;
    }

    public string GetName()
    {
        return gameObject.name;
    }

    public void SetAction(System.Action pointerEnterAction = null,
        System.Action leftClickAction = null,
        System.Action rightClickAction = null,
        System.Action middleClickAction = null)
    {
        if (pointerEnterAction != null) this.pointerEnterAction = pointerEnterAction;
        if (leftClickAction != null) this.leftClickAction = leftClickAction;
        if (rightClickAction != null) this.rightClickAction = rightClickAction;
        if (middleClickAction != null) this.middleClickAction = middleClickAction;
    }

    public void SetIsOpen(bool isOpen)
    {
        IsOpen = isOpen;
    }

    public void SetIsSoul(bool isSoul)
    {
        IsSoul = isSoul;
    }

    public void SetIsAwake(bool isAwake)
    {
        IsAwake = isAwake;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (m_photonView.Synchronization == ViewSynchronization.Unreliable) m_photonView.Synchronization = ViewSynchronization.UnreliableOnChange;

        if (stream.IsWriting)
        {
            //データの送信
            stream.SendNext((int)PhotonObjectType);
            stream.SendNext(transform.GetSiblingIndex());

            if (PhotonObjectType == ConstManager.PhotonObjectType.CARD)
            {
                stream.SendNext(transform.name);
                stream.SendNext(IsOpen);
                stream.SendNext(IsSoul);
                stream.SendNext(IsAwake);
                stream.SendNext(m_innerCardDetailListStr);
            }

            if (PhotonObjectType == ConstManager.PhotonObjectType.DAMAGE)
            {
                stream.SendNext(m_inputFields.text);
            }
        }
        else
        {
            //データの受信
            PhotonObjectType = (ConstManager.PhotonObjectType)(int)stream.ReceiveNext();
            transform.SetSiblingIndex((int)stream.ReceiveNext());

            if (PhotonObjectType == ConstManager.PhotonObjectType.CARD)
            {
                transform.name = (string)stream.ReceiveNext();
                IsOpen = (bool)stream.ReceiveNext();
                IsSoul = (bool)stream.ReceiveNext();
                IsAwake = (bool)stream.ReceiveNext();
                SetInnerCardDetailList((string)stream.ReceiveNext());
            }

            if (PhotonObjectType == ConstManager.PhotonObjectType.DAMAGE)
            {
                m_TextMeshPro.text = (string)stream.ReceiveNext();
            }
        }
    }
}
