using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Treasure : NetworkBehaviour {


    public override void OnStartClient()
    {
        LocalGameManager.Instance.m_TreasureIsInGame = true;
        LocalGameManager.Instance.m_Treasure = gameObject;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (isServer)
        {
            if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<FlagshipStatus>().m_Health > 0 && !other.gameObject.GetComponent<Player>().m_HasTreasure)
            {
                Debug.Log("Toccato il tesoro!");
                other.gameObject.GetComponent<Player>().CatchTheTreasure();
            }
        }
    }

}
