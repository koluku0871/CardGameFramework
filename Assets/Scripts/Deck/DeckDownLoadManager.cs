using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class DeckDownLoadManager : MonoBehaviour
{
    private static DeckDownLoadManager instance = null;
    public static DeckDownLoadManager Instance()
    {
        return instance;
    }

    private void Awake()
    {
        instance = this;
    }

    public IEnumerator DownLoadDeck(string deckPath, Action callback = null)
    {
        Debug.Log("url: " + deckPath);
        var directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_DECK;
        using (UnityWebRequest www = UnityWebRequest.Get(deckPath))
        {
            yield return www.SendWebRequest();
            byte[] bytes = www.downloadHandler.data;
            DateTime dt = DateTime.Now;
            String name = dt.ToString($"{dt:yyyyMMddHHmmss}");
            var filePath = directoryPath + name + ".json";
            File.WriteAllBytes(filePath, bytes);
            yield return null;
            StreamReader streamReader = new StreamReader(filePath);
            string tmpPath = Path.GetTempFileName();
            StreamWriter streamWriter = new StreamWriter(tmpPath);

            bool isWrite = false;
            List<int> cardCountList = new List<int>();
            List<string> cardIdList = new List<string>();
            while (streamReader.Peek() > -1)
            {
                string line = streamReader.ReadLine();
                if (!isWrite && line == "      <ul class=\"cardlistBox\">")
                {
                    isWrite = true;
                }

                if (isWrite && line == "    <div class=\"deckurl\">")
                {
                    isWrite = false;
                }

                if (isWrite)
                {
                    if (line.IndexOf("<p class=") != -1)
                    {
                        int start = line.IndexOf(">") + 1;
                        int end = line.LastIndexOf("枚");
                        line = line.Substring(start, end - start);
                        Debug.Log(line);
                        if (Int32.TryParse(line, out int count))
                        {
                            cardCountList.Add(count);
                        }
                    }
                    if (line.IndexOf("<a class=") != -1)
                    {
                        int start = line.LastIndexOf("=") + 1;
                        int end = line.LastIndexOf("\"");
                        line = line.Substring(start, end - start);
                        line = line.Replace(" ", "");
                        if (line.IndexOf("javascript:cardDetails") != -1)
                        {
                            start = line.IndexOf(",") + 1;
                            line = line.Substring(start, line.Length - start);
                            end = line.IndexOf(",");
                            line = line.Substring(0, end);
                            line = line.Replace("'", "");
                        }
                        Debug.Log(line);
                        cardIdList.Add(line);
                    }
                }
            }

            string jsonStr = "";
            if (cardCountList.Count == cardIdList.Count)
            {
                DeckManager.DeckDetail deckDetail = new DeckManager.DeckDetail();
                for (int index = 0; index < cardCountList.Count; index++)
                {
                    string cardId = cardIdList[index];
                    string tag = cardId.Split('-')[0];

                    if(tag.IndexOf("CP") != -1)
                    {
                        tag = "CP";
                    }
                    else if (tag.IndexOf("KA") != -1)
                    {
                        tag = "KA";
                    }
                    else if (tag.IndexOf("KF") != -1)
                    {
                        tag = "KF";
                    }
                    else if (tag.IndexOf("PB") != -1)
                    {
                        tag = "PB";
                    }
                    else if (tag.IndexOf("PX") != -1)
                    {
                        tag = "PX";
                    }
                    else if (tag.IndexOf("SJ") != -1)
                    {
                        tag = "SJ";
                    }
                    else if (tag.IndexOf("EX") != -1)
                    {
                        tag = "EX";
                    }
                    else if (tag.IndexOf("LM") != -1)
                    {
                        tag = "LM";
                    }
                    else if (tag.IndexOf("X") != -1)
                    {
                        tag = "X";
                    }
                    else if (tag.IndexOf("P") != -1)
                    {
                        tag = "P";
                    }

                    for (int count = 0; count < cardCountList[index]; count++)
                    {
                        deckDetail.Add(new DeckManager.CardDetail() {tag = tag, cardId = cardId});
                    }
                }

                jsonStr = JsonUtility.ToJson(deckDetail);
            }
            streamWriter.WriteLine(jsonStr);

            streamReader.Close();
            streamWriter.Close();

            File.Copy(tmpPath, filePath, true);
            File.Delete(tmpPath);
            if (string.IsNullOrEmpty(jsonStr)) File.Delete(filePath);
        }

        if (callback != null) callback();
    }
}
