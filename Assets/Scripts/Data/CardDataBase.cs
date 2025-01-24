using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "DB/CardDataBase")]
public class CardDataBase : ScriptableObject
{
    public List<CardDataList> cardDatasToBS = new List<CardDataList>();
    public List<CardDataList> cardDatasToBSC = new List<CardDataList>();
    public List<CardDataList> cardDatasToCB = new List<CardDataList>();
    public List<CardDataList> cardDatasToSD = new List<CardDataList>();
    public List<CardDataList> cardDatas = new List<CardDataList>();
}