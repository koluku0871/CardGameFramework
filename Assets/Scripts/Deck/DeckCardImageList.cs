using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.UIElements;

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
                deckCardImages[i].SetData(cardData.packNo, cardData.fileName, cardData.sprite);
                continue;
            }
            deckCardImages[i].gameObject.SetActive(false);
        }
    }
}
