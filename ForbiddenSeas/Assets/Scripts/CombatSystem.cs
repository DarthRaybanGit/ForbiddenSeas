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
        if (isLocalPlayer)
        {
            Debug.Log(gameObject.name + "Preso danno");

            if (other.tag.Equals("mainAttack") || other.tag.Equals("specialAttack"))
            {
                int dmg = 0;
                if (other.tag.Equals("mainAttack"))
                    dmg = other.GetComponentInParent<FlagshipStatus>().m_main.damage;
                else
                    dmg = other.GetComponentInParent<FlagshipStatus>().m_special.damage;
                
                GetComponent<FlagshipStatus>().CmdTakeDamage(dmg, other.transform.parent.gameObject.GetComponent<NetworkBehaviour>().netId.Value.ToString());
            }
        }

    }

}
