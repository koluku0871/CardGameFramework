using System.Collections.Generic;
using UnityEngine;

public class DeckCardImageList : MonoBehaviour
{
    [SerializeField]
    List<DeckCardImage> deckCardImages = new List<DeckCardImage>();

    public void UpdateItem(int count)
    {
        for (int i = 0; i < 4; i++)
        {
            CardData cardData = DeckSceneManager.Instance().GetCardDatas(count * 4 + i);
            if(cardData != null)
            {
                deckCardImages[i].SetData(cardData.PackNo, cardData.fileName, cardData.Sprite);
                continue;
            }
            deckCardImages[i].gameObject.SetActive(false);
        }
    }
}
