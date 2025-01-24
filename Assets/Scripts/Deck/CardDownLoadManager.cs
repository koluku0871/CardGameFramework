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

    private void SetDownLoadDataList() {
        // BS01
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS01",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS01-", startNo = 1, endNo = 149, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS01-X", startNo = 1, endNo = 4, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS01-X03_", startNo = 2, endNo = 2, digit = 1
                    }
                }
            }
        );
        // BS02
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS02",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS02-", startNo = 1, endNo = 111, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS02-X", startNo = 5, endNo = 8, digit = 2
                    }
                }
            }
        );
        // BS03
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS03",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS03-", startNo = 1, endNo = 149, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS03-X", startNo = 9, endNo = 12, digit = 2
                    }
                }
            }
        );
        // BS04
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS04",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS04-", startNo = 1, endNo = 114, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS04-X", startNo = 13, endNo = 16, digit = 2
                    }
                }
            }
        );
        // BS05
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS05",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS05-", startNo = 1, endNo = 84, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS05-X", startNo = 17, endNo = 20, digit = 2
                    }
                }
            }
        );
        // BS06
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS06",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS06-", startNo = 1, endNo = 114, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS06-X", startNo = 21, endNo = 24, digit = 2
                    }
                }
            }
        );
        // BS07
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS07",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS07-", startNo = 1, endNo = 84, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS07-X", startNo = 25, endNo = 28, digit = 2
                    }
                }
            }
        );
        // BS08
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS08",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS08-", startNo = 1, endNo = 84, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS08-X", startNo = 29, endNo = 34, digit = 2
                    }
                }
            }
        );
        // BS09
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS09",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS09-", startNo = 1, endNo = 84, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS09-X", startNo = 35, endNo = 40, digit = 2
                    }
                }
            }
        );
        // BS10
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS10",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS10-", startNo = 1, endNo = 114, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS10-X", startNo = 1, endNo = 6, digit = 2
                    }
                }
            }
        );
        // BS11
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS11",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS11-", startNo = 1, endNo = 84, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS11-X", startNo = 1, endNo = 6, digit = 2
                    }
                }
            }
        );
        // BS12
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS12",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS12-", startNo = 1, endNo = 84, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS12-X", startNo = 1, endNo = 6, digit = 2
                    }
                }
            }
        );
        // BS13
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS13",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS13-", startNo = 1, endNo = 84, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS13-X", startNo = 1, endNo = 6, digit = 2
                    }
                }
            }
        );
        // BS14
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS14",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS14-", startNo = 1, endNo = 114, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS14-X", startNo = 1, endNo = 6, digit = 2
                    }
                }
            }
        );
        // BS15
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS15",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS15-", startNo = 1, endNo = 84, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS15-X", startNo = 1, endNo = 6, digit = 2
                    }
                }
            }
        );
        // BS16
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS16",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS16-", startNo = 1, endNo = 84, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS16-X", startNo = 1, endNo = 6, digit = 2
                    }
                }
            }
        );
        // BS17
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS17",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS17-", startNo = 1, endNo = 84, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS17-X", startNo = 1, endNo = 6, digit = 2
                    }
                }
            }
        );
        // BS18
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS18",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS18-", startNo = 1, endNo = 84, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS18-X", startNo = 1, endNo = 6, digit = 2
                    }
                }
            }
        );
        // BS19
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS19",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS19-", startNo = 1, endNo = 114, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS19-X", startNo = 1, endNo = 8, digit = 2
                    }
                }
            }
        );
        // BS20
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS20",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS20-", startNo = 1, endNo = 84, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS20-X", startNo = 1, endNo = 8, digit = 2
                    }
                }
            }
        );
        // BS21
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS21",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS21-", startNo = 1, endNo = 84, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS21-X", startNo = 1, endNo = 8, digit = 2
                    }
                }
            }
        );
        // BS22
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS22",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS22-", startNo = 1, endNo = 84, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS22-X", startNo = 1, endNo = 8, digit = 2
                    }
                }
            }
        );
        // BS23
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS23",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS23-", startNo = 1, endNo = 84, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS23-X", startNo = 1, endNo = 8, digit = 2
                    }
                }
            }
        );
        // BS24
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS24",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS24-", startNo = 1, endNo = 114, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS24-X", startNo = 1, endNo = 8, digit = 2
                    }
                }
            }
        );
        // BS25
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS25",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS25-", startNo = 1, endNo = 83, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS25-X", startNo = 1, endNo = 8, digit = 2
                    }
                }
            }
        );
        // BS26
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS26",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS26-", startNo = 1, endNo = 83, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS26-X", startNo = 1, endNo = 8, digit = 2
                    }
                }
            }
        );
        // BS27
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS27",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS27-", startNo = 1, endNo = 83, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS27-X", startNo = 1, endNo = 8, digit = 2
                    }
                }
            }
        );
        // BS28
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS28",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS28-", startNo = 1, endNo = 83, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS28-X", startNo = 1, endNo = 8, digit = 2
                    }
                }
            }
        );
        // BS29
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS29",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS29-", startNo = 1, endNo = 83, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS29-X", startNo = 1, endNo = 8, digit = 2
                    }
                }
            }
        );
        // BS30
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS30",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS30-", startNo = 1, endNo = 83, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS30-X", startNo = 1, endNo = 8, digit = 2
                    }
                }
            }
        );
        // BS31
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS31",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS31-", startNo = 1, endNo = 114, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS31-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS31-XX", startNo = 1, endNo = 2, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS31-CP", startNo = 1, endNo = 6, digit = 2
                    }
                }
            }
        );
        // BS32
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS32",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS32-", startNo = 1, endNo = 83, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS32-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS32-XX", startNo = 1, endNo = 2, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS32-CP", startNo = 1, endNo = 2, digit = 2
                    }
                }
            }
        );
        // BS33
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS33",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS33-", startNo = 1, endNo = 83, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS33-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS33-XX", startNo = 1, endNo = 2, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS33-CP", startNo = 1, endNo = 3, digit = 2
                    }
                }
            }
        );
        // BS34
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS34",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS34-", startNo = 1, endNo = 83, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS34-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS34-XX", startNo = 1, endNo = 2, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS34-CP", startNo = 1, endNo = 7, digit = 2
                    }
                }
            }
        );
        // BS35
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS35",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS35-", startNo = 1, endNo = 102, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS35-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS35-XX", startNo = 1, endNo = 2, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS35-CP", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // BS36
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS36",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS36-", startNo = 1, endNo = 77, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS36-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS36-XX", startNo = 1, endNo = 2, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS36-CP", startNo = 1, endNo = 2, digit = 2
                    }
                }
            }
        );
        // BS37
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS37",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS37-", startNo = 1, endNo = 77, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS37-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS37-XX", startNo = 1, endNo = 2, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS37-CP", startNo = 1, endNo = 3, digit = 2
                    }
                }
            }
        );
        // BS38
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS38",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS38-", startNo = 1, endNo = 77, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS38-RVX", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS38-RV", startNo = 1, endNo = 41, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS38-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS38-XX", startNo = 1, endNo = 2, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS38-CP", startNo = 1, endNo = 8, digit = 2
                    }
                }
            }
        );
        // BS39
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS39",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS39-", startNo = 1, endNo = 77, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS39-RVX", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS39-RV", startNo = 1, endNo = 41, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS39-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS39-XX", startNo = 1, endNo = 3, digit = 2
                    }
                }
            }
        );
        // BS40
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS40",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS40-", startNo = 1, endNo = 95, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS40-RV", startNo = 1, endNo = 12, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS40-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS40-XX", startNo = 1, endNo = 2, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS40-CP", startNo = 1, endNo = 9, digit = 2
                    }
                }
            }
        );
        // BS41
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS41",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS41-", startNo = 1, endNo = 95, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS41-RV", startNo = 1, endNo = 12, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS41-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS41-XX", startNo = 1, endNo = 2, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS41-CP", startNo = 1, endNo = 9, digit = 2
                    }
                }
            }
        );
        // BS42
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS42",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS42-", startNo = 1, endNo = 95, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS42-RV", startNo = 1, endNo = 12, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS42-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS42-XX", startNo = 1, endNo = 2, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS42-CP", startNo = 1, endNo = 9, digit = 2
                    }
                }
            }
        );
        // BS43
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS43",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS43-", startNo = 1, endNo = 95, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS43-RV%20X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS43-RV", startNo = 1, endNo = 31, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS43-RVXX", startNo = 1, endNo = 1, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS43-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS43-XX", startNo = 1, endNo = 1, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS43-CP", startNo = 1, endNo = 3, digit = 2
                    }
                }
            }
        );
        // BS44
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS44",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS44-", startNo = 1, endNo = 96, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS44-10thX", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS44-RV", startNo = 1, endNo = 12, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS44-X", startNo = 1, endNo = 9, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS44-XX", startNo = 1, endNo = 2, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS44-CP", startNo = 1, endNo = 3, digit = 2
                    }
                }
            }
        );
        // BS45
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS45",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS45-", startNo = 1, endNo = 96, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS45-10thX", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS45-RV", startNo = 1, endNo = 12, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS45-X", startNo = 1, endNo = 9, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS45-XX", startNo = 1, endNo = 2, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS45-CP", startNo = 1, endNo = 2, digit = 2
                    }
                }
            }
        );
        // BS46
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS46",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS46-", startNo = 1, endNo = 96, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS46-10thX", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS46-RV", startNo = 1, endNo = 12, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS46-X", startNo = 1, endNo = 12, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS46-XX", startNo = 1, endNo = 2, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS46-CP", startNo = 1, endNo = 2, digit = 2
                    }
                }
            }
        );
        // BS47
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS47",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS47-", startNo = 1, endNo = 102, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS47-10thX", startNo = 1, endNo = 2, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS47-RV", startNo = 1, endNo = 6, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS47-X", startNo = 1, endNo = 9, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS47-XX", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS47-CP", startNo = 1, endNo = 7, digit = 2
                    }
                }
            }
        );
        // BS48
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS48",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS48-", startNo = 1, endNo = 96, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS48-10thX", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS48-RV", startNo = 1, endNo = 12, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS48-X", startNo = 1, endNo = 10, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS48-XX", startNo = 1, endNo = 2, digit = 2
                    }
                }
            }
        );
        // BS49
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS49",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS49-", startNo = 1, endNo = 95, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS49-10thX", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS49-RV", startNo = 1, endNo = 13, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS49-X", startNo = 1, endNo = 10, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS49-XX", startNo = 1, endNo = 2, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS49-CP", startNo = 1, endNo = 4, digit = 2
                    }
                }
            }
        );
        // BS50
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS50",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS50-", startNo = 1, endNo = 94, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS50-10thX", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS50-RV", startNo = 1, endNo = 14, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS50-X", startNo = 1, endNo = 13, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS50-XX", startNo = 1, endNo = 2, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS50-CP", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // BS51
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "BS51",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS51-", startNo = 1, endNo = 93, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS51-10thX", startNo = 1, endNo = 4, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS51-RV", startNo = 1, endNo = 15, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS51-X", startNo = 1, endNo = 10, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS51-XX", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS51-CP", startNo = 1, endNo = 6, digit = 2
                    }
                }
            }
        );
        // BS52_B
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BS52_B",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS52-", startNo = 7, endNo = 7, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS52-", startNo = 15, endNo = 15, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS52-", startNo = 25, endNo = 25, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS52-", startNo = 34, endNo = 34, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS52-", startNo = 46, endNo = 46, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS52-", startNo = 57, endNo = 57, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS52-", startNo = 60, endNo = 65, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS52-TX", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS52-TCP", startNo = 1, endNo = 3, digit = 2
                    }
                }
            }
        );
        // BS52
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BS52",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS52-", startNo = 1, endNo = 76, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS52-RV", startNo = 1, endNo = 8, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS52-TX", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS52-X", startNo = 1, endNo = 9, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS52-XX", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS52-CP", startNo = 1, endNo = 14, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS52-TCP", startNo = 1, endNo = 3, digit = 2
                    }
                }
            }
        );
        // BS53_B
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BS53_B",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS53-", startNo = 4, endNo = 4, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS53-", startNo = 7, endNo = 7, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS53-", startNo = 17, endNo = 17, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS53-", startNo = 21, endNo = 21, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS53-", startNo = 28, endNo = 28, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS53-", startNo = 31, endNo = 31, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS53-", startNo = 38, endNo = 38, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS53-", startNo = 44, endNo = 44, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS53-", startNo = 48, endNo = 48, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS53-", startNo = 54, endNo = 54, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS53-", startNo = 63, endNo = 63, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS53-", startNo = 66, endNo = 66, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS53-TCP", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS53-TX", startNo = 1, endNo = 4, digit = 2
                    },
                }
            }
        );
        // BS53
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BS53",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS53-", startNo = 1, endNo = 78, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS53-RV", startNo = 1, endNo = 6, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS53-TX", startNo = 1, endNo = 4, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS53-X", startNo = 1, endNo = 9, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS53-XX", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS53-CP", startNo = 1, endNo = 12, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS53-TCP", startNo = 1, endNo = 3, digit = 2
                    }
                }
            }
        );
        // BS54_B
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BS54_B",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS54-", startNo = 7, endNo = 7, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS54-", startNo = 16, endNo = 16, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS54-", startNo = 24, endNo = 24, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS54-", startNo = 32, endNo = 32, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS54-", startNo = 46, endNo = 46, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS54-", startNo = 55, endNo = 55, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS54-", startNo = 61, endNo = 66, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS54-TCP", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS54-TX", startNo = 1, endNo = 4, digit = 2
                    },
                }
            }
        );
        // BS54
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BS54",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS54-", startNo = 1, endNo = 78, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS54-RV", startNo = 1, endNo = 6, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS54-TX", startNo = 1, endNo = 4, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS54-X", startNo = 1, endNo = 9, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS54-XX", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS54-TCP", startNo = 1, endNo = 3, digit = 2
                    }
                }
            }
        );
        // BS55_B
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BS55_B",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS55-", startNo = 2, endNo = 2, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS55-", startNo = 4, endNo = 4, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS55-", startNo = 12, endNo = 12, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS55-", startNo = 21, endNo = 21, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS55-", startNo = 27, endNo = 27, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS55-", startNo = 34, endNo = 34, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS55-", startNo = 40, endNo = 40, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS55-", startNo = 44, endNo = 44, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS55-", startNo = 51, endNo = 51, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS55-", startNo = 53, endNo = 53, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS55-", startNo = 62, endNo = 63, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS55-TCP", startNo = 1, endNo = 10, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS55-TX", startNo = 1, endNo = 4, digit = 2
                    },
                }
            }
        );
        // BS55
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BS55",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS55-", startNo = 1, endNo = 78, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS55-RV", startNo = 1, endNo = 6, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS55-TX", startNo = 1, endNo = 4, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS55-X", startNo = 1, endNo = 9, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS55-XX", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS55-TCP", startNo = 1, endNo = 10, digit = 2
                    },
                }
            }
        );
        // BS56_B
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BS56_B",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS56-", startNo = 3, endNo = 3, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS56-", startNo = 14, endNo = 14, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS56-", startNo = 17, endNo = 17, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS56-", startNo = 31, endNo = 31, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS56-", startNo = 36, endNo = 36, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS56-", startNo = 38, endNo = 38, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS56-", startNo = 45, endNo = 45, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS56-", startNo = 47, endNo = 47, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS56-", startNo = 56, endNo = 56, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS56-", startNo = 63, endNo = 63, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS56-", startNo = 67, endNo = 67, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS56-", startNo = 72, endNo = 72, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS56-TCP", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS56-TX", startNo = 1, endNo = 2, digit = 2
                    },
                }
            }
        );
        // BS56
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BS56",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS56-", startNo = 1, endNo = 78, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS56-RV", startNo = 1, endNo = 6, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS56-TX", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS56-X", startNo = 1, endNo = 9, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS56-XX", startNo = 1, endNo = 2, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS56-TCP", startNo = 1, endNo = 3, digit = 2
                    },
                }
            }
        );
        // BS57_B
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BS57_B",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS57-", startNo = 5, endNo = 6, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS57-", startNo = 15, endNo = 15, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS57-", startNo = 18, endNo = 18, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS57-", startNo = 25, endNo = 25, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS57-", startNo = 34, endNo = 34, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS57-", startNo = 40, endNo = 40, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS57-", startNo = 43, endNo = 43, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS57-", startNo = 53, endNo = 53, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS57-", startNo = 64, endNo = 64, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS57-", startNo = 68, endNo = 68, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS57-", startNo = 70, endNo = 70, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS57-TCP", startNo = 1, endNo = 10, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS57-TX", startNo = 1, endNo = 3, digit = 2
                    },
                }
            }
        );
        // BS57
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BS57",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS57-", startNo = 1, endNo = 78, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS57-TX", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS57-X", startNo = 1, endNo = 9, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS57-XX", startNo = 1, endNo = 2, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BS57-TCP", startNo = 1, endNo = 10, digit = 2
                    },
                }
            }
        );



        // BSC05
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BSC05",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC05-", startNo = 1, endNo = 28, digit = 3
                    }
                }
            }
        );
        // BSC15
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BSC15",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC15-", startNo = 1, endNo = 15, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC15-X", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // BSC16
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BSC16",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC16-", startNo = 1, endNo = 39, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC16-X", startNo = 1, endNo = 3, digit = 2
                    }
                }
            }
        );
        // BSC17
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BSC17",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC17-", startNo = 1, endNo = 14, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC17-X", startNo = 1, endNo = 4, digit = 2
                    }
                }
            }
        );
        // BSC18
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BSC18",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC18-", startNo = 1, endNo = 45, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC18-X", startNo = 1, endNo = 3, digit = 2
                    }
                }
            }
        );
        // BSC19
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BSC19",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC19-", startNo = 1, endNo = 52, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC19-X", startNo = 1, endNo = 5, digit = 2
                    }
                }
            }
        );
        // BSC20
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BSC20",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC20-", startNo = 1, endNo = 22, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC20-X", startNo = 1, endNo = 3, digit = 2
                    }
                }
            }
        );
        // BSC21
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BSC21",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC21-", startNo = 1, endNo = 12, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC21-X", startNo = 1, endNo = 4, digit = 2
                    }
                }
            }
        );
        // BSC22
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BSC22",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC22-", startNo = 1, endNo = 146, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC22-X", startNo = 1, endNo = 4, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC22-CP", startNo = 1, endNo = 3, digit = 2
                    }
                }
            }
        );
        // BSC23
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BSC23",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC23-", startNo = 1, endNo = 52, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC23-X", startNo = 1, endNo = 5, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC23-CP", startNo = 1, endNo = 3, digit = 2
                    }
                }
            }
        );
        // BSC24
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BSC24",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC24-", startNo = 1, endNo = 52, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC24-X", startNo = 1, endNo = 5, digit = 2
                    }
                }
            }
        );
        // BSC25
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BSC25",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC25-", startNo = 1, endNo = 49, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC25-X", startNo = 1, endNo = 5, digit = 2
                    }
                }
            }
        );
        // BSC26
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BSC26",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC26-", startNo = 1, endNo = 49, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC26-X", startNo = 1, endNo = 5, digit = 2
                    }
                }
            }
        );
        // BSC28
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BSC28",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC28-", startNo = 1, endNo = 58, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC28-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC28-CP", startNo = 1, endNo = 3, digit = 2
                    }
                }
            }
        );
        // BSC29
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BSC29",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC29-", startNo = 1, endNo = 30, digit = 3
                    }
                }
            }
        );
        // BSC30
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BSC30",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC30-", startNo = 1, endNo = 24, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC30-X", startNo = 1, endNo = 4, digit = 2
                    }
                }
            }
        );
        // BSC31
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BSC31",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC31-", startNo = 1, endNo = 57, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC31-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC31-CP", startNo = 1, endNo = 4, digit = 2
                    }
                }
            }
        );
        // BSC32
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BSC32",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC32-", startNo = 1, endNo = 40, digit = 3
                    }
                }
            }
        );
        // BSC33
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BSC33",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC33-", startNo = 1, endNo = 56, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC33-RV", startNo = 1, endNo = 2, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC33-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC33-CP", startNo = 1, endNo = 2, digit = 2
                    }
                }
            }
        );
        // BSC34
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BSC34",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC34-", startNo = 1, endNo = 24, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC34-X", startNo = 1, endNo = 6, digit = 2
                    }
                }
            }
        );
        // BSC35
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BSC35",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC35-", startNo = 1, endNo = 39, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC35-A", startNo = 1, endNo = 19, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC35-X", startNo = 1, endNo = 4, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC35-XA", startNo = 1, endNo = 4, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC35-XXA", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );

        // BSC36
        // TODO オリジナルカード無し

        // BSC37
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "BSC37",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC37-", startNo = 1, endNo = 10, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC37-RV%20X", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC37-RV", startNo = 1, endNo = 12, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC37-X", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/BSC37-XX", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );

        // BSC38
        // TODO オリジナルカード無し


        // CB01
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "CB01",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB01-", startNo = 1, endNo = 62, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB01-X", startNo = 1, endNo = 6, digit = 2
                    }
                }
            }
        );
        // CB02
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "CB02",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB02-", startNo = 1, endNo = 71, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB02-X", startNo = 1, endNo = 8, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB02-XX", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // CB03
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "CB03",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB03-CP", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // CB04
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "CB04",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB04-", startNo = 1, endNo = 75, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB04-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB04-XX", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // CB05
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "CB05",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB05-", startNo = 1, endNo = 59, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB05-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB05-XX", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // CB06
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "CB06",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB06-", startNo = 1, endNo = 78, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB06-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB06-XX", startNo = 1, endNo = 1, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB06-CP", startNo = 1, endNo = 2, digit = 2
                    }
                }
            }
        );
        // CB07
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "CB07",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB07-", startNo = 1, endNo = 62, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB07-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB07-XX", startNo = 1, endNo = 1, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB07-CP", startNo = 1, endNo = 3, digit = 2
                    },
                }
            }
        );
        // CB08
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "CB08",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB08-", startNo = 1, endNo = 79, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB08-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB08-XX", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // CB09
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "CB09",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB09-", startNo = 1, endNo = 78, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB09-CP", startNo = 1, endNo = 4, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB09-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB09-XX", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // CB10
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "CB10",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB10-", startNo = 1, endNo = 79, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB10-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB10-XX", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // CB11
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "CB11",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB11-", startNo = 1, endNo = 22, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB11-X", startNo = 1, endNo = 3, digit = 2
                    }
                }
            }
        );
        // CB12
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "CB12",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB12-", startNo = 1, endNo = 28, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB12-CP", startNo = 1, endNo = 5, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB12-X", startNo = 1, endNo = 4, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB12-XX", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // CB13_B
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "CB13_B",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB13-", startNo = 4, endNo = 4, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB13-", startNo = 10, endNo = 10, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB13-", startNo = 14, endNo = 14, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB13-", startNo = 27, endNo = 31, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB13-", startNo = 38, endNo = 39, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB13-", startNo = 46, endNo = 47, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB13-", startNo = 49, endNo = 49, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB13-", startNo = 51, endNo = 52, digit = 3
                    },
                }
            }
        );
        // CB13
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "CB13",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB13-", startNo = 1, endNo = 79, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB13-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB13-XX", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // CB14
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "CB14",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB14-", startNo = 1, endNo = 62, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB14-CP", startNo = 1, endNo = 2, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB14-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB14-XX", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // CB15_B
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "CB15_B",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB15-", startNo = 4, endNo = 4, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB15-", startNo = 19, endNo = 19, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB15-", startNo = 33, endNo = 34, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB15-", startNo = 39, endNo = 40, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB15-", startNo = 42, endNo = 42, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB15-", startNo = 44, endNo = 44, digit = 3
                    },
                }
            }
        );
        // CB15
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "CB15",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB15-", startNo = 1, endNo = 79, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB15-CP", startNo = 1, endNo = 4, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB15-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB15-XX", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // CB16_B
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "CB16_B",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB16-", startNo = 7, endNo = 7, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB16-", startNo = 22, endNo = 23, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB16-", startNo = 28, endNo = 28, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB16-", startNo = 33, endNo = 33, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB16-", startNo = 40, endNo = 40, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB16-", startNo = 50, endNo = 50, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB16-", startNo = 65, endNo = 65, digit = 3
                    },
                }
            }
        );
        // CB16
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "CB16",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB16-", startNo = 1, endNo = 79, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB16-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB16-XX", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // CB17_B
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "CB17_B",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB17-", startNo = 19, endNo = 19, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB17-", startNo = 45, endNo = 45, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB17-", startNo = 57, endNo = 57, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB17-", startNo = 66, endNo = 70, digit = 3
                    },
                }
            }
        );
        // CB17
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "CB17",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB17-", startNo = 1, endNo = 79, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB17-CP", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB17-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB17-XX", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // CB18_B
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "CB18_B",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB18-", startNo = 1, endNo = 1, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB18-", startNo = 12, endNo = 13, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB18-", startNo = 29, endNo = 29, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB18-", startNo = 33, endNo = 33, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB18-", startNo = 38, endNo = 38, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB18-", startNo = 44, endNo = 44, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB18-", startNo = 49, endNo = 49, digit = 3
                    },
                }
            }
        );
        // CB18
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "CB18",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB18-", startNo = 1, endNo = 62, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB18-X", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CB18-XX", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );


        // SD01
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD01",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD01-", startNo = 1, endNo = 15, digit = 3
                    }
                }
            }
        );
        // SD02
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD02",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD02-", startNo = 1, endNo = 21, digit = 3
                    }
                }
            }
        );
        // SD03
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD03",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD03-", startNo = 1, endNo = 17, digit = 3
                    }
                }
            }
        );
        // SD06
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD06",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD06-", startNo = 1, endNo = 17, digit = 3
                    }
                }
            }
        );
        // SD10
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD10",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD10-", startNo = 1, endNo = 16, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD10-X", startNo = 1, endNo = 2, digit = 2
                    }
                }
            }
        );
        // SD11
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD11",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD11-", startNo = 1, endNo = 16, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD11-X", startNo = 1, endNo = 2, digit = 2
                    }
                }
            }
        );
        // SD13
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD13",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD13-", startNo = 1, endNo = 8, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD13-X", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // SD14
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD14",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD14-", startNo = 1, endNo = 8, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD14-X", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // SD15
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD15",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD15-", startNo = 1, endNo = 8, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD15-X", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // SD16
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD16",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD16-", startNo = 1, endNo = 8, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD16-X", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // SD17
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD17",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD17-", startNo = 1, endNo = 16, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD17-X", startNo = 1, endNo = 2, digit = 2
                    }
                }
            }
        );
        // SD19
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD19",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD19-", startNo = 1, endNo = 16, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD19-X", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // SD20
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD20",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD20-", startNo = 1, endNo = 16, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD20-X", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // SD22
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD22",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD22-", startNo = 1, endNo = 16, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD22-X", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // SD23
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD23",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD23-", startNo = 1, endNo = 16, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD23-X", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // SD24
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD24",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD24-", startNo = 1, endNo = 16, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD24-X", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // SD25
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD25",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD25-", startNo = 1, endNo = 7, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD25-X", startNo = 1, endNo = 2, digit = 2
                    }
                }
            }
        );
        // SD26
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD26",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD26-", startNo = 1, endNo = 15, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD26-X", startNo = 1, endNo = 2, digit = 2
                    }
                }
            }
        );
        // SD27
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD27",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD27-", startNo = 1, endNo = 6, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD27-X", startNo = 1, endNo = 2, digit = 2
                    }
                }
            }
        );
        // SD28
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD28",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD28-", startNo = 1, endNo = 15, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD28-X", startNo = 1, endNo = 2, digit = 2
                    }
                }
            }
        );
        // SD29
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD29",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD29-", startNo = 1, endNo = 15, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD29-X", startNo = 1, endNo = 1, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD29-CP", startNo = 1, endNo = 2, digit = 2
                    }
                }
            }
        );
        // SD30
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD30",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD30-", startNo = 1, endNo = 2, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD30-X", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // SD31
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD31",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD31-", startNo = 1, endNo = 15, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD31-X", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // SD32
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD32",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD32-", startNo = 1, endNo = 7, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD32-X", startNo = 1, endNo = 2, digit = 2
                    }
                }
            }
        );
        // SD33
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD33",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD33-", startNo = 1, endNo = 14, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD33-X", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // SD34
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD34",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD34-", startNo = 1, endNo = 14, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD34-X", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // SD35
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD35",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD35-", startNo = 1, endNo = 15, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD35-X", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // SD36
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD36",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD36-", startNo = 1, endNo = 6, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD36-X", startNo = 1, endNo = 3, digit = 2
                    }
                }
            }
        );
        // SD37
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD37",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD37-", startNo = 1, endNo = 14, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD37-X", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // SD38
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD38",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD38-", startNo = 1, endNo = 14, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD38-X", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // SD39
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD39",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD39-", startNo = 1, endNo = 14, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD39-X", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // SD40
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD40",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD40-", startNo = 1, endNo = 8, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD40-X", startNo = 1, endNo = 3, digit = 2
                    }
                }
            }
        );
        // SD41
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD41",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD41-", startNo = 1, endNo = 8, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD41-X", startNo = 1, endNo = 2, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD41-RV%20X", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // SD42
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD42",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD42-", startNo = 1, endNo = 15, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD42-X", startNo = 1, endNo = 1, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD42-CP", startNo = 1, endNo = 10, digit = 2
                    }
                }
            }
        );
        // SD43
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "SD43",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD43-", startNo = 1, endNo = 10, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD43-RV", startNo = 1, endNo = 4, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD43-X", startNo = 1, endNo = 2, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD43-CP", startNo = 1, endNo = 2, digit = 2
                    }
                }
            }
        );
        // SD44
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD44",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD44-", startNo = 1, endNo = 10, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD44-RV", startNo = 1, endNo = 4, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD44-X", startNo = 1, endNo = 2, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD44-CP", startNo = 1, endNo = 2, digit = 2
                    }
                }
            }
        );
        // SD45
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD45",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD45-", startNo = 1, endNo = 14, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD45-X", startNo = 1, endNo = 1, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD45-CP", startNo = 1, endNo = 6, digit = 2
                    }
                }
            }
        );
        // SD46
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD46",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD46-", startNo = 1, endNo = 7, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD46-X", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD46-CP", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // SD47
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD47",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD47-", startNo = 1, endNo = 7, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD47-X", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD47-CP", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );
        // SD48
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD48",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD48-", startNo = 1, endNo = 14, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD48-X", startNo = 1, endNo = 3, digit = 2
                    }
                }
            }
        );
        // SD49
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD49",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD49-", startNo = 1, endNo = 7, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD49-CP", startNo = 1, endNo = 1, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD49-X", startNo = 1, endNo = 4, digit = 2
                    }
                }
            }
        );
        // SD50
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD50",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD50-", startNo = 1, endNo = 14, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD50-CP", startNo = 1, endNo = 5, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD50-X", startNo = 1, endNo = 2, digit = 2
                    }
                }
            }
        );
        // SD51
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD51",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD51-", startNo = 1, endNo = 10, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD51-CP", startNo = 1, endNo = 1, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD51-X", startNo = 1, endNo = 3, digit = 2
                    }
                }
            }
        );
        // SD52
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD52",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD52-", startNo = 1, endNo = 15, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD52-X", startNo = 1, endNo = 2, digit = 2
                    }
                }
            }
        );
        // SD53
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD53",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD53-", startNo = 1, endNo = 16, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD53-X", startNo = 1, endNo = 2, digit = 2
                    }
                }
            }
        );
        // SD54
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD54",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD54-", startNo = 1, endNo = 16, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD54-X", startNo = 1, endNo = 2, digit = 2
                    }
                }
            }
        );
        // SD55_B
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD55_B",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD55-", startNo = 10, endNo = 11, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD55-TX", startNo = 1, endNo = 1, digit = 2
                    },
                }
            }
        );
        // SD55
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD55",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD55-", startNo = 1, endNo = 13, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD55-X", startNo = 1, endNo = 1, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD55-TX", startNo = 1, endNo = 1, digit = 2
                    },
                }
            }
        );
        // SD56_B
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD56_B",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD56-", startNo = 1, endNo = 1, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD56-", startNo = 3, endNo = 3, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD56-TX", startNo = 1, endNo = 1, digit = 2
                    },
                }
            }
        );
        // SD56
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD56",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD56-", startNo = 1, endNo = 3, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD56-CP", startNo = 1, endNo = 1, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD56-RV", startNo = 1, endNo = 10, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD56-X", startNo = 1, endNo = 2, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD56-TX", startNo = 1, endNo = 1, digit = 2
                    },
                }
            }
        );
        // SD57_B
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD57_B",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD57-", startNo = 6, endNo = 6, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD57-TX", startNo = 1, endNo = 1, digit = 2
                    },
                }
            }
        );
        // SD57
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD57",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD57-", startNo = 1, endNo = 6, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD57-CP", startNo = 1, endNo = 1, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD57-RV", startNo = 1, endNo = 5, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD57-X", startNo = 1, endNo = 2, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD57-TX", startNo = 1, endNo = 1, digit = 2
                    },
                }
            }
        );
        // SD58
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD58",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD58-", startNo = 1, endNo = 15, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD58-CP", startNo = 1, endNo = 1, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD58-X", startNo = 1, endNo = 3, digit = 2
                    },
                }
            }
        );
        // SD59_B
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD59_B",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD59-", startNo = 2, endNo = 2, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD59-", startNo = 4, endNo = 4, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD59-TX", startNo = 1, endNo = 1, digit = 2
                    },
                }
            }
        );
        // SD59
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SD59",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD59-", startNo = 1, endNo = 7, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD59-CP", startNo = 1, endNo = 1, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD59-RV", startNo = 1, endNo = 5, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD59-X", startNo = 1, endNo = 1, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SD57-TX", startNo = 1, endNo = 1, digit = 2
                    },
                }
            }
        );



        // CP
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "CP",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CP13-", startNo = 1, endNo = 1, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CP14-", startNo = 1, endNo = 2, digit = 2
                    },
                    // TODO CP14-X01系　CP14-X02系　未実装
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CP14-X", startNo = 3, endNo = 13, digit = 2
                    },
                    // TODO CP14-X14系　未実装
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/CP14-X", startNo = 15, endNo = 22, digit = 2
                    }
                }
            }
        );

        // KA
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "KA",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/KA12-", startNo = 1, endNo = 1, digit = 2
                    }
                }
            }
        );

        // KF
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "KF",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/KF-", startNo = 1, endNo = 15, digit = 2
                    }
                }
            }
        );

        // P_B
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "P_B",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/P20-", startNo = 6, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/P21-", startNo = 4, endNo = 4, digit = 2
                    },
                }
            }
        );
        // P
        m_downLoadDataList.Add(
            new DownLoadData() {
                tag = "P",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/P", startNo = 1, endNo = 84, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/P13-", startNo = 1, endNo = 28, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/P14-", startNo = 1, endNo = 47, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/P14-X", startNo = 1, endNo = 7, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/P15-", startNo = 1, endNo = 25, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/P16-", startNo = 1, endNo = 8, digit = 2
                    },
                    // TODO P16-09系　未実装
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/P16-", startNo = 10, endNo = 27, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/P17-", startNo = 1, endNo = 24, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/P17-EXG", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/P18-", startNo = 1, endNo = 9, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/P19-", startNo = 1, endNo = 22, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/P20-", startNo = 1, endNo = 19, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/P21-", startNo = 1, endNo = 8, digit = 2
                    },
                }
            }
        );

        // PB_B
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "PB_B",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/PB09-", startNo = 2, endNo = 4, digit = 3
                    },
                }
            }
        );
        // PB
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "PB",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/PB01-H", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/PB02-G", startNo = 1, endNo = 10, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/PB05-D", startNo = 1, endNo = 4, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/PB08-", startNo = 1, endNo = 13, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/PB08-CP", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/PB08-X", startNo = 1, endNo = 2, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/PB09-", startNo = 1, endNo = 4, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/PB12-", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/PB13-", startNo = 1, endNo = 5, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/PB15-", startNo = 1, endNo = 4, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/PB16-D", startNo = 1, endNo = 5, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/PB16-DD", startNo = 1, endNo = 1, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/PB17-RV%20X", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/PB17-X", startNo = 1, endNo = 5, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/PB18-U", startNo = 1, endNo = 7, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/PB18-X", startNo = 1, endNo = 3, digit = 2
                    },
                }
            }
        );

        // PX
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "PX",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/PX", startNo = 1, endNo = 19, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/PX13-", startNo = 1, endNo = 5, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/PX14-", startNo = 1, endNo = 9, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/PX15-", startNo = 1, endNo = 8, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/PX16-", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/PX17-", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/PX18-", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/PX19-", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/PX20-", startNo = 1, endNo = 5, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/PX21-", startNo = 1, endNo = 2, digit = 2
                    },
                }
            }
        );

        // SJ
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "SJ",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SJ12-", startNo = 1, endNo = 8, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SJ13-", startNo = 1, endNo = 26, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SJ14-", startNo = 1, endNo = 29, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SJ15-", startNo = 1, endNo = 20, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SJ16-", startNo = 1, endNo = 16, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SJ17-", startNo = 1, endNo = 11, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SJ18-", startNo = 1, endNo = 10, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/SJ19-", startNo = 1, endNo = 8, digit = 2
                    }
                }
            }
        );

        // X
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "X",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/X", startNo = 1, endNo = 2, digit = 3
                    },
                    // TODO X003系　未実装
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/X", startNo = 4, endNo = 4, digit = 3
                    },
                    // TODO X005系　未実装
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/X", startNo = 6, endNo = 11, digit = 3
                    },
                    // TODO X012系　未実装
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/X", startNo = 13, endNo = 22, digit = 3
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/X13-", startNo = 1, endNo = 17, digit = 2
                    },
                }
            }
        );

        // EX
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "EX",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/EX", startNo = 1, endNo = 9, digit = 3
                    },
                }
            }
        );

        // LM
        m_downLoadDataList.Add(
            new DownLoadData()
            {
                tag = "LM",
                downLoadDetailList = new List<DownLoadDetailData>() {
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/LM13-", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/LM14-", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/LM15-", startNo = 1, endNo = 3, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/LM16-", startNo = 1, endNo = 9, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/LM16-D", startNo = 1, endNo = 5, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/LM17-", startNo = 1, endNo = 12, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/LM18-", startNo = 1, endNo = 10, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/LM18-G", startNo = 1, endNo = 7, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/LM19-", startNo = 1, endNo = 6, digit = 2
                    },
                    new DownLoadDetailData(){
                        host = "https://www.battlespirits.com/image/card_serch/card/LM19-U", startNo = 1, endNo = 8, digit = 2
                    }
                }
            }
        );
    }

    public IEnumerator LocalLoadTexture(Action callback = null) {
        /*m_packDataList.Clear();
        m_awakeningDataList.Clear();
        foreach (var downLoadData in m_downLoadDataList) {
            var directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_CARD + downLoadData.tag + "/";
            if (!Directory.Exists(directoryPath)) {
                yield return Directory.CreateDirectory(directoryPath);
                continue;
            }

            bool isTypeB = (downLoadData.tag.IndexOf("_B") != -1 && downLoadData.tag.IndexOf("_B") == downLoadData.tag.Length - 2);

            PackData packData = new PackData(){
                tag = downLoadData.tag,
                fileNameList = new List<string>()
            };

            foreach (var downLoadDetail in downLoadData.downLoadDetailList) {
                for (var index = downLoadDetail.startNo; index <= downLoadDetail.endNo; index++) {
                    var cardImagePath = downLoadDetail.host + index.ToString("D"+downLoadDetail.digit) + ".jpg";
                    var imageNameStartIndex = cardImagePath.LastIndexOf("/") + 1;
                    var imageNameCount = (cardImagePath.LastIndexOf(".jpg")) - imageNameStartIndex;
                    var imageName = cardImagePath.Substring(imageNameStartIndex, imageNameCount);
                    if (File.Exists(directoryPath + imageName + ".jpg")) {
                        packData.fileNameList.Add(imageName);
                    } else {
                        Debug.Log("fileName: " + directoryPath + imageName + ".jpg");
                    }
                }
            }

            if (isTypeB)
            {
                m_awakeningDataList.Add(packData);
            }
            else
            {
                m_packDataList.Add(packData);
            }
        }*/

        if (callback != null) {
            yield return null;
            callback();
        }
    }

    public IEnumerator DownLoadTexture(Action callback = null) {
        Debug.Log("directoryPath: " + ConstManager.DIRECTORY_FULL_PATH_TO_CARD_NORMAL);
        m_packDataList.Clear();
        foreach (var downLoadData in m_downLoadDataList) {
            var directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_CARD_NORMAL + downLoadData.tag + "/";
            if (!Directory.Exists(directoryPath))
            {
                yield return Directory.CreateDirectory(directoryPath);
            }

            bool isTypeB = (downLoadData.tag.IndexOf("_B") != -1 && downLoadData.tag.IndexOf("_B") == downLoadData.tag.Length - 2);

            PackData packData = new PackData(){
                tag = downLoadData.tag,
                fileNameList = new List<string>()
            };

            foreach (var downLoadDetail in downLoadData.downLoadDetailList) {
                for (var index = downLoadDetail.startNo; index <= downLoadDetail.endNo; index++) {
                    var cardImagePath = downLoadDetail.host + index.ToString("D"+downLoadDetail.digit) + ".jpg";
                    var imageNameStartIndex = cardImagePath.LastIndexOf("/") + 1;
                    var imageNameCount = (cardImagePath.LastIndexOf(".jpg")) - imageNameStartIndex;
                    var imageName = cardImagePath.Substring(imageNameStartIndex, imageNameCount);
                    if (File.Exists(directoryPath + imageName + ".jpg")) {
                        packData.fileNameList.Add(imageName);
                    } else {
                        if (imageName == "BS01-014" || imageName == "X004")
                        {
                            cardImagePath = downLoadDetail.host + index.ToString("D" + downLoadDetail.digit) + "A.jpg";
                        }
                        else if (isTypeB)
                        {
                            cardImagePath = downLoadDetail.host + index.ToString("D" + downLoadDetail.digit) + "_b.jpg";
                        }
                        Debug.Log("url: " + cardImagePath);
#pragma warning disable CS0618 // 型またはメンバーが旧型式です
                        using (WWW www = new WWW(cardImagePath)) {
                            yield return www;
                            byte[] bytes = www.texture.EncodeToPNG();
                            File.WriteAllBytes(directoryPath + imageName + ".jpg", bytes);
                            packData.fileNameList.Add(imageName);
                        }
#pragma warning restore CS0618 // 型またはメンバーが旧型式です
                    }

                    directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_CARD_NORMAL + downLoadData.tag + "/";
                    if (!File.Exists(directoryPath + imageName + ".txt")) {
                        var searchName = imageName;
                        if (searchName.LastIndexOf("_") != -1) {
                            searchName = searchName.Substring(0, searchName.LastIndexOf("_"));
                        }

                        if (searchName.IndexOf("BS38-RVX") != -1)
                        {
                            int start = searchName.LastIndexOf("BS38-RV") + 7;
                            int count = searchName.Length - start;
                            searchName = searchName.Substring(0, start) + "%20" + searchName.Substring(start, count);
                        }

                        if (searchName.IndexOf("BS39-RVX") != -1)
                        {
                            int start = searchName.LastIndexOf("BS39-RV") + 7;
                            int count = searchName.Length - start;
                            searchName = searchName.Substring(0, start) + "%20" + searchName.Substring(start, count);
                        }

                        if (isTypeB)
                        {
                            searchName = searchName + "(B)";
                        }
                        else
                        {
                            foreach (var awakeningData in m_awakeningDataList)
                            {
                                if (awakeningData.tag != downLoadData.tag + "_B")
                                {
                                    continue;
                                }
                                if (!awakeningData.fileNameList.Contains(imageName))
                                {
                                    continue;
                                }
                                searchName = searchName + "(A)";
                                break;
                            }
                        }

                        var cardDetailPath = "https://batspi.com/index.php?%E3%82%AB%E3%83%BC%E3%83%89%E6%A4%9C%E7%B4%A2&wpara=" + searchName;
                        Debug.Log("url: " + cardDetailPath);
                        using (UnityWebRequest www = UnityWebRequest.Get(cardDetailPath)) {
                            yield return www.SendWebRequest();
                            byte[] bytes = www.downloadHandler.data;
                            Debug.Log(www.downloadHandler.text);
                            var filePath = directoryPath + imageName + ".txt";
                            File.WriteAllBytes(filePath, bytes);
                            yield return null;
                            StreamReader streamReader = new StreamReader(filePath);
                            string tmpPath = Path.GetTempFileName();
                            StreamWriter streamWriter = new StreamWriter(tmpPath);

                            bool isWrite = false;
                            List<string> writeTypeList = new List<string>();
                            while (streamReader.Peek() > -1) {
                                string line = streamReader.ReadLine();
                                if (!isWrite && line == "<!-- バトスピWiki-2018 -->") {
                                    isWrite = true;
                                    writeTypeList.Clear();
                                }
                                if (isWrite && line.IndexOf("<tr><td><table class=\"BSTbl\"><tr>") != -1) {
                                    line = line.Replace("	", "");

                                    if (line.IndexOf("<img src=\"/card/symbol/1.gif\" alt=\"赤\" title=\"赤\">") != -1) {
                                        line = line.Replace("<img src=\"/card/symbol/1.gif\" alt=\"赤\" title=\"赤\">", "赤");
                                    }
                                    if (line.IndexOf("<img src=\"/card/symbol/2.gif\" alt=\"紫\" title=\"紫\">") != -1) {
                                        line = line.Replace("<img src=\"/card/symbol/2.gif\" alt=\"紫\" title=\"紫\">", "紫");
                                    }
                                    if (line.IndexOf("<img src=\"/card/symbol/3.gif\" alt=\"緑\" title=\"緑\">") != -1) {
                                        line = line.Replace("<img src=\"/card/symbol/3.gif\" alt=\"緑\" title=\"緑\">", "緑");
                                    }
                                    if (line.IndexOf("<img src=\"/card/symbol/4.gif\" alt=\"白\" title=\"白\">") != -1) {
                                        line = line.Replace("<img src=\"/card/symbol/4.gif\" alt=\"白\" title=\"白\">", "白");
                                    }
                                    if (line.IndexOf("<img src=\"/card/symbol/5.gif\" alt=\"黄\" title=\"黄\">") != -1) {
                                        line = line.Replace("<img src=\"/card/symbol/5.gif\" alt=\"黄\" title=\"黄\">", "黄");
                                    }
                                    if (line.IndexOf("<img src=\"/card/symbol/6.gif\" alt=\"青\" title=\"青\">") != -1) {
                                        line = line.Replace("<img src=\"/card/symbol/6.gif\" alt=\"青\" title=\"青\">", "青");
                                    }

                                    if (!writeTypeList.Contains("カード番号") && line.IndexOf("カード番号") != -1)
                                    {
                                        var start = line.IndexOf("カード番号") + 5;
                                        var count = line.Length - start;
                                        line = "{\"CardNo\":\"" + line.Substring(start, count);
                                        line = line.Substring(0, line.IndexOf("収録弾"));

                                        if (line.LastIndexOf("<") != -1)
                                        {
                                            line = line.Substring(0, line.LastIndexOf("<"));
                                        }

                                        while (line.IndexOf("<") != -1)
                                        {
                                            start = line.IndexOf("<");
                                            var end = line.IndexOf(">") + 1;
                                            count = line.Length - end;
                                            line = line.Substring(0, start) + line.Substring(end, count);
                                        }
                                        line += "\"";
                                        writeTypeList.Add("カード番号");
                                    }
                                    else if (!writeTypeList.Contains("属性") && line.IndexOf("属性") != -1)
                                    {
                                        var start = line.IndexOf("属性") + 2;
                                        var count = line.Length - start;
                                        line = ",\"Element\":\"" + line.Substring(start, count);
                                        line = line.Replace("カードカテゴリ", "\",\"CardCategory\":\"");
                                        line = line.Replace("系統", "\",\"CardType\":\"");
                                        line = line.Replace("レア度", "\",\"Rarity\":\"");

                                        if (line.LastIndexOf("<") != -1)
                                        {
                                            line = line.Substring(0, line.LastIndexOf("<"));
                                        }

                                        while (line.IndexOf("<") != -1)
                                        {
                                            start = line.IndexOf("<");
                                            var end = line.IndexOf(">") + 1;
                                            count = line.Length - end;
                                            line = line.Substring(0, start) + line.Substring(end, count);
                                        }

                                        line = line.Replace("&nbsp;", "");
                                        line += "\"";
                                        writeTypeList.Add("属性");
                                    }
                                    else if (!writeTypeList.Contains("コスト") && line.IndexOf("コスト") != -1)
                                    {
                                        var start = line.IndexOf("コスト") + 3;
                                        var count = line.Length - start;
                                        line = ",\"Cost\":\"" + line.Substring(start, count);
                                        line = line.Replace("軽減コスト", "\",\"ReducedCost\":\"");
                                        line = line.Replace("シンボル", "\",\"Symbol\":\"");

                                        while (line.IndexOf("<") != -1)
                                        {
                                            start = line.IndexOf("<");
                                            var end = line.IndexOf(">") + 1;
                                            count = line.Length - end;
                                            line = line.Substring(0, start) + line.Substring(end, count);
                                        }
                                        line += "\"";
                                        writeTypeList.Add("コスト");
                                    }
                                    else if (!writeTypeList.Contains("カード名") && line.IndexOf("カード名") != -1)
                                    {
                                        var start = line.IndexOf("カード名") + 4;
                                        var count = line.Length - start;
                                        line = ",\"CardName\":\"" + line.Substring(start, count);

                                        if (line.LastIndexOf("<") != -1)
                                        {
                                            line = line.Substring(0, line.LastIndexOf("<"));
                                        }

                                        while (line.IndexOf("<") != -1)
                                        {
                                            start = line.IndexOf("<");
                                            var end = line.IndexOf(">") + 1;
                                            count = line.Length - end;
                                            line = line.Substring(0, start) + line.Substring(end, count);
                                        }
                                        line += "\"";
                                        writeTypeList.Add("カード名");
                                    }
                                    else if (!writeTypeList.Contains("英語名") && line.IndexOf("英語名") != -1)
                                    {
                                        var start = line.IndexOf("英語名") + 3;
                                        var count = line.Length - start;
                                        line = ",\"EnglishName\":\"" + line.Substring(start, count);

                                        if (line.LastIndexOf("<a href=\"http:") != -1)
                                        {
                                            line = line.Substring(0, line.LastIndexOf("<a href=\"http:"));
                                        }

                                        if (line.Length - line.Replace("<td width=\"500\">", "td width=\"500\">").Length > 1)
                                        {
                                            if (line.LastIndexOf("<td width=\"500\">") != -1)
                                            {
                                                line = line.Substring(0, line.LastIndexOf("<td width=\"500\">"));
                                            }
                                        }
                                        
                                        var text = "\",\"Text\":\"";
                                        if (line.LastIndexOf("<td width=\"500\">") != -1)
                                        {
                                            line = line.Substring(0, line.LastIndexOf("</td>"));
                                            start = line.LastIndexOf("<td width=\"500\">");
                                            var end = line.LastIndexOf("</td>");
                                            count = end - start;
                                            text += line.Substring(start, count);
                                            line = line.Substring(0, start);
                                        }

                                        line = line.Replace("Lv", "\",\"Lv\":\"");

                                        line = line.Replace("合体", ",\"Lv\":\"0");
                                        line = line.Replace("<table ", "<").Replace("<table", "<");
                                        line = line.Replace("</table ", "<").Replace("</table", "<");
                                        line = line.Replace("<tr>", "").Replace("</tr>", "");
                                        line = line.Replace("<td>", "").Replace("</td>", "");
                                        line = line.Replace("<th>", "").Replace("</th>", "");
                                        line = line.Replace("<br />", "");
                                        line = line.Replace("<center>", "").Replace("</center>", "");
                                        line = line.Replace("td width=", "").Replace("th width=", "");
                                        line = line.Replace("class=\"BSTbl\">", ">");
                                        line = line.Replace("class=\"BSSymbol\">", ">");
                                        line = line.Replace("<>", "");
                                        line = line.Replace(" ", "");
                                        line = line.Replace("&nbsp;", "");

                                        while (line.IndexOf("<") != -1)
                                        {
                                            start = line.IndexOf("<");
                                            var end = line.IndexOf(">") + 1;
                                            count = line.Length - end;
                                            if (line.Substring(start + 1, 1) == "\"" && line.Substring(end - 2, 1) == "\"")
                                            {
                                                line = line.Substring(0, start) + line.Substring(end, count);
                                            }
                                            else
                                            {
                                                var substr = line.Substring(0, start) + "-" + line.Substring(start + 1, end - start - 2) + "-";
                                                if (count > 0)
                                                {
                                                    substr += line.Substring(end, count);
                                                }
                                                line = substr;
                                            }
                                        }

                                        line += text;
                                        line += "\"}";
                                        writeTypeList.Add("英語名");
                                    }

                                    streamWriter.WriteLine(line);
                                }
                            }
                            streamReader.Close();
                            streamWriter.Close();

                            File.Copy(tmpPath, filePath, true);
                            File.Delete(tmpPath);
                        }
                    }
                }
            }

            if (isTypeB)
            {
                m_awakeningDataList.Add(packData);
            }
            else
            {
                m_packDataList.Add(packData);
            }
        }

        if (callback != null) {
            callback();
        }
    }

     public IEnumerator CreateSharpnessTexture(Action callback = null)
    {
        foreach (var downLoadData in m_downLoadDataList)
        {
            var directoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_CARD_NORMAL + downLoadData.tag + "/";
            if (!Directory.Exists(directoryPath))
            {
                yield return Directory.CreateDirectory(directoryPath);
                continue;
            }

            var qualityDirectoryPath = ConstManager.DIRECTORY_FULL_PATH_TO_CARD_QUALITY + downLoadData.tag + "/";
            if (!Directory.Exists(qualityDirectoryPath))
            {
                yield return Directory.CreateDirectory(qualityDirectoryPath);
            }
            /*else
            {
                string[] filePaths = Directory.GetFiles(qualityDirectoryPath);
                foreach (string filePath in filePaths)
                {
                    File.SetAttributes(filePath, FileAttributes.Normal);
                    File.Delete(filePath);
                }
                Directory.Delete(qualityDirectoryPath);
                continue;
            }*/

            foreach (var downLoadDetail in downLoadData.downLoadDetailList)
            {
                for (var index = downLoadDetail.startNo; index <= downLoadDetail.endNo; index++)
                {
                    var cardImagePath = downLoadDetail.host + index.ToString("D" + downLoadDetail.digit) + ".jpg";
                    var imageNameStartIndex = cardImagePath.LastIndexOf("/") + 1;
                    var imageNameCount = (cardImagePath.LastIndexOf(".jpg")) - imageNameStartIndex;
                    var imageName = cardImagePath.Substring(imageNameStartIndex, imageNameCount);
                    if (!File.Exists(directoryPath + imageName + ".jpg"))
                    {
                        continue;
                    }
                    if (File.Exists(qualityDirectoryPath + imageName + ".jpg"))
                    {
                        continue;
                    }

                    Debug.Log("url: " + qualityDirectoryPath + imageName + ".jpg");

                    // int scale = 1; float level = 0.0f;
                    // int scale = 3; float level = 1.0f;
                    // int scale = 4; float level = 1.0f;
                    // int scale = 4; float level = 3.0f;
                    int scale = 4; float level = 5.0f;
                    // int scale = 4; float level = 10.0f;
                    // int scale = 5; float level = 10.0f;
                    // int scale = 6; float level = 10.0f;
                    // int scale = 6; float level = 12.0f;
                    Bitmap orijinal = (Bitmap)System.Drawing.Image.FromFile(directoryPath + imageName + ".jpg");
                    Bitmap srcImg = new Bitmap(orijinal, orijinal.Width * scale, orijinal.Height * scale);

                    Bitmap dstImg = new Bitmap(srcImg);
                    BitmapData srcDat = srcImg.LockBits(
                        new Rectangle(0, 0, srcImg.Width, srcImg.Height),
                        ImageLockMode.ReadWrite,
                        PixelFormat.Format32bppArgb
                    );

                    byte[] srcBuf = new byte[srcImg.Width * srcImg.Height * 4];
                    Marshal.Copy(srcDat.Scan0, srcBuf, 0, srcBuf.Length);

                    BitmapData dstDat = dstImg.LockBits(
                        new Rectangle(0, 0, dstImg.Width, dstImg.Height),
                        ImageLockMode.ReadWrite,
                        PixelFormat.Format32bppArgb
                    );
                    byte[] dstBuf = new byte[dstImg.Width * dstImg.Height * 4];
                    Marshal.Copy(dstDat.Scan0, dstBuf, 0, dstBuf.Length);

                    for (int i = 1; i < srcImg.Height - 1; i++)
                    {

                        int dy1 = (i - 1) * (srcImg.Width * 4);
                        int dy = i * (srcImg.Width * 4);
                        int dy2 = (i + 1) * (srcImg.Width * 4);

                        for (int j = 1; j < srcImg.Width - 1; j++)
                        {
                            int dx1 = (j - 1) * 4;
                            int dx = j * 4;
                            int dx2 = (j + 1) * 4;

                            for (int k = 0; k < 3; k++)
                            {
                                int value = (int)((float)srcBuf[dy + dx + k] * (1f + level * 4))
                                          - (int)((float)srcBuf[dy1 + dx + k] * level)
                                          - (int)((float)srcBuf[dy + dx1 + k] * level)
                                          - (int)((float)srcBuf[dy + dx2 + k] * level)
                                          - (int)((float)srcBuf[dy2 + dx + k] * level);
                                value = Math.Min(value, 255);
                                value = Math.Max(value, 0);

                                dstBuf[dy + dx + k] = (byte)value;
                            }
                        }
                    }
                    Marshal.Copy(dstBuf, 0, dstDat.Scan0, dstBuf.Length);
                    dstImg.UnlockBits(dstDat);
                    srcImg.UnlockBits(srcDat);

                    dstImg.Save(@"" + qualityDirectoryPath + imageName + ".jpg", ImageFormat.Jpeg);
                    yield return null;
                }
            }
        }

        if (callback != null)
        {
            callback();
        }
    }
}
