using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Treasure : NetworkBehaviour {

    public void OnTriggerEnter(Collider other)
    {
        if (!isServer)
        {
            if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<Player>().isLocalPlayer && other.gameObject.GetComponent<FlagshipStatus>().m_Health > 0)
            {
                Debug.Log("Toccato il tesoro!");
                other.gameObject.GetComponent<Player>().CmdCatchTheTreasure((int)other.gameObject.GetComponent<Player>().netId.Value);
            }
        }
    }

}
