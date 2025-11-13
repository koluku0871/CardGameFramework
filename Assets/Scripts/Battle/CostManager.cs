using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class CostManager : MonoBehaviour, IPunObservable
{
    public List<Transform> m_listAreas = new List<Transform>();

    public UnityEngine.UI.Toggle m_toggle = null;

    public string m_toggleName = "toggle_0";

    private void Awake()
    {
        GameObject panel = GameObject.FindGameObjectWithTag("CostPanel");
        transform.SetParent(panel.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
    }

    private void Start()
    {
        m_toggle.name = "toggle_0";
        m_toggle.onValueChanged.AddListener(OnValueChanged);

        for (int index = 0; index < m_listAreas.Count; index++)
        {
            if (BattleSceneManager.Instance().m_playerStatusList[index].IsNoPlayer())
            {
                m_listAreas[index].gameObject.SetActive(false);
                continue;
            }

            m_listAreas[index].gameObject.SetActive(true);

            int num = 0;
            foreach (Transform c in m_listAreas[index])
            {
                num++;
                UnityEngine.UI.Toggle toggle = c.GetComponent<UnityEngine.UI.Toggle>();
                if (toggle == null)
                {
                    continue;
                }
                toggle.name = "toggle_" + BattleSceneManager.Instance().m_playerStatusList[index].m_playerName + "_" + num;
                toggle.onValueChanged.AddListener(OnValueChanged);
            }
        }
    }

    public void OnValueChanged(bool state)
    {
        if (state)
        {
            this.gameObject.GetComponent<PhotonView>().RequestOwnership();
            UnityEngine.UI.Toggle activeToggle = this.gameObject.GetComponent<ToggleGroup>().GetFirstActiveToggle();
            m_toggleName = (activeToggle.name);

            var textObj = activeToggle.transform.Find("Text (TMP)");
            if (textObj != null)
            {
                TMPro.TextMeshProUGUI text = textObj.GetComponent<TMPro.TextMeshProUGUI>();
                if (text != null)
                {
                    if (int.TryParse(text.text.Replace("toggle_", ""), out int index))
                    {
                        PlayerFieldManager.Instance().AddLogList("コストを" + index + "に変更");
                    }
                }
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //データの送信
            stream.SendNext(m_toggleName);
        }
        else
        {
            //データの受信
            var toggleName = (string)stream.ReceiveNext();
            if (toggleName != m_toggleName)
            {
                if (m_toggle.name == toggleName)
                {
                    m_toggle.isOn = true;
                    m_toggleName = toggleName;
                }
                else
                {
                    for (int index = 0; index < m_listAreas.Count; index++)
                    {
                        if (BattleSceneManager.Instance().m_playerStatusList[index].IsNoPlayer())
                        {
                            continue;
                        }
                        foreach (Transform c in m_listAreas[index])
                        {
                            UnityEngine.UI.Toggle toggle = c.GetComponent<UnityEngine.UI.Toggle>();
                            if (toggle == null)
                            {
                                continue;
                            }
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
    }
}
