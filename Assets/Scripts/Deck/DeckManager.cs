using Coffee.UIExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{
    [Serializable]
    public class CardDetail {
        public string tag = "";
        public string cardId = "";

        public new string ToString()
        {
            return tag + "^" + cardId;
        }
    }

    [Serializable]
    public class DeckDetail {
        public string type = "";
        public List<CardDetail> aceCardDetailList = new List<CardDetail>();
        public List<CardDetail> cardDetailList = new List<CardDetail>();
        public List<CardDetail> subCardDetailList = new List<CardDetail>();
        public List<CardDetail> tokenCardDetailList = new List<CardDetail>();

        public void Add(CardDetail cardDetail) {
            cardDetailList.Add(cardDetail);
        }
        public void AddToSub(CardDetail cardDetail)
        {
            subCardDetailList.Add(cardDetail);
        }
        public void AddToToken(CardDetail cardDetail)
        {
            tokenCardDetailList.Add(cardDetail);
        }
        public void AddToAce(CardDetail cardDetail)
        {
            aceCardDetailList.Add(cardDetail);
        }

        public string GetDeckType()
        {
            if (string.IsNullOrEmpty(type))
            {
                type = "bs";
            }
            return type;
        }

        public bool IsAce(int num, CardDetail cardDetail)
        {
            int count = 0;
            foreach(var aceCardDetail in aceCardDetailList)
            {
                if (aceCardDetail.ToString() != cardDetail.ToString())
                {
                    continue;
                }
                count++;
            }
            Debug.Log(cardDetail.ToString() + " : " + num + " : " + count);
            return num <= count;
        }
    }

    /// <summary>
    /// BS契約チェック
    /// </summary>
    /// <returns></returns>
    public static bool IsInContract(List<CardDetail> cardDetailList)
    {
        foreach (CardDetail cardDetail in cardDetailList)
        {
            CardData data = AssetBundleManager.Instance().GetBaseData(cardDetail.tag, cardDetail.cardId);
            if (!data.CardCategory.Contains("契約"))
            {
                continue;
            }

            return true;
        }

        return false;
    }

    [SerializeField]
    private Material m_effectMaterial = null;

    public List<string> m_deckList = new List<string>();

    public int m_maxRandomCount = 40;

    [SerializeField]
    private TMPro.TextMeshProUGUI m_maxRandomCountText = null;

    private static DeckManager instance = null;
    public static DeckManager Instance() {
        return instance;
    }

    private void Awake() {
        instance = this;
    }

    private void Start() {
        CheckDeckDirectory();
    }

    public void CheckDeckDirectory() {
        DeckSceneManager deckSceneManager = DeckSceneManager.Instance();
        deckSceneManager.GetDeckSelectDropdown().ClearOptions();
        deckSceneManager.GetDeckSelectDropdown().options.Add(new Dropdown.OptionData()
        {
            text = "no select"
        });

        var directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_DECK;
        if (!Directory.Exists(directoryPath)) {
            Directory.CreateDirectory(directoryPath);
            Debug.Log("directoryPath:" + directoryPath);
        } else {
            m_deckList.Clear();
            string[] deckFiles = Directory.GetFiles(directoryPath, "*.json", SearchOption.AllDirectories);
            for(int index = 0; index < deckFiles.Length; index++)
            {
                var sr = new StreamReader(deckFiles[index], Encoding.UTF8);
                var deckStr = sr.ReadToEnd();
                try
                {
                    DeckDetail deckCardList = JsonUtility.FromJson<DeckDetail>(deckStr);
                    if (deckCardList.GetDeckType() == AssetBundleManager.Instance().CardType)
                    {
                        m_deckList.Add(deckStr);
                        int start = deckFiles[index].LastIndexOf("/") + 1;
                        int end = deckFiles[index].LastIndexOf(".json");
                        int count = end - start;
                        string deckFileName = deckFiles[index].Substring(start, count);
                        deckSceneManager.GetDeckSelectDropdown().options.Add(new Dropdown.OptionData()
                        {
                            text = deckFileName
                        });
                    }
                }
                catch{ }

                sr.Close();
            }
        }
    }

    public void OnValueChangedToDeckSelectDropdown() {
        StartCoroutine(OnValueChangedToDeckSelectDropdownCoroutine());
    }
    private IEnumerator OnValueChangedToDeckSelectDropdownCoroutine() {
        DeckSceneManager deckSceneManager = DeckSceneManager.Instance();
        foreach ( Transform c in deckSceneManager.GetDeckContent().transform ) {
            Destroy(c.gameObject);
        }
        foreach (Transform c in deckSceneManager.GetSubDeckContent().transform)
        {
            Destroy(c.gameObject);
        }
        foreach (Transform c in deckSceneManager.GetTokenDeckContent().transform)
        {
            Destroy(c.gameObject);
        }

        yield return null;

        var selectId = deckSceneManager.GetDeckSelectDropdown().value;
        try
        {
            DeckDetail deckCardList = JsonUtility.FromJson<DeckDetail>(m_deckList[selectId-1]);
            Dictionary<string, int> strings = new Dictionary<string, int>();
            foreach (CardDetail card in deckCardList.cardDetailList)
            {
                if (!strings.ContainsKey(card.ToString()))
                {
                    strings.Add(card.ToString(), 1);
                }
                else
                {
                    strings[card.ToString()]++;
                }
                AddCard(card.tag, card.cardId, deckSceneManager.GetDeckContent(), deckCardList.IsAce(strings[card.ToString()], card));
            }

            foreach (CardDetail card in deckCardList.subCardDetailList)
            {
                AddCard(card.tag, card.cardId, deckSceneManager.GetSubDeckContent());
            }

            foreach (CardDetail card in deckCardList.tokenCardDetailList)
            {
                AddCard(card.tag, card.cardId, deckSceneManager.GetTokenDeckContent());
            }
        }
        catch
        {
            Debug.Log("OnValueChangedToDeckSelectDropdownCoroutine selectId = " + selectId);
        }
    }

    public void OnClickToDeckSaveButton() {
        DeckSceneManager deckSceneManager = DeckSceneManager.Instance();
        var selectId = deckSceneManager.GetDeckSelectDropdown().value;
        if (1 > selectId) {
            return;
        }
        var directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_DECK;
        string deckFileName = deckSceneManager.GetDeckSelectDropdown().options[selectId].text;

        DeckDetail deckCardList = new DeckDetail();
        deckCardList.type = AssetBundleManager.Instance().CardType;
        foreach ( Transform c in deckSceneManager.GetDeckContent().transform ) {
            string[] cardData = c.name.Split('^');
            if (cardData.Length < 2) {
                continue;
            }
            var cardId = cardData[1];
            CardDetail cardDetail = new CardDetail()
            {
                tag = cardData[0],
                cardId = cardId
            };
            deckCardList.Add(cardDetail);

            ShinyEffectForUGUI shiny = c.GetComponent<ShinyEffectForUGUI>();
            if (shiny != null)
            {
                deckCardList.AddToAce(cardDetail);
            }
        }

        foreach (Transform c in deckSceneManager.GetSubDeckContent().transform)
        {
            string[] cardData = c.name.Split('^');
            if (cardData.Length < 2)
            {
                continue;
            }
            var cardId = cardData[1];
            deckCardList.AddToSub(new CardDetail()
            {
                tag = cardData[0],
                cardId = cardId
            });
        }

        foreach (Transform c in deckSceneManager.GetTokenDeckContent().transform)
        {
            string[] cardData = c.name.Split('^');
            if (cardData.Length < 2)
            {
                continue;
            }
            var cardId = cardData[1];
            deckCardList.AddToToken(new CardDetail()
            {
                tag = cardData[0],
                cardId = cardId
            });
        }

        string deckData = JsonUtility.ToJson(deckCardList);
        Debug.Log(deckData);
        StreamWriter streamWriter = new StreamWriter(directoryPath + deckFileName + ".json");
        streamWriter.WriteLine(deckData);
        streamWriter.Close();

        CheckDeckDirectory();
    }

    public void OnClickToDeckCreateButton() {
        var directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_DECK;
        DeckSceneManager deckSceneManager = DeckSceneManager.Instance();
        string deckFileName = deckSceneManager.GetDeckCreateInputField().text;
        if (deckFileName == "") {
            return;
        }

        DeckDetail deckCardList = new DeckDetail();
        deckCardList.type = AssetBundleManager.Instance().CardType;
        foreach ( Transform c in deckSceneManager.GetDeckContent().transform ) {
            string[] cardData = c.name.Split('^');
            if (cardData.Length < 2) {
                continue;
            }
            var cardId = cardData[1];
            CardDetail cardDetail = new CardDetail()
            {
                tag = cardData[0],
                cardId = cardId
            };
            deckCardList.Add(cardDetail);

            ShinyEffectForUGUI shiny = c.GetComponent<ShinyEffectForUGUI>();
            if (shiny != null)
            {
                deckCardList.AddToAce(cardDetail);
            }
        }

        foreach (Transform c in deckSceneManager.GetSubDeckContent().transform)
        {
            string[] cardData = c.name.Split('^');
            if (cardData.Length < 2)
            {
                continue;
            }
            var cardId = cardData[1];
            deckCardList.AddToSub(new CardDetail()
            {
                tag = cardData[0],
                cardId = cardId
            });
        }

        foreach (Transform c in deckSceneManager.GetTokenDeckContent().transform)
        {
            string[] cardData = c.name.Split('^');
            if (cardData.Length < 2)
            {
                continue;
            }
            var cardId = cardData[1];
            deckCardList.AddToToken(new CardDetail()
            {
                tag = cardData[0],
                cardId = cardId
            });
        }

        string deckData = JsonUtility.ToJson(deckCardList);
        Debug.Log(deckData);
        StreamWriter streamWriter = new StreamWriter(directoryPath + deckFileName + ".json");
        streamWriter.WriteLine(deckData);
        streamWriter.Close();

        CheckDeckDirectory();
    }

    public void OnClickToDeckRCreateButton()
    {
        DeckSceneManager deckSceneManager = DeckSceneManager.Instance();
        List<CardData> cardDatas = deckSceneManager.GetCardDatas();
        if (cardDatas == null || cardDatas.Count < 1)
        {
            return;
        }
        int index = 0;
        System.Random rnd = new System.Random();
        while (index < m_maxRandomCount)
        {
            int i = rnd.Next(0, cardDatas.Count);
            string fileName = cardDatas[i].CardNo;
            string targetTag = cardDatas[i].PackNo;
            Debug.LogWarning(fileName);
            Debug.LogWarning(targetTag);
            AddCard(targetTag, fileName, deckSceneManager.GetDeckContent());
            index = deckSceneManager.GetDeckContent().childCount;
        }
    }

    public void OnClickToDeckRandomCreateMaxNumButton(int num)
    {
        m_maxRandomCount = num;
        m_maxRandomCountText.text = m_maxRandomCount.ToString();
    }

    public void OnClickToDeckClearButton() {
        DeckSceneManager deckSceneManager = DeckSceneManager.Instance();
        foreach ( Transform c in deckSceneManager.GetActiveDeckContent().transform ) {
            Destroy(c.gameObject);
        }
        StartCoroutine(UpdateDeckCardCountCoroutine());
    }

    public void AddCard(string tag, string cardId, RectTransform parent = null, bool isAce = false)
    {
        DeckSceneManager deckSceneManager = DeckSceneManager.Instance();
        if (parent == null)
        {
            parent = deckSceneManager.GetActiveDeckContent();
        }

        if (IsDuplicateLimit(tag, cardId, parent)) {
            return;
        }

        GameObject cardObj = new GameObject(tag + "^" + cardId);
        cardObj.transform.SetParent(parent);
        cardObj.transform.localScale = Vector3.one;
        
        Image cardImage = cardObj.AddComponent<Image>();
        cardImage.sprite = AssetBundleManager.Instance().GetBaseData(tag, cardId).Sprite;

        if (isAce)
        {
            ShinyEffectForUGUI shiny = cardObj.AddComponent<ShinyEffectForUGUI>();
            shiny.location = 0;
            shiny.width = 0.25f;
            shiny.softness = 1;
            shiny.brightness = 1;
            shiny.rotation = 0;
            shiny.highlight = 1;
            shiny.effectMaterial = m_effectMaterial;
            shiny.PlayToLoop();
        }

        EventTrigger cardEventTrigger = cardObj.AddComponent<EventTrigger>();
        cardEventTrigger.triggers = new List<EventTrigger.Entry>();
        // マウスオーバー
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;
        entry.callback.AddListener((_) => {
            if (CardDetailManager.Instance().isLock)
            {
                return;
            }
            deckSceneManager.GetTargetCardDetail().sprite = cardImage.sprite;
            CardDetailManager.Instance().SetCardDetail(tag, cardId);
        });
        cardEventTrigger.triggers.Add(entry);
        // マウスクリック
        entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((pointerEventData) => {
            bool isPointerEvent = pointerEventData is PointerEventData;
            if (!isPointerEvent) {
                return;
            }

            MouseManager.Instance().OnClick((pointerEventData as PointerEventData).pointerId, (int pointerId, bool isDoubleClick) => {
                switch (pointerId)
                {
                    case -1:
                        if (isDoubleClick)
                        {
                            Debug.Log("Left Double Click");
                            ShinyEffectForUGUI shiny = cardObj.GetComponent<ShinyEffectForUGUI>();
                            if (shiny == null)
                            {
                                shiny = cardObj.AddComponent<ShinyEffectForUGUI>();
                            }
                            shiny.location = 0;
                            shiny.width = 0.25f;
                            shiny.softness = 1;
                            shiny.brightness = 1;
                            shiny.rotation = 0;
                            shiny.highlight = 1;
                            shiny.effectMaterial = m_effectMaterial;
                            shiny.PlayToLoop();
                        }
                        else
                        {
                            Debug.Log("Left Click");
                            AddCard(tag, cardId);
                        }
                        break;
                    case -2:
                        if (isDoubleClick)
                        {
                            Debug.Log("Right Double Click");
                            ShinyEffectForUGUI shiny = cardObj.GetComponent<ShinyEffectForUGUI>();
                            if (shiny != null)
                            {
                                Destroy(shiny);
                            }
                        }
                        else
                        {
                            Debug.Log("Right Click");
                            Destroy(cardObj);
                            StartCoroutine(UpdateDeckCardCountCoroutine());
                        }
                        break;
                    case -3:
                        Debug.Log("Middle Click");
                        CardDetailManager.Instance().isLock = !CardDetailManager.Instance().isLock;
                        break;
                }
            });
        });
        cardEventTrigger.triggers.Add(entry);

        UpdateDeckCardCount();
    }

    private bool IsDuplicateLimit(string tag, string cardId, RectTransform parent)
    {
        int maxDuplicateCount = ConstManager.DECK_CARD_DUPLICATE_COUNT;
        switch (AssetBundleManager.Instance().CardType)
        {
            case "hololive":
            case "pokemon":
                return false;
            case "digimon":
                break;
        }

        if (ConstManager.DECK_CARD_DUPLICATE_OK_LIST.Contains(cardId)) {
            return false;
        }

        if (ConstManager.DECK_CARD_DUPLICATE_5_NG_LIST.Contains(cardId))
        {
            maxDuplicateCount = 5;
        }

        if (ConstManager.DECK_CARD_DUPLICATE_20_NG_LIST.Contains(cardId))
        {
            maxDuplicateCount = 20;
        }

        if (ConstManager.DECK_CARD_DUPLICATE_29_NG_LIST.Contains(cardId))
        {
            maxDuplicateCount = 29;
        }

        var duplicateCount = 0;
        var deckSceneManager = DeckSceneManager.Instance();
        foreach (Transform child in parent.transform) {
            if (child.name == tag + "^" + cardId) {
                duplicateCount++;
            }
        }
        return duplicateCount >= maxDuplicateCount;
    }

    private IEnumerator UpdateDeckCardCountCoroutine() {
        yield return null;
        UpdateDeckCardCount();
    }
    private void UpdateDeckCardCount() {
        List<Transform> cardList = new List<Transform>();
        var deckSceneManager = DeckSceneManager.Instance();

        foreach (Transform child in deckSceneManager.GetDeckContent()) {
            cardList.Add(child);
        }
        cardList.Sort((obj1, obj2) => string.Compare(obj1.name, obj2.name));
        foreach (var obj in cardList) {
            obj.SetSiblingIndex(cardList.Count - 1);
        }
        int childCount = deckSceneManager.GetDeckContent().childCount;
        Debug.Log("GetDeckCardCountText" + childCount);
        deckSceneManager.GetDeckCardCountText().text = "デッキ枚数：" + childCount.ToString("D2");

        cardList.Clear();
        foreach (Transform child in deckSceneManager.GetSubDeckContent())
        {
            cardList.Add(child);
        }
        cardList.Sort((obj1, obj2) => string.Compare(obj1.name, obj2.name));
        foreach (var obj in cardList)
        {
            obj.SetSiblingIndex(cardList.Count - 1);
        }
        childCount = deckSceneManager.GetSubDeckContent().childCount;
        Debug.Log("GetSubDeckCardCountText" + childCount);
        deckSceneManager.GetSubDeckCardCountText().text = "デッキ枚数：" + childCount.ToString("D2");

        cardList.Clear();
        foreach (Transform child in deckSceneManager.GetTokenDeckContent())
        {
            cardList.Add(child);
        }
        cardList.Sort((obj1, obj2) => string.Compare(obj1.name, obj2.name));
        foreach (var obj in cardList)
        {
            obj.SetSiblingIndex(cardList.Count - 1);
        }
        childCount = deckSceneManager.GetTokenDeckContent().childCount;
        Debug.Log("GetTokenDeckCardCountText" + childCount);
        deckSceneManager.GetTokenDeckCardCountText().text = "デッキ枚数：" + childCount.ToString("D2");

    }
}
