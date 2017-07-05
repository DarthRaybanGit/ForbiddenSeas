using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ActivateServerAi : NetworkBehaviour
{	
    public override void OnStartClient()
    {
        Destroy(gameObject);
    }
}
