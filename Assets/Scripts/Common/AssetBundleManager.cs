using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[Serializable]
public class AssetBundleBase
{
    public AssetBundle assetBundle = null;
}

[Serializable]
public class AssetBundleBaseText : AssetBundleBase
{
    public List<TextAsset> textAssetList = new List<TextAsset>();
}

[Serializable]
public class AssetBundleBaseSprite : AssetBundleBase
{
    public UnityEngine.U2D.SpriteAtlas spriteAtlas = null;
}

[Serializable]
public class AssetBundleBaseCardData
{
    public Dictionary<string, AssetBundleBaseText> assetBundleTextList = new Dictionary<string, AssetBundleBaseText>();
    public Dictionary<string, AssetBundleBaseSprite> assetBundleSpriteList = new Dictionary<string, AssetBundleBaseSprite>();

    public BaseData baseData = new BaseData();
}

public class AssetBundleManager : MonoBehaviour
{
    private string m_cardType = "";
    public string CardType
    {
        get
        {
            if (string.IsNullOrEmpty(m_cardType))
            {
                return "bs";
            }
            return m_cardType;
        }
        set
        {
            m_cardType = value;
        }
    }


    public Dictionary<string, AssetBundleBaseCardData> m_assetBundleCardDataList = new Dictionary<string, AssetBundleBaseCardData>();
    public AssetBundleBaseCardData AssetBundleBaseCardData
    {
        get
        {
            return m_assetBundleCardDataList[CardType];
        }
    }

    public List<string> AssetBundleBaseCardDataKeys
    {
        get
        {
            return m_assetBundleCardDataList.Keys.ToList();
        }
    }

    private static AssetBundleManager instance = null;
    public static AssetBundleManager Instance()
    {
        return instance;
    }

    public void OnEnable()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public IEnumerator ReadFileList(Action action, Action<string> logAction)
    {
        bool isCompleted = false;

        var sequence = new CoroutineSequence(this);
        string[] fs = Directory.GetFiles(ConstManager.DIRECTORY_FULL_PATH_TO_BUNDLES, "*_text_.a*.assetbundle", SearchOption.TopDirectoryOnly);
        foreach (string file in fs)
        {
            sequence.Insert(0f, ReadFileToTxt(file, logAction));
        }

        sequence.OnCompleted(() => {
            isCompleted = true;
        });
        sequence.Play();

        while (!isCompleted)
        {
            yield return null;
        }

        isCompleted = false;

        fs = Directory.GetFiles(ConstManager.DIRECTORY_FULL_PATH_TO_BUNDLES, "*_text_.b*.assetbundle", SearchOption.TopDirectoryOnly);
        foreach (string file in fs)
        {
            sequence.Insert(0f, ReadFileAddTxt(file, logAction));
        }

        sequence.OnCompleted(() => {
            isCompleted = true;
        });
        sequence.Play();

        while (!isCompleted)
        {
            yield return null;
        }

        sequence = new CoroutineSequence(this);
        fs = System.IO.Directory.GetFiles(ConstManager.DIRECTORY_FULL_PATH_TO_BUNDLES, "*_spriteatlas_.*.assetbundle", System.IO.SearchOption.TopDirectoryOnly);
        foreach (string file in fs)
        {
            sequence.Insert(0f, ReadFileToSpriteAtlas(file, logAction));
        }

        sequence.OnCompleted(() => {
            action();
        });
        sequence.Play();
    }

    public IEnumerator ReadFileToTxt(string file, Action<string> logAction)
    {
        string key = Path.GetFileNameWithoutExtension(file);
        string type = key.Split('_')[0];
        if (!m_assetBundleCardDataList.ContainsKey(type))
        {
            m_assetBundleCardDataList.Add(type, new AssetBundleBaseCardData());
        }

        if (!m_assetBundleCardDataList[type].assetBundleTextList.ContainsKey(key))
        {
            AssetBundle assetBundle = AssetBundle.LoadFromFile(file);
            m_assetBundleCardDataList[type].assetBundleTextList.Add(key, new AssetBundleBaseText() { assetBundle = assetBundle });
            var request = assetBundle.LoadAllAssetsAsync();

            while (!request.isDone)
            {
                yield return null;
            }

            logAction("ReadFilePath : " + file + "\n");

            //Debug.Log("textCount : " + request.allAssets.Length);
            List<TextAsset> textAssetList = new List<TextAsset>();
            foreach (var asset in request.allAssets)
            {
                if (asset.GetType() != typeof(TextAsset))
                {
                    continue;
                }
                TextAsset textAsset = (TextAsset)asset;
                //Debug.Log("Asset : " + textAsset.name + " text : " + textAsset.text);
                textAssetList.Add(textAsset);
            }
            m_assetBundleCardDataList[type].assetBundleTextList[key].textAssetList = textAssetList;
        }
    }

