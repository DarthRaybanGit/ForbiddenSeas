using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Treasure : NetworkBehaviour {

    public void OnCollisionEnter(Collision collision)
    {
        if (!isServer)
        {
            if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<Player>().isLocalPlayer && collision.gameObject.GetComponent<FlagshipStatus>().m_Health > 0)
            {
                Debug.Log("Toccato il tesoro!");
                collision.gameObject.GetComponent<Player>().CmdCatchTheTreasure((int)collision.gameObject.GetComponent<Player>().netId.Value);
            }
        }
    }
}
