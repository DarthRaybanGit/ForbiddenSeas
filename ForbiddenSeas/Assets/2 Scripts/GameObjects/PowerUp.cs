using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PowerUp : NetworkBehaviour {

    public PowerUP type;
    public Vector3 m_rot = new Vector3(15, 30, 45);

    public NetworkInstanceId playerInside;

    [SyncVar]
    public float m_health = 5000;


    public void OnTriggerEnter(Collider other)
    {
            if (type == PowerUP.REGEN || type == PowerUP.SPEED_UP)
            {
                if (other.tag.Equals("mainAttack") || other.tag.Equals("specialAttack"))
                {
                    int dmg = 0;
                    if (other.tag.Equals("mainAttack"))
                        dmg = other.GetComponentInParent<FlagshipStatus>().m_main;
                    else
                        dmg = other.GetComponentInParent<FlagshipStatus>().m_special;

                    other.gameObject.GetComponentInParent<CombatSystem>().CmdDamageThis(netId, dmg);
                }
            }
            else if(type == PowerUP.DAMAGE_UP && other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<FlagshipStatus>().m_Health > 0)
            {
                Debug.Log("Toccato un powerUp da " + other.gameObject.GetComponent<Player>().netId);
                powerUpFill(other.gameObject.GetComponentInParent<NetworkIdentity>().netId);
            }
    }

    public void OnTriggerStay(Collider other)
    {

    }

    IEnumerator powerUpFill(NetworkInstanceId who)
    {
        yield return null;

    }

    public void killMe()
    {
        Destroy(gameObject);
    }
}
