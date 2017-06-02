using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class InGameCanvasController : MonoBehaviour
{

    public GameObject m_Clock;

    public float gapTime;

    // Use this for initialization
    void Start()
    {
        m_Clock.transform.SetParent(transform);
        m_Clock.SetActive(true);
    }

    public void Update()
    {
        if (LocalGameManager.Instance.m_GameIsStarted && (LocalGameManager.Instance.m_timeIsSynced || LocalGameManager.Instance.m_serverTimeSended))
        {
            float time = LocalGameManager.Instance.syncedTime() - gapTime;
            int minutes = Mathf.FloorToInt(time / 60);
            m_Clock.GetComponent<Text>().text = String.Format("{0,2:D2}:{1,2:D2}", minutes, Mathf.FloorToInt(time - minutes * 60));
        }
    }


}
