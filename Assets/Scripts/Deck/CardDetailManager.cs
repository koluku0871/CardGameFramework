using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using Text = UnityEngine.UI.Text;

public class CardDetailManager : MonoBehaviour
{
    [SerializeField]
    private Image m_targetDetail = null;

    [SerializeField]
    private Text m_cost = null;

    [SerializeField]
    private Text m_cardNo = null;

    [SerializeField]
    private Button m_awake = null;

    [SerializeField]
    private Text m_cardCategory = null;

    [SerializeField]
    private Text m_cardType = null;

    [SerializeField]
    private Text m_cardName = null;

    [SerializeField]
    private RectTransform m_levelContent = null;

    [SerializeField]
    private RectTransform m_levelFrame = null;

    [SerializeField]
    private Text m_coreText = null;

    [SerializeField]
    private Text m_levelText = null;

    [SerializeField]
    private Text m_bpText = null;

    [SerializeField]
    private TextMeshProUGUI m_text = null;

    [SerializeField]
    private Transform m_symbolList = null;

    [SerializeField]
    private Image m_symbol = null;

    [SerializeField]
    private Sprite m_sleeveSprite = null;

    [SerializeField]
    private List<Sprite> m_symbolSpriteList = new List<Sprite>();

    public bool isLock = false;

    private static CardDetailManager instance = null;
    public static CardDetailManager Instance() {
        return instance;
    }

    private void Awake() {
        instance = this;
        isLock = false;
        Init();
    }

    private void Init() {
        m_cost.text = "";
        m_cost.gameObject.SetActive(false);
        m_cardNo.text = "";
        m_cardNo.gameObject.SetActive(false);
        m_cardCategory.text = "";
        m_cardCategory.gameObject.SetActive(false);
        m_cardType.text = "";
        m_cardType.gameObject.SetActive(false);
        m_cardName.text = "";
        m_cardName.gameObject.SetActive(false);
        foreach ( Transform c in m_levelContent.transform ) {
            if (c == m_levelFrame.transform) {
                continue;
            }
            Destroy(c.gameObject);
        }
        m_text.text = "";
        m_text.gameObject.SetActive(false);
        foreach (Transform c in m_symbolList.transform)
        {
            if (c == m_symbol.transform)
            {
                continue;
            }
            Destroy(c.gameObject);
        }
    }

    public void SetSprite(Sprite sprite)
    {
        if (m_targetDetail == null)
        {
            return;
        }

        m_targetDetail.sprite = sprite;
    }

    public Sprite GetCardSprite(DeckManager.CardDetail cardDetail, bool isAwake = false)
    {
        string key = cardDetail.tag;
        string fileName = cardDetail.cardId;
        if (isAwake) fileName = fileName + "_b";
        return AssetBundleManager.Instance().GetBaseData(key, fileName).Sprite;
    }

    public Sprite GetSleeveSprite()
    {
        return m_sleeveSprite;
    }

    public Sprite GetSymbolSprite(string symbolStr)
    {
        switch(symbolStr)
        {
            case "赤":
                return m_symbolSpriteList[0];
            case "紫":
                return m_symbolSpriteList[1];
            case "緑":
                return m_symbolSpriteList[2];
            case "白":
                return m_symbolSpriteList[3];
            case "黄":
                return m_symbolSpriteList[4];
            case "青":
                return m_symbolSpriteList[5];
            case "究":
                return m_symbolSpriteList[6];
            case "神":
                return m_symbolSpriteList[7];
        }

        return null;
    }

    private class LevelClass
    {
        public string level;
        public string bp;
        public string core;
    }
    public void SetCardDetail(string tag, string cardId) {
        Init();

        Debug.Log("tag : " + tag);

        var isAwake = AssetBundleManager.Instance().ExistsBaseData(tag, cardId + "_b");
        m_awake.gameObject.SetActive(isAwake);
        if (isAwake)
        {
            m_awake.image.sprite = GetCardSprite(new DeckManager.CardDetail() { tag = tag, cardId = cardId }, isAwake);
            m_awake.onClick.AddListener(() => {
                SetSprite(m_awake.image.sprite);
                SetCardDetail(tag, cardId + "_b");
            });
        }

        CardData data = AssetBundleManager.Instance().GetBaseData(tag, cardId);

        if (!string.IsNullOrEmpty(data.Cost))
        {
            m_cost.text = data.Cost;
            m_cost.gameObject.SetActive(true);
        }
        if (!string.IsNullOrEmpty(data.CardNo))
        {
            m_cardNo.text = data.CardNo;
            m_cardNo.gameObject.SetActive(true);
        }
        if (!string.IsNullOrEmpty(data.CardCategory))
        {
            m_cardCategory.text = data.CardCategory;
            m_cardCategory.gameObject.SetActive(true);
        }
        if (!string.IsNullOrEmpty(data.CardType))
        {
            m_cardType.text = data.CardType;
            m_cardType.gameObject.SetActive(true);
        }
        if (!string.IsNullOrEmpty(data.CardName))
        {
            m_cardName.text = data.CardName;
            m_cardName.gameObject.SetActive(true);
        }

        List<LevelClass> levelClassList = new List<LevelClass>();
        for (int i = 0; i < data.Lv.Count; i++)
        {
            string level = data.Lv[i];
            LevelClass levelClass = new LevelClass();

            string[] textStrSplit = level.Split(':');
            levelClass.level = textStrSplit[0].Replace("LV", "");

            if (i==0 && levelClass.level != "1")
            {
                if (data.CardCategory.Contains("ネクサス") && !data.CardType.Contains("創界"))
                {
                    levelClassList.Add(new LevelClass()
                    {
                        level = "1",
                        bp = "",
                        core = "0",
                    });
                }
            }

            textStrSplit = textStrSplit[1].Split('／');
            levelClass.bp = textStrSplit[0];
            levelClass.core = textStrSplit[1];

            levelClassList.Add(levelClass);
        }
        if(data.CardCategory.Contains("ネクサス"))
        {
            if (levelClassList.Count < 1)
            {
                levelClassList.Add(new LevelClass()
                {
                    level = "1",
                    bp = "",
                    core = "0",
                });
            }
        }

        foreach (LevelClass levelClass in levelClassList)
        {
            m_levelText.text = levelClass.level;
            m_bpText.text = levelClass.bp;
            m_coreText.text = levelClass.core;

            RectTransform copied = UnityEngine.Object.Instantiate(m_levelFrame);
            copied.SetParent(m_levelContent);
            copied.transform.localScale = Vector3.one;
            copied.gameObject.SetActive(true);
        }

        m_text.text = data.Text;
        m_text.gameObject.SetActive(true);
    }
}
