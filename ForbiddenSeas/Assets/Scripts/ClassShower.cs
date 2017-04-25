using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ClassShower : NetworkBehaviour {

    [SyncVar]
    public Color m_LocalClassViewer;


    public void setFlag(Color color)
    {
        m_LocalClassViewer = color;

    }

    [ClientRpc]
    public void RpcFlagUpdate()
    {
        if(isLocalPlayer)
            GetComponent<Image>().color = m_LocalClassViewer;
    }

}
