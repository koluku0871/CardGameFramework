using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour, IPunObservable
{
    [SerializeField]
    private TMPro.TextMeshProUGUI m_openText = null;

    private bool m_isOpen = false;
    public bool IsOpen
    {
        private set
        {
            m_isOpen = value;
            if (m_isOpen)
            {
                m_openText.text = "1P";
            }
            else
            {
                m_openText.text = "2P";
            }
            PlayerFieldManager.Instance().AddLogList("先行は" + m_openText.text);
        }
        get
        {
            return m_isOpen;
        }
    }

    private void Awake()
    {
        GameObject coinPanel = GameObject.FindGameObjectWithTag("CoinPanel");
        transform.SetParent(coinPanel.transform);
        transform.localPosition = Vector3.zero;
    }

    public void OnClickButton()
    {
        this.gameObject.SetActive(false);
    }

    public void SetIsOpen(bool isOpen)
    {
        this.gameObject.GetComponent<PhotonView>().RequestOwnership();
        IsOpen = isOpen;
        this.gameObject.SetActive(true);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //データの送信
            stream.SendNext(IsOpen);
        }
        else
        {
            //データの受信
            IsOpen = (bool)stream.ReceiveNext();
        }
    }
}
