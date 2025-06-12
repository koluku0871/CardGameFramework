using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static CardOptionWindow;
public class CardOptionWindow : MonoBehaviour
{
    public enum OPTION_TYPE
    {
        NONE = 0,
        CORE,
        BURST,
        DECK,
        HAND,
        AT_HAND,
        FIELD,
        FIELD_ROT,          // Stand Or Rest
        FLASH,
        TRASH,
        EXCLUSION,
        CARD_LIST,          // カード一覧表示時
        CONTRACT,           // 契約
        STEP,
    }

    [Serializable]
    public class OptionButton
    {
        public OPTION_TYPE optionType = OPTION_TYPE.NONE;

        public OPTION_TYPE detailOptionType = OPTION_TYPE.NONE;

        public Button button = null;

        public string title = "";
    }

    [SerializeField]
    private Button m_button = null;

    [SerializeField]
    private Text m_text = null;

    [SerializeField]
    private Image m_NumButtonMask = null;

    [SerializeField]
    private Text m_NumButtonText = null;

    public int numButtonTextNum = 1;

    private List<OptionButton> m_optionButtonList = new List<OptionButton>();

    private Image target = null;
    private bool m_isAction = true;
    private Action<bool> m_callback = null;

    private static CardOptionWindow instance = null;
    public static CardOptionWindow Instance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;

        SetButtonToCore();
        SetButtonToDeck();
        SetButtonToHand();
        SetButtonToAtHand();
        SetButtonToField();
        SetButtonToExclusion();
        SetButtonToTrash();

