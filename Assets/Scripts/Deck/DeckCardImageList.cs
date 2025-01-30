using System.Collections.Generic;
using UnityEngine;

public class DeckCardImageList : MonoBehaviour
{
    public bool isSearchCard = true;

    [SerializeField]
    List<DeckCardImage> deckCardImages = new List<DeckCardImage>();

    public void UpdateItem(int count)
    {
        for (int i = 0; i < 4; i++)
        {
            if (isSearchCard)
            {
                CardData cardData = DeckSceneManager.Instance().GetCardDatas(count * 4 + i);
                if (cardData != null)
                {
                    deckCardImages[i].SetData(cardData.PackNo, cardData.fileName, cardData.Sprite);
                    continue;
                }
                deckCardImages[i].gameObject.SetActive(false);
            }
            else
            {
                CardData cardData = DeckSceneManager.Instance().GetFavoriteCardDatas(count * 4 + i);
                if (cardData != null)
                {
                    deckCardImages[i].SetData(cardData.PackNo, cardData.fileName, cardData.Sprite);
                    continue;
                }
                deckCardImages[i].gameObject.SetActive(false);
            }
        }
    }
}
