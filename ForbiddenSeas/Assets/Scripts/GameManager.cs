using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {

    public static GameManager Instance = null;

    [SyncVar]
    private int m_LocalClass = 0;

    public void Awake()
    {
        if (GameObject.FindGameObjectsWithTag("GameManager").Length > 1 && isLocalPlayer)
            Destroy(gameObject);

        if (Instance == null)
        {
            Instance = this;
        }
        DontDestroyOnLoad(transform.gameObject);
    }

    public override void OnStartClient()
    {

    }


    public int getLocalClass() {
        return m_LocalClass;
    }

    public void setLocalClass(int playerClass)
    {
        m_LocalClass = playerClass;
        Debug.Log("Class " + m_LocalClass);
    }

}