    public IEnumerator ReadFileAddTxt(string file, Action<string> logAction)
    {
        string key = Path.GetFileNameWithoutExtension(file);
        string type = key.Split('_')[0];

        AssetBundle assetBundle = AssetBundle.LoadFromFile(file);
        var request = assetBundle.LoadAllAssetsAsync();
        while (!request.isDone)
        {
            yield return null;
        }

        Dictionary<string, TextAsset> textAssetList = new Dictionary<string, TextAsset>();
        foreach (var asset in request.allAssets)
        {
            if (asset.GetType() != typeof(TextAsset))
            {
                continue;
            }
            TextAsset textAsset = (TextAsset)asset;
            //Debug.Log("Asset : " + textAsset.name + " text : " + textAsset.text);
            textAssetList.Add(textAsset.name, textAsset);
        }

        foreach(var assetBundleTextKey in m_assetBundleCardDataList[type].assetBundleTextList.Keys)
        {
            for(int i = 0; i < m_assetBundleCardDataList[type].assetBundleTextList[assetBundleTextKey].textAssetList.Count; i++)
            {
                var name = m_assetBundleCardDataList[type].assetBundleTextList[assetBundleTextKey].textAssetList[i].name;
                if (!textAssetList.ContainsKey(name))
                {
                    continue;
                }
                m_assetBundleCardDataList[type].assetBundleTextList[assetBundleTextKey].textAssetList[i] = textAssetList[name];
            }
        }

        logAction("ReadFilePath : " + file + "\n");
    }

    public IEnumerator ReadFileToSpriteAtlas(string file, Action<string> logAction)
    {
        string key = Path.GetFileNameWithoutExtension(file);
        string type = key.Split('_')[0];
        if (!m_assetBundleCardDataList[type].assetBundleSpriteList.ContainsKey(key))
        {
            AssetBundle assetBundle = AssetBundle.LoadFromFile(file);
            m_assetBundleCardDataList[type].assetBundleSpriteList.Add(key, new AssetBundleBaseSprite() { assetBundle = assetBundle });
            var request = assetBundle.LoadAllAssetsAsync();

            while (!request.isDone)
            {
                yield return null;
            }

            logAction("ReadFilePath : " + file + "\n");

            if (request.asset.GetType() == typeof(UnityEngine.U2D.SpriteAtlas))
            {
                var atlas = (UnityEngine.U2D.SpriteAtlas)request.asset;
                Debug.Log("Asset : " + atlas.name + " spriteCount : " + atlas.spriteCount);
                m_assetBundleCardDataList[type].assetBundleSpriteList[key].spriteAtlas = atlas;
            }
        }
    }

    public Sprite GetCardSprite(string key, string name)
    {
        try
        {
            return AssetBundleBaseCardData.assetBundleSpriteList[key].spriteAtlas.GetSprite(name);
        }
        catch
        {
            return null;
        }
    }

    public bool ExistsBaseData(string key, string cardNo)
    {
        if (!AssetBundleBaseCardData.baseData.packDatas.ContainsKey(key))
        {
            return false;
        }

        foreach(var cardData in AssetBundleBaseCardData.baseData.packDatas[key])
        {
            if (cardData.CardNo != cardNo)
            {
                continue;
            }

            return true;
        }

        return false;
    }

    public CardData GetBaseData(string key, string cardNo)
    {
        if (!AssetBundleBaseCardData.baseData.packDatas.ContainsKey(key))
        {
            return null;
        }

        foreach (var cardData in AssetBundleBaseCardData.baseData.packDatas[key])
        {
            if (cardData.CardNo != cardNo)
            {
                continue;
            }

            return cardData;
        }

        return null;
    }

    public void SetCardTextList()
    {
        foreach (string type in m_assetBundleCardDataList.Keys)
        {
            m_assetBundleCardDataList[type].baseData = new BaseData();

            foreach (var assetBundle in m_assetBundleCardDataList[type].assetBundleTextList)
            {
                foreach (var textAsset in assetBundle.Value.textAssetList)
                {
                    //Debug.Log("Asset : " + textAsset.name + " text : " + textAsset.text);
                    CardData cardData = null;
                    cardData = new CardData(JsonUtility.FromJson<CardDataFromJson>(textAsset.text));
                    cardData.assetBundleSpriteName = assetBundle.Key.Replace("_text_", "_spriteatlas_");
                    cardData.fileName = textAsset.name;

                    if (!m_assetBundleCardDataList[type].baseData.packDatas.ContainsKey(cardData.PackNo))
                    {
                        m_assetBundleCardDataList[type].baseData.packDatas.Add(cardData.PackNo, new List<CardData>() { cardData });
                    }
                    else
                    {
                        m_assetBundleCardDataList[type].baseData.packDatas[cardData.PackNo].Add(cardData);
                    }
                }
            }

            List<string> allPackNameList = new List<string>();

            foreach (var data in m_assetBundleCardDataList[type].baseData.packDatas)
            {
                List<string> packNameList = new List<string>();

                foreach (var cardData in data.Value)
                {
                    if (cardData.CardNo.EndsWith("_b"))
                    {
                        continue;
                    }

                    packNameList.Add(cardData.CardNo);
                    allPackNameList.Add(cardData.PackNo + "*" + cardData.CardNo);
                }
                m_assetBundleCardDataList[type].baseData.packNameList.Add(data.Key, packNameList);
            }

            m_assetBundleCardDataList[type].baseData.packNameList.Add("All", allPackNameList);
        }
    }
}
