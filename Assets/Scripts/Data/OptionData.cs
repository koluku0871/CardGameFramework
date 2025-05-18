using System.Collections.Generic;
using static DeckManager;

public class OptionData
{
    public string name = "";
    public float homeBgmVolume = 0.1f;
    public float battleBgmVolume = 0.1f;
    public float deckBgmVolume = 0.1f;
    public string apiPath = "https://ss971571.stars.ne.jp/list.php";
}

public class FavoriteData
{
    public List<CardDetail> cardDetails = new List<CardDetail>();
}