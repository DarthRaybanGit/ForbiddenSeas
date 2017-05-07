using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Porto : NetworkBehaviour {

    public void OnCollisionEnter(Collision collision)
    {
        if (isServer)
        {
            if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<Player>().m_HasTreasure && collision.gameObject.GetComponent<FlagshipStatus>().m_Health > 0)
            {
                Debug.Log("Toccato il porto!");
                collision.gameObject.GetComponent<Player>().m_HasTreasure = false;
                collision.gameObject.GetComponent<Player>().ScoreAnARRH(collision.gameObject.GetComponent<Player>().netId);
            }
        }
    }
}
