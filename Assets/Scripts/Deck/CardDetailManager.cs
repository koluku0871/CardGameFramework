using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Directory = System.IO.Directory;
using File = System.IO.File;
using Image = UnityEngine.UI.Image;
using Text = UnityEngine.UI.Text;

public class CardDetailManager : MonoBehaviour
{
    [SerializeField]
    private Image m_targetDetail = null;

    [SerializeField]
    private Text m_cost = null;

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

    [SerializeField]
    private CardDataBase cardDataBase;

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
        var directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_CARD;
        string tag = cardDetail.tag;
        string fileName = cardDetail.cardId;
        if (isAwake) fileName = fileName + "_b";
        return GetCardData(directoryPath + tag + "/" + fileName).sprite;
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

        var isAwake = File.Exists(ConstManager.DIRECTORY_FULL_PATH_TO_CARD + tag + "/" + cardId + "_b.jpg");
        m_awake.gameObject.SetActive(isAwake);
        if (isAwake)
        {
            m_awake.image.sprite = GetCardSprite(new DeckManager.CardDetail() { tag = tag, cardId = cardId }, isAwake);
            m_awake.onClick.AddListener(() => {
                SetSprite(m_awake.image.sprite);
                SetCardDetail(tag, cardId + "_b");
            });
        }

        var directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_CARD + tag + "/" + cardId;
        CardData data = GetCardData(directoryPath);

        if (!string.IsNullOrEmpty(data.cost))
        {
            m_cost.text = data.cost;
            m_cost.gameObject.SetActive(true);
        }
        if (!string.IsNullOrEmpty(data.cardCategory))
        {
            m_cardCategory.text = data.cardCategory;
            m_cardCategory.gameObject.SetActive(true);
        }
        if (!string.IsNullOrEmpty(data.cardType))
        {
            m_cardType.text = data.cardType;
            m_cardType.gameObject.SetActive(true);
        }
        if (!string.IsNullOrEmpty(data.cardName))
        {
            m_cardName.text = data.cardName;
            m_cardName.gameObject.SetActive(true);
        }

