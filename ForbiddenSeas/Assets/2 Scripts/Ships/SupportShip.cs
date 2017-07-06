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
    [SyncVar]
    public int supportID;

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




    public float lerpMoveRate = 10;
    public float lerpRotRate = 20;


    public float syncPosX;
    public float syncPosZ;


    private Vector3 lastPos;
    public float threshold = 0.5f;

    public bool canSync = true;


    public float syncRotY;
    private Vector3 lastRot;
    public float rotThreshold;

    public void Start()
    {
        if (!isServer)
            m_AI.SetActive(false);

        lastPos = transform.position;
        lastRot = transform.rotation.eulerAngles;
        syncPosX = transform.position.x;
        syncPosZ = transform.position.y;
        syncRotY = transform.rotation.eulerAngles.y;

    }

    private void FixedUpdate()
    {
        if (isServer)
        {
            if ((Vector3.Distance(transform.position, lastPos) > threshold || Vector3.Distance(transform.rotation.eulerAngles, lastRot) > rotThreshold))
            {
                RpcTransmitPosition(transform.position.x, transform.position.z, transform.rotation.eulerAngles.y);
                lastPos = transform.position;
                lastRot = transform.rotation.eulerAngles;
            }
        }
        else
        {
            LerpPosition();
        }
    }

    [ClientRpc]
    void RpcTransmitPosition(float x, float z, float rot)
    {
        syncPosX = x;
        syncPosZ = z;
        syncRotY = rot;
    }

    void LerpPosition()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(syncPosX, 0, syncPosZ), lerpMoveRate);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0f, syncRotY, 0f)), lerpRotRate);
    }




    public void InizializeSupportShip(SupportShipType tipo, GameObject owner, Transform dest, int index)
    {

        m_Flagship = owner;
        m_Tipo = tipo;
        m_Destination = dest;

        supportID = m_Flagship.GetComponent<Player>().playerId + (index * 100);

        if (!isServer)
            return;

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

        m_AI.GetComponent<RAIN.Core.AIRig>().AI.IsActive = true;
        Debug.Log("Eseguito");


    }



    private bool canAttack = false;

    public void Attack()
    {
        if (!canAttack)
        {
            canAttack = true;
            StartCoroutine(attivaTrigger());

        }
    }

    IEnumerator attivaTrigger()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(MainAttack("MA"));
    }

    private IEnumerator MainAttack(string tag)
    {
        RpcSetActiveTrigger("MAP");
        yield return new WaitForSeconds(Symbols.mainAttackDelay);
        RpcSetUnactiveTrigger("MAP");
        if (m_Classe == FlagshipStatus.ShipClass.egyptians)
        {
            StartCoroutine(continuousAttack(tag));
        }
        else
        {
            RpcSetActiveTrigger(tag);
            yield return new WaitForSeconds(0.2f);
            RpcSetUnactiveTrigger(tag);
        }
        yield return new WaitForSeconds(m_mainCD);
        EndMainCoolDown();
    }


    public void EndMainCoolDown()
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

            switch (m_Classe)
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

            switch (m_Classe)
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
        if (!g.GetComponentInParent<SupportShip>())
        {
            Destroy(g);
        }
        else
        {
            g.SetActive(false);
        }

    }




}
