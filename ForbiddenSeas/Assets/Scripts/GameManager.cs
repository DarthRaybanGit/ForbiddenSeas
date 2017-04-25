using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour {

    [SyncVar]
    private int m_LocalClass = 0;

    public GameObject m_ClassViewerPrefab;

    private GameObject m_LocalClassViewer;

    private List<GameObject> m_ToSpawn;

    public void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("GameManager").Length > 1 && isLocalPlayer)
            Destroy(gameObject);

        DontDestroyOnLoad(transform.gameObject);

        GameObject[] p_list = GameObject.FindGameObjectsWithTag("ClassViewer");

        int which = (int)this.netId.Value % 4;

        var viewer = (GameObject)Instantiate(m_ClassViewerPrefab);
        viewer.transform.SetParent(GameObject.Find("Canvas").transform);
        viewer.GetComponent<RectTransform>().anchoredPosition = p_list[which].GetComponent<RectTransform>().anchoredPosition;

        viewer.GetComponent<Image>().color = Color.blue;

        m_LocalClassViewer = viewer;
    }

    public override void OnStartServer()
    {
        NetworkServer.Spawn(m_LocalClassViewer);
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

        m_LocalClassViewer.GetComponent<ClassShower>().setFlag(m_LocalClass == 0 ? Color.black : m_LocalClass == 1 ? Color.green : m_LocalClass == 2 ? Color.yellow : Color.white);
    }

}
