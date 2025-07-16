using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchManager : MonoBehaviourPunCallbacks, IBeginDragHandler, IDragHandler, IEndDragHandler, IPunObservable
{
    [SerializeField]
    private Text m_openText = null;

    [SerializeField]
    private Image m_soulImage = null;

    public ConstManager.PhotonObjectType m_photonObjectType = ConstManager.PhotonObjectType.NONE;

    private PhotonView m_photonView = null;
    private BoxCollider2D m_boxCollider = null;
    private Image m_image = null;
    private EventTrigger m_eventTrigger = null;

    private Action pointerEnterAction = null;
    private Action leftClickAction = null;
    private Action rightClickAction = null;
    private Action middleClickAction = null;

    [SerializeField]
    public string m_endTagStr = "";
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
                    transform.SetParent(playerFieldManager.GetCoreField());
                    break;
                case ConstManager.PhotonObjectType.MARK:
                    transform.SetParent(playerFieldManager.GetMarkField());
                    break;
            }
        }

        m_boxCollider = gameObject.GetComponent<BoxCollider2D>();
        if (m_boxCollider == null) m_boxCollider = gameObject.AddComponent<BoxCollider2D>();

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

        m_boxCollider.enabled = m_photonView.IsMine;
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

        transform.SetAsLastSibling();
    }

    public void OnDrag(PointerEventData data)
	{
        if (!m_photonView.IsMine) return;

        transform.position = data.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!m_photonView.IsMine) return;

        string[] list = m_image.name.Split('^');

        switch (m_endTagStr)
        {
            case "Deck":
                // ドラッグで戻すとデッキの上に戻る
                if (m_photonObjectType != ConstManager.PhotonObjectType.CARD) return;
                FieldCardManager.Instance().AddDeckFromFieldOrBurstOrFlash(true, m_image, list[0], list[1]);
                AudioSourceManager.Instance().PlayOneShot(0);
                break;
            case "Trash":
                if (m_photonObjectType != ConstManager.PhotonObjectType.CARD) return;
                FieldCardManager.Instance().AddTrashOrExclusionFromFieldOrBurstOrFlash(m_image, list[0], list[1], true);
                AudioSourceManager.Instance().PlayOneShot(0);
                break;
            case "Exclusion":
                if (m_photonObjectType != ConstManager.PhotonObjectType.CARD) return;
                FieldCardManager.Instance().AddTrashOrExclusionFromFieldOrBurstOrFlash(m_image, list[0], list[1], false);
                AudioSourceManager.Instance().PlayOneShot(0);
                break;
            case "Hand":
                if (m_photonObjectType != ConstManager.PhotonObjectType.CARD) return;
                FieldCardManager.Instance().AddHandFromFieldOrBurstOrFlash(m_image, list[0], list[1]);
                AudioSourceManager.Instance().PlayOneShot(0);
                break;
            case "AtHand":
                if (m_photonObjectType != ConstManager.PhotonObjectType.CARD) return;
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.FIELD, OPTION_TYPE.AT_HAND, m_image, list[0], list[1]);
                AudioSourceManager.Instance().PlayOneShot(0);
                break;
            case "Securlty":
                if (m_photonObjectType != ConstManager.PhotonObjectType.CARD) return;
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.FIELD, OPTION_TYPE.SECURLTY, true, m_image, list[0], list[1]);
                AudioSourceManager.Instance().PlayOneShot(0);
                break;
            case "Digitama":
                if (m_photonObjectType != ConstManager.PhotonObjectType.CARD) return;
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.FIELD, OPTION_TYPE.DIGITAMA, true, m_image, list[0], list[1]);
                AudioSourceManager.Instance().PlayOneShot(0);
                break;
        }
    }

    void OnTriggerStay2D(Collider2D coll)
    {
        if (!m_photonView.IsMine) return;

        if (m_endTagStr == coll.gameObject.tag) return;

        m_endTagStr = coll.gameObject.tag;
    }

    public Image GetImage()
    {
        return m_image;
    }

    public string GetName()
    {
        return gameObject.name;
    }

    public void SetAction(Action pointerEnterAction = null,
        Action leftClickAction = null,
        Action rightClickAction = null,
        Action middleClickAction = null)
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
        if (m_photonObjectType != ConstManager.PhotonObjectType.CARD) return;
        if (m_photonView.Synchronization == ViewSynchronization.Unreliable) m_photonView.Synchronization = ViewSynchronization.UnreliableOnChange;

        if (stream.IsWriting)
        {
            //データの送信
            stream.SendNext(transform.name);
            stream.SendNext(transform.GetSiblingIndex());
            stream.SendNext(IsOpen);
            stream.SendNext(IsSoul);
            stream.SendNext(IsAwake);
        }
        else
        {
            //データの受信
            transform.name = (string)stream.ReceiveNext();
            transform.SetSiblingIndex((int)stream.ReceiveNext());
            IsOpen = (bool)stream.ReceiveNext();
            IsSoul = (bool)stream.ReceiveNext();
            IsAwake = (bool)stream.ReceiveNext();
        }
    }
}
