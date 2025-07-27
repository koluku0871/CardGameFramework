using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using static DeckManager;

public class OptionData : JsonFileData
{
    public string name = "";
    public string cardType = "";
    public float homeBgmVolume = 0.1f;
    public float battleBgmVolume = 0.1f;
    public float deckBgmVolume = 0.1f;
    public float seVolume = 0.1f;
    public string apiPath = "https://ss971571.stars.ne.jp/list.php";

    public OptionData()
    {
        this.directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_OPTION;
        this.fileName = "opt.json";
    }

    public bool LoadTxt()
    {
        try
        {
            string[] optFiles = Directory.GetFiles(this.directoryPath, this.fileName, SearchOption.AllDirectories);
            StreamReader sr = new StreamReader(optFiles[0], Encoding.UTF8);
            OptionData optionData = JsonUtility.FromJson<OptionData>(sr.ReadToEnd());
            sr.Close();

            this.name = optionData.name;
            this.cardType = optionData.cardType;
            this.homeBgmVolume = optionData.homeBgmVolume;
            this.battleBgmVolume = optionData.battleBgmVolume;
            this.deckBgmVolume = optionData.deckBgmVolume;
            this.seVolume = optionData.seVolume;
            this.apiPath = optionData.apiPath;

            return true;
        }
        catch { }

        return false;
    }
}

public class FavoriteData
{
    public List<CardDetail> cardDetails = new List<CardDetail>();
}

public class JsonFileData
{
    protected string directoryPath = "";
    protected string fileName = "";

    public bool IsFileExists()
    {
        if (!Directory.Exists(this.directoryPath))
        {
            Directory.CreateDirectory(this.directoryPath);
        }

        var path = this.directoryPath + this.fileName;
        var isFile = File.Exists(path);
        return isFile;
    }

    public bool SaveTxt()
    {
        try
        {
            var path = this.directoryPath + this.fileName;
            StreamWriter streamWriter = new StreamWriter(path);
            streamWriter.WriteLine(JsonUtility.ToJson(this));
            streamWriter.Close();

            return true;
        }
        catch { }

        return false;
    }
}