using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class SupportShip : NetworkBehaviour {

    public enum SupportShipType { Attacker, Defenser };

    public GameObject m_Flagship;
    public Transform m_Destination;

    public FlagshipStatus.ShipClass m_Classe;
    public SupportShipType m_Tipo;

    public int m_MaxHealth;

    [SyncVar]
    public float m_maxSpeed;

    public float m_maksuSpeedo;

    [SyncVar]
    public int m_Health;

    [SyncVar]
    public bool m_isDead = false;
    [SyncVar]
    public int m_DoT = 0;

    [SyncVar]
    public int m_main;
    [SyncVar]
    public float m_mainCD;

    [SyncVar]
    public SyncListBool debuffList = new SyncListBool();
    [SyncVar]
    public SyncListBool buffList = new SyncListBool();

    [SyncVar]
    public float m_defense;

    public GameObject m_AI;


    public GameObject[] MainParticles;

    public void Start()
    {
        InizializeSupportShip(SupportShipType.Attacker, m_Flagship);
    }

    public void InizializeSupportShip(SupportShipType tipo, GameObject owner)
    {
        /*
        if (!isServer)
            return;
        */


        m_Flagship = owner;
        m_Tipo = tipo;

        switch ((int)m_Classe)
        {
            case (int)FlagshipStatus.ShipClass.pirates:
                if(tipo == SupportShipType.Attacker)
                {
                    m_MaxHealth = (int)(Pirates.maxHealth * (int)SupportShipStatistics.ATTACKER_HP / 100);
                    m_main = (int)(Pirates.mainAttackDmg * (int)SupportShipStatistics.ATTACKER_DMG / 100);
                    m_defense = (int)(Pirates.defense * (int)SupportShipStatistics.ATTACKER_DEF / 100);
                }
                else
                {
                    m_MaxHealth = (int)(Pirates.maxHealth * (int)SupportShipStatistics.DEFENSER_HP / 100);
                    m_main = (int)(Pirates.mainAttackDmg * (int)SupportShipStatistics.DEFENSER_DMG / 100);
                    m_defense = (int)(Pirates.defense * (int)SupportShipStatistics.DEFENSER_DEF / 100);
                }

                m_maxSpeed = Pirates.maxSpeed;

                break;

            case (int)FlagshipStatus.ShipClass.egyptians:
                if (tipo == SupportShipType.Attacker)
                {
                    m_MaxHealth = (int)(Egyptians.maxHealth * (int)SupportShipStatistics.ATTACKER_HP / 100);
                    m_main = (int)(Egyptians.mainAttackDmg * (int)SupportShipStatistics.ATTACKER_DMG / 100);
                    m_defense = (int)(Egyptians.defense * (int)SupportShipStatistics.ATTACKER_DEF / 100);
                }
                else
                {
                    m_MaxHealth = (int)(Egyptians.maxHealth * (int)SupportShipStatistics.DEFENSER_HP / 100);
                    m_main = (int)(Egyptians.mainAttackDmg * (int)SupportShipStatistics.DEFENSER_DMG / 100);
                    m_defense = (int)(Egyptians.defense * (int)SupportShipStatistics.DEFENSER_DEF / 100);
                }

                m_maxSpeed = Egyptians.maxSpeed;
                break;

            case (int)FlagshipStatus.ShipClass.vikings:
                if (tipo == SupportShipType.Attacker)
                {
                    m_MaxHealth = (int)(Vikings.maxHealth * (int)SupportShipStatistics.ATTACKER_HP / 100);
                    m_main = (int)(Vikings.mainAttackDmg * (int)SupportShipStatistics.ATTACKER_DMG / 100);
                    m_defense = (int)(Vikings.defense * (int)SupportShipStatistics.ATTACKER_DEF / 100);
                }
                else
                {
                    m_MaxHealth = (int)(Vikings.maxHealth * (int)SupportShipStatistics.DEFENSER_HP / 100);
                    m_main = (int)(Vikings.mainAttackDmg * (int)SupportShipStatistics.DEFENSER_DMG / 100);
                    m_defense = (int)(Vikings.defense * (int)SupportShipStatistics.DEFENSER_DEF / 100);
                }

                m_maxSpeed = Vikings.maxSpeed;
                break;

            case (int)FlagshipStatus.ShipClass.orientals:
                if (tipo == SupportShipType.Attacker)
                {
                    m_MaxHealth = (int)(Orientals.maxHealth * (int)SupportShipStatistics.ATTACKER_HP / 100);
                    m_main = (int)(Orientals.mainAttackDmg * (int)SupportShipStatistics.ATTACKER_DMG / 100);
                    m_defense = (int)(Orientals.defense * (int)SupportShipStatistics.ATTACKER_DEF / 100);
                }
                else
                {
                    m_MaxHealth = (int)(Orientals.maxHealth * (int)SupportShipStatistics.DEFENSER_HP / 100);
                    m_main = (int)(Orientals.mainAttackDmg * (int)SupportShipStatistics.DEFENSER_DMG / 100);
                    m_defense = (int)(Orientals.defense * (int)SupportShipStatistics.DEFENSER_DEF / 100);
                }

                m_maxSpeed = Orientals.maxSpeed;
                break;

        }

        m_AI.GetComponent<RAIN.Core.AIRig>().AI.Body = gameObject;
        m_AI.GetComponent<RAIN.Core.AIRig>().AI.Senses.GetSensor("FlagShipView").MountPoint = owner.transform;
        m_AI.GetComponent<RAIN.Core.AIRig>().AI.Senses.GetSensor("SupportShipView").MountPoint = transform;

        Debug.Log("Eseguito");


        m_AI.GetComponent<RAIN.Core.AIRig>().AI.AIInit();
        m_AI.GetComponent<RAIN.Core.AIRig>().AI.BodyInit();




    }



    private bool canAttack = false;

    public void Attack()
    {
        if (!canAttack)
        {
            canAttack = true;
        }
    }

    IEnumerator attivaTrigger()
    {
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(MainAttack("MA"));
        canAttack = false;
    }

    private IEnumerator MainAttack(string tag)
    {
        RpcSetActiveTrigger("MAP");
        yield return new WaitForSeconds(Symbols.mainAttackDelay);
        RpcSetUnactiveTrigger("MAP");
        if (GetComponent<FlagshipStatus>().shipClass == FlagshipStatus.ShipClass.egyptians)
        {
            StartCoroutine(continuousAttack(tag));
        }
        else
        {
            RpcSetActiveTrigger(tag);
            yield return new WaitForSeconds(0.2f);
            RpcSetUnactiveTrigger(tag);
        }
        yield return new WaitForSeconds(GetComponent<FlagshipStatus>().m_mainCD - Symbols.mainAttackDelay - 0.2f);
        RpcEndMainCoolDown();
    }

    [ClientRpc]
    public void RpcEndMainCoolDown()
    {
        canAttack = false;
    }

    IEnumerator continuousAttack(string tag)
    {
        yield return new WaitForSeconds(0.4f);
        for (int i = 0; i < 5; i++)
        {
            RpcSetActiveTrigger(tag);
            yield return new WaitForSeconds(0.2f);
            RpcSetUnactiveTrigger(tag);
            //yield return new WaitForSeconds(0.1f);
        }
    }

    [ClientRpc]
    public void RpcSetActiveTrigger(string tag)
    {


        GetComponent<Animator>().SetTrigger("MainAttack");

        Utility.FindChildWithTag(gameObject, tag).SetActive(true);

    }

    [ClientRpc]
    public void RpcSetUnactiveTrigger(string tag)
    {
        //Debug.Log("trigger playerId: "+playerId + "go: " + Utility.FindChildWithTag(LocalGameManager.Instance.GetPlayer(playerId), tag).name);
        Utility.FindChildWithTag(gameObject, tag).SetActive(false);
        if (tag.Equals("MAP") || tag.Equals("SAP"))
        {
            StartCoroutine(CreaScia());
        }

    }

    IEnumerator CreaScia()
    {

            switch (GetComponent<FlagshipStatus>().shipClass)
            {
                case FlagshipStatus.ShipClass.pirates:
                    GameObject g = GameObject.Instantiate(MainParticles[1], transform);
                    g.transform.SetParent(null);
                    g.SetActive(true);
                    StartCoroutine(shutdownParticle(MainParticles[1].GetComponent<ParticleSystem>().main.duration, g));
                    break;
                case FlagshipStatus.ShipClass.egyptians:
                    break;
                case FlagshipStatus.ShipClass.orientals:
                    break;
                case FlagshipStatus.ShipClass.vikings:
                    break;

            }

        yield return null;
    }


    public void SparoMain()
    {
        Debug.Log("SparoMain");
        StartCoroutine(startParticle());
    }

    IEnumerator startParticle()
    {

            switch (GetComponent<FlagshipStatus>().shipClass)
            {
                case FlagshipStatus.ShipClass.pirates:

                    MainParticles[0].SetActive(false);
                    MainParticles[0].SetActive(true);
                    StartCoroutine(shutdownParticle(MainParticles[0].GetComponent<ParticleSystem>().main.duration, MainParticles[0]));
                    break;
                case FlagshipStatus.ShipClass.egyptians:
                    MainParticles[0].SetActive(false);
                    MainParticles[0].SetActive(true);
                    StartCoroutine(shutdownParticle(MainParticles[0].GetComponent<ParticleSystem>().main.duration, MainParticles[0]));
                    break;
                case FlagshipStatus.ShipClass.orientals:
                    MainParticles[0].SetActive(false);
                    MainParticles[0].SetActive(true);
                    MainParticles[1].SetActive(false);
                    StartCoroutine(shutdownParticle(MainParticles[0].GetComponent<ParticleSystem>().main.duration, MainParticles[0]));
                    yield return new WaitForSeconds(MainParticles[0].GetComponent<ParticleSystem>().main.duration);
                    MainParticles[1].SetActive(true);
                    break;
                case FlagshipStatus.ShipClass.vikings:
                    MainParticles[0].SetActive(false);
                    MainParticles[0].SetActive(true);
                    StartCoroutine(shutdownParticle(MainParticles[0].GetComponent<ParticleSystem>().main.duration, MainParticles[0]));
                    break;

            }



        yield return null;
    }

    IEnumerator shutdownParticle(float time, GameObject g)
    {
        yield return new WaitForSeconds(time);
        if (!g.GetComponentInParent<Player>())
        {
            Destroy(g);
        }
        else
        {
            g.SetActive(false);
        }

    }




}
