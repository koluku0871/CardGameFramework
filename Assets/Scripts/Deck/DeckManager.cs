using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
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
        public string type = "";
        public List<CardDetail> cardDetailList = new List<CardDetail>();
        public List<CardDetail> subCardDetailList = new List<CardDetail>();

        public void Add(CardDetail cardDetail) {
            cardDetailList.Add(cardDetail);
        }

        public string GetDeckType()
        {
            if (string.IsNullOrEmpty(type))
            {
                type = "bs";
            }
            return type;
        }

        /// <summary>
        /// BS契約チェック
        /// </summary>
        /// <returns></returns>
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
        OptionData optionData = new OptionData();
        optionData.LoadTxt();

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
                    if (deckCardList.GetDeckType() == optionData.cardType)
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

    public void OnClickToDeckCheckButton()
    {
        var directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_DECK;
        string[] deckFiles = Directory.GetFiles(directoryPath, "*.json", SearchOption.AllDirectories);
        for (int index = 0; index < deckFiles.Length; index++)
        {
            StreamReader sr = new StreamReader(deckFiles[index], Encoding.UTF8);
            DeckDetail deckDetail = JsonUtility.FromJson<DeckDetail>(sr.ReadToEnd());
            sr.Close();

            bool isHit = false;
            List<CardDetail> cardDetailList = new List<CardDetail>();
            foreach (var cardDetail in deckDetail.cardDetailList)
            {
                if (cardDetail.cardId.Split("-").Length != 1)
                {
                    cardDetailList.Add(new CardDetail() { tag = cardDetail.tag, cardId = cardDetail.cardId });
                }
                else
                {
                    isHit = true;
                    var splitId = cardDetail.cardId.Split("_");
                    var cardId = splitId[0] + "-" + splitId[1];
                    if (splitId.Length > 2)
                    {
                        cardId = cardId + "_" + splitId[2];
                    }

                    cardDetailList.Add(new CardDetail() { tag = cardDetail.tag, cardId = cardId });
                }
            }

            if (!isHit)
            {
                continue;
            }

            deckDetail.cardDetailList.Clear();
            deckDetail.cardDetailList.AddRange(cardDetailList);
            string deckData = JsonUtility.ToJson(deckDetail);

            StreamWriter streamWriter = new StreamWriter(deckFiles[index]);
            streamWriter.WriteLine(deckData);
            streamWriter.Close();
        }
    }

    public void OnClickToDeckSaveButton() {
        OptionData optionData = new OptionData();
        optionData.LoadTxt();

        DeckSceneManager deckSceneManager = DeckSceneManager.Instance();
        var selectId = deckSceneManager.GetDeckSelectDropdown().value;
        if (1 > selectId) {
            return;
        }
        var directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_DECK;
        string deckFileName = deckSceneManager.GetDeckSelectDropdown().options[selectId].text;

        DeckDetail deckCardList = new DeckDetail();
        deckCardList.type = optionData.cardType;
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

    public void OnClickToDeckRCreateButton()
    {
        DeckSceneManager deckSceneManager = DeckSceneManager.Instance();
        (var key, var packNameList) = deckSceneManager.GetPackNameList();
        if (packNameList == null || packNameList.Count < 12)
        {
            return;
        }
        int index = 0;
        System.Random rnd = new System.Random();
        while (index < 40)
        {
            int i = rnd.Next(0, packNameList.Count);
            string fileName = packNameList[i];
            string targetTag = key;
            AddCard(targetTag, fileName);
            index = deckSceneManager.GetDeckContent().childCount;
        }
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
