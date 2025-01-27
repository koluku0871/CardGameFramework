using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AssetBundleBaseText
{
    public AssetBundle assetBundle = null;
    public List<TextAsset> textAssetList = new List<TextAsset>();
}

public class AssetBundleBaseSprite
{
    public AssetBundle assetBundle = null;
    public UnityEngine.U2D.SpriteAtlas spriteAtlas = null;
}

public class AssetBundleManager : MonoBehaviour
{
    public Dictionary<string, AssetBundleBaseText> m_assetBundleTextList = new Dictionary<string, AssetBundleBaseText>();
    public Dictionary<string, AssetBundleBaseSprite> m_assetBundleSpriteList = new Dictionary<string, AssetBundleBaseSprite>();

    public BaseData m_bs_baseData = new BaseData();

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
        string[] fs = Directory.GetFiles(ConstManager.DIRECTORY_FULL_PATH_TO_BUNDLES, "*_text.assetbundle", SearchOption.TopDirectoryOnly);
        foreach (string file in fs)
        {
            string key = Path.GetFileNameWithoutExtension(file);
            if (m_assetBundleTextList.ContainsKey(key))
            {
                continue;
            }

            logAction("ReadFilePath : " + file + "\n");
            AssetBundle assetBundle = AssetBundle.LoadFromFile(file);
            m_assetBundleTextList.Add(key, new AssetBundleBaseText() { assetBundle = assetBundle });
            var request = assetBundle.LoadAllAssetsAsync();

            while (!request.isDone)
            {
                yield return null;
            }

            Debug.Log("textCount : " + request.allAssets.Length);
            List<TextAsset> textAssetList = new List<TextAsset>();
            foreach (var asset in request.allAssets)
            {
                if (asset.GetType() != typeof(TextAsset))
                {
                    continue;
                }
                TextAsset textAsset = (TextAsset)asset;
                Debug.Log("Asset : " + textAsset.name + " text : " + textAsset.text);
                textAssetList.Add(textAsset);
            }
            m_assetBundleTextList[key].textAssetList = textAssetList;
        }

        yield return null;

        fs = System.IO.Directory.GetFiles(ConstManager.DIRECTORY_FULL_PATH_TO_BUNDLES, "*_spriteatlas.assetbundle", System.IO.SearchOption.TopDirectoryOnly);
        foreach (string file in fs)
        {
            string key = Path.GetFileNameWithoutExtension(file);
            if (m_assetBundleSpriteList.ContainsKey(key))
            {
                continue;
            }

            logAction("ReadFilePath : " + file + "\n");
            AssetBundle assetBundle = AssetBundle.LoadFromFile(file);
            m_assetBundleSpriteList.Add(key, new AssetBundleBaseSprite() { assetBundle = assetBundle });
            var request = assetBundle.LoadAllAssetsAsync();

            while (!request.isDone)
            {
                yield return null;
            }

            if (request.asset.GetType() == typeof(UnityEngine.U2D.SpriteAtlas))
            {
                var atlas = (UnityEngine.U2D.SpriteAtlas)request.asset;
                Debug.Log("Asset : " + atlas.name + " spriteCount : " + atlas.spriteCount);
                m_assetBundleSpriteList[key].spriteAtlas = atlas;
            }
        }

        yield return null;

        action();
    }

    public Sprite GetCardSprite(string key, string name)
    {
        try
        {
            return m_assetBundleSpriteList[key].spriteAtlas.GetSprite(name);
        }
        catch
        {
            return null;
        }
    }

    public bool ExistsBaseData(string key, string cardNo)
    {
        if (!m_bs_baseData.packDatas.ContainsKey(key))
        {
            return false;
        }

        foreach(var cardData in m_bs_baseData.packDatas[key])
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
        if (!m_bs_baseData.packDatas.ContainsKey(key))
        {
            return null;
        }

        foreach (var cardData in m_bs_baseData.packDatas[key])
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
        m_bs_baseData = new BaseData();

        foreach (var assetBundle in m_assetBundleTextList)
        {
            foreach (var textAsset in assetBundle.Value.textAssetList)
            {
                Debug.Log("Asset : " + textAsset.name + " text : " + textAsset.text);
                CardData cardData = new CardData(JsonUtility.FromJson<CardDataFromJson>(textAsset.text));
                cardData.assetBundleSpriteName = assetBundle.Key.Replace("_text", "_spriteatlas");
                cardData.fileName = textAsset.name;

                if (!m_bs_baseData.packDatas.ContainsKey(cardData.PackNo))
                {
                    m_bs_baseData.packDatas.Add(cardData.PackNo, new List<CardData>() { cardData });
                }
                else
                {
                    m_bs_baseData.packDatas[cardData.PackNo].Add(cardData);
                }
            }
        }

        List<string> allPackNameList = new List<string>();

        foreach (var data in m_bs_baseData.packDatas)
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
            m_bs_baseData.packNameList.Add(data.Key, packNameList);
        }

        m_bs_baseData.packNameList.Add("All", allPackNameList);
    }
}
