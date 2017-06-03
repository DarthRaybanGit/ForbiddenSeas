using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateToCanali : MonoBehaviour {

	private void OnTriggerEnter(Collider other)
    {
        if (LocalGameManager.Instance.isServer)
        {
            if (other.gameObject.GetComponent<Player>())
            {
                other.gameObject.GetComponent<Player>().m_InsideArena = !other.gameObject.GetComponent<Player>().m_InsideArena;
            }
        }
    }
}
