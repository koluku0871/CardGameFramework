using UnityEngine;
using UnityEngine.UI;

public class DropdownScrollTmp : MonoBehaviour
{
    private ScrollRect sr;

    public void Awake()
    {
        sr = this.gameObject.GetComponent<ScrollRect>();
    }

    public void Start()
    {
        int index = -1;
        TMPro.TMP_Dropdown tmpDropdown = GetComponentInParent<TMPro.TMP_Dropdown>();
        if (tmpDropdown != null)
        {
            index = tmpDropdown.value;
        }

        if (index > 0)
        {
            Dropdown dropdown = GetComponentInParent<Dropdown>();
            if (dropdown != null)
            {
                index = dropdown.value;
            }
        }

        if (index > 0)
        {
            var viewport = this.transform.Find("Viewport").GetComponent<RectTransform>();
            var contentArea = this.transform.Find("Viewport/Content").GetComponent<RectTransform>();
            var contentItem = this.transform.Find("Viewport/Content/Item").GetComponent<RectTransform>();

            // Viewportに対するContentのスクロール位置を求める
            var areaHeight = contentArea.rect.height - viewport.rect.height;
            var cellHeight = contentItem.rect.height;
            var scrollRatio = (cellHeight * index) / areaHeight;
            sr.verticalNormalizedPosition = 1.0f - Mathf.Clamp(scrollRatio, 0.0f, 1.0f);
        }
    }
}
