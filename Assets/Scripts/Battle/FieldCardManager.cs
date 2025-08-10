using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static CardOptionWindow;
using static DeckManager;

public class FieldCardManager : MonoBehaviour
{
    [Serializable]
    public class FieldCardData
    {
        public List<CardDetail> deckDetailList = new List<CardDetail>();

        public List<string> atHandList = new List<string>();

        public List<string> handList = new List<string>();

        public List<string> fieldCardDetailList = new List<string>();

        public string flash = "";

        public List<CardDetail> trashDetailList = new List<CardDetail>();

        public List<CardDetail> exclusionDetailList = new List<CardDetail>();

        public List<CardDetail> damageDetailList = new List<CardDetail>();

        public List<CardDetail> subDetailList = new List<CardDetail>();
    }

    [SerializeField]
    private PhotonView m_photonView = null;

    [SerializeField]
    private Image m_deckCard = null;

    [SerializeField]
    private Text m_deckCardCountText = null;

    [SerializeField]
    private Image m_trashCard = null;

    [SerializeField]
    private Text m_trashCardCountText = null;

    [SerializeField]
    private Image m_exclusionCard = null;

    [SerializeField]
    private Text m_exclusionCardCountText = null;

    [SerializeField]
    private Image m_damageCard = null;

    [SerializeField]
    private Text m_damageCardCountText = null;

    [SerializeField]
    private Image m_subCard = null;

    [SerializeField]
    private Text m_subCardCountText = null;

    [SerializeField]
    private RectTransform m_atHandContent = null;

    [SerializeField]
    private Image m_atHandCard = null;

    [SerializeField]
    private RectTransform m_handContent = null;

    [SerializeField]
    private Image m_handCard = null;

    private List<CardDetail> m_deckDetailList = new List<CardDetail>();

    private List<CardDetail> m_aceDetailList = new List<CardDetail>();

    private List<CardDetail> m_trashDetailList = new List<CardDetail>();

    private List<CardDetail> m_exclusionDetailList = new List<CardDetail>();

    private List<CardDetail> m_damageDetailList = new List<CardDetail>();

    private List<CardDetail> m_subDetailList = new List<CardDetail>();

    private static FieldCardManager instance = null;
    public static FieldCardManager Instance()
    {
        return instance;
    }

    private void Awake()
    {
        if (m_photonView.IsMine || BattleSceneManager.IsNoPlayerInstance(m_photonView)) instance = this;

        if (m_atHandCard != null)
        {
            m_atHandCard.gameObject.SetActive(false);
        }
        
        m_handCard.gameObject.SetActive(false);

        // m_trashCard.gameObject.SetActive(false);
    }

    public void InitSetting()
    {
        if (BattleSceneManager.m_type == "bs")
        {
            if (DeckManager.IsInContract(m_deckDetailList))
            {
                CardOptionWindow.Instance().Open(null, CardOptionWindow.OPTION_TYPE.DECK, CardOptionWindow.OPTION_TYPE.CONTRACT);
            }
            else
            {
                AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.HAND, true, 4);
            }
        }

