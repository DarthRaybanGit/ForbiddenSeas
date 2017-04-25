using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour {

    [SyncVar]
    public int m_LocalClass = 0;

    public GameObject m_ClassViewerPrefab;

    public GameObject m_LocalClassViewer;

    private List<GameObject> m_ToSpawn;

    public void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("GameManager").Length > 1 && isLocalPlayer)
            Destroy(gameObject);

        DontDestroyOnLoad(transform.gameObject);


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


        m_LocalClassViewer.transform.position = p_list[which].transform.position;

        m_LocalClassViewer.GetComponent<ClassShower>().m_LocalClassViewer = (m_LocalClass == 0 ? Color.black : m_LocalClass == 1 ? Color.green : m_LocalClass == 2 ? Color.yellow : Color.white);

        m_LocalClassViewer.GetComponent<MeshRenderer>().material.color = m_LocalClassViewer.GetComponent<ClassShower>().m_LocalClassViewer;

        NetworkServer.Spawn(m_LocalClassViewer);

    }



}
