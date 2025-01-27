using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
    }

    [Serializable]
    public class DeckDetail {
        public List<CardDetail> cardDetailList = new List<CardDetail>();

        public void Add(CardDetail cardDetail) {
            cardDetailList.Add(cardDetail);
        }

        public bool IsInContract()
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
    }

    public List<string> m_deckList = new List<string>();

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
                StreamReader sr = new StreamReader(deckFiles[index], Encoding.UTF8);
                m_deckList.Add(sr.ReadToEnd());
                int start = deckFiles[index].LastIndexOf("/") + 1;
                int end = deckFiles[index].LastIndexOf(".json");
                int count = end - start;
                string deckFileName = deckFiles[index].Substring(start, count);
                deckSceneManager.GetDeckSelectDropdown().options.Add(new Dropdown.OptionData(){
                    text = deckFileName
                });
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

        yield return null;

        var selectId = deckSceneManager.GetDeckSelectDropdown().value;
        try
        {
            DeckDetail deckCardList = JsonUtility.FromJson<DeckDetail>(m_deckList[selectId-1]);
            foreach (CardDetail card in deckCardList.cardDetailList)
            {
                AddCard(card.tag, card.cardId);
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
        foreach ( Transform c in deckSceneManager.GetDeckContent().transform ) {
            string[] cardData = c.name.Split('^');
            if (cardData.Length < 2) {
                continue;
            }
            var cardId = cardData[1];
            deckCardList.Add(new CardDetail(){
                tag = cardData[0], cardId = cardId
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
        foreach ( Transform c in deckSceneManager.GetDeckContent().transform ) {
            string[] cardData = c.name.Split('^');
            if (cardData.Length < 2) {
                continue;
            }
            var cardId = cardData[1];
            deckCardList.Add(new CardDetail()
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

    public void OnClickToDeckClearButton() {
        DeckSceneManager deckSceneManager = DeckSceneManager.Instance();
        foreach ( Transform c in deckSceneManager.GetDeckContent().transform ) {
            Destroy(c.gameObject);
        }
        StartCoroutine(UpdateDeckCardCountCoroutine());
    }

    public void AddCard(string tag, string cardId) {
        if (IsDuplicateLimit(tag, cardId)) {
            return;
        }

        DeckSceneManager deckSceneManager = DeckSceneManager.Instance();

        GameObject cardObj = new GameObject(tag + "^" + cardId);
        cardObj.transform.SetParent(deckSceneManager.GetDeckContent());
        cardObj.transform.localScale = Vector3.one;
        Image cardImage = cardObj.AddComponent<Image>();
        cardImage.sprite = AssetBundleManager.Instance().GetBaseData(tag, cardId).Sprite;
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

            switch((pointerEventData as PointerEventData).pointerId)
            {
                case -1:
                    Debug.Log("Left Click");
                    AddCard(tag, cardId);
                    break;
                case -2:
                    Debug.Log("Right Click");
                    Destroy(cardObj);
                    StartCoroutine(UpdateDeckCardCountCoroutine());
                    break;
                case -3:
                    Debug.Log("Middle Click");
                    CardDetailManager.Instance().isLock = !CardDetailManager.Instance().isLock;
                    break;
            }
        });
        cardEventTrigger.triggers.Add(entry);

        UpdateDeckCardCount();
    }

    private bool IsDuplicateLimit(string tag, string cardId) {
        if (ConstManager.DECK_CARD_DUPLICATE_OK_LIST.Contains(cardId)) {
            return false;
        }

        int maxDuplicateCount = ConstManager.DECK_CARD_DUPLICATE_COUNT;

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
        foreach (Transform child in deckSceneManager.GetDeckContent()) {
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

        deckSceneManager.GetDeckCardCountText().text = "デッキ枚数：" + cardList.Count.ToString("D2");
    }
}
