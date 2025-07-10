using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CostManager : MonoBehaviour, IPunObservable
{
    public List<Toggle> m_toggleList = new List<Toggle>();

    public string m_toggleName = "toggle_0";

    private void Awake()
    {
        GameObject panel = GameObject.FindGameObjectWithTag("CostPanel");
        transform.SetParent(panel.transform);
        transform.localPosition = Vector3.zero;

        if (PhotonNetwork.IsMasterClient)
        {
            this.transform.localRotation = Quaternion.identity;
        }
        else
        {
            this.transform.localRotation = Quaternion.Euler(0, 0, 180);
        }
    }

    private void Start()
    {
        for(int index = 0; index < m_toggleList.Count; index++)
        {
            var toggle = m_toggleList[index];
            toggle.name = "toggle_" + index;
            toggle.onValueChanged.AddListener(OnValueChanged);
        }
    }

    public void OnValueChanged(bool state)
    {
        if (state)
        {
            this.gameObject.GetComponent<PhotonView>().RequestOwnership();
            Toggle activeToggle = this.gameObject.GetComponent<ToggleGroup>().GetFirstActiveToggle();
            m_toggleName = (activeToggle.name);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //�f�[�^�̑��M
            stream.SendNext(m_toggleName);
        }
        else
        {
            //�f�[�^�̎�M
            var toggleName = (string)stream.ReceiveNext();
            if (toggleName != m_toggleName)
            {
                for (int index = 0; index < m_toggleList.Count; index++)
                {
                    var toggle = m_toggleList[index];
                    if (toggle.name != toggleName)
                    {
                        continue;
                    }
                    toggle.isOn = true;
                    m_toggleName = toggleName;
                }
            }
        }
    }
}
