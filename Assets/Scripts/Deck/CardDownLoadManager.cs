using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class CardDownLoadManager : MonoBehaviour
{
    public List<DownLoadData> m_downLoadDataList = new List<DownLoadData>();
    public List<PackData> m_packDataList = new List<PackData>();
    public List<PackData> m_awakeningDataList = new List<PackData>();

    private static CardDownLoadManager instance = null;
    public static CardDownLoadManager Instance() {
        return instance;
    }

    private void Awake() {
        instance = this;

        //SetDownLoadDataList();
        SetDownLoadDataListToDirectory();
    }

    private void SetDownLoadDataListToDirectory()
    {
        var allPackData = new PackData()
        {
            tag = "All",
            fileNameList = new List<string>()
        };
        string[] directories = System.IO.Directory.GetDirectories(ConstManager.DIRECTORY_FULL_PATH_TO_CARD, "*");
        foreach (var directory in directories)
        {
            var directorySplit = directory.Split("/");
            string directoryName = directorySplit[directorySplit.Length - 1];
            m_downLoadDataList.Add(new DownLoadData() { tag = directoryName, downLoadDetailList = new List<DownLoadDetailData>() });

            PackData packData = new PackData()
            {
                tag = directoryName,
                fileNameList = new List<string>()
            };

            string[] files = System.IO.Directory.GetFiles(directory, "*.txt");
            foreach (var file in files)
            {
                if (file.IndexOf("_b") != -1)
                {
                    continue;
                }
                packData.fileNameList.Add(System.IO.Path.GetFileNameWithoutExtension(file));
                string fileName = directoryName + "*" + System.IO.Path.GetFileNameWithoutExtension(file);
                allPackData.fileNameList.Add(fileName);
            }

            m_packDataList.Add(packData);
        }

        m_packDataList.Add(allPackData);
    }
}
