using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PowerUp : NetworkBehaviour {

    public PowerUP type;
    public Vector3 m_rot = new Vector3(15, 30, 45);

    public NetworkInstanceId playerInside;
    public bool m_LockForFirstInside = false;

    public GameObject m_Inner;
    public GameObject m_Boundaries;


    [SyncVar]
    public float m_health = 5000;


    public override void OnStartClient()
    {
        if (type == PowerUP.DAMAGE_UP)
            transform.GetChild(0).gameObject.GetComponent<Animation>().Play();
    }

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
            else if(type == PowerUP.DAMAGE_UP && !m_LockForFirstInside && other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<FlagshipStatus>().m_Health > 0)
            {
                Debug.Log("Toccato un powerUp da " + other.gameObject.GetComponent<Player>().netId);
                StartCoroutine(powerUpFill(other.gameObject.GetComponent<Player>().netId));
            }
    }


    public void OnTriggerExit(Collider other)
    {
        if (type == PowerUP.DAMAGE_UP && other.gameObject.CompareTag("Player"))
        {
            if(!isServer && m_Inner.GetComponent<Animation>().isPlaying)
                m_Inner.GetComponent<Animation>()["DamageUPScaling"].speed = -3.0f;
            m_LockForFirstInside = false;
        }
    }

    IEnumerator powerUpFill(NetworkInstanceId who)
    {
        yield return new WaitForSeconds(0.2f);
        if (!m_LockForFirstInside)
        {
            m_LockForFirstInside = true;
            playerInside = who;

            //Invio da startare l'animazione solo al client...forse è meglio
            if (isServer)
            {
                yield return new WaitForSeconds((int)FixedDelayInGame.DAMAGEUP_TIME);
                if (m_LockForFirstInside)
                {
                    Debug.Log("Power up ottenuto da " + who);
                    NetworkServer.FindLocalObject(who).GetComponent<Player>().CatchAPowerUp(PowerUP.DAMAGE_UP);
                    //Play animation di rewarding in client.
                    RpcAnimatePowerUPGained();
                    killMe();
                }
            }
            else
            {
                m_Inner.GetComponent<Animation>()["DamageUPScaling"].speed = 1.0f;
                m_Inner.GetComponent<Animation>().Play();
            }
        }
    }

    [ClientRpc]
    public void RpcAnimatePowerUPGained()
    {

        //Aggiungere i particles.

    }

    public void killMe()
    {
        Destroy(gameObject);
    }
}
