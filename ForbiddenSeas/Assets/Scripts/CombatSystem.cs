using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CombatSystem : NetworkBehaviour
{


    void mainAttack()
    {
    }

    void SpecialAttack()
    {

    }

    void OnTriggerStay(Collider other)
    {
        if (isLocalPlayer)//isServer)
        {
            Debug.Log(gameObject.name + "Preso danno");
            //gameObject.GetComponent<FlagshipStatus>().shipClass
            GetComponent<FlagshipStatus>().CmdTakeDamage(100, other.transform.parent.gameObject.GetComponent<NetworkBehaviour>().netId.Value.ToString());
        }

    }

}
