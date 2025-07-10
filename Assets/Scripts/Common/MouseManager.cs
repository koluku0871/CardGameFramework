using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MouseManager : MonoBehaviour
{
    private static MouseManager instance = null;
    public static MouseManager Instance()
    {
        return instance;
    }

    private int pointerId = 0;

    private int clickCount = 0;

    private Coroutine coroutine = null;

    public void OnEnable()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void OnClick(int pointerId, Action<int, bool> action)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        if (this.pointerId != pointerId)
        {
            this.pointerId = pointerId;
            clickCount = 0;
        }

        clickCount++;

        coroutine = StartCoroutine(DelayMethod(0.3f, action));
    }

    private IEnumerator DelayMethod(float waitTime, Action<int, bool> action)
    {
        yield return new WaitForSeconds(waitTime);
        action(this.pointerId, clickCount > 1);
        pointerId = 0;
        clickCount = 0;
    }
}
