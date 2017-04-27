using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OnlineManager : NetworkLobbyManager {


    public static OnlineManager s_Singleton;

    public GameObject[] m_playerPlacement;

    void Start()
    {
        s_Singleton = this;
        m_playerPlacement = new GameObject[4];
    }

}
