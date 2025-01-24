using AsyncReader;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(menuName = "DB/CardData")]
public class CardData : ScriptableObject
//public class CardData
{
    public string directoryPath;
    public string fileName;

    public byte[] bytes;
    public Sprite sprite = null;

    /*public async Task<Sprite> GetSprite()
    {
        if (sprite != null)
        {
            return sprite;
        }

        using AsyncFileReader reader = new AsyncFileReader();
        (IntPtr ptr, long size) = await reader.LoadAsync(directoryPath);
        ImageInfo info = AsyncReader.ImageConverter.Decode(ptr, (int)size);

        Texture2D texture = new Texture2D(info.header.width, info.header.height, info.header.Format, false);
        texture.LoadRawTextureData(info.buffer, info.fileSize);
        texture.Apply();

        sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        return sprite;
    }*/

    public string cardNo;
    public string packNo;
    public string cardRarity;
    public string cost;
    public string cardCategory;
    public string cardType;
    public string cardName;
    public string element;
    public string reducedCost;
    public List<string> levelTextList = new List<string>();
    public string text;
}

public static class ResourceRequestExtenion
{
    // Resources.LoadAsyncの戻り値であるResourceRequestにGetAwaiter()を追加する
    public static TaskAwaiter<UnityEngine.Object> GetAwaiter(this ResourceRequest resourceRequest)
    {
        var tcs = new TaskCompletionSource<UnityEngine.Object>();
        resourceRequest.completed += operation =>
        {
            // ロードが終わった時点でTaskCompletionSource.TrySetResult
            tcs.TrySetResult(resourceRequest.asset);
        };

        // TaskCompletionSource.Task.GetAwaiter()を返す
        return tcs.Task.GetAwaiter();
    }
}
