using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiCreateManager : MonoBehaviour
{
    [SerializeField]
    private Transform m_parent = null;

    [SerializeField]
    private GameObject m_background = null;

    private bool isEnd = false;

    private void Update()
    {
        if (isEnd)
        {
            return;
        }

        if (Time.time < 0.5f)
        {
            return;
        }

        Instantiate(m_background, m_parent);
        isEnd = true;
    }
}
