using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class CoinManager : MonoBehaviour, IPunObservable
{
    [SerializeField]
    private TMPro.TextMeshProUGUI m_openText = null;

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

    public void SetIsOpen(string text)
    {
        this.gameObject.GetComponent<PhotonView>().RequestOwnership();
        m_openText.text = text;
        PlayerFieldManager.Instance().AddLogList("コインの結果は" + m_openText.text);
        this.gameObject.SetActive(true);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //データの送信
            stream.SendNext(m_openText.text);
        }
        else
        {
            //データの受信
            m_openText.text = (string)stream.ReceiveNext();
        }
    }
}
