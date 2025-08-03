using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

    [Header("カードフィルター")]

    [SerializeField]
    private Dropdown m_elementDropdown = null;

    [SerializeField]
    private Toggle m_elementToggle = null;

    [SerializeField]
    private Dropdown m_categoryDropdown = null;

    [SerializeField]
    private Toggle m_categoryToggle = null;

    [SerializeField]
    private Dropdown m_cardTypeDropdown = null;

    [SerializeField]
    private Toggle m_typeToggle = null;

    [SerializeField]
    private Dropdown m_cardSubTypeDropdown = null;

    [SerializeField]
    private Toggle m_subTypeToggle = null;

    [SerializeField]
    private Dropdown m_cardCostDropdown = null;

    [SerializeField]
    private List<Toggle> m_costToggleList = new List<Toggle>();

    [SerializeField]
    private Dropdown m_cardRarityDropdown = null;

    [SerializeField]
    private TMP_InputField m_cardNameInputField = null;

    [Header("カードソート")]

    [SerializeField]
    private Dropdown m_sortDropdown = null;

    [SerializeField]
    private Toggle m_sortToggle = null;

    [Header("カード詳細")]

    [SerializeField]
    private Image m_targetCardDetail = null;

    [Header("デッキ情報")]

    [SerializeField]
    private Text m_deckCardCountText = null;

    [SerializeField]
    private RectTransform m_deckContent = null;

    [SerializeField]
    private Text m_subDeckCardCountText = null;

    [SerializeField]
    private RectTransform m_subDeckContent = null;

    [SerializeField]
    private Text m_tokenDeckCardCountText = null;

    [SerializeField]
    private RectTransform m_tokenDeckContent = null;

    [Header("検索結果")]

    [SerializeField]
    private Text m_searchCardCountText = null;

    [SerializeField]
    private InfiniteScroll m_searchInfiniteScroll = null;

    [SerializeField]
    private DeckCardScrollSetup m_searchCardScrollSetup = null;

    [Header("お気に入り")]

    [SerializeField]
    private Text m_favoriteCardCountText = null;

    [SerializeField]
    private InfiniteScroll m_favoriteInfiniteScroll = null;

    [SerializeField]
    private DeckCardScrollSetup m_favoriteCardScrollSetup = null;

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
    public List<CardData> GetCardDatas()
    {
        return cardDatas;
    }
    private int oldSelectId = -1;

    private List<CardData> favoriteCardDatas = new List<CardData>();
    public CardData GetFavoriteCardDatas(int index)
    {
        if (index < 0 || favoriteCardDatas.Count - 1 < index)
        {
            return null;
        }
        return favoriteCardDatas[index];
    }

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

        var directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_OPTION;
        string[] optFiles = Directory.GetFiles(directoryPath, "favoriteCardData.json", SearchOption.AllDirectories);
        StreamReader sr = new StreamReader(optFiles[0], Encoding.UTF8);
        FavoriteData favoriteData = JsonUtility.FromJson<FavoriteData>(sr.ReadToEnd());
        sr.Close();

        favoriteCardDatas.Clear();
        foreach (var cardData in favoriteData.cardDetails)
        {
            favoriteCardDatas.Add(AssetBundleManager.Instance().GetBaseData(cardData.tag, cardData.cardId));
        }
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

    public RectTransform GetActiveDeckContent()
    {
        if (m_deckContent.gameObject.activeInHierarchy)
        {
            return GetDeckContent();
        }
        else if (m_subDeckContent.gameObject.activeInHierarchy)
        {
            return GetSubDeckContent();
        }
        else if (m_tokenDeckContent.gameObject.activeInHierarchy)
        {
            return GetTokenDeckContent();
        }

        return null;
    }

    public RectTransform GetDeckContent()
    {
        return m_deckContent;
    }

    public RectTransform GetSubDeckContent()
    {
        return m_subDeckContent;
    }

    public RectTransform GetTokenDeckContent()
    {
        return m_tokenDeckContent;
    }

    public Text GetDeckCardCountText()
    {
        return m_deckCardCountText;
    }

    public Text GetSubDeckCardCountText()
    {
        return m_subDeckCardCountText;
    }

    public Text GetTokenDeckCardCountText()
    {
        return m_tokenDeckCardCountText;
    }

    public Text GetSearchCardCountText()
    {
        return m_searchCardCountText;
    }

    public void OnClickToCloseButton()
    {
        FavoriteData favoriteData = new FavoriteData();
        if (favoriteData.cardDetails == null)
        {
            favoriteData.cardDetails = new List<DeckManager.CardDetail>();
        }

        if (favoriteCardDatas != null)
        {
            foreach (var cardData in favoriteCardDatas)
            {
                if (cardData == null)
                {
                    continue;
                }
                if (string.IsNullOrEmpty(cardData.PackNo))
                {
                    continue;
                }
                if (string.IsNullOrEmpty(cardData.CardNo))
                {
                    continue;
                }
                favoriteData.cardDetails.Add(new DeckManager.CardDetail() { tag = cardData.PackNo, cardId = cardData.CardNo });
            }
        }
        
        StreamWriter streamWriter = new StreamWriter(ConstManager.DIRECTORY_FULL_PATH_TO_OPTION + "favoriteCardData.json");
        streamWriter.WriteLine(JsonUtility.ToJson(favoriteData));
        streamWriter.Close();

        FadeManager.Instance().OnStart("HomeScene");
    }

    public void DownLoadCallback()
    {
        m_packTypeDropdown.ClearOptions();
        m_packTypeDropdown.options.Add(new Dropdown.OptionData()
        {
            text = "no select"
        });
        foreach (var packData in AssetBundleManager.Instance().AssetBundleBaseCardData.baseData.packNameList)
        {
            m_packTypeDropdown.options.Add(new Dropdown.OptionData()
            {
                text = packData.Key
            });
        }
        //m_packTypeDropdown.value = m_packTypeDropdown.options.Count - 1;
        m_packTypeDropdown.value = -1;

        m_elementDropdown.ClearOptions();
        m_elementDropdown.options.Add(new Dropdown.OptionData()
        {
            text = "All"
        });
        m_elementDropdown.value = -1;

        m_cardTypeDropdown.ClearOptions();
        m_cardTypeDropdown.options.Add(new Dropdown.OptionData()
        {
            text = "All"
        });
        m_cardTypeDropdown.value = -1;

        m_cardSubTypeDropdown.ClearOptions();
        m_cardSubTypeDropdown.options.Add(new Dropdown.OptionData()
        {
            text = "All"
        });
        m_cardSubTypeDropdown.value = -1;

        m_categoryDropdown.ClearOptions();
        m_categoryDropdown.options.Add(new Dropdown.OptionData()
        {
            text = "All"
        });
        m_categoryDropdown.value = -1;

        m_cardCostDropdown.ClearOptions();
        m_cardCostDropdown.options.Add(new Dropdown.OptionData()
        {
            text = "All"
        });
        m_cardCostDropdown.value = -1;

        m_cardRarityDropdown.ClearOptions();
        m_cardRarityDropdown.options.Add(new Dropdown.OptionData()
        {
            text = "All"
        });
        m_cardRarityDropdown.value = -1;

        OnValueChangedToPackTypeDropdown();
    }

    public (string, List<string>) GetPackNameList()
    {
        var key = m_packTypeDropdown.options[m_packTypeDropdown.value].text;
        AssetBundleManager assetBundleManager = AssetBundleManager.Instance();

        List<string> packNameList = new List<string>();
        if (!assetBundleManager.AssetBundleBaseCardData.baseData.packNameList.ContainsKey(key))
        {
            foreach (var packName in assetBundleManager.AssetBundleBaseCardData.baseData.packNameList)
            {
                packNameList.AddRange(packName.Value);
            }
        }
        else
        {
            packNameList = assetBundleManager.AssetBundleBaseCardData.baseData.packNameList[key];
        }
        return (key, packNameList);
    }

    public Dictionary<string, List<string>> GetPackNameListFromDictionary()
    {
        Dictionary<string, List<string>> packNameList = new Dictionary<string, List<string>>();

        var key = m_packTypeDropdown.options[m_packTypeDropdown.value].text;
        AssetBundleManager assetBundleManager = AssetBundleManager.Instance();
        if (key == "All" || !assetBundleManager.AssetBundleBaseCardData.baseData.packNameList.ContainsKey(key))
        {
            foreach (var list in assetBundleManager.AssetBundleBaseCardData.baseData.packNameList)
            {
                if (list.Key == "All")
                {
                    continue;
                }
                packNameList.Add(list.Key, list.Value);
            }
        }
        else
        {
            packNameList.Add(key, assetBundleManager.AssetBundleBaseCardData.baseData.packNameList[key]);
        }
        return packNameList;
    }

    public void OnValueChangedToPackTypeDropdown()
    {
        if (!m_searchInfiniteScroll.gameObject.activeInHierarchy)
        {
            return;
        }

        cardDatas.Clear();

        var selectId = m_packTypeDropdown.value - 1;
        if (0 > selectId)
        {
            return;
        }

        (var key, var packNameList) = GetPackNameList();
        if (packNameList == null)
        {
            return;
        }

        CardDetailManager cardDetailManager = CardDetailManager.Instance();

        int searchCardCount = 0;
        SortedSet<string> elementList = new SortedSet<string>();
        SortedSet<string> cardTypeList = new SortedSet<string>();
        SortedSet<string> cardSubTypeList = new SortedSet<string>();
        SortedSet<string> categoryList = new SortedSet<string>();
        SortedSet<string> costList = new SortedSet<string>();
        SortedSet<string> rarityList = new SortedSet<string>();

        if (oldSelectId != selectId)
        {
            m_elementDropdown.value = 0;
            m_cardTypeDropdown.value = 0;
            m_cardSubTypeDropdown.value = 0;
            m_categoryDropdown.value = 0;
            m_cardCostDropdown.value = 0;
            m_cardRarityDropdown.value = 0;
            m_cardNameInputField.text = "";

            m_sortDropdown.ClearOptions();
            m_sortDropdown.options.Add(new Dropdown.OptionData()
            {
                text = "default"
            });
            foreach (CardData.ConstParam Value in Enum.GetValues(typeof(CardData.ConstParam)))
            {
                m_sortDropdown.options.Add(new Dropdown.OptionData()
                {
                    text = Enum.GetName(typeof(CardData.ConstParam), Value)
                });
            }
            m_sortDropdown.value = -1;
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

            CardData cardData = AssetBundleManager.Instance().GetBaseData(targetTag, name);
            if (cardData == null)
            {
                continue;
            }

            if (oldSelectId != selectId)
            {
                if (!string.IsNullOrEmpty(cardData.Element))
                {
                    elementList.Add(cardData.Element.Replace("\r", "").Replace("\n", ""));
                }

                if (!string.IsNullOrEmpty(cardData.CardType))
                {
                    var cardTypeSplit = cardData.CardType.Split("・");
                    if (cardTypeSplit.Length > 1)
                    {
                        foreach (var item in cardTypeSplit)
                        {
                            cardTypeList.Add(item);
                        }
                    }
                    else
                    {
                        cardTypeSplit = cardData.CardType.Split("／");
                        if (cardTypeSplit.Length > 1)
                        {
                            foreach (var item in cardTypeSplit)
                            {
                                cardTypeList.Add(item);
                            }
                        }
                        else
                        {
                            cardTypeSplit = cardData.CardType.Split("/");
                            if (cardTypeSplit.Length > 1)
                            {
                                foreach (var item in cardTypeSplit)
                                {
                                    cardTypeList.Add(item);
                                }
                            }
                            else
                            {
                                cardTypeList.Add(cardData.CardType);
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(cardData.CardSubType))
                {
                    var cardTypeSplit = cardData.CardSubType.Split("／");
                    if (cardTypeSplit.Length > 1)
                    {
                        foreach (var item in cardTypeSplit)
                        {
                            cardSubTypeList.Add(item);
                        }
                    }
                    else
                    {
                        cardTypeSplit = cardData.CardSubType.Split("/");
                        if (cardTypeSplit.Length > 1)
                        {
                            foreach (var item in cardTypeSplit)
                            {
                                cardSubTypeList.Add(item);
                            }
                        }
                        else
                        {
                            cardSubTypeList.Add(cardData.CardSubType);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(cardData.CardCategory))
                {
                    var split = cardData.CardCategory.Split("／");
                    if (split.Length > 1)
                    {
                        foreach (var item in split)
                        {
                            categoryList.Add(item);
                        }
                    }
                    else
                    {
                        split = cardData.CardCategory.Split("/");
                        if (split.Length > 1)
                        {
                            foreach (var item in split)
                            {
                                categoryList.Add(item);
                            }
                        }
                        else
                        {
                            categoryList.Add(cardData.CardCategory);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(cardData.Cost))
                {
                    costList.Add(cardData.Cost);
                }

                if (!string.IsNullOrEmpty(cardData.CardRarity))
                {
                    rarityList.Add(cardData.CardRarity);
                }
            }

            if (!CheckCardData(cardData))
            {
                continue;
            }
            cardDatas.Add(cardData);
        }
        if (oldSelectId != selectId && AssetBundleManager.Instance().CardType == "yugioh")
        {
            categoryList.Add("融合＊シンクロ＊エクシーズ＊リンク");
            categoryList.Add("融合＊シンクロ＊エクシーズ");
            categoryList.Add("融合＊シンクロ＊リンク");
            categoryList.Add("融合＊エクシーズ＊リンク");
            categoryList.Add("シンクロ＊エクシーズ＊リンク");
            categoryList.Add("融合＊シンクロ");
            categoryList.Add("融合＊エクシーズ");
            categoryList.Add("融合＊リンク");
            categoryList.Add("シンクロ＊エクシーズ");
            categoryList.Add("シンクロ＊リンク");
            categoryList.Add("エクシーズ＊リンク");
        }

        SortToCardDataList((CardData.ConstParam)m_sortDropdown.value, m_sortToggle.isOn);

        searchCardCount = cardDatas.Count;

        if (elementList.Count > 0)
        {
            m_elementDropdown.ClearOptions();
            m_elementDropdown.options.Add(new Dropdown.OptionData()
            {
                text = "All"
            });
            foreach (var item in elementList)
            {
                m_elementDropdown.options.Add(new Dropdown.OptionData()
                {
                    text = item
                });
            }
            m_elementDropdown.value = -1;
        }

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

        if (cardSubTypeList.Count > 0)
        {
            m_cardSubTypeDropdown.ClearOptions();
            m_cardSubTypeDropdown.options.Add(new Dropdown.OptionData()
            {
                text = "All"
            });
            foreach (var item in cardSubTypeList)
            {
                m_cardSubTypeDropdown.options.Add(new Dropdown.OptionData()
                {
                    text = item
                });
            }
            m_cardSubTypeDropdown.value = -1;
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

        if (costList.Count > 0)
        {
            m_cardCostDropdown.ClearOptions();
            m_cardCostDropdown.options.Add(new Dropdown.OptionData()
            {
                text = "All"
            });
            foreach (var item in costList)
            {
                m_cardCostDropdown.options.Add(new Dropdown.OptionData()
                {
                    text = item
                });
            }
            m_cardCostDropdown.value = -1;
        }

        if (rarityList.Count > 0)
        {
            m_cardRarityDropdown.ClearOptions();
            m_cardRarityDropdown.options.Add(new Dropdown.OptionData()
            {
                text = "All"
            });
            foreach (var item in rarityList)
            {
                m_cardRarityDropdown.options.Add(new Dropdown.OptionData()
                {
                    text = item
                });
            }
            m_cardRarityDropdown.value = -1;
        }

        m_searchCardScrollSetup.max = (cardDatas.Count / 4);
        if (cardDatas.Count % 4 != 0)
        {
            m_searchCardScrollSetup.max++;
        }
        m_searchCardScrollSetup.SetRectTransform();
        m_searchInfiniteScroll.Init();
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
            if ((!cardData.Element.Replace("\r", "").Replace("\n", "").Contains(m_elementDropdown.options[m_elementDropdown.value].text)) != m_elementToggle.isOn)
            {
                return false;
            }
        }

        if (m_cardTypeDropdown.value > 0)
        {
            if ((!cardData.CardType.Contains(m_cardTypeDropdown.options[m_cardTypeDropdown.value].text) != m_typeToggle.isOn))
            {
                return false;
            }
        }

        if (m_cardSubTypeDropdown.value > 0)
        {
            if ((!cardData.CardSubType.Contains(m_cardSubTypeDropdown.options[m_cardSubTypeDropdown.value].text) != m_subTypeToggle.isOn))
            {
                return false;
            }
        }

        if (m_categoryDropdown.value > 0)
        {
            var category = m_categoryDropdown.options[m_categoryDropdown.value].text.Split("＊");
            if (category.Count() > 1)
            {
                bool isBreak = false;
                foreach (var c in category)
                {
                    if (cardData.CardCategory.Contains(c))
                    {
                        isBreak = true;
                        break;
                    }
                }
                if ((!isBreak) != m_categoryToggle.isOn)
                {
                    return false;
                }
            }
            else
            {
                if ((!cardData.CardCategory.Contains(category[0]) != m_categoryToggle.isOn))
                {
                    return false;
                }
            }
        }

        if (m_cardCostDropdown.value > 0)
        {
            int index = -1;
            for (var i = 0; i < m_costToggleList.Count; i++)
            {
                if (m_costToggleList[i].isOn)
                {
                    index = i;
                    break;
                }
            }
            string optionCostStr = m_cardCostDropdown.options[m_cardCostDropdown.value].text.Replace("\r", "").Replace("\n", "");
            int optionCost = 0;
            if (int.TryParse(optionCostStr, out optionCost) && int.TryParse(cardData.Cost.Replace("\r", "").Replace("\n", ""), out int cost))
            {
                switch (index)
                {
                    case 0: // 同等
                        if (optionCost != cost)
                        {
                            return false;
                        }
                        break;
                    case 1: // 以上
                        if (optionCost > cost)
                        {
                            return false;
                        }
                        break;
                    case 2: // 以下
                        if (optionCost < cost)
                        {
                            return false;
                        }
                        break;
                    default:
                        return false;
                }
            }
            else if (index == 0)
            {
                if (!cardData.Cost.Contains(optionCostStr))
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        if (m_cardRarityDropdown.value > 0)
        {
            if (!cardData.CardRarity.Contains(m_cardRarityDropdown.options[m_cardRarityDropdown.value].text))
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

    private void SortToCardDataList(CardData.ConstParam index, bool isAscending)
    {
        switch(index)
        {
            case CardData.ConstParam.CardNo:
                if (isAscending)
                {
                    cardDatas.Sort((x, y) => x.CardNo.CompareTo(y.CardNo));
                }
                else
                {
                    cardDatas.Sort((x, y) => y.CardNo.CompareTo(x.CardNo));
                }
                break;
            case CardData.ConstParam.PackNo:
                if (isAscending)
                {
                    cardDatas.Sort((x, y) => x.PackNo.CompareTo(y.PackNo));
                }
                else
                {
                    cardDatas.Sort((x, y) => y.PackNo.CompareTo(x.PackNo));
                }
                break;
            case CardData.ConstParam.CardName:
                if (isAscending)
                {
                    cardDatas.Sort((x, y) => x.CardName.CompareTo(y.CardName));
                }
                else
                {
                    cardDatas.Sort((x, y) => y.CardName.CompareTo(x.CardName));
                }
                break;
            case CardData.ConstParam.CardLevel:
                if (isAscending)
                {
                    cardDatas.Sort((x, y) => x.CardLevel.CompareTo(y.CardLevel));
                }
                else
                {
                    cardDatas.Sort((x, y) => y.CardLevel.CompareTo(x.CardLevel));
                }
                break;
            case CardData.ConstParam.CardRarity:
                if (isAscending)
                {
                    cardDatas.Sort((x, y) => x.CardRarity.CompareTo(y.CardRarity));
                }
                else
                {
                    cardDatas.Sort((x, y) => y.CardRarity.CompareTo(x.CardRarity));
                }
                break;
            case CardData.ConstParam.CardCategory:
                if (isAscending)
                {
                    cardDatas.Sort((x, y) => x.CardCategory.CompareTo(y.CardCategory));
                }
                else
                {
                    cardDatas.Sort((x, y) => y.CardCategory.CompareTo(x.CardCategory));
                }
                break;
            case CardData.ConstParam.Element:
                if (isAscending)
                {
                    cardDatas.Sort((x, y) => x.Element.CompareTo(y.Element));
                }
                else
                {
                    cardDatas.Sort((x, y) => y.Element.CompareTo(x.Element));
                }
                break;
            case CardData.ConstParam.Cost:
                if (isAscending)
                {
                    cardDatas.Sort((x, y) => x.Cost.CompareTo(y.Cost));
                }
                else
                {
                    cardDatas.Sort((x, y) => y.Cost.CompareTo(x.Cost));
                }
                break;
            case CardData.ConstParam.ReducedCost:
                if (isAscending)
                {
                    cardDatas.Sort((x, y) => x.ReducedCost.CompareTo(y.ReducedCost));
                }
                else
                {
                    cardDatas.Sort((x, y) => y.ReducedCost.CompareTo(x.ReducedCost));
                }
                break;
            case CardData.ConstParam.CardType:
                if (isAscending)
                {
                    cardDatas.Sort((x, y) => x.CardType.CompareTo(y.CardType));
                }
                else
                {
                    cardDatas.Sort((x, y) => y.CardType.CompareTo(x.CardType));
                }
                break;
            case CardData.ConstParam.CardSubType:
                if (isAscending)
                {
                    cardDatas.Sort((x, y) => x.CardSubType.CompareTo(y.CardSubType));
                }
                else
                {
                    cardDatas.Sort((x, y) => y.CardSubType.CompareTo(x.CardSubType));
                }
                break;
        }
    }

    public void AddFavoriteCardData(string tag, string cardId)
    {
        favoriteCardDatas.Add(AssetBundleManager.Instance().GetBaseData(tag, cardId));
    }

    public void RemoveFavoriteCardData(string tag, string cardId)
    {
        favoriteCardDatas.Remove(AssetBundleManager.Instance().GetBaseData(tag, cardId));
        UpdateFavoriteCardData(true);
    }

    public void UpdateFavoriteCardData(bool isActive)
    {
        if (!isActive)
        {
            return;
        }

        m_favoriteCardScrollSetup.max = (favoriteCardDatas.Count / 4);
        if (favoriteCardDatas.Count % 4 != 0)
        {
            m_favoriteCardScrollSetup.max++;
        }
        m_favoriteCardScrollSetup.SetRectTransform();
        m_favoriteInfiniteScroll.Init();

        m_favoriteCardCountText.text = "お気に入り枚数：" + favoriteCardDatas.Count;
    }
}
