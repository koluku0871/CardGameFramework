using System.Collections.Generic;


public class DownLoadDetailData {
    public string host = "";
    public int startNo = 0;
    public int endNo = 0;
    public int digit = 0;
}

public class DownLoadData {
    public string tag = "";
    public List<DownLoadDetailData> downLoadDetailList = new List<DownLoadDetailData>();
}
