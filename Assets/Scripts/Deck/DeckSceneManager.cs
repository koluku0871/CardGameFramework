using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckSceneManager : MonoBehaviour
{
    [SerializeField]
    private Button m_closeButton = null;

    [SerializeField]
    private Button m_downloadButton = null;

    [SerializeField]
    private Button m_createSharpnessButton = null;

    [SerializeField]
    private InputField m_downloadDeckInputField = null;

    [SerializeField]
    private Button m_downloadDeckButton = null;

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

    private CardDownLoadManager m_cardDownLoadManager = null;
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

        m_cardDownLoadManager = CardDownLoadManager.Instance();
        m_deckManager = DeckManager.Instance();

        DownLoadCallback();
        m_closeButton.interactable = true;
        m_downloadButton.interactable = true;
        m_createSharpnessButton.interactable = true;
        m_downloadDeckButton.interactable = true;
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
        foreach (var packData in m_cardDownLoadManager.m_packDataList)
        {
            m_packTypeDropdown.options.Add(new Dropdown.OptionData()
            {
                text = packData.tag
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
        if (0 > selectId || selectId >= m_cardDownLoadManager.m_packDataList.Count)
        {
            return;
        }

        CardDetailManager cardDetailManager = CardDetailManager.Instance();

        var tag = m_cardDownLoadManager.m_packDataList[selectId].tag;
        var directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_CARD + tag + "/";
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
        bool isAll = tag == "All";
#if !UNITY_EDITOR
        if (isAll)
        {
            foreach (var cardDataKey in new List<string>() { "BS", "BSC", "CB", "SD", "P", })
            {
                foreach (var cardDataList in cardDetailManager.GetCardDataList(cardDataKey))
                {
                    foreach (var cardData in cardDataList.cardDatas.FindAll(CheckCardData))
                    {
                        if (oldSelectId != selectId)
                        {
                            foreach (var item in cardData.cardType.Split("・"))
                            {
                                cardTypeList.Add(item);
                            }
                            categoryList.Add(cardData.cardCategory);
                        }
                        cardDatas.Add(cardData);
                    }
                }
            }
        }
        else
#endif
        {
            var fileNameList = m_cardDownLoadManager.m_packDataList[selectId].fileNameList;
            for (int i = 0; i < fileNameList.Count; i++)
            {
                string fileName = fileNameList[i];
                string name = fileName;
                string targetTag = tag;
                if (isAll)
                {
                    targetTag = fileName.Split("*")[0];
                    name = fileName.Split("*")[1];
                    directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_CARD + targetTag + "/";
                }

                CardData cardData = cardDetailManager.GetCardData(directoryPath + name);
                if (cardData == null)
                {
                    continue;
                }

                if (oldSelectId != selectId)
                {
                    foreach (var item in cardData.cardType.Split("・"))
                    {
                        cardTypeList.Add(item);
                    }
                    categoryList.Add(cardData.cardCategory);
                }

                if (!CheckCardData(cardData))
                {
                    continue;
                }
                cardDatas.Add(cardData);
            }
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
            if (!cardData.element.Contains(ConstManager.CardElementType[m_elementDropdown.value]))
            {
                return false;
            }
        }

        if (m_cardTypeDropdown.value > 0)
        {
            if (!cardData.cardType.Contains(m_cardTypeDropdown.options[m_cardTypeDropdown.value].text))
            {
                return false;
            }
        }

        if (m_categoryDropdown.value > 0)
        {
            if (!cardData.cardCategory.Contains(m_categoryDropdown.options[m_categoryDropdown.value].text))
            {
                return false;
            }
        }

        if (!string.IsNullOrEmpty(m_cardNameInputField.text))
        {
            if (string.IsNullOrEmpty(cardData.cardName))
            {
                return false;
            }
            if (string.IsNullOrEmpty(cardData.text))
            {
                return false;
            }

            if (!cardData.cardName.Contains(m_cardNameInputField.text)
                && !cardData.text.Contains(m_cardNameInputField.text))
            {
                return false;
            }
        }

        return true;
    }
}
