using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LocalGameManager : NetworkBehaviour {

    public static LocalGameManager Instance = null;

    public GameObject m_LocalPlayer;

    public GameObject[] m_LocalClassViewer;

    public bool m_GameIsStarted = false;

    public bool m_serverTimeSended = false;
    public bool m_timeIsSynced = false;
    public float m_ServerOffsetTime;
    public float m_InitialTimer = 0f;

    public static float m_MatchEndTime;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        DontDestroyOnLoad(transform.gameObject);

    }

    [ClientRpc]
    public void RpcNotifyServerTime(float time)
    {
        Debug.Log("Sto notificando il time");
        m_ServerOffsetTime = time - Time.time;
        m_timeIsSynced = true;
        m_serverTimeSended = true;
    }

    public float syncedTime()
    {
        return isServer ? Time.timeSinceLevelLoad  : Time.timeSinceLevelLoad + m_ServerOffsetTime;
    }


}
