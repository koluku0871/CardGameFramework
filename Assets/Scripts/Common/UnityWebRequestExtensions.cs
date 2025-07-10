using System.IO;
using System.Threading.Tasks;
using UnityEngine.Networking;

internal class StreamedDownloadHandler : DownloadHandlerScript
{
    private readonly Stream _writeStream;

    public StreamedDownloadHandler(Stream writeStream)
    {
        _writeStream = writeStream;
    }

    protected override bool ReceiveData(byte[] data, int dataLength)
    {
        _writeStream.Write(data, 0, dataLength);
        return true;
    }

    protected override void CompleteContent()
    {
        _writeStream.Flush();
        _writeStream.Seek(0, SeekOrigin.Begin);
    }
}

public static class UnityWebRequestExtensions
{
    public static async Task WriteToStreamAsync(this UnityWebRequest request, Stream stream)
    {
        var downloadHandler = new StreamedDownloadHandler(stream);
        request.downloadHandler = downloadHandler;

        request.SendWebRequest();

        while (request.isDone == false)
        {
            await Task.Delay(1);
        }
    }
}
