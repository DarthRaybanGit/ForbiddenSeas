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

    private List<GameObject> m_ToSpawn;

    public void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    public void Start()
    {
        LocalGameManager.Instance.m_LocalPlayer = gameObject;
    }

    public int getLocalClass() {
        return m_LocalClass;
    }

    public void setLocalClass(int playerClass)
    {
        if (isLocalPlayer)
        {
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

        GameObject[] p_list = GameObject.FindGameObjectsWithTag("ClassViewer");
        int which = (int)this.netId.Value % 4;

        if(OnlineManager.s_Singleton.m_playerPlacement[which] == null)
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

        m_LocalClassViewer.transform.position = p_list[which].transform.position;

        m_LocalClassViewer.GetComponent<ClassShower>().m_LocalClassViewer = (m_LocalClass == 0 ? Color.black : m_LocalClass == 1 ? Color.green : m_LocalClass == 2 ? Color.yellow : Color.white);

        m_LocalClassViewer.GetComponent<MeshRenderer>().material.color = m_LocalClassViewer.GetComponent<ClassShower>().m_LocalClassViewer;

        NetworkServer.Spawn(m_LocalClassViewer);

    }



}
