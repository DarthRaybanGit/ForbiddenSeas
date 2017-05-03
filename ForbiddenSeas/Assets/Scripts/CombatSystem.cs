using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CombatSystem : NetworkBehaviour
{
    [ClientRpc]
    void RpcTakenDamage(string o)
    {
            Debug.Log(gameObject.name + "Colpito " + o);
    }

    void mainAttack()
    {
    }

    void SpecialAttack()
    {
        
    }

    void OnTriggerStay(Collider other)
    {
        if (true)//isServer)
        {
            Debug.Log(gameObject.name + "Preso danno");
            //gameObject.GetComponent<FlagshipStatus>().shipClass
            GetComponent<FlagshipStatus>().CmdTakeDamage(100);
            if(isServer)
                RpcTakenDamage(other.name);
        }
    }

}