        if (BattleSceneManager.m_type == "digimon")
        {
            AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.HAND, true, 5);
            AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.DAMAGE, true, 5);
        }

        if (BattleSceneManager.m_type == "hololive")
        {
            AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.HAND, true, 7);
            // TODO 後々
            AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.DAMAGE, true, 5);
        }
    }

    public EventTrigger GetOrAddComponentToEventTrigger(GameObject obj, EventTrigger.Entry entry)
    {
        EventTrigger cardEventTrigger = obj.GetComponent<EventTrigger>();
        if (cardEventTrigger == null)
        {
            cardEventTrigger = obj.AddComponent<EventTrigger>();
        }
        cardEventTrigger.triggers = new List<EventTrigger.Entry>();
        cardEventTrigger.triggers.Add(entry);
        return cardEventTrigger;
    }

    public void SetActiveToButton(bool isActive)
    {
        bool isMine = isActive && m_photonView.IsMine;
        if (isMine)
        {
            // マウスクリック
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((pointerEventData) => {
                bool isPointerEvent = pointerEventData is PointerEventData;
                if (!isPointerEvent)
                {
                    return;
                }

                switch ((pointerEventData as PointerEventData).pointerId)
                {
                    case -1:
                        Debug.Log("Left Click");
                        CardOptionWindow.Instance().Open(null, CardOptionWindow.OPTION_TYPE.DECK);
                        break;
                    case -2:
                        Debug.Log("Right Click");
                        break;
                    case -3:
                        Debug.Log("Middle Click");
                        break;
                }
            });
            GetOrAddComponentToEventTrigger(m_deckCard.gameObject, entry);

            // マウスクリック
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((pointerEventData) => {
                bool isPointerEvent = pointerEventData is PointerEventData;
                if (!isPointerEvent)
                {
                    return;
                }

                switch ((pointerEventData as PointerEventData).pointerId)
                {
                    case -1:
                        Debug.Log("Left Click");
                        CardOptionWindow.Instance().Open(null, CardOptionWindow.OPTION_TYPE.DAMAGE);
                        break;
                    case -2:
                        Debug.Log("Right Click");
                        break;
                    case -3:
                        Debug.Log("Middle Click");
                        break;
                }
            });
            GetOrAddComponentToEventTrigger(m_damageCard.gameObject, entry);

            // マウスクリック
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((pointerEventData) => {
                bool isPointerEvent = pointerEventData is PointerEventData;
                if (!isPointerEvent)
                {
                    return;
                }

                switch ((pointerEventData as PointerEventData).pointerId)
                {
                    case -1:
                        Debug.Log("Left Click");
                        CardOptionWindow.Instance().Open(null, CardOptionWindow.OPTION_TYPE.SUB);
                        break;
                    case -2:
                        Debug.Log("Right Click");
                        break;
                    case -3:
                        Debug.Log("Middle Click");
                        break;
                }
            });
            GetOrAddComponentToEventTrigger(m_subCard.gameObject, entry);
        }

        if (isActive)
        {
            // マウスクリック
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((pointerEventData) => {
                bool isPointerEvent = pointerEventData is PointerEventData;
                if (!isPointerEvent)
                {
                    return;
                }

                switch ((pointerEventData as PointerEventData).pointerId)
                {
                    case -1:
                        Debug.Log("Left Click");
                        CardOptionWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.TRASH);
                        break;
                    case -2:
                        Debug.Log("Right Click");
                        break;
                    case -3:
                        Debug.Log("Middle Click");
                        break;
                }
            });
            GetOrAddComponentToEventTrigger(m_trashCard.gameObject, entry);

            // マウスクリック
            entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerClick;
            entry.callback.AddListener((pointerEventData) => {
                bool isPointerEvent = pointerEventData is PointerEventData;
                if (!isPointerEvent)
                {
                    return;
                }

                switch ((pointerEventData as PointerEventData).pointerId)
                {
                    case -1:
                        Debug.Log("Left Click");
                        CardOptionWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.EXCLUSION);
                        break;
                    case -2:
                        Debug.Log("Right Click");
                        break;
                    case -3:
                        Debug.Log("Middle Click");
                        break;
                }
            });
            GetOrAddComponentToEventTrigger(m_exclusionCard.gameObject, entry);
        }
    }

    public List<DeckManager.CardDetail> RemoveDeckToContract()
    {
        List<DeckManager.CardDetail> cardDetailList = new List<DeckManager.CardDetail>();

        foreach (DeckManager.CardDetail cardDetail in m_deckDetailList)
        {
            CardData data = AssetBundleManager.Instance().GetBaseData(cardDetail.tag, cardDetail.cardId);
            if (!data.CardCategory.Contains("契約"))
            {
                continue;
            }

            cardDetailList.Add(cardDetail);
            m_deckDetailList.Remove(cardDetail);
            break;
        }

        return cardDetailList;
    }

    public void ShuffleCardDetailList(CardOptionWindow.OPTION_TYPE option)
    {
        List<CardDetail> deckDetailList = new List<CardDetail>();
        switch (option)
        {
            case OPTION_TYPE.DECK:
                deckDetailList = m_deckDetailList;
                break;
            case OPTION_TYPE.TRASH:
                deckDetailList = m_trashDetailList;
                break;
            case OPTION_TYPE.EXCLUSION:
                deckDetailList = m_exclusionDetailList;
                break;
            case OPTION_TYPE.DAMAGE:
                deckDetailList = m_damageDetailList;
                break;
            case OPTION_TYPE.SUB:
                deckDetailList = m_subDetailList;
                break;
            case OPTION_TYPE.TOKEN:
                deckDetailList = m_aceDetailList;
                break;
        }

        for (int index = 0; index < deckDetailList.Count; index++)
        {
            DeckManager.CardDetail tmp = deckDetailList[index];
            int randomIndex = UnityEngine.Random.Range(0, deckDetailList.Count);
            deckDetailList[index] = deckDetailList[randomIndex];
            deckDetailList[randomIndex] = tmp;
        }

        switch (option)
        {
            case OPTION_TYPE.DECK:
                m_deckDetailList = deckDetailList;
                break;
            case OPTION_TYPE.TRASH:
                m_trashDetailList = deckDetailList;
                break;
            case OPTION_TYPE.EXCLUSION:
                m_exclusionDetailList = deckDetailList;
                break;
            case OPTION_TYPE.DAMAGE:
                m_damageDetailList = deckDetailList;
                break;
            case OPTION_TYPE.SUB:
                m_subDetailList = deckDetailList;
                break;
            case OPTION_TYPE.TOKEN:
                m_aceDetailList = deckDetailList;
                break;
        }
    }

    public void AddHandFromDeckToContract()
    {
        List<DeckManager.CardDetail> cardDetailList = RemoveDeckToContract();

        if (cardDetailList.Count < 1)
        {
            Debug.LogWarning("契約スピリットがデッキに入っていません");
        }

        AddCardDetailList(OPTION_TYPE.HAND, cardDetailList);
    }

    public List<GameObject> GetHandGameObject()
    {
        List<GameObject> handObjectList = new List<GameObject>();
        foreach (Transform hand in m_handContent)
        {
            if (hand.gameObject.activeSelf)
            {
                handObjectList.Add(hand.gameObject);
            }
        }
        return handObjectList;
    }

    public void SetDeckDetail(CardOptionWindow.OPTION_TYPE option, List<CardDetail> deckDetailList)
    {
        switch (option)
        {
            case OPTION_TYPE.DECK:
                m_deckDetailList = deckDetailList;
                m_deckCardCountText.text = m_deckDetailList.Count.ToString();
                break;
            case OPTION_TYPE.TRASH:
                m_trashDetailList = deckDetailList;
                m_trashCardCountText.text = m_trashDetailList.Count.ToString();
                break;
            case OPTION_TYPE.EXCLUSION:
                m_exclusionDetailList = deckDetailList;
                m_exclusionCardCountText.text = m_exclusionDetailList.Count.ToString();
                break;
            case OPTION_TYPE.DAMAGE:
                m_damageDetailList = deckDetailList;
                m_damageCardCountText.text = m_damageDetailList.Count.ToString();
                break;
            case OPTION_TYPE.SUB:
                m_subDetailList = deckDetailList;
                m_subCardCountText.text = m_subDetailList.Count.ToString();
                break;
            case OPTION_TYPE.TOKEN:
                m_aceDetailList = deckDetailList;
                break;
        }
    }

    public List<CardDetail> GetCardDetailList(CardOptionWindow.OPTION_TYPE option)
    {
        List<CardDetail> cardDetailList = new List<CardDetail>();
        switch (option)
        {
            case OPTION_TYPE.DECK:
                cardDetailList = m_deckDetailList;
                break;
            case OPTION_TYPE.TRASH:
                if (m_trashDetailList != null)
                {
                    cardDetailList = m_trashDetailList;
                }
                break;
            case OPTION_TYPE.EXCLUSION:
                if (m_exclusionDetailList != null)
                {
                    cardDetailList = m_exclusionDetailList;
                }
                break;
            case OPTION_TYPE.DAMAGE:
                if (m_damageDetailList != null)
                {
                    cardDetailList = m_damageDetailList;
                }
                break;
            case OPTION_TYPE.SUB:
                if (m_subDetailList != null)
                {
                    cardDetailList = m_subDetailList;
                }
                break;
            case OPTION_TYPE.TOKEN:
                if (m_aceDetailList != null)
                {
                    cardDetailList = m_aceDetailList;
                }
                break;
        }
        return cardDetailList;
    }

    public void AddDstFromSrc(OPTION_TYPE optionSrc, CardOptionWindow.OPTION_TYPE optionDst, bool isUp, string tag, string cardId)
    {
        DeckManager.CardDetail cardDetailList = RemoveCardDetail(optionSrc, tag, cardId)[0];
        AddCardDetailList(optionDst, isUp, cardDetailList);
    }

    public void AddDstFromSrc(CardOptionWindow.OPTION_TYPE optionSrc, CardOptionWindow.OPTION_TYPE optionDst, bool isUp, Image card, string tag, string cardId)
    {
        RemoveCardImage(optionSrc, card);
        AddCardDetailList(optionDst, isUp, new DeckManager.CardDetail() { tag = tag, cardId = cardId });
    }

    public DeckManager.CardDetail AddDstFromSrc(
        CardOptionWindow.OPTION_TYPE optionSrc, CardOptionWindow.OPTION_TYPE optionDst, Image card, string tag, string cardId)
    {
        RemoveCardImage(optionSrc, card);
        DeckManager.CardDetail cardDetail = new DeckManager.CardDetail() { tag = tag, cardId = cardId };
        AddCardDetailList(optionDst, new List<DeckManager.CardDetail>() { cardDetail });
        return cardDetail;
    }

    public List<CardDetail> AddDstFromSrc(
        CardOptionWindow.OPTION_TYPE optionSrc, CardOptionWindow.OPTION_TYPE optionDst, bool isUp, int count
    ){
        List<CardDetail> cardDetailList = new List<CardDetail>();
        int index = 0;
        switch (optionSrc)
        {
            case CardOptionWindow.OPTION_TYPE.DECK:
                if (!isUp)
                {
                    index = m_deckDetailList.Count - count;
                }
                cardDetailList = m_deckDetailList.GetRange(index, count);
                m_deckDetailList.RemoveRange(index, count);
                m_deckCardCountText.text = m_deckDetailList.Count.ToString();
                break;
            case CardOptionWindow.OPTION_TYPE.TRASH:
                if (!isUp)
                {
                    index = m_trashDetailList.Count - count;
                }
                cardDetailList = m_trashDetailList.GetRange(index, count);
                m_trashDetailList.RemoveRange(index, count);
                m_trashCardCountText.text = m_trashDetailList.Count.ToString();
                break;
            case CardOptionWindow.OPTION_TYPE.EXCLUSION:
                if (!isUp)
                {
                    index = m_exclusionDetailList.Count - count;
                }
                cardDetailList = m_exclusionDetailList.GetRange(index, count);
                m_exclusionDetailList.RemoveRange(index, count);
                m_exclusionCardCountText.text = m_exclusionDetailList.Count.ToString();
                break;
            case CardOptionWindow.OPTION_TYPE.DAMAGE:
                if (!isUp)
                {
                    index = m_damageDetailList.Count - count;
                }
                cardDetailList = m_damageDetailList.GetRange(index, count);
                m_damageDetailList.RemoveRange(index, count);
                m_damageCardCountText.text = m_damageDetailList.Count.ToString();
                break;
            case CardOptionWindow.OPTION_TYPE.SUB:
                if (!isUp)
                {
                    index = m_subDetailList.Count - count;
                }
                cardDetailList = m_subDetailList.GetRange(index, count);
                m_subDetailList.RemoveRange(index, count);
                m_subCardCountText.text = m_subDetailList.Count.ToString();
                break;
        }
        AddCardDetailList(optionDst, cardDetailList);
        return cardDetailList;
    }

    public List<DeckManager.CardDetail> AddDstFromSrc(
        CardOptionWindow.OPTION_TYPE optionSrc, CardOptionWindow.OPTION_TYPE optionDst, string tag, string cardId
    ){
        List<DeckManager.CardDetail> cardDetailList = RemoveCardDetail(optionSrc, tag, cardId);
        AddCardDetailList(optionDst, cardDetailList);
        return cardDetailList;
    }



    public List<CardDetail> GetCardDetailList(CardOptionWindow.OPTION_TYPE option, bool isUp, int count)
    {
        List<CardDetail> cardDetailList = new List<CardDetail>();
        int index = 0;
        switch (option)
        {
            case CardOptionWindow.OPTION_TYPE.DECK:
                if (!isUp)
                {
                    index = m_deckDetailList.Count - count;
                }
                cardDetailList = m_deckDetailList.GetRange(index, count);
                break;
            case CardOptionWindow.OPTION_TYPE.TRASH:
                if (!isUp)
                {
                    index = m_trashDetailList.Count - count;
                }
                cardDetailList = m_trashDetailList.GetRange(index, count);
                break;
            case CardOptionWindow.OPTION_TYPE.EXCLUSION:
                if (!isUp)
                {
                    index = m_exclusionDetailList.Count - count;
                }
                cardDetailList = m_exclusionDetailList.GetRange(index, count);
                break;
            case CardOptionWindow.OPTION_TYPE.DAMAGE:
                if (!isUp)
                {
                    index = m_damageDetailList.Count - count;
                }
                cardDetailList = m_damageDetailList.GetRange(index, count);
                break;
            case CardOptionWindow.OPTION_TYPE.SUB:
                if (!isUp)
                {
                    index = m_subDetailList.Count - count;
                }
                cardDetailList = m_subDetailList.GetRange(index, count);
                break;
        }
        return cardDetailList;
    }

    public void AddCardDetailList(CardOptionWindow.OPTION_TYPE option, bool isUp, DeckManager.CardDetail cardDetail)
    {
        int index = 0;
        switch (option)
        {
            case CardOptionWindow.OPTION_TYPE.DECK:
                if (!isUp)
                {
                    index = m_deckDetailList.Count;
                }
                m_deckDetailList.Insert(index, cardDetail);
                m_deckCardCountText.text = m_deckDetailList.Count.ToString();
                break;
            case CardOptionWindow.OPTION_TYPE.TRASH:
                if (!isUp)
                {
                    index = m_trashDetailList.Count;
                }
                m_trashDetailList.Insert(index, cardDetail);
                m_trashCardCountText.text = m_trashDetailList.Count.ToString();
                break;
            case CardOptionWindow.OPTION_TYPE.EXCLUSION:
                if (!isUp)
                {
                    index = m_exclusionDetailList.Count;
                }
                m_exclusionDetailList.Insert(index, cardDetail);
                m_exclusionCardCountText.text = m_exclusionDetailList.Count.ToString();
                break;
            case CardOptionWindow.OPTION_TYPE.DAMAGE:
                if (!isUp)
                {
                    index = m_damageDetailList.Count;
                }
                m_damageDetailList.Insert(index, cardDetail);
                m_damageCardCountText.text = m_damageDetailList.Count.ToString();
                break;
            case CardOptionWindow.OPTION_TYPE.SUB:
                if (!isUp)
                {
                    index = m_subDetailList.Count;
                }
                m_subDetailList.Insert(index, cardDetail);
                m_subCardCountText.text = m_subDetailList.Count.ToString();
                break;
        }
    }

    public void AddCardDetailList(CardOptionWindow.OPTION_TYPE option, List<DeckManager.CardDetail> cardDetailList)
    {
        switch (option)
        {
            case OPTION_TYPE.HAND:
                foreach (var cardDetail in cardDetailList)
                {
                    Image card = CreateCard(cardDetail, true, m_handCard, m_handContent,
                        (Image target, string tag, string cardId, bool isDoubleClick) => {
                            CardOptionWindow.Instance().Open(target, CardOptionWindow.OPTION_TYPE.HAND);
                        });
                    card.name = m_handCard.name;
                }
                break;
            case OPTION_TYPE.AT_HAND:
                if (m_atHandCard == null || m_atHandContent == null)
                {
                    return;
                }

                foreach (var cardDetail in cardDetailList)
                {
                    Image card = CreateCard(cardDetail, true, m_atHandCard, m_atHandContent,
                        (Image target, string tag, string cardId, bool isDoubleClick) => {
                            CardOptionWindow.Instance().Open(target, CardOptionWindow.OPTION_TYPE.AT_HAND);
                        });
                    card.name = m_atHandCard.name;
                }
                break;
            case CardOptionWindow.OPTION_TYPE.DECK:
                foreach (var cardDetail in cardDetailList)
                {
                    m_deckDetailList.Add(cardDetail);
                    m_deckCardCountText.text = m_deckDetailList.Count.ToString();
                }
                break;
            case CardOptionWindow.OPTION_TYPE.TRASH:
                foreach (var cardDetail in cardDetailList)
                {
                    m_trashDetailList.Add(cardDetail);
                    m_trashCardCountText.text = m_trashDetailList.Count.ToString();
                }
                break;
            case CardOptionWindow.OPTION_TYPE.EXCLUSION:
                foreach (var cardDetail in cardDetailList)
                {
                    m_exclusionDetailList.Add(cardDetail);
                    m_exclusionCardCountText.text = m_exclusionDetailList.Count.ToString();
                }
                break;
            case CardOptionWindow.OPTION_TYPE.DAMAGE:
                foreach (var cardDetail in cardDetailList)
                {
                    m_damageDetailList.Add(cardDetail);
                    m_damageCardCountText.text = m_damageDetailList.Count.ToString();
                }
                break;
            case CardOptionWindow.OPTION_TYPE.SUB:
                foreach (var cardDetail in cardDetailList)
                {
                    m_subDetailList.Add(cardDetail);
                    m_subCardCountText.text = m_subDetailList.Count.ToString();
                }
                break;
        }
    }

    public List<DeckManager.CardDetail> RemoveCardDetail(CardOptionWindow.OPTION_TYPE option, string tag, string cardId)
    {
        List<DeckManager.CardDetail> cardDetailList = new List<DeckManager.CardDetail>();
        int count = 0;
        switch (option)
        {
            case OPTION_TYPE.DECK:
                foreach (DeckManager.CardDetail cardDetail in m_deckDetailList)
                {
                    if (cardDetail.tag != tag || cardDetail.cardId != cardId)
                    {
                        continue;
                    }

                    cardDetailList.Add(cardDetail);
                    m_deckDetailList.Remove(cardDetail);
                    break;
                }
                m_deckCardCountText.text = m_deckDetailList.Count.ToString();
                break;
            case OPTION_TYPE.TRASH:
                foreach (var cardDetail in m_trashDetailList)
                {
                    if (cardDetail.tag != tag || cardDetail.cardId != cardId)
                    {
                        continue;
                    }
                    cardDetailList.Add(cardDetail);
                    m_trashDetailList.Remove(cardDetail);
                    break;
                }
                count = m_trashDetailList.Count;
                m_trashCardCountText.text = count.ToString();
                if (count <= 0)
                {
                    // m_trashCard.gameObject.SetActive(false);
                }
                break;
            case OPTION_TYPE.EXCLUSION:
                foreach (var cardDetail in m_exclusionDetailList)
                {
                    if (cardDetail.tag != tag || cardDetail.cardId != cardId)
                    {
                        continue;
                    }
                    cardDetailList.Add(cardDetail);
                    m_exclusionDetailList.Remove(cardDetail);
                    break;
                }
                count = m_exclusionDetailList.Count;
                m_exclusionCardCountText.text = count.ToString();
                if (count <= 0) { }
                break;
            case OPTION_TYPE.DAMAGE:
                foreach (var cardDetail in m_damageDetailList)
                {
                    if (cardDetail.tag != tag || cardDetail.cardId != cardId)
                    {
                        continue;
                    }
                    cardDetailList.Add(cardDetail);
                    m_damageDetailList.Remove(cardDetail);
                    break;
                }
                count = m_damageDetailList.Count;
                m_damageCardCountText.text = count.ToString();
                if (count <= 0) { }
                break;
            case OPTION_TYPE.SUB:
                foreach (var cardDetail in m_subDetailList)
                {
                    if (cardDetail.tag != tag || cardDetail.cardId != cardId)
                    {
                        continue;
                    }
                    cardDetailList.Add(cardDetail);
                    m_subDetailList.Remove(cardDetail);
                    break;
                }
                count = m_subDetailList.Count;
                m_subCardCountText.text = count.ToString();
                if (count <= 0) { }
                break;
        }
        return cardDetailList;
    }

    public void RemoveCardImage(CardOptionWindow.OPTION_TYPE option, Image card)
    {
        switch(option)
        {
            case OPTION_TYPE.HAND:
            case OPTION_TYPE.AT_HAND:
                Destroy(card.gameObject);
                break;
            case OPTION_TYPE.FIELD:
            case OPTION_TYPE.BURST:
            case OPTION_TYPE.FLASH:
                card.sprite = CardDetailManager.Instance().GetSleeveSprite();
                card.name = "";
                card.gameObject.SetActive(false);
                break;
        }
    }

    public void SetCardToStand(Image card)
    {
        card.rectTransform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public void SetCardToRest(Image card)
    {
        card.rectTransform.localRotation = Quaternion.Euler(0, 0, -90);
    }

    public void SetCardToDualRest(Image card)
    {
        card.rectTransform.localRotation = Quaternion.Euler(0, 0, -180);
    }

    public Image CreateCard(DeckManager.CardDetail cardDetail, bool isInstantiate, Image targetImage,
        Transform parent = null,
        Action<Image, string, string, bool> leftClickAction = null,
        Action<Image, string, string, bool> rightClickAction = null,
        Action<Image, string, string, bool> middleClickAction = null)
    {
        string tag = cardDetail.tag;
        string fileName = cardDetail.cardId;

        Image copied = targetImage;
        copied.name = tag + "^" + fileName;
        if (isInstantiate)
        {
            copied = UnityEngine.Object.Instantiate(targetImage);
        }
        if (parent != null)
        {
            copied.rectTransform.SetParent(parent);
        }
        copied.rectTransform.localPosition = Vector3.zero;
        copied.rectTransform.localRotation = Quaternion.Euler(0, 0, 0);
        copied.rectTransform.localScale = targetImage.rectTransform.localScale;
        copied.sprite = CardDetailManager.Instance().GetCardSprite(cardDetail);

        // マウスオーバー
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((_) => {
            if (CardDetailManager.Instance().isLock)
            {
                return;
            }
            CardDetailManager.Instance().SetSprite(copied.sprite);
            CardDetailManager.Instance().SetCardDetail(tag, fileName);
        });
        EventTrigger cardEventTrigger = GetOrAddComponentToEventTrigger(copied.gameObject, entry);

        // マウスクリック
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((pointerEventData) => {
            bool isPointerEvent = pointerEventData is PointerEventData;
            if (!isPointerEvent)
            {
                return;
            }

            MouseManager.Instance().OnClick((pointerEventData as PointerEventData).pointerId, (int pointerId, bool isDoubleClick) =>
            {
                switch (pointerId)
                {
                    case -1:
                        Debug.Log("Left Click");
                        if (leftClickAction != null)
                        {
                            leftClickAction(copied, tag, fileName, isDoubleClick);
                        }
                        break;
                    case -2:
                        Debug.Log("Right Click");
                        if (rightClickAction != null)
                        {
                            rightClickAction(copied, tag, fileName, isDoubleClick);
                        }
                        break;
                    case -3:
                        Debug.Log("Middle Click");
                        if (middleClickAction != null)
                        {
                            middleClickAction(copied, tag, fileName, isDoubleClick);
                        }
                        break;
                }
            });
        });
        cardEventTrigger.triggers.Add(entry);
        copied.gameObject.SetActive(true);
        return copied;
    }

    // 差分確認用Json
    private string m_fieldCardManagerDataJson = "";

    public void SetFieldCardManagerDataJson(string fieldCardManagerDataJson)
    {
        if (m_fieldCardManagerDataJson == fieldCardManagerDataJson) return;
        m_fieldCardManagerDataJson = fieldCardManagerDataJson;

        FieldCardData fieldCardData = JsonUtility.FromJson<FieldCardData>(fieldCardManagerDataJson);

        SetDeckDetail(OPTION_TYPE.DECK, fieldCardData.deckDetailList);

        List<GameObject> handObjectList = new List<GameObject>();
        foreach (Transform hand in m_handContent)
        {
            if (hand.gameObject.activeSelf)
            {
                handObjectList.Add(hand.gameObject);
            }
        }
        for (var index = 0; index < handObjectList.Count; index++)
        {
            if (fieldCardData.handList.Count <= index)
            {
                Destroy(handObjectList[index]);
                continue;
            }

            handObjectList[index].name = fieldCardData.handList[index];
        }
        if (fieldCardData.handList.Count - handObjectList.Count > 0)
        {
            for (var index = handObjectList.Count; index < fieldCardData.handList.Count; index++)
            {
                Image copied = UnityEngine.Object.Instantiate(m_handCard);
                copied.rectTransform.SetParent(m_handContent);
                copied.rectTransform.localPosition = Vector3.zero;
                copied.rectTransform.localScale = m_handCard.rectTransform.localScale;
                copied.sprite = CardDetailManager.Instance().GetSleeveSprite();
                copied.name = fieldCardData.handList[index];
                copied.GetComponent<HandCard>().enabled = false;
                copied.gameObject.SetActive(true);
            }
        }

        List<GameObject> atHandObjectList = new List<GameObject>();
        if(m_atHandContent != null)
        {
            foreach (Transform atHand in m_atHandContent)
            {
                if (atHand.gameObject.activeSelf)
                {
                    atHandObjectList.Add(atHand.gameObject);
                }
            }
        }
        for (var index = 0; index < atHandObjectList.Count; index++)
        {
            if (fieldCardData.atHandList.Count <= index)
            {
                Destroy(atHandObjectList[index]);
                continue;
            }

            atHandObjectList[index].name = fieldCardData.atHandList[index].Split('#')[0];
        }

        Sprite sleeveSprite = CardDetailManager.Instance().GetSleeveSprite();

        for (var index = 0; index < fieldCardData.atHandList.Count; index++)
        {
            Image copied = null;
            if (index < atHandObjectList.Count)
            {
                copied = atHandObjectList[index].GetComponent<Image>();
            }
            else
            {
                if (m_atHandCard == null || m_atHandContent == null)
                {
                    continue;
                }
                copied = UnityEngine.Object.Instantiate(m_atHandCard);
                copied.rectTransform.SetParent(m_atHandContent);
                copied.rectTransform.localPosition = Vector3.zero;
                copied.rectTransform.localScale = m_atHandCard.rectTransform.localScale;
                copied.rectTransform.localRotation = Quaternion.Euler(0, 0, 0);
            }

            HandCard handCard = copied.GetComponent<HandCard>();
            EventTrigger cardEventTrigger = copied.GetComponent<EventTrigger>();
            string[] list = fieldCardData.atHandList[index].Split('#');
            copied.name = list[0];
            if (list[1] == "False" && copied.sprite != sleeveSprite)
            {
                handCard.SetIsOpen(false);
                copied.sprite = sleeveSprite;
                cardEventTrigger.triggers = new List<EventTrigger.Entry>();
            }
            else if (list[1] == "True" && copied.sprite == sleeveSprite)
            {
                handCard.SetIsOpen(true);
                string[] namelist = copied.name.Split('^');
                copied.sprite = CardDetailManager.Instance().GetCardSprite(new DeckManager.CardDetail() { tag = namelist[0], cardId = namelist[1] });
                cardEventTrigger.triggers = new List<EventTrigger.Entry>();
                // マウスオーバー
                EventTrigger.Entry entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerEnter;
                entry.callback.AddListener((_) => {
                    if (CardDetailManager.Instance().isLock)
                    {
                        return;
                    }
                    CardDetailManager.Instance().SetSprite(copied.sprite);
                    CardDetailManager.Instance().SetCardDetail(namelist[0], namelist[1]);
                });
                cardEventTrigger.triggers.Add(entry);
            }

            copied.GetComponent<HandCard>().enabled = false;
            copied.gameObject.SetActive(true);
        }

        SetDeckDetail(OPTION_TYPE.TRASH, fieldCardData.trashDetailList);
        SetDeckDetail(OPTION_TYPE.EXCLUSION, fieldCardData.exclusionDetailList);

        SetDeckDetail(OPTION_TYPE.DAMAGE, fieldCardData.damageDetailList);
        SetDeckDetail(OPTION_TYPE.SUB, fieldCardData.subDetailList);
    }

    public string GetFieldCardManagerDataJson()
    {
        FieldCardData fieldCardData = new FieldCardData();

        fieldCardData.deckDetailList = m_deckDetailList;

        foreach (Transform hand in m_handContent)
        {
            if (hand.gameObject.activeSelf)
            {
                fieldCardData.handList.Add(hand.name);
            }
        }

        if (m_atHandContent != null)
        {
            foreach (Transform atHand in m_atHandContent)
            {
                if (atHand.gameObject.activeSelf)
                {
                    HandCard handCard = atHand.GetComponent<HandCard>();
                    fieldCardData.atHandList.Add(atHand.name + "#" + handCard.IsOpen.ToString());
                }
            }
        }

        fieldCardData.trashDetailList = m_trashDetailList;
        fieldCardData.exclusionDetailList = m_exclusionDetailList;

        fieldCardData.damageDetailList = m_damageDetailList;
        fieldCardData.subDetailList = m_subDetailList;

        return JsonUtility.ToJson(fieldCardData);
    }
}
