﻿using Photon.Pun;
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
        public DeckManager.DeckDetail deckDetail = new DeckManager.DeckDetail();

        public List<string> atHandList = new List<string>();

        public List<string> handList = new List<string>();

        public List<string> fieldCardDetailList = new List<string>();

        public string flash = "";

        public DeckManager.DeckDetail trashDetail = new DeckManager.DeckDetail();

        public DeckManager.DeckDetail exclusionDetail = new DeckManager.DeckDetail();

        public DeckManager.DeckDetail securltyDetail = new DeckManager.DeckDetail();

        public DeckManager.DeckDetail digitamaDetail = new DeckManager.DeckDetail();
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
    private RectTransform m_atHandContent = null;

    [SerializeField]
    private Image m_atHandCard = null;

    [SerializeField]
    private RectTransform m_handContent = null;

    [SerializeField]
    private Image m_handCard = null;

    private DeckManager.DeckDetail m_deckDetail = new DeckManager.DeckDetail();

    private DeckManager.DeckDetail m_trashDetail = new DeckManager.DeckDetail();

    private DeckManager.DeckDetail m_exclusionDetail = new DeckManager.DeckDetail();

    private DeckManager.DeckDetail m_securltyDetail = new DeckManager.DeckDetail();

    private DeckManager.DeckDetail m_digitamaDetail = new DeckManager.DeckDetail();

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
        if (m_deckDetail.IsInContract())
        {
            CardOptionWindow.Instance().Open(null, CardOptionWindow.OPTION_TYPE.DECK, CardOptionWindow.OPTION_TYPE.CONTRACT);
        }
        else
        {
            AddSrcFromDst(OPTION_TYPE.DECK, OPTION_TYPE.HAND, true, 4);
        }
    }

    public void SetActiveToButton(bool isActive)
    {
        EventTrigger cardEventTrigger = null;

        bool isMine = isActive && m_photonView.IsMine;
        if (isMine)
        {
            // デッキ
            cardEventTrigger = m_deckCard.GetComponent<EventTrigger>();
            if (cardEventTrigger == null)
            {
                cardEventTrigger = m_deckCard.gameObject.AddComponent<EventTrigger>();
            }
            cardEventTrigger.triggers = new List<EventTrigger.Entry>();
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
            cardEventTrigger.triggers.Add(entry);
        }

        if (isActive)
        {
            // トラッシュ
            cardEventTrigger = m_trashCard.GetComponent<EventTrigger>();
            if (cardEventTrigger == null)
            {
                cardEventTrigger = m_trashCard.gameObject.AddComponent<EventTrigger>();
            }
            cardEventTrigger.triggers = new List<EventTrigger.Entry>();
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
            cardEventTrigger.triggers.Add(entry);

            // 除外
            cardEventTrigger = m_exclusionCard.GetComponent<EventTrigger>();
            if (cardEventTrigger == null)
            {
                cardEventTrigger = m_exclusionCard.gameObject.AddComponent<EventTrigger>();
            }
            cardEventTrigger.triggers = new List<EventTrigger.Entry>();
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
            cardEventTrigger.triggers.Add(entry);
        }
    }

    public void SetDeckOrTrashOrExclusion(CardOptionWindow.OPTION_TYPE option, DeckManager.DeckDetail deckDetail)
    {
        switch (option)
        {
            case OPTION_TYPE.DECK:
                m_deckDetail = deckDetail;
                m_deckCardCountText.text = m_deckDetail.cardDetailList.Count.ToString();
                break;
            case OPTION_TYPE.TRASH:
                m_trashDetail = deckDetail;
                m_trashCardCountText.text = m_trashDetail.cardDetailList.Count.ToString();
                break;
            case OPTION_TYPE.EXCLUSION:
                m_exclusionDetail = deckDetail;
                m_exclusionCardCountText.text = m_exclusionDetail.cardDetailList.Count.ToString();
                break;
        }
    }

    public void AddDeckFromDeck(bool isUp, Image card, string tag, string cardId)
    {
        List<DeckManager.CardDetail> cardDetailList = RemoveDeck(tag, cardId);
        AddDeck(isUp, cardDetailList[0]);
        // Destroy(card.gameObject);
    }

    public void AddDeckFromHandOrAtHand(bool isUp, Image card, string tag, string cardId)
    {
        AddDeck(isUp, new DeckManager.CardDetail() { tag = tag, cardId = cardId });
        Destroy(card.gameObject);
    }

    public void AddDeckFromFieldOrBurstOrFlash(bool isUp, Image card, string tag, string cardId)
    {
        AddDeck(isUp, new DeckManager.CardDetail() { tag = tag, cardId = cardId });
        card.sprite = CardDetailManager.Instance().GetSleeveSprite();
        card.name = "";
        card.gameObject.SetActive(false);
    }

    public DeckManager.CardDetail AddDeckFromTrash(bool isUp, string tag, string cardId)
    {
        DeckManager.CardDetail cardDetail = RemoveTrashOrExclusion(OPTION_TYPE.TRASH, tag, cardId);
        AddDeck(isUp, cardDetail);
        return cardDetail;
    }

    public DeckManager.CardDetail AddDeckFromExclusion(bool isUp, string tag, string cardId)
    {
        DeckManager.CardDetail cardDetail = RemoveTrashOrExclusion(OPTION_TYPE.EXCLUSION, tag, cardId);
        AddDeck(isUp, cardDetail);
        return cardDetail;
    }

    public void AddDeck(bool isUp, DeckManager.CardDetail cardDetail)
    {
        int index = 0;
        if (!isUp)
        {
            index = m_deckDetail.cardDetailList.Count;
        }
        m_deckDetail.cardDetailList.Insert(index, cardDetail);
        m_deckCardCountText.text = m_deckDetail.cardDetailList.Count.ToString();
    }

    public List<DeckManager.CardDetail> RemoveDeckToContract()
    {
        List<DeckManager.CardDetail> cardDetailList = new List<DeckManager.CardDetail>();

        foreach (DeckManager.CardDetail cardDetail in m_deckDetail.cardDetailList)
        {
            CardData data = AssetBundleManager.Instance().GetBaseData(cardDetail.tag, cardDetail.cardId);
            if (!data.CardCategory.Contains("契約"))
            {
                continue;
            }

            cardDetailList.Add(cardDetail);
            m_deckDetail.cardDetailList.Remove(cardDetail);
            break;
        }

        return cardDetailList;
    }

    public List<DeckManager.CardDetail> RemoveDeck(string tag, string cardId)
    {
        List<DeckManager.CardDetail> cardDetailList = new List<DeckManager.CardDetail>();

        foreach (DeckManager.CardDetail cardDetail in m_deckDetail.cardDetailList)
        {
            if (cardDetail.tag != tag || cardDetail.cardId != cardId)
            {
                continue;
            }

            cardDetailList.Add(cardDetail);
            m_deckDetail.cardDetailList.Remove(cardDetail);
            break;
        }
        m_deckCardCountText.text = m_deckDetail.cardDetailList.Count.ToString();

        return cardDetailList;
    }

    public List<DeckManager.CardDetail> RemoveDeck(bool isUp, int count)
    {
        int index = 0;
        if (!isUp)
        {
            index = m_deckDetail.cardDetailList.Count - count;
        }
        List<DeckManager.CardDetail> cardDetailList = m_deckDetail.cardDetailList.GetRange(index, count);
        m_deckDetail.cardDetailList.RemoveRange(index, count);
        m_deckCardCountText.text = m_deckDetail.cardDetailList.Count.ToString();

        return cardDetailList;
    }

    public List<DeckManager.CardDetail> GetDeck(bool isUp, int count)
    {
        int index = 0;
        if (!isUp)
        {
            index = m_deckDetail.cardDetailList.Count - count;
        }

        return m_deckDetail.cardDetailList.GetRange(index, count);
    }

    public List<DeckManager.CardDetail> GetDeckOrTrashOrExclusion(CardOptionWindow.OPTION_TYPE option)
    {
        List<DeckManager.CardDetail> cardDetailList = new List<CardDetail>();
        switch (option)
        {
            case OPTION_TYPE.DECK:
                cardDetailList = m_deckDetail.cardDetailList;
                break;
            case OPTION_TYPE.TRASH:
                if (m_trashDetail != null && m_trashDetail.cardDetailList != null)
                {
                    cardDetailList = m_trashDetail.cardDetailList;
                }
                break;
            case OPTION_TYPE.EXCLUSION:
                if (m_exclusionDetail != null && m_exclusionDetail.cardDetailList != null)
                {
                    cardDetailList = m_exclusionDetail.cardDetailList;
                }
                break;
        }
        return cardDetailList;
    }

    public void ShuffleDeck()
    {
        for (int index = 0; index < m_deckDetail.cardDetailList.Count; index++)
        {
            DeckManager.CardDetail tmp = m_deckDetail.cardDetailList[index];
            int randomIndex = UnityEngine.Random.Range(0, m_deckDetail.cardDetailList.Count);
            m_deckDetail.cardDetailList[index] = m_deckDetail.cardDetailList[randomIndex];
            m_deckDetail.cardDetailList[randomIndex] = tmp;
        }
    }

    public void AddHandFromDeckToContract()
    {
        List<DeckManager.CardDetail> cardDetailList = RemoveDeckToContract();

        if (cardDetailList.Count < 1)
        {
            Debug.LogWarning("契約スピリットがデッキに入っていません");
        }

        AddHand(cardDetailList);
    }

    public void AddHandFromAtHand(Image card, string tag, string cardId)
    {
        AddHand(new List<DeckManager.CardDetail>() { new DeckManager.CardDetail() { tag = tag, cardId = cardId } });
        Destroy(card.gameObject);
    }

    public void AddHandFromFieldOrBurstOrFlash(Image card, string tag, string cardId)
    {
        AddHand(new List<DeckManager.CardDetail>() { new DeckManager.CardDetail() { tag = tag, cardId = cardId } });
        card.sprite = CardDetailManager.Instance().GetSleeveSprite();
        card.name = "";
        card.gameObject.SetActive(false);
    }

    public List<DeckManager.CardDetail> AddSrcFromDst(
        CardOptionWindow.OPTION_TYPE optionSrc, CardOptionWindow.OPTION_TYPE optionDst, bool isUp, int count
    ){
        List<DeckManager.CardDetail> cardDetailList = new List<CardDetail>();
        switch (optionSrc)
        {
            case OPTION_TYPE.DECK:
                cardDetailList = RemoveDeck(isUp, count);
                break;
        }
        AddDst(optionDst, cardDetailList);
        return cardDetailList;
    }

    public List<DeckManager.CardDetail> AddSrcFromDst(
        CardOptionWindow.OPTION_TYPE optionSrc, CardOptionWindow.OPTION_TYPE optionDst, string tag, string cardId
    ){
        List<DeckManager.CardDetail> cardDetailList = new List<CardDetail>();
        switch (optionSrc)
        {
            case OPTION_TYPE.DECK:
                cardDetailList = RemoveDeck(tag, cardId);
                break;
            case OPTION_TYPE.TRASH:
                cardDetailList.Add(RemoveTrashOrExclusion(OPTION_TYPE.TRASH, tag, cardId));
                break;
            case OPTION_TYPE.EXCLUSION:
                cardDetailList.Add(RemoveTrashOrExclusion(OPTION_TYPE.EXCLUSION, tag, cardId));
                break;
        }
        AddDst(optionDst, cardDetailList);
        return cardDetailList;
    }

    public void AddDst(CardOptionWindow.OPTION_TYPE option, List<DeckManager.CardDetail> cardDetailList)
    {
        switch (option)
        {
            case OPTION_TYPE.HAND:
                AddHand(cardDetailList);
                break;
            case OPTION_TYPE.AT_HAND:
                AddAtHand(cardDetailList);
                break;
            case CardOptionWindow.OPTION_TYPE.TRASH:
                foreach (var cardDetail in cardDetailList)
                {
                    AddTrashOrExclusion(cardDetail, true);
                }
                break;
            case CardOptionWindow.OPTION_TYPE.EXCLUSION:
                foreach (var cardDetail in cardDetailList)
                {
                    AddTrashOrExclusion(cardDetail, false);
                }
                break;
        }
    }

    public void AddHand(List<DeckManager.CardDetail> cardDetailList)
    {
        foreach (var cardDetail in cardDetailList)
        {
            Image card = CreateCard(cardDetail, true, m_handCard, m_handContent,
                (Image target, string tag, string cardId) => {
                    CardOptionWindow.Instance().Open(target, CardOptionWindow.OPTION_TYPE.HAND);
                });
            card.name = m_handCard.name;
        }
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
    

    public void AddAtHandFromHand(Image card, string tag, string cardId)
    {
        AddAtHand(new List<DeckManager.CardDetail>() { new DeckManager.CardDetail() { tag = tag, cardId = cardId } });
        Destroy(card.gameObject);
    }

    public void AddAtHandFromFieldOrBurstOrFlash(Image card, string tag, string cardId)
    {
        AddAtHand(new List<DeckManager.CardDetail>() { new DeckManager.CardDetail() { tag = tag, cardId = cardId } });
        card.sprite = CardDetailManager.Instance().GetSleeveSprite();
        card.name = "";
        card.gameObject.SetActive(false);
    }

    public void AddAtHand(List<DeckManager.CardDetail> cardDetailList)
    {
        if (m_atHandCard == null || m_atHandContent == null)
        {
            return;
        }

        foreach (var cardDetail in cardDetailList)
        {
            Image card = CreateCard(cardDetail, true, m_atHandCard, m_atHandContent,
                (Image target, string tag, string cardId) => {
                    CardOptionWindow.Instance().Open(target, CardOptionWindow.OPTION_TYPE.AT_HAND);
                });
            card.name = m_atHandCard.name;
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

    public void RemoveField(Image card)
    {
        Destroy(card.gameObject);
    }

    public DeckManager.CardDetail AddTrashOrExclusionFromHandOrAtHand(Image card, string tag, string cardId, bool isTrash)
    {
        DeckManager.CardDetail cardDetail = new DeckManager.CardDetail() { tag = tag, cardId = cardId };
        AddTrashOrExclusion(cardDetail, isTrash);
        Destroy(card.gameObject);
        return cardDetail;
    }

    public void AddTrashOrExclusionFromFieldOrBurstOrFlash(Image card, string tag, string cardId, bool isTrash)
    {
        AddTrashOrExclusion(new DeckManager.CardDetail() { tag = tag, cardId = cardId }, isTrash);
        card.sprite = CardDetailManager.Instance().GetSleeveSprite();
        card.name = "";
        card.gameObject.SetActive(false);
    }

    public void AddTrashOrExclusion(string tag, string cardId, bool isTrash)
    {
        AddTrashOrExclusion(new DeckManager.CardDetail() { tag = tag, cardId = cardId }, isTrash);
    }

    public void AddTrashOrExclusion(DeckManager.CardDetail cardDetail, bool isTrash)
    {
        if (isTrash)
        {
            m_trashDetail.cardDetailList.Add(cardDetail);
            m_trashCardCountText.text = m_trashDetail.cardDetailList.Count.ToString();
            return;
        }

        m_exclusionDetail.cardDetailList.Add(cardDetail);
        m_exclusionCardCountText.text = m_exclusionDetail.cardDetailList.Count.ToString();
    }

    public DeckManager.CardDetail RemoveTrashOrExclusion(CardOptionWindow.OPTION_TYPE option, string tag, string cardId)
    {
        DeckManager.CardDetail trashCard = null;
        int count = 0;
        switch (option)
        {
            case OPTION_TYPE.TRASH:
                foreach (var cardDetail in m_trashDetail.cardDetailList)
                {
                    if (cardDetail.tag != tag || cardDetail.cardId != cardId)
                    {
                        continue;
                    }

                    trashCard = new DeckManager.CardDetail()
                    {
                        tag = cardDetail.tag,
                        cardId = cardDetail.cardId
                    };

                    m_trashDetail.cardDetailList.Remove(cardDetail);
                    break;
                }
                count = m_trashDetail.cardDetailList.Count;
                m_trashCardCountText.text = count.ToString();
                if (count <= 0)
                {
                    // m_trashCard.gameObject.SetActive(false);
                }
                break;
            case OPTION_TYPE.EXCLUSION:
                foreach (var cardDetail in m_exclusionDetail.cardDetailList)
                {
                    if (cardDetail.tag != tag || cardDetail.cardId != cardId)
                    {
                        continue;
                    }

                    trashCard = new DeckManager.CardDetail()
                    {
                        tag = cardDetail.tag,
                        cardId = cardDetail.cardId
                    };

                    m_exclusionDetail.cardDetailList.Remove(cardDetail);
                    break;
                }

                count = m_exclusionDetail.cardDetailList.Count;
                m_exclusionCardCountText.text = count.ToString();
                if (count <= 0) { }
                break;
        }
        return trashCard;
    }

    public Image CreateCard(DeckManager.CardDetail cardDetail, bool isInstantiate, Image targetImage,
        Transform parent = null,
        Action<Image, string, string> leftClickAction = null,
        Action<Image, string, string> rightClickAction = null,
        Action<Image, string, string> middleClickAction = null)
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
        EventTrigger cardEventTrigger = copied.GetComponent<EventTrigger>();
        if (cardEventTrigger == null)
        {
            cardEventTrigger = copied.gameObject.AddComponent<EventTrigger>();
        }
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
            CardDetailManager.Instance().SetCardDetail(tag, fileName);
        });
        cardEventTrigger.triggers.Add(entry);
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
                    if (leftClickAction != null)
                    {
                        leftClickAction(copied, tag, fileName);
                    }
                    break;
                case -2:
                    Debug.Log("Right Click");
                    if (rightClickAction != null)
                    {
                        rightClickAction(copied, tag, fileName);
                    }
                    break;
                case -3:
                    Debug.Log("Middle Click");
                    if (middleClickAction != null)
                    {
                        middleClickAction(copied, tag, fileName);
                    }
                    break;
            }
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

        SetDeckOrTrashOrExclusion(OPTION_TYPE.DECK, fieldCardData.deckDetail);

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

        SetDeckOrTrashOrExclusion(OPTION_TYPE.TRASH, fieldCardData.trashDetail);
        SetDeckOrTrashOrExclusion(OPTION_TYPE.EXCLUSION, fieldCardData.exclusionDetail);
    }

    public string GetFieldCardManagerDataJson()
    {
        FieldCardData fieldCardData = new FieldCardData();

        fieldCardData.deckDetail = m_deckDetail;

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

        fieldCardData.trashDetail = m_trashDetail;
        fieldCardData.exclusionDetail = m_exclusionDetail;

        fieldCardData.securltyDetail = m_securltyDetail;
        fieldCardData.digitamaDetail = m_digitamaDetail;

        return JsonUtility.ToJson(fieldCardData);
    }
}
