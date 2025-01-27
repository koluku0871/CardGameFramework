using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckSceneManager : MonoBehaviour
{
    [SerializeField]
    private Button m_closeButton = null;

    [SerializeField]
    private Dropdown m_deckSelectDropdown = null;

    [SerializeField]
    private InputField m_deckCreateInputField = null;

    [SerializeField]
    private Dropdown m_packTypeDropdown = null;

    [SerializeField]
    private Dropdown m_elementDropdown = null;

    [SerializeField]
    private Dropdown m_categoryDropdown = null;

    [SerializeField]
    private Dropdown m_cardTypeDropdown = null;

    [SerializeField]
    private TMP_InputField m_cardNameInputField = null;

    [SerializeField]
    private Image m_targetCardDetail = null;

    [SerializeField]
    private Text m_deckCardCountText = null;

    [SerializeField]
    private RectTransform m_deckContent = null;

    [SerializeField]
    private Text m_searchCardCountText = null;

    [SerializeField]
    private InfiniteScroll m_infiniteScroll = null;

    [SerializeField]
    private DeckCardScrollSetup m_deckCardScrollSetup = null;
    
    private DeckManager m_deckManager = null;

    private List<CardData> cardDatas = new List<CardData>();
    public CardData GetCardDatas(int index)
    {
        if (index < 0 || cardDatas.Count-1 < index)
        {
            return null;
        }
        return cardDatas[index];
    }
    private int oldSelectId = -1;

    private static DeckSceneManager instance = null;
    public static DeckSceneManager Instance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        AudioSourceManager.Instance().PlayOneShot((int)AudioSourceManager.BGM_NUM.DECK_1, true);

        m_deckManager = DeckManager.Instance();

        DownLoadCallback();
        m_closeButton.interactable = true;
    }

    public Dropdown GetDeckSelectDropdown()
    {
        return m_deckSelectDropdown;
    }

    public InputField GetDeckCreateInputField()
    {
        return m_deckCreateInputField;
    }

    public Image GetTargetCardDetail()
    {
        return m_targetCardDetail;
    }

    public RectTransform GetDeckContent()
    {
        return m_deckContent;
    }

    public Text GetDeckCardCountText()
    {
        return m_deckCardCountText;
    }

    public Text GetSearchCardCountText()
    {
        return m_searchCardCountText;
    }

    public void OnClickToCloseButton()
    {
        FadeManager.Instance().OnStart("HomeScene");
    }

    public void DownLoadCallback()
    {
        m_packTypeDropdown.ClearOptions();
        m_packTypeDropdown.options.Add(new Dropdown.OptionData()
        {
            text = "no select"
        });
        foreach (var packData in AssetBundleManager.Instance().m_bs_baseData.packNameList)
        {
            m_packTypeDropdown.options.Add(new Dropdown.OptionData()
            {
                text = packData.Key
            });
        }
        //m_packTypeDropdown.value = m_packTypeDropdown.options.Count - 1;
        m_packTypeDropdown.value = -1;

        m_elementDropdown.ClearOptions();
        foreach (var item in ConstManager.CardElementType.Values)
        {
            m_elementDropdown.options.Add(new Dropdown.OptionData()
            {
                text = item
            });
        }
        m_elementDropdown.value = -1;

        m_cardTypeDropdown.ClearOptions();
        m_cardTypeDropdown.options.Add(new Dropdown.OptionData()
        {
            text = "All"
        });
        m_cardTypeDropdown.value = -1;

        m_categoryDropdown.ClearOptions();
        m_categoryDropdown.options.Add(new Dropdown.OptionData()
        {
            text = "All"
        });
        m_categoryDropdown.value = -1;

        OnValueChangedToPackTypeDropdown();
    }

    public void OnValueChangedToPackTypeDropdown()
    {
        cardDatas.Clear();

        var selectId = m_packTypeDropdown.value - 1;
        if (0 > selectId)
        {
            return;
        }

        var key = m_packTypeDropdown.options[m_packTypeDropdown.value].text;
        AssetBundleManager assetBundleManager = AssetBundleManager.Instance();
        if (!assetBundleManager.m_bs_baseData.packNameList.ContainsKey(key))
        {
            return;
        }
        var packNameList = assetBundleManager.m_bs_baseData.packNameList[key];

        CardDetailManager cardDetailManager = CardDetailManager.Instance();

        int searchCardCount = 0;
        HashSet<string> cardTypeList = new HashSet<string>();
        HashSet<string> categoryList = new HashSet<string>();

        if (oldSelectId != selectId)
        {
            m_elementDropdown.value = 0;
            m_cardTypeDropdown.value = 0;
            m_categoryDropdown.value = 0;
            m_cardNameInputField.text = "";
        }
        bool isAll = key == "All";
        for (int i = 0; i < packNameList.Count; i++)
        {
            string fileName = packNameList[i];
            string name = fileName;
            string targetTag = key;
            Debug.Log("key :" + key + ", fileName : " + fileName);
            if (isAll)
            {
                targetTag = fileName.Split("*")[0];
                name = fileName.Split("*")[1];
            }

            CardData cardData = assetBundleManager.GetBaseData(targetTag, name);
            if (cardData == null)
            {
                continue;
            }

            if (oldSelectId != selectId)
            {
                foreach (var item in cardData.CardType.Split("・"))
                {
                    cardTypeList.Add(item);
                }
                categoryList.Add(cardData.CardCategory);
            }

            if (!CheckCardData(cardData))
            {
                continue;
            }
            cardDatas.Add(cardData);
        }
        searchCardCount = cardDatas.Count;

        if (cardTypeList.Count > 0)
        {
            m_cardTypeDropdown.ClearOptions();
            m_cardTypeDropdown.options.Add(new Dropdown.OptionData()
            {
                text = "All"
            });
            foreach (var item in cardTypeList)
            {
                m_cardTypeDropdown.options.Add(new Dropdown.OptionData()
                {
                    text = item
                });
            }
            m_cardTypeDropdown.value = -1;
        }
        if (categoryList.Count > 0)
        {
            m_categoryDropdown.ClearOptions();
            m_categoryDropdown.options.Add(new Dropdown.OptionData()
            {
                text = "All"
            });
            foreach (var item in categoryList)
            {
                m_categoryDropdown.options.Add(new Dropdown.OptionData()
                {
                    text = item
                });
            }
            m_categoryDropdown.value = -1;
        }
        m_deckCardScrollSetup.max = (cardDatas.Count / 4);
        if (cardDatas.Count % 4 != 0)
        {
            m_deckCardScrollSetup.max++;
        }
        m_deckCardScrollSetup.SetRectTransform();
        m_infiniteScroll.Init();
        oldSelectId = selectId;

        m_searchCardCountText.text = "検索結果枚数：" + searchCardCount;
    }

    public bool CheckCardData(CardData cardData)
    {
        if (cardData == null)
        {
            return false;
        }

        if (m_elementDropdown.value > 0)
        {
            if (!cardData.Element.Contains(ConstManager.CardElementType[m_elementDropdown.value]))
            {
                return false;
            }
        }

        if (m_cardTypeDropdown.value > 0)
        {
            if (!cardData.CardType.Contains(m_cardTypeDropdown.options[m_cardTypeDropdown.value].text))
            {
                return false;
            }
        }

        if (m_categoryDropdown.value > 0)
        {
            if (!cardData.CardCategory.Contains(m_categoryDropdown.options[m_categoryDropdown.value].text))
            {
                return false;
            }
        }

        if (!string.IsNullOrEmpty(m_cardNameInputField.text))
        {
            if (string.IsNullOrEmpty(cardData.CardName))
            {
                return false;
            }
            if (string.IsNullOrEmpty(cardData.Text))
            {
                return false;
            }

            if (!cardData.CardName.Contains(m_cardNameInputField.text)
                && !cardData.Text.Contains(m_cardNameInputField.text))
            {
                return false;
            }
        }

        return true;
    }
}
