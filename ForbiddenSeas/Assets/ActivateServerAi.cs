using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ActivateServerAi : NetworkBehaviour
{	
    public bool isPiranha = false;

    void Start()
    {
        if (isPiranha)
        {
            if (!isServer)
                transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            if (!isServer)
                transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
