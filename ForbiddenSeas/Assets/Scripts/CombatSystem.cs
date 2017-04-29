using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CombatSystem : NetworkBehaviour
{
    [ClientRpc]
    void RpcTakeDamage(string o)
    {
        Debug.Log(gameObject.name + "Colpito " + o);
    }

    void mainAttack()
    {
    }

    void SpecialAttack()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (isServer)
        {
            RpcTakeDamage(other.name);
        }
    }

}
