using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    public GameObject[] m_AdmiralList = new GameObject[4];
    public GameObject m_LocalCamera;



    [SyncVar]
    public int m_Class = 0;

    public override void OnStartLocalPlayer()
    {
        if (!isServer)
        {

            if (isLocalPlayer)
            {
                m_LocalCamera.SetActive(true);
                Camera.main.enabled = false;
                m_LocalCamera.tag = "MainCamera";
                CmdAskForCurrentTime();

            }
            LocalGameManager.Instance.m_GameIsStarted = true;
            LocalGameManager.Instance.m_InitialTimer = Time.time;
        }

    }

    public override void OnStartServer()
    {
        LocalGameManager.Instance.m_GameIsStarted = true;
        LocalGameManager.Instance.m_InitialTimer = Time.time;
    }

    [Command]
    public void CmdAskForCurrentTime()
    {
        LocalGameManager.Instance.m_serverTimeSended = true;
        LocalGameManager.Instance.RpcNotifyServerTime(Time.time);
    }

    public void SetClass(int playerClass)
    {
        m_Class = playerClass;
    }

}
