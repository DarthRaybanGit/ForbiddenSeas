using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Porto : NetworkBehaviour {


    public void OnTriggerEnter(Collider other)
    {
        if (isServer)
        {
            if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<Player>().m_HasTreasure && other.gameObject.GetComponent<FlagshipStatus>().m_Health > 0)
            {
                Debug.Log("Toccato il porto!");
                other.gameObject.GetComponent<Player>().m_HasTreasure = false;
				LocalGameManager.Instance.m_TreasureOwned = false;
                other.gameObject.GetComponent<Player>().ScoreAnARRH();
            }
        }
    }
}
