using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerManager : NetworkBehaviour {

    [SyncVar]
    public int m_LocalClass = 0;

    public GameObject m_ClassViewerPrefab;
    public GameObject m_PlayerPrefab;

    public GameObject m_LocalClassViewer;
    public int m_LocalClassViewerIndex = 99;

    private List<GameObject> m_ToSpawn;

    public void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    public void Start()
    {
        if(isLocalPlayer)
            LocalGameManager.Instance.m_LocalPlayer = gameObject;
        if (isServer)
        {
            LocalGameManager.Instance.m_LocalClassViewer = GameObject.FindGameObjectsWithTag("ClassViewer");
            LocalGameManager.Instance.m_serverTimeSended = true;
            LocalGameManager.Instance.RpcNotifyServerTime(Time.time);
        }
    }

    public int getLocalClass() {
        return m_LocalClass;
    }

    public void setLocalClass(int playerClass)
    {
        if (isLocalPlayer)
        {
            Debug.Log("Sto chiedendo al server di impostare la mia classe a " + playerClass);
            CmdSetLocalClass(playerClass);
        }
    }



    [Command]
    public void CmdSetLocalClass(int playerClass)
    {
        m_LocalClass = playerClass;
        Debug.Log("Class " + m_LocalClass);

        if(m_LocalClassViewer)
            Destroy(m_LocalClassViewer);

        var viewer = (GameObject)Instantiate(m_ClassViewerPrefab);


        m_LocalClassViewer = viewer;

        //m_LocalClassViewer.GetComponent<ClassShower>().m_PlayerOwner = gameObject;


        int which = (int)this.netId.Value % 4;

        if (m_LocalClassViewerIndex != 99)
        {
            which = m_LocalClassViewerIndex;
        }
        else
        {
            if (OnlineManager.s_Singleton.m_playerPlacement[which] == null)
                OnlineManager.s_Singleton.m_playerPlacement[which] = m_LocalClassViewer;
            else
            {
                bool finded = false;
                for (which = 0; which < 4; which++)
                {
                    if (OnlineManager.s_Singleton.m_playerPlacement[which] == null)
                    {
                        finded = true;
                        break;
                    }
                }
                if (!finded)
                    return;
            }
        }

        m_LocalClassViewer.transform.position = LocalGameManager.Instance.m_LocalClassViewer[which].transform.position;

        if(m_LocalClassViewerIndex == 99)
            m_LocalClassViewerIndex = which;

        m_LocalClassViewer.GetComponent<ClassShower>().m_LocalClassViewer = (m_LocalClass == (int) FlagshipStatus.ShipClass.pirates ? Color.black : m_LocalClass == (int)FlagshipStatus.ShipClass.venetians ? Color.green : m_LocalClass == (int)FlagshipStatus.ShipClass.vikings ? Color.yellow : Color.white);

        m_LocalClassViewer.GetComponent<MeshRenderer>().material.color = m_LocalClassViewer.GetComponent<ClassShower>().m_LocalClassViewer;

        NetworkServer.Spawn(m_LocalClassViewer);

    }



}
