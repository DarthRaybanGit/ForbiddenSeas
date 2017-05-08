using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ClassShower : NetworkBehaviour {

    [SyncVar]
    public Color m_LocalClassViewer = Color.white;

    public GameObject m_PlayerOwner;


    public void setFlag(Color color)
    {
        m_LocalClassViewer = color;

    }

    public void Update()
    {
        if (!isServer)
            return;

        RpcSetFlag();
    }

    [ClientRpc]
    public void RpcSetFlag()
    {
        gameObject.GetComponent<MeshRenderer>().material.color = m_LocalClassViewer;
    }

}
