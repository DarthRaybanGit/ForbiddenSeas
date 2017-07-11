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
    public int supportID = -1;
    [SyncVar]
    public int fatherID;
    [SyncVar]
    public bool m_Left = false;


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

    float journeyLenght;
    float startTime;

    bool isLerping = false;

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

            if (m_Flagship)
            {
                m_maxSpeed = m_Flagship.GetComponent<MoveSimple>().maxSpeed;


            }

        }
        else
        {
            /*
            if((Vector3.Distance(transform.position, new Vector3(syncPosX, 0, syncPosZ)) > threshold))
            {
                startTime = Time.time;
                journeyLenght = Vector3.Distance(transform.position, new Vector3(syncPosX, 0, syncPosZ));

                if (!isLerping)
                    StartCoroutine(lerpa());
            }*/

            LerpPosition(0.5f);

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0f, syncRotY, 0f)), lerpRotRate);

            if (LocalGameManager.Instance.GetPlayer(fatherID))
            {
                GetComponent<Animator>().SetFloat("Speed", LocalGameManager.Instance.GetPlayer(fatherID).GetComponent<Animator>().GetFloat("Speed"));
            }
        }
    }

    IEnumerator lerpa()
    {
        isLerping = true;
        while (true)
        {
            float distCovered = (Time.time - startTime) * lerpMoveRate;
            float fracJourney = distCovered / journeyLenght;
            LerpPosition(fracJourney);
            if (fracJourney == 1)
            {
                isLerping = false;
                break;
            }

            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }

    [ClientRpc]
    void RpcTransmitPosition(float x, float z, float rot)
    {
        syncPosX = x;
        syncPosZ = z;
        syncRotY = rot;
    }

    void LerpPosition(float frac)
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(syncPosX, 0, syncPosZ), frac);

    }




    public void InizializeSupportShip(SupportShipType tipo, GameObject owner, Transform dest, int index, bool left)
    {

        m_Flagship = owner;
        m_Tipo = tipo;
        m_Destination = dest;

        supportID = m_Flagship.GetComponent<Player>().playerId + (index * 100);
        fatherID = m_Flagship.GetComponent<Player>().playerId;
        m_Left = left;

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

        m_Health = m_MaxHealth;

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
            attivaTrigger();

        }
    }

    public void attivaTrigger()
    {
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
        yield return new WaitForSeconds(m_mainCD + 3f);
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



    bool lockForDoubleAttack = false;
    int lastAttacker = -1;

    private void OnTriggerEnter(Collider other)
    {
        if (!isServer && LocalGameManager.Instance.GetPlayer(fatherID).GetComponent<Player>().isLocalPlayer && !m_isDead)
        {
            if (other.gameObject.Equals(gameObject) || (other.gameObject.GetComponentInParent<Player>() && other.gameObject.GetComponentInParent<Player>().playerId == fatherID) || (other.gameObject.GetComponentInParent<SupportShip>() && other.gameObject.GetComponentInParent<SupportShip>().fatherID == fatherID))
                return;

            int dmg;
            switch (other.tag)
            {
                case "mainAttack":
                    dmg = (other.GetComponentInParent<FlagshipStatus>()) ? other.GetComponentInParent<FlagshipStatus>().m_main : other.GetComponentInParent<SupportShip>().m_main;

                    FlagshipStatus.ShipClass classe = (other.GetComponentInParent<FlagshipStatus>()) ? other.GetComponentInParent<FlagshipStatus>().shipClass : other.GetComponentInParent<SupportShip>().m_Classe;

                    if (classe != FlagshipStatus.ShipClass.egyptians)
                    {
                        bool iCanSufferDmg = (other.gameObject.GetComponentInParent<Player>()) ? lastAttacker != other.gameObject.GetComponentInParent<Player>().playerId : lastAttacker != other.gameObject.GetComponentInParent<SupportShip>().supportID;

                        if (!lockForDoubleAttack && iCanSufferDmg)
                        {
                            lockForDoubleAttack = true;
                            lastAttacker = (other.gameObject.GetComponentInParent<Player>()) ? other.gameObject.GetComponentInParent<Player>().playerId : other.gameObject.GetComponentInParent<SupportShip>().supportID;
                            LocalGameManager.Instance.GetPlayer(fatherID).GetComponent<FlagshipStatus>().CmdSupportTakeDamage(Mathf.RoundToInt(dmg * (1f - m_defense)), supportID);
                            StartCoroutine(doppioCollider());
                        }
                    }
                    else
                    {
                        LocalGameManager.Instance.GetPlayer(fatherID).GetComponent<FlagshipStatus>().CmdSupportTakeDamage(Mathf.RoundToInt(dmg * (1f - m_defense)), supportID);
                    }

                    break;
                case "specialAttack":

                    dmg = other.GetComponentInParent<FlagshipStatus>().m_special;
                    if (!lockForDoubleAttack && lastAttacker != other.gameObject.GetComponentInParent<Player>().playerId)
                    {
                        lockForDoubleAttack = true;
                        lastAttacker = other.gameObject.GetComponentInParent<Player>().playerId;
                        LocalGameManager.Instance.GetPlayer(fatherID).GetComponent<FlagshipStatus>().CmdSupportTakeDamage(Mathf.RoundToInt(dmg * (1f - m_defense)), supportID);
                        StartCoroutine(doppioCollider());
                    }
                    break;
                case "Miasma":
                    /*
                    bool check = GetComponent<FlagshipStatus>().debuffList[(int)DebuffStatus.poison];
                    GetComponent<FlagshipStatus>().CmdMiasma(check);
                    */
                    break;
                case "RasEye":
                    /*
                    bool check1 = GetComponent<FlagshipStatus>().debuffList[(int)DebuffStatus.blind];
                    GetComponent<FlagshipStatus>().CmdBlind(GetComponent<NetworkIdentity>(), check1);
                    */
                    break;
                default:
                    return;
            }
        }
    }

    IEnumerator doppioCollider()
    {
        yield return new WaitForSeconds(0.4f);
        lockForDoubleAttack = false;
        lastAttacker = -1;
    }


    [Server]
    public void OnDeath()
    {
        m_isDead = true;
        m_Flagship.GetComponent<FlagshipStatus>().m_reputation += ReputationValues.SUPPKILLED;
        m_Flagship.GetComponent<FlagshipStatus>().m_reputation = (m_Flagship.GetComponent<FlagshipStatus>().m_reputation < 0) ? 0 : m_Flagship.GetComponent<FlagshipStatus>().m_reputation;
        m_Flagship.GetComponent<Player>().TargetRpcUpdateReputationUI(m_Flagship.GetComponent<NetworkIdentity>().connectionToClient);

        m_Flagship.GetComponent<CombatSystem>().m_NumOfSupport--;
        Invoke("killMe", 4f);
    }

    public void killMe()
    {
        //NetworkServer.UnSpawn(gameObject);
        m_Flagship.GetComponent<CombatSystem>().m_NumOfSupport--;
        if (m_Left)
            m_Flagship.GetComponent<CombatSystem>().LeftSupportShip = null;
        else
            m_Flagship.GetComponent<CombatSystem>().RightSupportShip = null;
        Destroy(gameObject);
    }

    [ClientRpc]
    public void RpcSupportShipDeath()
    {
        Debug.Log("La support " + supportID + " del player " + fatherID + " è affondata!");
    }
}
