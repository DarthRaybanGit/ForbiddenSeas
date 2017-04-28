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
                transform.GetChild(1).gameObject.SetActive(true);
                Camera.main.enabled = false;
                transform.GetChild(1).tag = "MainCamera";
            }
        }
    }

    public void SetClass(int playerClass)
    {
        m_Class = playerClass;
    }

}
