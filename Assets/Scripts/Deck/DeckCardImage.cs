using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DeckCardImage : MonoBehaviour
{
    [SerializeField]
    private Image m_image = null;

    private string targetTag = "";
    private string targetName = "";

    public void SetData(string tag, string name, Sprite sprite)
    {
        targetTag = tag;
        targetName = name;
        gameObject.name = name;
        m_image.sprite = sprite;
        gameObject.SetActive(true);
    }

    public void PointerEnter()
    {
        if (CardDetailManager.Instance().isLock)
        {
            return;
        }
        CardDetailManager.Instance().SetSprite(m_image.sprite);
        CardDetailManager.Instance().SetCardDetail(targetTag, targetName);
    }

    public void PointerClick(BaseEventData pointerEventData)
    {
        bool isPointerEvent = pointerEventData is PointerEventData;
        if (!isPointerEvent)
        {
            return;
        }

        switch ((pointerEventData as PointerEventData).pointerId)
        {
            case -1:
                Debug.Log("Left Click");
                break;
            case -2:
                Debug.Log("Right Click");
                DeckManager.Instance().AddCard(targetTag, targetName);
                break;
            case -3:
                Debug.Log("Middle Click");
                CardDetailManager.Instance().isLock = !CardDetailManager.Instance().isLock;
                break;
        }
    }
}