        Close();
    }

    private void SetButtonToCore()
    {

        Dictionary<string, UnityAction> actionList = new Dictionary<string, UnityAction>();

        actionList.Add("ドロー&コア&リフレッシュステップ", () => {
                FieldCardManager.Instance().AddSrcFromDst(OPTION_TYPE.DECK, OPTION_TYPE.HAND, true, 1);
                PlayerFieldManager.Instance().OnClickPlusButton();
                PlayerFieldManager.Instance().SetCorePos(ConstManager.CorePosType.TRASH, ConstManager.CorePosType.RESERVE);
                PlayerFieldManager.Instance().SetAllMyCardToRecovery();
                Close();
        });
        actionList.Add("コアをリザーブからトラッシュにX個移動する", () => {
            PlayerFieldManager.Instance().SetCorePos(ConstManager.CorePosType.RESERVE, ConstManager.CorePosType.TRASH, numButtonTextNum);
            Close();
        });
        actionList.Add("自分の手札からランダムにX枚捨てる", () => {
            for (int index = 0; index > numButtonTextNum; index++)
            {
                List<GameObject> handObjectList = FieldCardManager.Instance().GetHandGameObject();
                GameObject handObject = handObjectList[UnityEngine.Random.Range(0, handObjectList.Count)];
                string[] list = handObject.name.Split('^');
                FieldCardManager.Instance().AddTrashOrExclusionFromHandOrAtHand(handObject.GetComponent<Image>(), list[0], list[1], true);
                OpenExclusionListToMine();
            }
        });
        actionList.Add("コアをフィールドからリザーブにすべて移動する", () => {
            PlayerFieldManager.Instance().SetCorePos(ConstManager.CorePosType.FIELD, ConstManager.CorePosType.RESERVE);
            Close();
        });
        actionList.Add("フィールドのカードを一段階回復する", () => {
            PlayerFieldManager.Instance().SetAllMyCardToRecovery();
            CloseOnSound();
        });
        actionList.Add("コアをトラッシュからリザーブにすべて移動する", () => {
            PlayerFieldManager.Instance().SetCorePos(ConstManager.CorePosType.TRASH, ConstManager.CorePosType.RESERVE);
            Close();
        });
        actionList.Add("リフレッシュステップ", () => {
            PlayerFieldManager.Instance().SetCorePos(ConstManager.CorePosType.TRASH, ConstManager.CorePosType.RESERVE);
            PlayerFieldManager.Instance().SetAllMyCardToRecovery();
            Close();
        });
        actionList.Add("ドロー&リフレッシュステップ", () => {
                FieldCardManager.Instance().AddSrcFromDst(OPTION_TYPE.DECK, OPTION_TYPE.HAND, true, 1);
                PlayerFieldManager.Instance().SetCorePos(ConstManager.CorePosType.TRASH, ConstManager.CorePosType.RESERVE);
                PlayerFieldManager.Instance().SetAllMyCardToRecovery();
                Close();
        });

        foreach (var action in actionList)
        {
            CreateOptionButton(action.Key, OPTION_TYPE.STEP, OPTION_TYPE.NONE, action.Value);
        }
    }

    private void SetButtonToDeck()
    {
        Dictionary<OPTION_TYPE, Dictionary<string, UnityAction>> subOptionList = new Dictionary<OPTION_TYPE, Dictionary<string, UnityAction>>();
        Dictionary<string, UnityAction> actionList = new Dictionary<string, UnityAction>();

        actionList.Add("デッキをシャッフルする", () => {
            FieldCardManager.Instance().ShuffleDeck();
            Close();
            AudioSourceManager.Instance().PlayOneShot(1);
        });

        actionList.Add("デッキの上からX枚を手札に加える", () => {
            FieldCardManager.Instance().AddSrcFromDst(OPTION_TYPE.DECK, OPTION_TYPE.HAND, true, numButtonTextNum);
            CloseOnSound();
        });

        actionList.Add("デッキの下からX枚を手札に加える", () => {
            FieldCardManager.Instance().AddSrcFromDst(OPTION_TYPE.DECK, OPTION_TYPE.HAND, false, numButtonTextNum);
            CloseOnSound();
        });

        actionList.Add("デッキの上からX枚を公開する", () => {
            var cardList = FieldCardManager.Instance().GetDeck(true, numButtonTextNum);
            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.DECK, cardList);
            CloseOnSound();
        });

        actionList.Add("デッキの上からX枚を手元に置く", () => {
            FieldCardManager.Instance().AddSrcFromDst(OPTION_TYPE.DECK, OPTION_TYPE.AT_HAND, true, numButtonTextNum);
            CloseOnSound();
        });

        actionList.Add("デッキの上からX枚をトラッシュに送る", () => {
            var cardList = FieldCardManager.Instance().AddSrcFromDst(OPTION_TYPE.DECK, OPTION_TYPE.TRASH, true, numButtonTextNum);
            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.CARD_LIST, cardList);
            CloseOnSound();
        });
        actionList.Add("デッキの上からX枚を除外一覧に送る", () => {
            var cardList = FieldCardManager.Instance().AddSrcFromDst(OPTION_TYPE.DECK, OPTION_TYPE.EXCLUSION, true, numButtonTextNum);
            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.CARD_LIST, cardList);
            CloseOnSound();
        });

        subOptionList.Add(OPTION_TYPE.NONE, actionList);
        actionList = new Dictionary<string, UnityAction>();

        actionList.Add("デッキの上に戻す", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDeckFromDeck(true, target, list[0], list[1]);
            CloseOnSound();
        });
        actionList.Add("デッキの下に戻す", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDeckFromDeck(false, target, list[0], list[1]);
            CloseOnSound();
        });

        actionList.Add("デッキから手札に加える", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddSrcFromDst(OPTION_TYPE.DECK, OPTION_TYPE.HAND, list[0], list[1]);
            CloseOnSound();
        });

        actionList.Add("デッキから手元に置く", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddSrcFromDst(OPTION_TYPE.DECK, OPTION_TYPE.AT_HAND, list[0], list[1]);
            CloseOnSound();
        });

        List<string>keyStrList = new List<string>() { "トラッシュ", "除外一覧" };

        actionList.Add("デッキからトラッシュに送る", () => {
            string[] list = target.name.Split('^');
            List<bool> isTrashList = new List<bool>() { true, false };
            FieldCardManager.Instance().AddSrcFromDst(OPTION_TYPE.DECK, OPTION_TYPE.TRASH, list[0], list[1]);
            CloseOnSound();
        });

        actionList.Add("デッキから除外一覧に送る", () => {
            string[] list = target.name.Split('^');
            List<bool> isTrashList = new List<bool>() { true, false };
            FieldCardManager.Instance().AddSrcFromDst(OPTION_TYPE.DECK, OPTION_TYPE.EXCLUSION, list[0], list[1]);
            CloseOnSound();
        });

        subOptionList.Add(OPTION_TYPE.CARD_LIST, actionList);
        actionList = new Dictionary<string, UnityAction>();

        actionList.Add("デッキから契約カードを手札に加える", () => {
            FieldCardManager.Instance().AddHandFromDeckToContract();
            FieldCardManager.Instance().AddSrcFromDst(OPTION_TYPE.DECK, OPTION_TYPE.HAND, true, 3);
            CloseOnSound();
        });

        actionList.Add("デッキから契約カードを手札に加えない", () => {
            FieldCardManager.Instance().AddSrcFromDst(OPTION_TYPE.DECK, OPTION_TYPE.HAND, true, 4);
            CloseOnSound();
        });

        subOptionList.Add(OPTION_TYPE.CONTRACT, actionList);

        foreach (var subOption in subOptionList)
        {
            foreach (var action in subOption.Value)
            {
                CreateOptionButton(action.Key, OPTION_TYPE.DECK, subOption.Key, action.Value);
            }
        }
    }

    public void SetButtonToHand()
    {
        Dictionary<string, UnityAction> actionList = new Dictionary<string, UnityAction>();

        actionList.Add("手札からデッキの上に戻す", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDeckFromHandOrAtHand(true, target, list[0], list[1]);
            CloseOnSound();
        });
        actionList.Add("手札からデッキの下に戻す", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDeckFromHandOrAtHand(false, target, list[0], list[1]);
            CloseOnSound();
        });
        actionList.Add("手札から手元に置く", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddAtHandFromHand(target, list[0], list[1]);
            CloseOnSound();
        });
        actionList.Add("手札からトラッシュに送る", () => {
            string[] list = target.name.Split('^');
            var card = FieldCardManager.Instance().AddTrashOrExclusionFromHandOrAtHand(target, list[0], list[1], true);
            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.CARD_LIST, new List<DeckManager.CardDetail>() { card });
            CloseOnSound();
        });
        actionList.Add("手札から除外一覧に送る", () => {
            string[] list = target.name.Split('^');
            var card = FieldCardManager.Instance().AddTrashOrExclusionFromHandOrAtHand(target, list[0], list[1], false);
            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.CARD_LIST, new List<DeckManager.CardDetail>() { card });
            CloseOnSound();
        });

        foreach (var action in actionList)
        {
            CreateOptionButton(action.Key, OPTION_TYPE.HAND, OPTION_TYPE.NONE, action.Value);
        }
    }

    public void SetButtonToAtHand()
    {
        Dictionary<string, UnityAction> actionList = new Dictionary<string, UnityAction>();

        actionList.Add("カードを開く", () => {
            HandCard handCard = target.GetComponent<HandCard>();
            if (handCard != null)
            {
                handCard.SetIsOpen(true);
            }
            CloseOnSound();
        });

        actionList.Add("カードを閉じる", () => {
            HandCard handCard = target.GetComponent<HandCard>();
            if (handCard != null)
            {
                handCard.SetIsOpen(false);
            }
            CloseOnSound();
        });

        actionList.Add("手元からデッキの上に戻す", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDeckFromHandOrAtHand(true, target, list[0], list[1]);
            CloseOnSound();
        });

        actionList.Add("手元からデッキの下に戻す", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDeckFromHandOrAtHand(false, target, list[0], list[1]);
            CloseOnSound();
        });

        actionList.Add("手元から手札に加える", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddHandFromAtHand(target, list[0], list[1]);
            CloseOnSound();
        });

        actionList.Add("手元からトラッシュに送る", () => {
            string[] list = target.name.Split('^');
            var card = FieldCardManager.Instance().AddTrashOrExclusionFromHandOrAtHand(target, list[0], list[1], true);
            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.CARD_LIST, new List<DeckManager.CardDetail>() { card });
            CloseOnSound();
        });

        actionList.Add("手元から除外一覧に送る", () => {
            string[] list = target.name.Split('^');
            var card = FieldCardManager.Instance().AddTrashOrExclusionFromHandOrAtHand(target, list[0], list[1], false);
            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.CARD_LIST, new List<DeckManager.CardDetail>() { card });
            CloseOnSound();
        });

        foreach (var action in actionList)
        {
            CreateOptionButton(action.Key, OPTION_TYPE.AT_HAND, OPTION_TYPE.NONE, action.Value);
        }
    }

    public void SetButtonToField()
    {
        Dictionary<OPTION_TYPE, Dictionary<string, UnityAction>> subOptionList = new Dictionary<OPTION_TYPE, Dictionary<string, UnityAction>>();
        Dictionary<string, UnityAction> actionList = new Dictionary<string, UnityAction>();

        actionList.Add("フィールドからデッキの上に戻す", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDeckFromFieldOrBurstOrFlash(true, target, list[0], list[1]);
            CloseOnSound();
        });

        actionList.Add("フィールドからデッキの下に戻す", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDeckFromFieldOrBurstOrFlash(false, target, list[0], list[1]);
            CloseOnSound();
        });

        actionList.Add("フィールドから手札に加える", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddHandFromFieldOrBurstOrFlash(target, list[0], list[1]);
            CloseOnSound();
        });

        actionList.Add("フィールドから手元に置く", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddAtHandFromFieldOrBurstOrFlash(target, list[0], list[1]);
            CloseOnSound();
        });

        actionList.Add("フィールドからトラッシュに送る", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddTrashOrExclusionFromFieldOrBurstOrFlash(target, list[0], list[1], true);
            CloseOnSound();
        });

        actionList.Add("フィールドから除外一覧に送る", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddTrashOrExclusionFromFieldOrBurstOrFlash(target, list[0], list[1], false);
            CloseOnSound();
        });

        subOptionList.Add(OPTION_TYPE.NONE, actionList);
        actionList = new Dictionary<string, UnityAction>();

        actionList.Add("カードを開く", () => {
            TouchManager touchManager = target.GetComponent<TouchManager>();
            if (touchManager != null)
            {
                touchManager.SetIsOpen(true);
            }
            CloseOnSound();
        });

        actionList.Add("カードを閉じる", () => {
            TouchManager touchManager = target.GetComponent<TouchManager>();
            if (touchManager != null)
            {
                touchManager.SetIsOpen(false);
            }
            CloseOnSound();
        });

        actionList.Add("カードをスタンドさせる", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().SetCardToStand(target);
            CloseOnSound();
        });

        actionList.Add("カードを疲労させる", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().SetCardToRest(target);
            CloseOnSound();
        });

        actionList.Add("カードを重疲労させる", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().SetCardToDualRest(target);
            CloseOnSound();
        });

        actionList.Add("カードを裏返す", () => {
            TouchManager touchManager = target.GetComponent<TouchManager>();
            if (touchManager != null)
            {
                touchManager.SetIsAwake(!touchManager.IsAwake);
            }
            CloseOnSound();
        });

        actionList.Add("カードを魂状態を入れ替える", () => {
            TouchManager touchManager = target.GetComponent<TouchManager>();
            if (touchManager != null)
            {
                touchManager.SetIsSoul(!touchManager.IsSoul);
            }
            CloseOnSound();
        });

        actionList.Add("デッキの上に戻す", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDeckFromFieldOrBurstOrFlash(true, target, list[0], list[1]);
            CloseOnSound();
        });

        actionList.Add("デッキの下に戻す", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDeckFromFieldOrBurstOrFlash(false, target, list[0], list[1]);
            CloseOnSound();
        });

        subOptionList.Add(OPTION_TYPE.FIELD_ROT, actionList);

        foreach (var subOption in subOptionList)
        {
            foreach (var action in subOption.Value)
            {
                CreateOptionButton(action.Key, OPTION_TYPE.FIELD, subOption.Key, action.Value);
            }
        }
    }

    private void OpenExclusionListToMine()
    {
        CardListWindow.Instance().Close();
        List<DeckManager.CardDetail> openCardList = new List<DeckManager.CardDetail>();
        List<DeckManager.CardDetail> cardList = FieldCardManager.Instance().GetDeckOrTrashOrExclusion(OPTION_TYPE.EXCLUSION);
        for (int index = 0; index < cardList.Count; index++)
        {
            openCardList.Add(cardList[index]);
        }

        CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.EXCLUSION, openCardList);
        CloseOnSound();
    }

    public void SetButtonToExclusion()
    {
        CreateOptionButton(
            "自分の除外一覧を確認する", OPTION_TYPE.EXCLUSION, OPTION_TYPE.NONE,
            () => {
                CardListWindow.Instance().Close();
                List<DeckManager.CardDetail> openCardList = new List<DeckManager.CardDetail>();
                List<DeckManager.CardDetail> cardList = FieldCardManager.Instance().GetDeckOrTrashOrExclusion(OPTION_TYPE.EXCLUSION);
                for (int index = 0; index < cardList.Count; index++)
                {
                    openCardList.Add(cardList[index]);
                }

                CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.EXCLUSION, openCardList);
                Close();
            }
        );

        CreateOptionButton(
            "相手の除外一覧を確認する", OPTION_TYPE.EXCLUSION, OPTION_TYPE.NONE,
            () => {
                GameObject[] playFieldList = GameObject.FindGameObjectsWithTag("PlayField");
                FieldCardManager playField = null;
                foreach (var playFieldObj in playFieldList)
                {
                    FieldCardManager fieldCardManager = playFieldObj.GetComponent<FieldCardManager>();
                    if (fieldCardManager == null || fieldCardManager == FieldCardManager.Instance())
                    {
                        continue;
                    }
                    playField = fieldCardManager;
                    break;
                }
                if (playField != null)
                {
                    List<DeckManager.CardDetail> openCardList = new List<DeckManager.CardDetail>();
                    List<DeckManager.CardDetail> cardList = playField.GetDeckOrTrashOrExclusion(OPTION_TYPE.EXCLUSION);
                    for (int index = 0; index < cardList.Count; index++)
                    {
                        openCardList.Add(cardList[index]);
                    }

                    CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.EXCLUSION, openCardList, false);
                }
                Close();
            }
        );

        CreateOptionButton(
            "除外一覧からデッキの上に戻す", OPTION_TYPE.EXCLUSION, OPTION_TYPE.CARD_LIST,
            () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDeckFromExclusion(true, list[0], list[1]);
                OpenExclusionListToMine();
            }
        );

        CreateOptionButton(
            "除外一覧からデッキの下に戻す", OPTION_TYPE.EXCLUSION, OPTION_TYPE.CARD_LIST,
            () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDeckFromExclusion(false, list[0], list[1]);
                OpenExclusionListToMine();
            }
        );

        CreateOptionButton(
            "除外一覧から手札に加える", OPTION_TYPE.EXCLUSION, OPTION_TYPE.CARD_LIST,
            () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddSrcFromDst(OPTION_TYPE.EXCLUSION, OPTION_TYPE.HAND, list[0], list[1]);
                OpenExclusionListToMine();
            }
        );

        CreateOptionButton(
            "除外一覧から手元に置く", OPTION_TYPE.EXCLUSION, OPTION_TYPE.CARD_LIST,
            () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddSrcFromDst(OPTION_TYPE.EXCLUSION, OPTION_TYPE.AT_HAND, list[0], list[1]);
                OpenExclusionListToMine();
            }
        );
    }

    private void OpenTrashListToMine()
    {
        CardListWindow.Instance().Close();
        List <DeckManager.CardDetail> openCardList = new List<DeckManager.CardDetail>();
        List<DeckManager.CardDetail> cardList = FieldCardManager.Instance().GetDeckOrTrashOrExclusion(OPTION_TYPE.TRASH);
        for (int index = 0; index < cardList.Count; index++)
        {
            openCardList.Add(cardList[index]);
        }

        CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.TRASH, openCardList);
        CloseOnSound();
    }

    public void SetButtonToTrash()
    {
        CreateOptionButton(
            "自分のトラッシュを確認する", OPTION_TYPE.TRASH, OPTION_TYPE.NONE,
            () => {
                CardListWindow.Instance().Close();
                List<DeckManager.CardDetail> openCardList = new List<DeckManager.CardDetail>();
                List<DeckManager.CardDetail> cardList = FieldCardManager.Instance().GetDeckOrTrashOrExclusion(OPTION_TYPE.TRASH);
                for (int index = 0; index < cardList.Count; index++)
                {
                    openCardList.Add(cardList[index]);
                }

                CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.TRASH, openCardList);
                Close();
            }
        );

        CreateOptionButton(
            "相手のトラッシュを確認する", OPTION_TYPE.TRASH, OPTION_TYPE.NONE,
            () => {
                GameObject[] playFieldList = GameObject.FindGameObjectsWithTag("PlayField");
                FieldCardManager playField = null;
                foreach (var playFieldObj in playFieldList)
                {
                    FieldCardManager fieldCardManager = playFieldObj.GetComponent<FieldCardManager>();
                    if (fieldCardManager == null || fieldCardManager == FieldCardManager.Instance())
                    {
                        continue;
                    }
                    playField = fieldCardManager;
                    break;
                }
                if (playField != null)
                {
                    List<DeckManager.CardDetail> openCardList = new List<DeckManager.CardDetail>();
                    List<DeckManager.CardDetail> cardList = playField.GetDeckOrTrashOrExclusion(OPTION_TYPE.TRASH);
                    for (int index = 0; index < cardList.Count; index++)
                    {
                        openCardList.Add(cardList[index]);
                    }
                    CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.TRASH, openCardList, false);
                }
                Close();
            }
        );

        CreateOptionButton(
            "トラッシュからデッキの上に戻す", OPTION_TYPE.TRASH, OPTION_TYPE.CARD_LIST,
            () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDeckFromTrash(true, list[0], list[1]);
                OpenTrashListToMine();
            }
        );

        CreateOptionButton(
            "トラッシュからデッキの下に戻す", OPTION_TYPE.TRASH, OPTION_TYPE.CARD_LIST,
            () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDeckFromTrash(false, list[0], list[1]);
                OpenTrashListToMine();
            }
        );

        CreateOptionButton(
            "トラッシュから手札に加える", OPTION_TYPE.TRASH, OPTION_TYPE.CARD_LIST,
            () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddSrcFromDst(OPTION_TYPE.TRASH, OPTION_TYPE.HAND, list[0], list[1]);
                OpenTrashListToMine();
            }
        );

        CreateOptionButton(
            "トラッシュから手元に置く", OPTION_TYPE.TRASH, OPTION_TYPE.CARD_LIST,
            () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddSrcFromDst(OPTION_TYPE.TRASH, OPTION_TYPE.AT_HAND, list[0], list[1]);
                OpenTrashListToMine();
            }
        );

        CreateOptionButton(
            "トラッシュから除外一覧に送る", OPTION_TYPE.TRASH, OPTION_TYPE.CARD_LIST,
            () => {
                string[] list = target.name.Split('^');
                var card = FieldCardManager.Instance().RemoveTrashOrExclusion(OPTION_TYPE.TRASH, list[0], list[1]);
                FieldCardManager.Instance().AddTrashOrExclusion(card, false);
                OpenTrashListToMine();
            }
        );

        CreateOptionButton(
            "トラッシュからフィールドに出す", OPTION_TYPE.TRASH, OPTION_TYPE.CARD_LIST,
            () => {
                string[] list = target.name.Split('^');
                var card = FieldCardManager.Instance().RemoveTrashOrExclusion(OPTION_TYPE.TRASH, list[0], list[1]);
                PlayerFieldManager.Instance().CreateCard(target.name);
                OpenTrashListToMine();
            }
        );
    }

    public void CreateOptionButton(string text, OPTION_TYPE main, OPTION_TYPE sub, UnityAction action)
    {
        OptionButton optionButton = new OptionButton();
        m_text.text = text;
        optionButton.optionType = main;
        optionButton.detailOptionType = sub;
        optionButton.button = UnityEngine.Object.Instantiate(m_button);
        optionButton.button.transform.SetParent(m_button.transform.parent);
        optionButton.button.transform.localScale = Vector3.one;
        optionButton.button.onClick.AddListener(action);
        m_optionButtonList.Add(optionButton);
    }

    public void Open(Image card, OPTION_TYPE optionType, OPTION_TYPE detailOptionType = OPTION_TYPE.NONE, Action<bool> callback = null)
    {
        target = card;

        Open(optionType, detailOptionType, callback);
    }

    public void Open(OPTION_TYPE optionType, OPTION_TYPE detailOptionType = OPTION_TYPE.NONE, Action<bool> callback = null)
    {
        foreach (OptionButton optionButton in m_optionButtonList)
        {
            bool isActive = (optionButton.optionType == optionType && optionButton.detailOptionType == detailOptionType);
            optionButton.button.gameObject.SetActive(isActive);
        }

        Open(callback);
    }

    public void Open(Action<bool> callback = null)
    {
        m_isAction = true;
        m_callback = callback;

        this.gameObject.SetActive(true);
    }

    public void CloseOnSound()
    {
        AudioSourceManager.Instance().PlayOneShot(0);
        Close();
    }

    public void Close()
    {
        target = null;
        if (m_callback != null) m_callback(m_isAction);
        m_callback = null;
        this.gameObject.SetActive(false);
    }

    public void OnClickToNumButton()
    {
        m_NumButtonMask.gameObject.SetActive(true);
    }

    public void OnClickToNumSelectButton(int num)
    {
        numButtonTextNum = num;

        m_NumButtonText.text = num.ToString();

        m_NumButtonMask.gameObject.SetActive(false);
    }

    public void OnClickToCloseButton()
    {
        m_isAction = false;
        Close();
    }
}
