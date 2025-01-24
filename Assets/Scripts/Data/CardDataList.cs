using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(menuName = "DB/CardDataList")]
public class CardDataList : ScriptableObject
{
    public List<CardData> cardDatas = new List<CardData>();
}
