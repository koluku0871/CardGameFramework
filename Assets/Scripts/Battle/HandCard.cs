using Photon.Pun;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField]
    private Text m_openText = null;

    [SerializeField]
    private bool m_isMoveHand = true;

    public bool m_isOpenHand = true;

    private Image m_image = null;
    private EventTrigger m_eventTrigger = null;
    public Vector2 m_startPos = new Vector2();

    private bool m_isOpen = false;
    public bool IsOpen
    {
        private set
        {
            m_isOpen = value;

            if (m_isOpen)
            {
                m_openText.text = "表";
                string[] namelist = this.gameObject.name.Split('^');
                m_image.sprite = CardDetailManager.Instance().GetCardSprite(new DeckManager.CardDetail() { tag = namelist[0], cardId = namelist[1] });
            }
            else
            {
                m_openText.text = "裏";
                m_image.sprite = CardDetailManager.Instance().GetSleeveSprite();
            }
        }
        get
        {
            return m_isOpen;
        }
    }

    public void Awake()
    {
        m_image = gameObject.GetComponent<Image>();
        if (m_image == null) m_image = gameObject.AddComponent<Image>();

        m_eventTrigger = gameObject.GetComponent<EventTrigger>();
        if (m_eventTrigger == null) m_eventTrigger = gameObject.AddComponent<EventTrigger>();
    }

    public void OnBeginDrag(PointerEventData data)
    {
        m_startPos = data.position;
    }

    public void OnDrag(PointerEventData data)
    {
    }

    public void OnEndDrag(PointerEventData data)
    {
        if (!m_isMoveHand)
        {
            return;
        }

        Vector2 pos = m_startPos - data.position;
        Debug.Log(pos.magnitude);
        if (pos.magnitude > 50)
        {
            PlayerFieldManager.Instance().CreateCard(gameObject.name, false);
            PlayerFieldManager.Instance().AddLogList(gameObject.name + "をフィールドに出す");
            AudioSourceManager.Instance().PlayOneShot(0);
            Destroy(gameObject);
            return;
        }
    }

    public void SetIsOpen(bool isOpen)
    {
        IsOpen = isOpen;
    }
}
