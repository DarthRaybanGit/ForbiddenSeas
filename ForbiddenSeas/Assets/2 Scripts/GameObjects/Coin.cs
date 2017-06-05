using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Coin : NetworkBehaviour {

    public int m_IndexInPool;

    private void OnTriggerEnter(Collider other)
    {
        if (isServer)
        {
            if (other.gameObject.GetComponent<Player>())
            {
                other.gameObject.GetComponent<FlagshipStatus>().m_reputation += ReputationValues.COIN;
                LocalGameManager.Instance.m_CoinsPresence[m_IndexInPool] = false;
                Destroy(gameObject);
            }
        }
    }
}