        List<LevelClass> levelClassList = new List<LevelClass>();
        for (int i = 0; i < data.levelTextList.Count; i++)
        {
            string level = data.levelTextList[i];
            LevelClass levelClass = new LevelClass();

            string[] textStrSplit = level.Split(':');
            levelClass.level = textStrSplit[0].Replace("LV", "");

            if (i==0 && levelClass.level != "1")
            {
                if (data.cardCategory.Contains("ネクサス"))
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
        if(data.cardCategory.Contains("ネクサス"))
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

        m_text.text = data.text;
        m_text.gameObject.SetActive(true);
    }

    public List<CardDataList> GetCardDataList(string indexStr)
    {
        List<CardDataList> cardDataListTmp = new List<CardDataList>();
        switch (indexStr)
        {
            case "SD":
                cardDataListTmp = cardDataBase.cardDatasToSD;
                break;
            case "CB":
                cardDataListTmp = cardDataBase.cardDatasToCB;
                break;
            case "BSC":
                cardDataListTmp = cardDataBase.cardDatasToBSC;
                break;
            case "BS":
                cardDataListTmp = cardDataBase.cardDatasToBS;
                break;
            case "P":
                cardDataListTmp = cardDataBase.cardDatas;
                break;
        }
        return cardDataListTmp;
    }

    public CardData GetCardData(string directoryPath)
    {
        var directoryList = directoryPath.Split("/");
        var fileName = directoryList[directoryList.Length - 1];
        var packNo = fileName.Split("-")[0];

        List<CardDataList> cardDataListTmp = new List<CardDataList>();
        bool isNormalPack = true;
        if (cardDataBase != null)
        {
            if (packNo.Contains("SD"))
            {
                cardDataListTmp = cardDataBase.cardDatasToSD;
            }
            else if (packNo.Contains("CB"))
            {
                cardDataListTmp = cardDataBase.cardDatasToCB;
            }
            else if (packNo.Contains("BSC"))
            {
                cardDataListTmp = cardDataBase.cardDatasToBSC;
            }
            else if (packNo.Contains("BS"))
            {
                cardDataListTmp = cardDataBase.cardDatasToBS;
            }
            else
            {
                cardDataListTmp = cardDataBase.cardDatas;
                if (!packNo.Contains("LM") && !packNo.Contains("EX") && !packNo.Contains("CP")
                    && !packNo.Contains("SJ") && !packNo.Contains("PX") && !packNo.Contains("PC") && !packNo.Contains("PB") && !packNo.Contains("KF"))
                {
                    isNormalPack = false;
                }

                if (packNo.Contains("CP"))
                {
                    packNo = "CP";
                }
                else if (packNo.Contains("EX"))
                {
                    packNo = "EX";
                }
                else if (packNo.Contains("LM"))
                {
                    packNo = "LM";
                }
                else if (packNo.Contains("KA"))
                {
                    packNo = "P";
                }
                else if (packNo.Contains("PB"))
                {
                    packNo = "PB";
                }
                else if (packNo.Contains("PC"))
                {
                    packNo = "PC";
                }
                else if (packNo.Contains("PX"))
                {
                    packNo = "PX";
                }
                else if (packNo.Contains("SJ"))
                {
                    packNo = "SJ";
                }
                else if (packNo.Contains("X"))
                {
                    packNo = "X";
                }
                else if (packNo == fileName && packNo.Contains("P"))
                {
                    packNo = "P";
                }
            }
        }
        foreach (var cardDatas in cardDataListTmp)
        {
            if (isNormalPack)
            {
                if (cardDatas.name != packNo)
                {
                    continue;
                }
            }

            foreach (var cardData in cardDatas.cardDatas)
            {
                if (cardData == null)
                {
                    continue;
                }

                if (cardData.name != fileName)
                {
                    continue;
                }

                if (cardData.directoryPath == directoryPath)
                {
                    return cardData;
                }
            }
        }

        CardData data = ScriptableObject.CreateInstance<CardData>();
        data.directoryPath = directoryPath;
        data.fileName = fileName;
        
        //data.bytes = System.IO.File.ReadAllBytes(directoryPath + ".jpg");

        data.sprite = Resources.Load<Sprite>("DB/Img/" + packNo + "/" + fileName);

        StreamReader streamReader = new StreamReader(directoryPath + ".txt");
        string fullText = "";
        while (streamReader.Peek() > -1)
        {
            fullText += streamReader.ReadLine().Replace("\"", "").Replace("\'", "").Replace("{", "").Replace("}", "").Replace("\n", " ").Replace("\t", " ").Replace("\r", " ").Replace(" ", "");
        }

        string[] textList = fullText.Split(',');
        bool isReducedCost = false;
        foreach (string text in textList)
        {
            if (text.IndexOf("Text:") != -1)
            {
                string textStr = text.Replace("Text:", "");
                textStr = textStr.Replace("<pclass=txtcardtext>", "");
                textStr = textStr.Replace("<br/>", "");
                textStr = textStr.Replace("</p>", "");

                /*if (textStr.LastIndexOf("［バースト：") != -1)
                {
                    var line = textStr.Substring(0, textStr.LastIndexOf("］"));
                    var start = line.LastIndexOf("［バースト：");
                    var end = line.LastIndexOf("］");
                    var count = end - start + 1;
                    var str = line.Substring(start, count);
                    if (string.IsNullOrEmpty(str))
                    {
                        Debug.Log(textStr);
                    }
                    else
                    {
                        textStr = textStr.Replace(str, "<mark=#ff000055>" + str + "</mark>");
                    }
                }*/

                textStr = textStr.Replace("《神託》", "<sprite=\"CoreCharge\" index=0>");
                textStr = textStr.Replace("煌臨：", "<sprite=\"Advent\" index=0>");
                textStr = textStr.Replace("ミラージュ：", "<sprite=\"Mirage\" index=0>");
                textStr = textStr.Replace("ミラージュ", "<sprite=\"Mirage\" index=0>");
                textStr = textStr.Replace("【Uトリガー】", "<sprite=\"UltimateTrigger\" index=0>");
                textStr = textStr.Replace("Uトリガー", "<sprite=\"UltimateTrigger\" index=0>");
                textStr = textStr.Replace("【Ｕトリガー】", "<sprite=\"UltimateTrigger\" index=0>");
                textStr = textStr.Replace("Ｕトリガー", "<sprite=\"UltimateTrigger\" index=0>");
                textStr = textStr.Replace("転醒", "<sprite=\"Awakening\" index=0>");
                textStr = textStr.Replace("<imgsrc=../image/card_serch/icon_red.gif/>", "<sprite=\"SymbolRed\" index=0>");
                textStr = textStr.Replace("<imgsrc=../image/card_serch/icon_pup.gif/>", "<sprite=\"SymbolPurple\" index=0>");
                textStr = textStr.Replace("<imgsrc=../image/card_serch/icon_green.gif/>", "<sprite=\"SymbolGreen\" index=0>");
                textStr = textStr.Replace("<imgsrc=../image/card_serch/icon_white.gif/>", "<sprite=\"SymbolWhite\" index=0>");
                textStr = textStr.Replace("<imgsrc=../image/card_serch/icon_yellow.gif/>", "<sprite=\"SymbolYellow\" index=0>");
                textStr = textStr.Replace("<imgsrc=../image/card_serch/icon_blue.gif/>", "<sprite=\"SymbolBlue\" index=0>");
                textStr = textStr.Replace("<imgsrc=../image/card_serch/icon_god.gif/>", "<sprite=\"SymbolGod\" index=0>");
                textStr = textStr.Replace("<imgsrc=../image/card_serch/icon_soulcore.gif/>", "<sprite=\"SoulCore\" index=0>");

                if (textStr.IndexOf("\n") == 0)
                {
                    textStr = textStr.Substring(1, textStr.Length - 2);
                }

                data.text = textStr;
                continue;
            }

            if (text.IndexOf("Lv:") != -1 || text.IndexOf("LV") != -1)
            {
                string textStr = text.Replace("Lv:[", "").Replace("]", "");
                if (string.IsNullOrEmpty(textStr))
                {
                    continue;
                }
                data.levelTextList.Add(textStr);
            }

            if (text.IndexOf("CardType:") != -1)
            {
                data.cardType = text.Replace("CardType:", "");
                continue;
            }

            if (text.IndexOf("ReducedCost:") != -1)
            {
                isReducedCost = true;
                if (text.IndexOf("[]") != -1)
                {
                    isReducedCost = false;
                    data.reducedCost = "";
                }
                else if(text.IndexOf("]") != -1)
                {
                    isReducedCost = false;
                    data.reducedCost += text.Replace("ReducedCost:[", "").Replace("]", "");
                }
                else
                {
                    data.reducedCost += text.Replace("ReducedCost:[", "");
                }
                continue;
            }
            if (isReducedCost)
            {
                if (text.IndexOf("]") != -1)
                {
                    isReducedCost = false;
                }
                data.reducedCost += text.Replace("]", "");
                continue;
            }

            if (text.IndexOf("Cost:") != -1)
            {
                data.cost = text.Replace("Cost:", "");
                continue;
            }

            if (text.IndexOf("Element:") != -1)
            {
                data.element = text.Replace("Element:", "").Replace("・", "");
                continue;
            }

            if (text.IndexOf("CardCategory:") != -1)
            {
                data.cardCategory = text.Replace("CardCategory:", "");
                continue;
            }

            if (text.IndexOf("CardRarity") != -1)
            {
                data.cardRarity = text.Replace("CardRarity:", "");
                continue;
            }

            if (text.IndexOf("CardName:") != -1)
            {
                data.cardName = text.Replace("CardName:", "");
                continue;
            }

            if (text.IndexOf("PackNo:") != -1)
            {
                data.packNo = text.Replace("PackNo:", "");
                continue;
            }

            if (text.IndexOf("CardNo:") != -1)
            {
                data.cardNo = text.Replace("CardNo:", "");
                continue;
            }
        }

#if UNITY_EDITOR
        var path = "Assets/Resources/DB";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
            
        var pathToCard = path + "/Card";
        if (!Directory.Exists(pathToCard))
        {
            Directory.CreateDirectory(pathToCard);
        }
            
        if (!File.Exists(pathToCard + "/" + fileName + ".asset"))
        {
            AssetDatabase.CreateAsset(data, Path.Combine(pathToCard, fileName + ".asset"));
            AssetDatabase.SaveAssets();
        }

        if (!Directory.Exists(path + "/CardDataList"))
        {
            Directory.CreateDirectory(path + "/CardDataList");
        }

        CardDataList cardDataList = null;
        if (!File.Exists(path + "/CardDataList/" + data.packNo + ".asset"))
        {
            cardDataList = ScriptableObject.CreateInstance<CardDataList>();
            AssetDatabase.CreateAsset(cardDataList, Path.Combine(path + "/CardDataList", data.packNo + ".asset"));

            if (!cardDataListTmp.Contains(cardDataList))
            {
                cardDataListTmp.Add(cardDataList);
                EditorUtility.SetDirty(cardDataBase);
            }

            AssetDatabase.SaveAssets();
        }
        else
        {
            cardDataList = AssetDatabase.LoadAssetAtPath<CardDataList>(Path.Combine(path + "/CardDataList", data.packNo + ".asset"));
        }

        if (!cardDataList.cardDatas.Contains(data))
        {
            cardDataList.cardDatas.Add(data);
            EditorUtility.SetDirty(cardDataList);
            AssetDatabase.SaveAssets();
        }
#endif
        return data;
    }
}
