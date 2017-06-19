using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PowerUp : NetworkBehaviour {

    public PowerUP type;
    public NetworkInstanceId playerInside;
    public bool m_LockForFirstInside = false;

    public GameObject m_Inner;
    public GameObject m_Boundaries;

    public GameObject m_EmitterBound;
    public GameObject m_EmitterGain;

    [SyncVar]
    public int m_maxHealth = 5000;
    [SyncVar]
    public int m_health = 5000;


    public override void OnStartClient()
    {
        if (type == PowerUP.DAMAGE_UP)
        {
            transform.GetChild(0).gameObject.GetComponent<Animation>().Play();
            m_EmitterBound.GetComponent<ParticleSystem>().Play();
            m_EmitterBound.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
            m_EmitterBound.transform.GetChild(1).GetComponent<ParticleSystem>().Stop();
        }

    }

    public void OnTriggerEnter(Collider other)
    {

            if (!isServer && (type == PowerUP.REGEN || type == PowerUP.SPEED_UP))
            {
                if (other.tag.Equals("mainAttack") || other.tag.Equals("specialAttack"))
                {
                    int dmg = 0;
                    if (other.tag.Equals("mainAttack"))
                        dmg = other.GetComponentInParent<FlagshipStatus>().m_main;
                    else
                        dmg = other.GetComponentInParent<FlagshipStatus>().m_special;

                    if(other.gameObject.GetComponentInParent<Player>().isLocalPlayer)
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
            m_EmitterBound.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
            m_EmitterBound.transform.GetChild(1).GetComponent<ParticleSystem>().Stop();
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
                float time = Time.time;
                yield return new WaitUntil(() => Time.time - time >= (int)FixedDelayInGame.DAMAGEUP_TIME || !m_LockForFirstInside);
                if (m_LockForFirstInside)
                {
                    Debug.Log("Power up ottenuto da " + who);
                    NetworkServer.FindLocalObject(who).GetComponent<Player>().CatchAPowerUp(PowerUP.DAMAGE_UP, NetworkServer.FindLocalObject(who).GetComponent<Player>().playerId);
                    //Play animation di rewarding in client.
                    NetworkServer.FindLocalObject(who).GetComponent<Player>().RpcAvvisoPowerUp(who, "DamageUP");
                    RpcAnimatePowerUPGained();
                    yield return new WaitForSeconds(3.5f);
                    killMe();
                }
            }
            else
            {
                m_Inner.GetComponent<Animation>()["DamageUPScaling"].speed = 1.0f;
                m_Inner.GetComponent<Animation>().Play();
                m_EmitterBound.transform.GetChild(0).GetComponent<ParticleSystem>().Play();
                m_EmitterBound.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
            }
        }
    }

    [ClientRpc]
    public void RpcAnimatePowerUPGained()
    {

        //Aggiungere i particles.
        m_EmitterBound.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
        m_EmitterBound.transform.GetChild(1).GetComponent<ParticleSystem>().Stop();

        m_EmitterGain.SetActive(true);
    }

    public void killMe()
    {
        LocalGameManager.Instance.StartCoroutine(LocalGameManager.Instance.c_LoopPowerUp((int)FixedDelayInGame.POWERUP_SPAWN, SpawnIndex.REGEN + (int)type));
        Destroy(gameObject);
    }
}
