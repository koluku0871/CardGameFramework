using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DeckCardScrollSetup : UIBehaviour, IInfiniteScrollSetup
{
    [SerializeField, Range(1, 4000)]
    public int max = 30;

    public void OnPostSetupItems()
    {
        var infiniteScroll = GetComponent<InfiniteScroll>();
        infiniteScroll.onUpdateItem.AddListener(OnUpdateItem);
        GetComponentInParent<ScrollRect>().movementType = ScrollRect.MovementType.Elastic;

        SetRectTransform();
    }

    public void SetRectTransform()
    {
        var infiniteScroll = GetComponent<InfiniteScroll>();
        var rectTransform = GetComponent<RectTransform>();
        var delta = rectTransform.sizeDelta;
        delta.y = infiniteScroll.itemScale * max;
        rectTransform.sizeDelta = delta;
    }

    public void OnUpdateItem(int itemCount, GameObject obj)
    {
        if (itemCount < 0 || itemCount >= max)
        {
            obj.SetActive(false);
        }
        else
        {
            obj.SetActive(true);

            var item = obj.GetComponentInChildren<DeckCardImageList>();
            item.UpdateItem(itemCount);
        }
    }
}
