using System.Collections.Generic;
using UnityEngine;

public class BaseData
{
    public Dictionary<string, List<CardData>> packDatas = new Dictionary<string, List<CardData>> ();
    public Dictionary<string, List<string>> packNameList = new Dictionary<string, List<string>>();
}

public class CardData
{
    public string assetBundleSpriteName;
    public string fileName;

    private Sprite sprite = null;

    public Sprite Sprite
    {
        get
        {
            if (sprite != null)
            {
                return sprite;
            }

            sprite = AssetBundleManager.Instance().GetCardSprite(assetBundleSpriteName, CardNo);
            return sprite;
        }
    }

    public string CardNo;
    public string PackNo;
    public string CardName;
    public string CardRarity;
    public string CardCategory;
    public string Element;
    public string Cost;
    public string ReducedCost;
    public string CardType;
    public List<string> Lv = new List<string>();

    public string text;

    public string Text {
        get { return text; }
        set {
            var textStr = value;
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
            textStr = textStr.Replace("<img src=\"/renewal/images/cardlist/ico_red.png\"/>", "<sprite=\"SymbolRed\" index=0>");
            textStr = textStr.Replace("<img src=\"/renewal/images/cardlist/ico_pup.png\"/>", "<sprite=\"SymbolPurple\" index=0>");
            textStr = textStr.Replace("<img src=\"/renewal/images/cardlist/ico_green.png\"/>", "<sprite=\"SymbolGreen\" index=0>");
            textStr = textStr.Replace("<img src=\"/renewal/images/cardlist/ico_white.png\"/>", "<sprite=\"SymbolWhite\" index=0>");
            textStr = textStr.Replace("<img src=\"/renewal/images/cardlist/ico_yellow.png\"/>", "<sprite=\"SymbolYellow\" index=0>");
            textStr = textStr.Replace("<img src=\"/renewal/images/cardlist/ico_blue.png\"/>", "<sprite=\"SymbolBlue\" index=0>");
            textStr = textStr.Replace("<img src=\"/renewal/images/cardlist/ico_god.png\"/>", "<sprite=\"SymbolGod\" index=0>");
            textStr = textStr.Replace("<img src=\"/renewal/images/cardlist/ico_soulcore.png\"/>", "<sprite=\"SoulCore\" index=0>");
            textStr = textStr.Replace("<img src=\"/renewal/images/cardlist/ico_all.png\"/>", "<sprite=\"ico_all\" index=0>");

            if (textStr.IndexOf("\n") == 0)
            {
                textStr = textStr.Substring(1, textStr.Length - 2);
            }

            text = textStr;
        }
    }

    public CardData(CardDataFromJson cardData)
    {
        CardNo = cardData.CardNo;
        PackNo = cardData.PackNo;
        CardName = cardData.CardName;
        CardRarity = cardData.CardRarity;
        CardCategory = cardData.CardCategory;
        Element = cardData.Element;
        Cost = cardData.Cost;
        ReducedCost = cardData.ReducedCost;
        CardType = cardData.CardType;
        Lv = cardData.Lv;
        Text = cardData.Text;
    }
}

public class CardDataFromJson
{
    public string CardNo;
    public string PackNo;
    public string CardName;
    public string CardRarity;
    public string CardCategory;
    public string Element;
    public string Cost;
    public string ReducedCost;
    public string CardType;
    public List<string> Lv = new List<string>();
    public string Text;
}
