using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ActivateServerAi : NetworkBehaviour
{	
    void Start()
    {
        if (!isServer)
            transform.GetChild(0).gameObject.SetActive(false);
    }
}
