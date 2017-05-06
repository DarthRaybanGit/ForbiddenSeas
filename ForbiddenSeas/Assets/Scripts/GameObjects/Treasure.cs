using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Treasure : NetworkBehaviour {

    public void OnCollisionEnter(Collision collision)
    {
        if (!isServer)
        {
            if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<Player>().isLocalPlayer)
            {
                collision.gameObject.GetComponent<Player>().CmdCatchTheTreasure(LocalGameManager.Instance.GetPlayerId(collision.gameObject));
            }
        }
    }
}
