using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using Text = UnityEngine.UI.Text;
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
        TOKEN,              // トークン

        DAMAGE,
        SUB,
    }

    [Serializable]
    public class OptionButton
    {
        public OPTION_TYPE optionType = OPTION_TYPE.NONE;
        public OPTION_TYPE detailOptionType = OPTION_TYPE.NONE;
        public KeyCode keyCode = KeyCode.None;
        public bool isPrivateOpen = false;
        public string title = "";
        public UnityAction action = null;
        public Button button = null;
        public UnityAction onClickAction = null;

        public OptionButton(
            OPTION_TYPE optionType,
            OPTION_TYPE detailOptionType,
            KeyCode keyCode,
            bool isPrivateOpen,
            string title,
            UnityAction action
            ){
            this.optionType = optionType;
            this.detailOptionType = detailOptionType;
            this.keyCode = keyCode;
            this.isPrivateOpen = isPrivateOpen;
            this.title = title;
            this.action = action;
        }
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
    public OPTION_TYPE innerListFromType = OPTION_TYPE.TRASH;
    private bool m_isAction = true;
    private Action<bool> m_callback = null;

    private static CardOptionWindow instance = null;
    public static CardOptionWindow Instance()
    {
        return instance;
    }

    string damageStr = "ダメージ";
    string subStr = "サブデッキ";

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        switch (BattleSceneManager.m_type)
        {
            case "bs":
                SetButtonToBs();
                break;
            case "digimon":
                damageStr = "セキュリティ";
                subStr = "デジタマ";
                SetButtonToDigimon();
                break;
            case "hololive":
                damageStr = "ライフ";
                subStr = "エールデッキ";
                break;
        }

        SetButtonToOption();

        SetButtonToDeck();
        SetButtonToHand();
        SetButtonToAtHand();
        SetButtonToField();
        SetButtonToExclusion();
        SetButtonToTrash();
        SetButtonToDamage();
        SetButtonToSub();

        CreateOptionButton();

        Close();
    }

    private void SetButtonToBs()
    {
        OPTION_TYPE optionType = OPTION_TYPE.STEP;
        OPTION_TYPE detailOptionType = OPTION_TYPE.NONE;

        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.F1, false, "ドロー&コア&リフレッシュステップ", () => {
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.HAND, true, 1);
            PlayerFieldManager.Instance().OnClickPlusButton();
            PlayerFieldManager.Instance().SetCorePos(ConstManager.CorePosType.TRASH, ConstManager.CorePosType.RESERVE);
            PlayerFieldManager.Instance().SetAllMyCardToRecovery();
            Close();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "コアをリザーブからトラッシュにX個移動する", () => {
            PlayerFieldManager.Instance().SetCorePos(ConstManager.CorePosType.RESERVE, ConstManager.CorePosType.TRASH, numButtonTextNum);
            Close();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "コアをフィールドからリザーブにすべて移動する", () => {
            PlayerFieldManager.Instance().SetCorePos(ConstManager.CorePosType.FIELD, ConstManager.CorePosType.RESERVE);
            Close();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.F12, false, "フィールドのカードを一段階回復する", () => {
            PlayerFieldManager.Instance().SetAllMyCardToRecovery();
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "コアをトラッシュからリザーブにすべて移動する", () => {
            PlayerFieldManager.Instance().SetCorePos(ConstManager.CorePosType.TRASH, ConstManager.CorePosType.RESERVE);
            Close();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "リフレッシュステップ", () => {
            PlayerFieldManager.Instance().SetCorePos(ConstManager.CorePosType.TRASH, ConstManager.CorePosType.RESERVE);
            PlayerFieldManager.Instance().SetAllMyCardToRecovery();
            Close();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "ドロー&リフレッシュステップ", () => {
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.HAND, true, 1);
            PlayerFieldManager.Instance().SetCorePos(ConstManager.CorePosType.TRASH, ConstManager.CorePosType.RESERVE);
            PlayerFieldManager.Instance().SetAllMyCardToRecovery();
            Close();
        }));

        AddInputActionList(KeyCodeManager.InputType.GET_DOWN, KeyCode.Tab, "コアをリザーブからトラッシュに1個移動する", () =>
        {
            PlayerFieldManager.Instance().SetCorePos(ConstManager.CorePosType.RESERVE, ConstManager.CorePosType.TRASH, 1);
        });
    }

    private void SetButtonToDigimon()
    {
        OPTION_TYPE optionType = OPTION_TYPE.STEP;
        OPTION_TYPE detailOptionType = OPTION_TYPE.NONE;

        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.F1, false, "アクティブ＆ドローフェイズ", () => {
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.HAND, true, 1);
            PlayerFieldManager.Instance().SetAllMyCardToRecovery();
            Close();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.F12, false, damageStr + "と手札をデッキに戻して引き直す", () => {
            List<GameObject> handObjectList = FieldCardManager.Instance().GetHandGameObject();
            foreach (GameObject handObject in handObjectList)
            {
                string[] list = handObject.name.Split('^');
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.HAND, OPTION_TYPE.DECK, handObject.GetComponent<Image>(), list[0], list[1]);
            }
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DAMAGE, OPTION_TYPE.DECK, true, 5);

            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.HAND, true, 5);
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.DAMAGE, true, 5);
            CloseOnSound();
        }));
    }

    private void SetButtonToOption()
    {
        OPTION_TYPE optionType = OPTION_TYPE.STEP;
        OPTION_TYPE detailOptionType = OPTION_TYPE.NONE;

        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "自分のトークン一覧を確認する", () => {
            CardListWindow.Instance().Close();
            List<DeckManager.CardDetail> openCardList = new List<DeckManager.CardDetail>();
            List<DeckManager.CardDetail> cardList = FieldCardManager.Instance().GetCardDetailList(OPTION_TYPE.TOKEN);
            for (int index = 0; index < cardList.Count; index++)
            {
                openCardList.Add(cardList[index]);
            }

            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.TOKEN, openCardList);
            Close();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "自分の手札からランダムにX枚捨てる", () => {
            for (int index = 0; index < numButtonTextNum; index++)
            {
                List<GameObject> handObjectList = FieldCardManager.Instance().GetHandGameObject();
                GameObject handObject = handObjectList[UnityEngine.Random.Range(0, handObjectList.Count)];
                string[] list = handObject.name.Split('^');
                Debug.Log(handObject.name);
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.HAND, OPTION_TYPE.TRASH, handObject.GetComponent<Image>(), list[0], list[1]);
            }
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.F9, false, "手札をデッキに戻してX枚引き直す", () => {
            List<GameObject> handObjectList = FieldCardManager.Instance().GetHandGameObject();
            foreach (GameObject handObject in handObjectList)
            {
                string[] list = handObject.name.Split('^');
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.HAND, OPTION_TYPE.DECK, handObject.GetComponent<Image>(), list[0], list[1]);
            }
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.HAND, true, numButtonTextNum);
            CloseOnSound();
        }));

        optionType = OPTION_TYPE.TOKEN;
        detailOptionType = OPTION_TYPE.CARD_LIST;

        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "トークンをフィールドに出す", () => {
            PlayerFieldManager.Instance().CreateCard(target.name);
            CloseOnSound();
        }));

        AddInputActionList(KeyCodeManager.InputType.GET_DOWN, KeyCode.T, "トークン一覧の一種類目をフィールドに出す", () =>
        {
            List<DeckManager.CardDetail> cardList = FieldCardManager.Instance().GetCardDetailList(OPTION_TYPE.TOKEN);
            if (cardList.Count > 0)
            {
                PlayerFieldManager.Instance().CreateCard(cardList[0].ToString());
            }
        });

        AddInputActionList(KeyCodeManager.InputType.GET_DOWN, KeyCode.Alpha1, "Xの設定値を1に変更する。", () =>
        {
            OnClickToNumSelectButton(1);
        });
        AddInputActionList(KeyCodeManager.InputType.GET_DOWN, KeyCode.Alpha2, "Xの設定値を2に変更する。", () =>
        {
            OnClickToNumSelectButton(2);
        });
        AddInputActionList(KeyCodeManager.InputType.GET_DOWN, KeyCode.Alpha3, "Xの設定値を3に変更する。", () =>
        {
            OnClickToNumSelectButton(3);
        });
        AddInputActionList(KeyCodeManager.InputType.GET_DOWN, KeyCode.Alpha4, "Xの設定値を4に変更する。", () =>
        {
            OnClickToNumSelectButton(4);
        });
        AddInputActionList(KeyCodeManager.InputType.GET_DOWN, KeyCode.Alpha5, "Xの設定値を5に変更する。", () =>
        {
            OnClickToNumSelectButton(5);
        });
        AddInputActionList(KeyCodeManager.InputType.GET_DOWN, KeyCode.Alpha6, "Xの設定値を6に変更する。", () =>
        {
            OnClickToNumSelectButton(6);
        });
        AddInputActionList(KeyCodeManager.InputType.GET_DOWN, KeyCode.Alpha7, "Xの設定値を7に変更する。", () =>
        {
            OnClickToNumSelectButton(7);
        });
        AddInputActionList(KeyCodeManager.InputType.GET_DOWN, KeyCode.Alpha8, "Xの設定値を8に変更する。", () =>
        {
            OnClickToNumSelectButton(8);
        });
        AddInputActionList(KeyCodeManager.InputType.GET_DOWN, KeyCode.Alpha9, "Xの設定値を9に変更する。", () =>
        {
            OnClickToNumSelectButton(9);
        });
        AddInputActionList(KeyCodeManager.InputType.GET_DOWN, KeyCode.Alpha0, "Xの設定値を10に変更する。", () =>
        {
            OnClickToNumSelectButton(10);
        });
    }

    

    private void SetButtonToDeck()
    {
        OPTION_TYPE optionType = OPTION_TYPE.DECK;
        OPTION_TYPE detailOptionType = OPTION_TYPE.NONE;

        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "デッキをシャッフルする", () => {
            FieldCardManager.Instance().ShuffleCardDetailList(OPTION_TYPE.DECK);
            Close();
            AudioSourceManager.Instance().PlayOneShot(1);
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "自分のデッキを確認する", () => {
            CardListWindow.Instance().Close();
            List<DeckManager.CardDetail> openCardList = new List<DeckManager.CardDetail>();
            List<DeckManager.CardDetail> cardList = FieldCardManager.Instance().GetCardDetailList(OPTION_TYPE.DECK);
            for (int index = 0; index < cardList.Count; index++)
            {
                openCardList.Add(cardList[index]);
            }

            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.DECK, openCardList);
            Close();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "デッキの上からX枚を手札に加える", () => {
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.HAND, true, numButtonTextNum);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "デッキの下からX枚を手札に加える", () => {
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.HAND, false, numButtonTextNum);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "相手に見せつつデッキの上からX枚を公開する", () => {
            var cardList = FieldCardManager.Instance().GetCardDetailList(OPTION_TYPE.DECK, true, numButtonTextNum);
            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.DECK, cardList, true, true);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "デッキの上からX枚を公開する", () => {
            var cardList = FieldCardManager.Instance().GetCardDetailList(OPTION_TYPE.DECK, true, numButtonTextNum);
            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.DECK, cardList);
            CloseOnSound();
        }));

        if (BattleSceneManager.m_type != "digimon")
        {
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "デッキの上からX枚を手元に置く", () => {
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.AT_HAND, true, numButtonTextNum);
                CloseOnSound();
            }));
        }

        if (BattleSceneManager.m_type != "bs")
        {
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "デッキの上から" + damageStr + "の上に送る", () => {
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.DAMAGE, true, numButtonTextNum);
                CloseOnSound();
            }));
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "デッキの上から" + subStr + "の上に送る", () => {
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.SUB, true, numButtonTextNum);
                CloseOnSound();
            }));
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "デッキの上から" + damageStr + "の下に送る", () => {
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.DAMAGE, false, numButtonTextNum);
                CloseOnSound();
            }));
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "デッキの上から" + subStr + "の下に送る", () => {
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.SUB, false, numButtonTextNum);
                CloseOnSound();
            }));
        }

        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "デッキの上からX枚をトラッシュに送る", () => {
            var cardList = FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.TRASH, true, numButtonTextNum);
            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.CARD_LIST, cardList);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "デッキの上からX枚を除外一覧に送る", () => {
            var cardList = FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.EXCLUSION, true, numButtonTextNum);
            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.CARD_LIST, cardList);
            CloseOnSound();
        }));

        detailOptionType = OPTION_TYPE.CARD_LIST;

        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "デッキの上からX枚を除外一覧に送る", () => {
            var cardList = FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.EXCLUSION, true, numButtonTextNum);
            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.CARD_LIST, cardList);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "デッキからフィールドに出す", () => {
            string[] list = target.name.Split('^');
            var card = FieldCardManager.Instance().RemoveCardDetail(OPTION_TYPE.DECK, list[0], list[1])[0];
            PlayerFieldManager.Instance().CreateCard(target.name);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "デッキの上に戻す", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.DECK, true, list[0], list[1]);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "デッキの下に戻す", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.DECK, false, list[0], list[1]);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "デッキから手札に加える", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.HAND, list[0], list[1]);
            CloseOnSound();
        }));

        if (BattleSceneManager.m_type != "digimon")
        {
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "デッキから手元に置く", () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.AT_HAND, list[0], list[1]);
                CloseOnSound();
            }));
        }

        if (BattleSceneManager.m_type != "bs")
        {
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "デッキから" + damageStr + "の上に加える", () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.DAMAGE, true, list[0], list[1]);
                CloseOnSound();
            }));
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "デッキから" + subStr + "の上に加える", () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.SUB, true, list[0], list[1]);
                CloseOnSound();
            }));
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "デッキから" + damageStr + "の下に加える", () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.DAMAGE, false, list[0], list[1]);
                CloseOnSound();
            }));
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "デッキから" + subStr + "の下に加える", () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.SUB, false, list[0], list[1]);
                CloseOnSound();
            }));
        }

        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "デッキからトラッシュに送る", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.TRASH, list[0], list[1]);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "デッキから除外一覧に送る", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.EXCLUSION, list[0], list[1]);
            CloseOnSound();
        }));

        detailOptionType = OPTION_TYPE.CONTRACT;

        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "デッキから契約カードを手札に加える", () => {
            FieldCardManager.Instance().AddHandFromDeckToContract();
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.HAND, true, 3);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "デッキから契約カードを手札に加えない", () => {
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.HAND, true, 4);
            CloseOnSound();
        }));

        AddInputActionList(KeyCodeManager.InputType.GET_DOWN, KeyCode.Q, "デッキの上から1枚を手札に加える", () =>
        {
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DECK, OPTION_TYPE.HAND, true, 1);
            CloseOnSound();
        });
    }

    public void SetButtonToHand()
    {
        OPTION_TYPE optionType = OPTION_TYPE.HAND;
        OPTION_TYPE detailOptionType = OPTION_TYPE.NONE;

        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "手札からデッキの上に戻す", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.HAND, OPTION_TYPE.DECK, true, target, list[0], list[1]);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "手札からデッキの下に戻す", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.HAND, OPTION_TYPE.DECK, false, target, list[0], list[1]);
            CloseOnSound();
        }));

        if (BattleSceneManager.m_type != "digimon")
        {
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "手札から手元に置く", () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.HAND, OPTION_TYPE.AT_HAND, target, list[0], list[1]);
                CloseOnSound();
            }));
        }

        if (BattleSceneManager.m_type != "bs")
        {
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "手札から" + damageStr + "の上に加える", () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.HAND, OPTION_TYPE.DAMAGE, true, target, list[0], list[1]);
                CloseOnSound();
            }));
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "手札から" + subStr + "の上に加える", () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.HAND, OPTION_TYPE.SUB, true, target, list[0], list[1]);
                CloseOnSound();
            }));
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "手札から" + damageStr + "の下に加える", () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.HAND, OPTION_TYPE.DAMAGE, false, target, list[0], list[1]);
                CloseOnSound();
            }));
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "手札から" + subStr + "の下に加える", () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.HAND, OPTION_TYPE.SUB, false, target, list[0], list[1]);
                CloseOnSound();
            }));
        }

        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "手札からトラッシュに送る", () => {
            string[] list = target.name.Split('^');
            var card = FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.HAND, OPTION_TYPE.TRASH, target, list[0], list[1]);
            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.CARD_LIST, new List<DeckManager.CardDetail>() { card });
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "手札から除外一覧に送る", () => {
            string[] list = target.name.Split('^');
            var card = FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.HAND, OPTION_TYPE.EXCLUSION, target, list[0], list[1]);
            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.CARD_LIST, new List<DeckManager.CardDetail>() { card });
            CloseOnSound();
        }));
    }

    public void SetButtonToAtHand()
    {
        OPTION_TYPE optionType = OPTION_TYPE.AT_HAND;
        OPTION_TYPE detailOptionType = OPTION_TYPE.NONE;

        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "カードを開く", () => {
            HandCard handCard = target.GetComponent<HandCard>();
            if (handCard != null)
            {
                handCard.SetIsOpen(true);
            }
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "カードを閉じる", () => {
            HandCard handCard = target.GetComponent<HandCard>();
            if (handCard != null)
            {
                handCard.SetIsOpen(false);
            }
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "手元からデッキの上に戻す", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.AT_HAND, OPTION_TYPE.DECK, true, target, list[0], list[1]);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "手元からデッキの下に戻す", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.AT_HAND, OPTION_TYPE.DECK, false, target, list[0], list[1]);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "手元から手札に加える", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.AT_HAND, OPTION_TYPE.HAND, target, list[0], list[1]);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "手元からトラッシュに送る", () => {
            string[] list = target.name.Split('^');
            var card = FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.AT_HAND, OPTION_TYPE.TRASH, target, list[0], list[1]);
            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.CARD_LIST, new List<DeckManager.CardDetail>() { card });
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "手元から除外一覧に送る", () => {
            string[] list = target.name.Split('^');
            var card = FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.AT_HAND, OPTION_TYPE.EXCLUSION, target, list[0], list[1]);
            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.CARD_LIST, new List<DeckManager.CardDetail>() { card });
            CloseOnSound();
        }));
    }

    public void SetButtonToField()
    {
        OPTION_TYPE optionType = OPTION_TYPE.FIELD;
        OPTION_TYPE detailOptionType = OPTION_TYPE.NONE;

        detailOptionType = OPTION_TYPE.FIELD_ROT;

        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "カードを開く", () => {
            TouchManager touchManager = target.GetComponent<TouchManager>();
            if (touchManager != null)
            {
                touchManager.SetIsOpen(true);
            }
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "カードを閉じる", () => {
            TouchManager touchManager = target.GetComponent<TouchManager>();
            if (touchManager != null)
            {
                touchManager.SetIsOpen(false);
            }
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "カードをスタンドさせる", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().SetCardToStand(target);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "カードを疲労させる", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().SetCardToRRest(target);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "カードを重疲労させる", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().SetCardToDualRest(target);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "カードを反疲労させる", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().SetCardToLRest(target);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "カードを裏返す", () => {
            TouchManager touchManager = target.GetComponent<TouchManager>();
            if (touchManager != null)
            {
                touchManager.SetIsAwake(!touchManager.IsAwake);
            }
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "カードを魂状態を入れ替える", () => {
            TouchManager touchManager = target.GetComponent<TouchManager>();
            if (touchManager != null)
            {
                touchManager.SetIsSoul(!touchManager.IsSoul);
            }
            CloseOnSound();
        }));

        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "フィールドからデッキの上に戻す", () => {
            TouchManager touchManager = target.GetComponent<TouchManager>();
            if (touchManager != null)
            {
                foreach (var cardDetail in touchManager.m_innerCardDetailList)
                {
                    FieldCardManager.Instance().AddDstFromSrc(CardOptionWindow.OPTION_TYPE.FIELD, innerListFromType, true, cardDetail.tag, cardDetail.cardId);
                }
            }
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.FIELD, OPTION_TYPE.DECK, true, target, list[0], list[1]);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "フィールドからデッキの下に戻す", () => {
            TouchManager touchManager = target.GetComponent<TouchManager>();
            if (touchManager != null)
            {
                foreach (var cardDetail in touchManager.m_innerCardDetailList)
                {
                    FieldCardManager.Instance().AddDstFromSrc(CardOptionWindow.OPTION_TYPE.FIELD, innerListFromType, true, cardDetail.tag, cardDetail.cardId);
                }
            }
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.FIELD, OPTION_TYPE.DECK, false, target, list[0], list[1]);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "フィールドから手札に加える", () => {
            TouchManager touchManager = target.GetComponent<TouchManager>();
            if (touchManager != null)
            {
                foreach (var cardDetail in touchManager.m_innerCardDetailList)
                {
                    FieldCardManager.Instance().AddDstFromSrc(CardOptionWindow.OPTION_TYPE.FIELD, innerListFromType, true, cardDetail.tag, cardDetail.cardId);
                }
            }
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.FIELD, OPTION_TYPE.HAND, target, list[0], list[1]);
            CloseOnSound();
        }));

        if (BattleSceneManager.m_type != "digimon")
        {
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "フィールドから手元に置く", () => {
                TouchManager touchManager = target.GetComponent<TouchManager>();
                if (touchManager != null)
                {
                    foreach (var cardDetail in touchManager.m_innerCardDetailList)
                    {
                        FieldCardManager.Instance().AddDstFromSrc(CardOptionWindow.OPTION_TYPE.FIELD, innerListFromType, true, cardDetail.tag, cardDetail.cardId);
                    }
                }
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.FIELD, OPTION_TYPE.AT_HAND, target, list[0], list[1]);
                CloseOnSound();
            }));
        }

        if (BattleSceneManager.m_type != "bs")
        {
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "フィールドから" + damageStr + "の上に加える", () => {
                TouchManager touchManager = target.GetComponent<TouchManager>();
                if (touchManager != null)
                {
                    foreach (var cardDetail in touchManager.m_innerCardDetailList)
                    {
                        FieldCardManager.Instance().AddDstFromSrc(CardOptionWindow.OPTION_TYPE.FIELD, innerListFromType, true, cardDetail.tag, cardDetail.cardId);
                    }
                }
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.FIELD, OPTION_TYPE.DAMAGE, true, target, list[0], list[1]);
                CloseOnSound();
            }));
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "フィールドから" + subStr + "の上に加える", () => {
                TouchManager touchManager = target.GetComponent<TouchManager>();
                if (touchManager != null)
                {
                    foreach (var cardDetail in touchManager.m_innerCardDetailList)
                    {
                        FieldCardManager.Instance().AddDstFromSrc(CardOptionWindow.OPTION_TYPE.FIELD, innerListFromType, true, cardDetail.tag, cardDetail.cardId);
                    }
                }
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.FIELD, OPTION_TYPE.SUB, true, target, list[0], list[1]);
                CloseOnSound();
            }));
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "フィールドから" + damageStr + "の下に加える", () => {
                TouchManager touchManager = target.GetComponent<TouchManager>();
                if (touchManager != null)
                {
                    foreach (var cardDetail in touchManager.m_innerCardDetailList)
                    {
                        FieldCardManager.Instance().AddDstFromSrc(CardOptionWindow.OPTION_TYPE.FIELD, innerListFromType, true, cardDetail.tag, cardDetail.cardId);
                    }
                }
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.FIELD, OPTION_TYPE.DAMAGE, false, target, list[0], list[1]);
                CloseOnSound();
            }));
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "フィールドから" + subStr + "の下に加える", () => {
                TouchManager touchManager = target.GetComponent<TouchManager>();
                if (touchManager != null)
                {
                    foreach (var cardDetail in touchManager.m_innerCardDetailList)
                    {
                        FieldCardManager.Instance().AddDstFromSrc(CardOptionWindow.OPTION_TYPE.FIELD, innerListFromType, true, cardDetail.tag, cardDetail.cardId);
                    }
                }
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.FIELD, OPTION_TYPE.SUB, false, target, list[0], list[1]);
                CloseOnSound();
            }));
        }

        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "フィールドからトラッシュに送る", () => {
            TouchManager touchManager = target.GetComponent<TouchManager>();
            if (touchManager != null)
            {
                foreach (var cardDetail in touchManager.m_innerCardDetailList)
                {
                    FieldCardManager.Instance().AddDstFromSrc(CardOptionWindow.OPTION_TYPE.FIELD, innerListFromType, true, cardDetail.tag, cardDetail.cardId);
                }
            }
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.FIELD, OPTION_TYPE.TRASH, target, list[0], list[1]);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "フィールドから除外一覧に送る", () => {
            TouchManager touchManager = target.GetComponent<TouchManager>();
            if (touchManager != null)
            {
                foreach (var cardDetail in touchManager.m_innerCardDetailList)
                {
                    FieldCardManager.Instance().AddDstFromSrc(CardOptionWindow.OPTION_TYPE.FIELD, innerListFromType, true, cardDetail.tag, cardDetail.cardId);
                }
            }
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.FIELD, OPTION_TYPE.EXCLUSION, target, list[0], list[1]);
            CloseOnSound();
        }));
    }

    public void SetButtonToExclusion()
    {
        OPTION_TYPE optionType = OPTION_TYPE.EXCLUSION;
        OPTION_TYPE detailOptionType = OPTION_TYPE.NONE;

        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "自分の除外一覧を確認する", () => {
            CardListWindow.Instance().Close();
            List<DeckManager.CardDetail> openCardList = new List<DeckManager.CardDetail>();
            List<DeckManager.CardDetail> cardList = FieldCardManager.Instance().GetCardDetailList(OPTION_TYPE.EXCLUSION);
            for (int index = 0; index < cardList.Count; index++)
            {
                openCardList.Add(cardList[index]);
            }

            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.EXCLUSION, openCardList);
            Close();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "見ないで自分の除外一覧の上からX枚をトラッシュに送る", () => {
            var cardList = FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.EXCLUSION, OPTION_TYPE.TRASH, true, numButtonTextNum);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "相手に見せつつ自分の除外一覧を確認する", () => {
            CardListWindow.Instance().Close();
            List<DeckManager.CardDetail> openCardList = new List<DeckManager.CardDetail>();
            List<DeckManager.CardDetail> cardList = FieldCardManager.Instance().GetCardDetailList(OPTION_TYPE.EXCLUSION);
            for (int index = 0; index < cardList.Count; index++)
            {
                openCardList.Add(cardList[index]);
            }

            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.EXCLUSION, openCardList, true, true);
            Close();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "相手の除外一覧を確認する", () => {
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
                List<DeckManager.CardDetail> cardList = playField.GetCardDetailList(OPTION_TYPE.EXCLUSION);
                for (int index = 0; index < cardList.Count; index++)
                {
                    openCardList.Add(cardList[index]);
                }

                CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.EXCLUSION, openCardList, false);
            }
            Close();
        }));

        detailOptionType = OPTION_TYPE.CARD_LIST;

        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "除外一覧からデッキの上に戻す", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.EXCLUSION, OPTION_TYPE.DECK, true, list[0], list[1]);
            OpenCardDetailListToMine(OPTION_TYPE.EXCLUSION);
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "除外一覧からデッキの下に戻す", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.EXCLUSION, OPTION_TYPE.DECK, false, list[0], list[1]);
            OpenCardDetailListToMine(OPTION_TYPE.EXCLUSION);
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "除外一覧から手札に加える", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.EXCLUSION, OPTION_TYPE.HAND, list[0], list[1]);
            OpenCardDetailListToMine(OPTION_TYPE.EXCLUSION);
        }));

        if (BattleSceneManager.m_type != "digimon")
        {
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "除外一覧から手元に置く", () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.EXCLUSION, OPTION_TYPE.AT_HAND, list[0], list[1]);
                OpenCardDetailListToMine(OPTION_TYPE.EXCLUSION);
            }));
        }

        if (BattleSceneManager.m_type != "bs")
        {
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "除外一覧から" + damageStr + "の上に加える", () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.EXCLUSION, OPTION_TYPE.DAMAGE, true, list[0], list[1]);
                OpenCardDetailListToMine(OPTION_TYPE.EXCLUSION);
            }));
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "除外一覧から" + subStr + "の上に加える", () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.EXCLUSION, OPTION_TYPE.SUB, true, list[0], list[1]);
                OpenCardDetailListToMine(OPTION_TYPE.EXCLUSION);
            }));
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "除外一覧から" + damageStr + "の下に加える", () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.EXCLUSION, OPTION_TYPE.DAMAGE, false, list[0], list[1]);
                OpenCardDetailListToMine(OPTION_TYPE.EXCLUSION);
            }));
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "除外一覧から" + subStr + "の下に加える", () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.EXCLUSION, OPTION_TYPE.SUB, false, list[0], list[1]);
                OpenCardDetailListToMine(OPTION_TYPE.EXCLUSION);
            }));
        }

        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "除外一覧からトラッシュに送る", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.EXCLUSION, OPTION_TYPE.TRASH, list[0], list[1]);
            OpenCardDetailListToMine(OPTION_TYPE.EXCLUSION);
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "除外一覧からフィールドに出す", () => {
            string[] list = target.name.Split('^');
            var card = FieldCardManager.Instance().RemoveCardDetail(OPTION_TYPE.EXCLUSION, list[0], list[1])[0];
            PlayerFieldManager.Instance().CreateCard(target.name);
            OpenCardDetailListToMine(OPTION_TYPE.EXCLUSION);
        }));
    }

    public void SetButtonToTrash()
    {
        OPTION_TYPE optionType = OPTION_TYPE.TRASH;
        OPTION_TYPE detailOptionType = OPTION_TYPE.NONE;

        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "自分のトラッシュを確認する", () => {
            CardListWindow.Instance().Close();
            List<DeckManager.CardDetail> openCardList = new List<DeckManager.CardDetail>();
            List<DeckManager.CardDetail> cardList = FieldCardManager.Instance().GetCardDetailList(OPTION_TYPE.TRASH);
            for (int index = 0; index < cardList.Count; index++)
            {
                openCardList.Add(cardList[index]);
            }

            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.TRASH, openCardList);
            Close();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "相手に見せつつ自分のトラッシュを確認する", () => {
            CardListWindow.Instance().Close();
            List<DeckManager.CardDetail> openCardList = new List<DeckManager.CardDetail>();
            List<DeckManager.CardDetail> cardList = FieldCardManager.Instance().GetCardDetailList(OPTION_TYPE.TRASH);
            for (int index = 0; index < cardList.Count; index++)
            {
                openCardList.Add(cardList[index]);
            }

            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.TRASH, openCardList, true, true);
            Close();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "相手のトラッシュを確認する", () => {
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
                List<DeckManager.CardDetail> cardList = playField.GetCardDetailList(OPTION_TYPE.TRASH);
                for (int index = 0; index < cardList.Count; index++)
                {
                    openCardList.Add(cardList[index]);
                }
                CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.TRASH, openCardList, false);
            }
            Close();
        }));

        detailOptionType = OPTION_TYPE.CARD_LIST;

        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "トラッシュからデッキの上に戻す", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.TRASH, OPTION_TYPE.DECK, true, list[0], list[1]);
            OpenCardDetailListToMine(OPTION_TYPE.TRASH);
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "トラッシュからデッキの下に戻す", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.TRASH, OPTION_TYPE.DECK, false, list[0], list[1]);
            OpenCardDetailListToMine(OPTION_TYPE.TRASH);
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "トラッシュから手札に加える", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.TRASH, OPTION_TYPE.HAND, list[0], list[1]);
            OpenCardDetailListToMine(OPTION_TYPE.TRASH);
        }));

        if (BattleSceneManager.m_type != "digimon")
        {
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "トラッシュから手元に置く", () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.TRASH, OPTION_TYPE.AT_HAND, list[0], list[1]);
                OpenCardDetailListToMine(OPTION_TYPE.TRASH);
            }));
        }

        if (BattleSceneManager.m_type != "bs")
        {
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "トラッシュから" + damageStr + "の上に加える", () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.TRASH, OPTION_TYPE.DAMAGE, true, list[0], list[1]);
                OpenCardDetailListToMine(OPTION_TYPE.EXCLUSION);
            }));
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "トラッシュから" + subStr + "の上に加える", () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.TRASH, OPTION_TYPE.SUB, true, list[0], list[1]);
                OpenCardDetailListToMine(OPTION_TYPE.EXCLUSION);
            }));
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "トラッシュから" + damageStr + "の下に加える", () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.TRASH, OPTION_TYPE.DAMAGE, false, list[0], list[1]);
                OpenCardDetailListToMine(OPTION_TYPE.EXCLUSION);
            }));
            m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "トラッシュから" + subStr + "の下に加える", () => {
                string[] list = target.name.Split('^');
                FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.TRASH, OPTION_TYPE.SUB, false, list[0], list[1]);
                OpenCardDetailListToMine(OPTION_TYPE.EXCLUSION);
            }));
        }

        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "トラッシュから除外一覧に送る", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.TRASH, OPTION_TYPE.EXCLUSION, list[0], list[1]);
            OpenCardDetailListToMine(OPTION_TYPE.TRASH);
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "トラッシュからフィールドに出す", () => {
            string[] list = target.name.Split('^');
            var card = FieldCardManager.Instance().RemoveCardDetail(OPTION_TYPE.TRASH, list[0], list[1])[0];
            PlayerFieldManager.Instance().CreateCard(target.name);
            OpenCardDetailListToMine(OPTION_TYPE.TRASH);
        }));
    }

    private void SetButtonToDamage()
    {
        OPTION_TYPE optionType = OPTION_TYPE.DAMAGE;
        OPTION_TYPE detailOptionType = OPTION_TYPE.NONE;

        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, damageStr + "をシャッフルする", () => {
            FieldCardManager.Instance().ShuffleCardDetailList(OPTION_TYPE.DAMAGE);
            Close();
            AudioSourceManager.Instance().PlayOneShot(1);
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "相手に見せつつ" + damageStr + "の上からX枚を公開する", () => {
            var cardList = FieldCardManager.Instance().GetCardDetailList(OPTION_TYPE.DAMAGE, true, numButtonTextNum);
            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.DAMAGE, cardList, true, true);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, damageStr + "の上からX枚を公開する", () => {
            var cardList = FieldCardManager.Instance().GetCardDetailList(OPTION_TYPE.DAMAGE, true, numButtonTextNum);
            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.DAMAGE, cardList);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, damageStr + "の上からX枚を手札に加える", () => {
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DAMAGE, OPTION_TYPE.HAND, true, numButtonTextNum);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, damageStr + "の下からX枚を手札に加える", () => {
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DAMAGE, OPTION_TYPE.HAND, false, numButtonTextNum);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, damageStr + "の上からX枚を" + subStr + "に送る", () => {
            var cardList = FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DAMAGE, OPTION_TYPE.SUB, true, numButtonTextNum);
            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.CARD_LIST, cardList);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, damageStr + "の上からX枚をトラッシュに送る", () => {
            var cardList = FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DAMAGE, OPTION_TYPE.TRASH, true, numButtonTextNum);
            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.CARD_LIST, cardList);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, damageStr + "の上からX枚を除外一覧に送る", () => {
            var cardList = FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DAMAGE, OPTION_TYPE.EXCLUSION, true, numButtonTextNum);
            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.CARD_LIST, cardList);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "見ないで" + damageStr + "の上からX枚をトラッシュに送る", () => {
            var cardList = FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DAMAGE, OPTION_TYPE.TRASH, true, numButtonTextNum);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "見ないで" + damageStr + "の上からX枚を除外一覧に送る", () => {
            var cardList = FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DAMAGE, OPTION_TYPE.EXCLUSION, true, numButtonTextNum);
            CloseOnSound();
        }));

        detailOptionType = OPTION_TYPE.CARD_LIST;

        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, damageStr + "からフィールドに出す", () => {
            string[] list = target.name.Split('^');
            var card = FieldCardManager.Instance().RemoveCardDetail(OPTION_TYPE.DAMAGE, list[0], list[1])[0];
            PlayerFieldManager.Instance().CreateCard(target.name);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, damageStr + "の上に戻す", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DAMAGE, OPTION_TYPE.DAMAGE, true, list[0], list[1]);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, damageStr + "の下に戻す", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DAMAGE, OPTION_TYPE.DAMAGE, false, list[0], list[1]);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, damageStr + "から手札に加える", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DAMAGE, OPTION_TYPE.HAND, list[0], list[1]);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, damageStr + "からトラッシュに送る", () => {
            string[] list = target.name.Split('^');
            List<bool> isTrashList = new List<bool>() { true, false };
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DAMAGE, OPTION_TYPE.TRASH, list[0], list[1]);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, damageStr + "から除外一覧に送る", () => {
            string[] list = target.name.Split('^');
            List<bool> isTrashList = new List<bool>() { true, false };
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.DAMAGE, OPTION_TYPE.EXCLUSION, list[0], list[1]);
            CloseOnSound();
        }));
    }

    private void SetButtonToSub()
    {
        OPTION_TYPE optionType = OPTION_TYPE.SUB;
        OPTION_TYPE detailOptionType = OPTION_TYPE.NONE;

        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, subStr + "をシャッフルする", () => {
            FieldCardManager.Instance().ShuffleCardDetailList(OPTION_TYPE.SUB);
            Close();
            AudioSourceManager.Instance().PlayOneShot(1);
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "相手に見せつつ" + subStr + "の上からX枚を公開する", () => {
            var cardList = FieldCardManager.Instance().GetCardDetailList(OPTION_TYPE.SUB, true, numButtonTextNum);
            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.SUB, cardList, true, true);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, subStr + "の上からX枚を公開する", () => {
            var cardList = FieldCardManager.Instance().GetCardDetailList(OPTION_TYPE.SUB, true, numButtonTextNum);
            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.SUB, cardList);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, subStr + "の上からX枚を" + damageStr + "に送る", () => {
            var cardList = FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.SUB, OPTION_TYPE.TRASH, true, numButtonTextNum);
            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.CARD_LIST, cardList);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, subStr + "の上からX枚をトラッシュに送る", () => {
            var cardList = FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.SUB, OPTION_TYPE.TRASH, true, numButtonTextNum);
            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.CARD_LIST, cardList);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, subStr + "の上からX枚を除外一覧に送る", () => {
            var cardList = FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.SUB, OPTION_TYPE.EXCLUSION, true, numButtonTextNum);
            CardListWindow.Instance().Open(CardOptionWindow.OPTION_TYPE.CARD_LIST, cardList);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "見ないで" + subStr + "の上からX枚をトラッシュに送る", () => {
            var cardList = FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.SUB, OPTION_TYPE.TRASH, true, numButtonTextNum);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, "見ないで" + subStr + "の上からX枚を除外一覧に送る", () => {
            var cardList = FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.SUB, OPTION_TYPE.EXCLUSION, true, numButtonTextNum);
            CloseOnSound();
        }));

        detailOptionType = OPTION_TYPE.CARD_LIST;

        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, subStr + "からフィールドに出す", () => {
            string[] list = target.name.Split('^');
            var card = FieldCardManager.Instance().RemoveCardDetail(OPTION_TYPE.SUB, list[0], list[1])[0];
            PlayerFieldManager.Instance().CreateCard(target.name);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, subStr + "の上に戻す", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.SUB, OPTION_TYPE.SUB, true, list[0], list[1]);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, subStr + "の下に戻す", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.SUB, OPTION_TYPE.SUB, false, list[0], list[1]);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, subStr + "から手札に加える", () => {
            string[] list = target.name.Split('^');
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.SUB, OPTION_TYPE.HAND, list[0], list[1]);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, subStr + "からトラッシュに送る", () => {
            string[] list = target.name.Split('^');
            List<bool> isTrashList = new List<bool>() { true, false };
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.SUB, OPTION_TYPE.TRASH, list[0], list[1]);
            CloseOnSound();
        }));
        m_optionButtonList.Add(new OptionButton(optionType, detailOptionType, KeyCode.None, false, subStr + "から除外一覧に送る", () => {
            string[] list = target.name.Split('^');
            List<bool> isTrashList = new List<bool>() { true, false };
            FieldCardManager.Instance().AddDstFromSrc(OPTION_TYPE.SUB, OPTION_TYPE.EXCLUSION, list[0], list[1]);
            CloseOnSound();
        }));


        AddInputActionList(KeyCodeManager.InputType.GET_DOWN, KeyCode.R, subStr + "の上から1枚をフィールドに出す", () =>
        {
            var cardList = FieldCardManager.Instance().GetCardDetailList(OPTION_TYPE.SUB, true, 1)[0];
            var card = FieldCardManager.Instance().RemoveCardDetail(OPTION_TYPE.SUB, cardList.tag, cardList.cardId)[0];
            PlayerFieldManager.Instance().CreateCard(card.ToString());
            CloseOnSound();
        });
    }

    public void CreateOptionButton()
    {
        for (int i = 0; i < m_optionButtonList.Count; i++)
        {
            OptionButton optionButton = m_optionButtonList[i];
            m_text.text = m_optionButtonList[i].title;
            optionButton.button = UnityEngine.Object.Instantiate(m_button);
            optionButton.button.transform.SetParent(m_button.transform.parent);
            optionButton.button.transform.localScale = Vector3.one;
            optionButton.onClickAction = () =>
            {
                AddLogList(optionButton.title);
                optionButton.action();
            };
            optionButton.button.onClick.AddListener(optionButton.onClickAction);

            if (optionButton.keyCode != KeyCode.None)
            {
                AddInputActionList(
                    KeyCodeManager.InputType.GET_DOWN, optionButton.keyCode, optionButton.title, () => { optionButton.onClickAction(); }, false
                );
            }
        }
    }

    public void AddLogList(string str)
    {
        if (str.Contains("X"))
        {
            str = str.Replace("X", numButtonTextNum.ToString());
        }
        PlayerFieldManager.Instance().AddLogList(str);
    }

    public void AddInputActionList(KeyCodeManager.InputType inputType, KeyCode keyCode, string title, Action action, bool isLog = true)
    {
        KeyCodeManager.Instance().AddInputActionList(
            KeyCodeManager.InputType.GET_DOWN, keyCode, title, () => {
                if (isLog) AddLogList(title);
                action();
            }
        );
    }

    private void OpenCardDetailListToMine(OPTION_TYPE option, List<DeckManager.CardDetail> cardList = null)
    {
        CardListWindow.Instance().Close();
        List<DeckManager.CardDetail> openCardList = new List<DeckManager.CardDetail>();
        if (cardList == null)
        {
            cardList = FieldCardManager.Instance().GetCardDetailList(option);
        }
       
        for (int index = 0; index < cardList.Count; index++)
        {
            openCardList.Add(cardList[index]);
        }

        CardListWindow.Instance().Open(option, openCardList);
        CloseOnSound();
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
