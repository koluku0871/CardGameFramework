using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardListWindow : MonoBehaviour, IPunObservable
{
    [SerializeField]
    private PhotonView m_photonView = null;

    [SerializeField]
    private Image m_card = null;

    private CardOptionWindow.OPTION_TYPE m_optionType = CardOptionWindow.OPTION_TYPE.NONE;

    public List<DeckManager.CardDetail> m_cardDetailList = new List<DeckManager.CardDetail>();

    private bool m_isAction = false;

    private bool m_isWriting = true;

    private bool m_isEnemyOpen = false;

    private static CardListWindow instance = null;
    public static CardListWindow Instance()
    {
        return instance;
    }

    private void Awake()
    {
        if (m_photonView.IsMine || BattleSceneManager.IsNoPlayerInstance(m_photonView)) instance = this;

        GameObject canvasPanel = GameObject.FindGameObjectWithTag("BattleScenePanel");
        transform.SetParent(canvasPanel.transform);
        transform.SetAsLastSibling();
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        transform.localScale = Vector3.one;

        Close();
    }

    public void CreateCardList(List<DeckManager.CardDetail> cardDetailList)
    {
        foreach (var cardDetail in cardDetailList)
        {
            Image card = FieldCardManager.Instance().CreateCard(cardDetail, true, m_card, m_card.transform.parent,
                (Image target, string tag, string cardId, bool isDoubleClick) => {
                    if (!m_isAction || !m_photonView.IsMine) return;
                    CardOptionWindow.Instance().Open(target, m_optionType, CardOptionWindow.OPTION_TYPE.CARD_LIST,
                        (isAction) => {
                            if (isAction) DeleteCard(new DeckManager.CardDetail() { tag = tag, cardId = cardId });
                        });
                });
            card.name = m_card.name;
            card.gameObject.SetActive(true);
        }
    }

    public void DeleteCardList()
    {
        FieldCardManager fieldCardManager = FieldCardManager.Instance();

        foreach (Transform c in m_card.transform.parent)
        {
            if (c == m_card.transform)
            {
                continue;
            }

            Destroy(c.gameObject);
        }
    }

    public void DeleteCard(DeckManager.CardDetail cardDetail)
    {
        Debug.Log(JsonUtility.ToJson(cardDetail));

        m_cardDetailList.Remove(cardDetail);
        foreach (Transform c in m_card.transform.parent)
        {
            if (c == m_card.transform)
            {
                continue;
            }

            if (cardDetail.tag + "^" + cardDetail.cardId != c.name)
            {
                continue;
            }

            Destroy(c.gameObject);
            break;
        }
    }

    public void Open(CardOptionWindow.OPTION_TYPE optionType, List<DeckManager.CardDetail> cardDetailList, bool isAction = true, bool isEnemyOpen = false)
    {
        m_optionType = optionType;
        m_cardDetailList = cardDetailList;
        m_isAction = isAction;
        m_isWriting = true;
        m_isEnemyOpen = isEnemyOpen;

        CreateCardList(cardDetailList);

        Open();
    }

    public void Open()
    {
        transform.localScale = Vector3.one;
    }

    public void Close(bool isWriting = true)
    {
        m_isWriting = isWriting;

        DeleteCardList();

        m_cardDetailList.Clear();

        m_optionType = CardOptionWindow.OPTION_TYPE.NONE;

        transform.localScale = Vector3.zero;
    }

    public void OnClickToCloseButton()
    {
        Close();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //データの送信
            List<string> cardStrList = new List<string>();
            foreach (var cardDetail in m_cardDetailList)
            {
                cardStrList.Add(cardDetail.tag + "^" + cardDetail.cardId);
            }
            string cardListStr = string.Join(",", cardStrList);
            stream.SendNext(cardListStr);

            if (!m_isWriting)
            {
                if (transform.localScale != Vector3.zero)
                {
                    stream.SendNext(Vector3.one);
                }
            }
            else
            {
                switch (m_optionType)
                {
                    case CardOptionWindow.OPTION_TYPE.DECK:
                    case CardOptionWindow.OPTION_TYPE.CARD_LIST:
                        stream.SendNext(transform.localScale);
                        break;
                    case CardOptionWindow.OPTION_TYPE.SUB:
                    case CardOptionWindow.OPTION_TYPE.DAMAGE:
                    case CardOptionWindow.OPTION_TYPE.TRASH:
                    case CardOptionWindow.OPTION_TYPE.EXCLUSION:
                        if (m_isEnemyOpen)
                        {
                            stream.SendNext(transform.localScale);
                        }
                        else
                        {
                            stream.SendNext(Vector3.zero);
                        }
                        break;
                    default:
                        stream.SendNext(Vector3.zero);
                        break;
                }
            }
        }
        else
        {
            //データの受信
            List<string> cardStrList = new List<string>();
            string cardListStr = (string)stream.ReceiveNext();
            cardStrList.AddRange(cardListStr.Split(','));
            if (cardStrList.Count > 0 && !string.IsNullOrEmpty(cardStrList[0]))
            {
                DeleteCardList();
                m_cardDetailList.Clear();
                foreach (var cardStr in cardStrList)
                {
                    string[] cardList = cardStr.Split('^');
                    if (cardList.Length < 2) return;
                    var cardId = cardList[1];
                    m_cardDetailList.Add(new DeckManager.CardDetail() { tag = cardList[0], cardId = cardId });
                }
                CreateCardList(m_cardDetailList);
            }
            
            transform.localScale = (Vector3)stream.ReceiveNext();
        }
    }
}
