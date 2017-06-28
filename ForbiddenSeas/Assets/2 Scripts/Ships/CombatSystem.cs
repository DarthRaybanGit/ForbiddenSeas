using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CombatSystem : NetworkBehaviour
{
    bool mainCoolDown = false;
    bool specCoolDown = false;
    bool fire = false;
    public GameObject MainUI;
    public GameObject SpecUI;
    public GameObject GlobalMUI;
    public GameObject GlobalSUI;
    public StatusHUD statusHUD;
    private GameObject danno;

    public GameObject[] MainParticles;
    public GameObject[] SpecialParticles;
    private Vector3 waterLocation;

    public GameObject YohohoParticle;

    public GameObject PrimaryWeapon;
    public GameObject SecondWeapon;

    void Awake()
    {
        MainUI = GameObject.FindGameObjectWithTag("mainCD_UI");
        SpecUI = GameObject.FindGameObjectWithTag("specCD_UI");
        GlobalMUI = GameObject.FindGameObjectWithTag("GlobalM_UI");
        GlobalSUI = GameObject.FindGameObjectWithTag("GlobalS_UI");
        statusHUD = GameObject.FindGameObjectWithTag("StatusHUD").GetComponent<StatusHUD>();
        danno = GameObject.FindGameObjectWithTag("Damage");
    }

    void Update()
    {
        if (!isLocalPlayer)
        {

            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LocalGameManager.Instance.m_CanvasHUD.SetActive(false);

            GameObject g = LocalGameManager.Instance.gameObject;

            Destroy(LocalGameManager.Instance);
            Destroy(g);

            OnlineManager.s_Singleton.StopClient();
        }

        if (!GetComponent<FlagshipStatus>().m_isDead && LocalGameManager.Instance.GameCanStart() && LocalGameManager.Instance.m_canAttack)
        {
            if (Input.GetMouseButtonDown(0))
            {
                GlobalMUI.GetComponent<CoolDownIndicator>().OnGlobalPressed();
                if (!mainCoolDown && !fire)
                {
                    Debug.Log("Sparo main");
                    fire = true;
                    mainCoolDown = true;
                    StartCoroutine(GlobalCoolDown(SpecUI));
                    CmdMainAttack(GetComponent<Player>().playerId, "MA");
                    Debug.Log("main cd: " + GetComponent<FlagshipStatus>().m_mainCD);
                    MainUI.GetComponent<CoolDownIndicator>().OnCoolDown(GetComponent<FlagshipStatus>().m_mainCD);
                }
                else
                {
                    MainUI.transform.parent.GetComponent<CoolDownIndicator>().NotAvailable();
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                GlobalSUI.GetComponent<CoolDownIndicator>().OnGlobalPressed();
                if (!specCoolDown && !fire)
                {
                    Debug.Log("Sparo special");
                    fire = true;
                    specCoolDown = true;
                    StartCoroutine(GlobalCoolDown(MainUI));
                    CmdSpecialAttack(GetComponent<Player>().playerId, "SA");
                    SpecUI.GetComponent<CoolDownIndicator>().OnCoolDown(GetComponent<FlagshipStatus>().m_specialCD);
                }
                else
                {
                    SpecUI.transform.parent.GetComponent<CoolDownIndicator>().NotAvailable();
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if(GetComponent<FlagshipStatus>().m_yohoho == 100)
                {

                    GameObject.FindGameObjectWithTag("YohohoTag").transform.GetChild(0).gameObject.SetActive(false);

                    CmdAttivaYohoho();
                }
            }

            if(Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.N) && Input.GetKey(KeyCode.G) && Input.GetKey(KeyCode.D))
            {
                CmdResetMyLife();
                transform.position = GetComponent<Player>().m_SpawnPoint;

                transform.LookAt(new Vector3(0, 0, 0));
                transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 180 + transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);

                GetComponent<MoveSimple>().TransmitPosition();
                LocalGameManager.Instance.m_canAttack = true;
                GetComponent<MoveSimple>().DontPush = false;
                GetComponent<MoveSimple>().ActualSpeed = 0;
                GetComponent<MoveSimple>().State = 0;
                GetComponent<Animator>().SetFloat("Speed", 0);


                foreach(Transform t in GameObject.FindGameObjectWithTag("StatusHUD").transform)
                {
                    t.gameObject.SetActive(false);
                }

                GetComponent<Player>().m_Avviso_ARRH.transform.GetChild(0).gameObject.SetActive(false);
                GetComponent<Player>().m_Avviso_Kill.transform.GetChild(0).gameObject.SetActive(false);
                GetComponent<Player>().m_Avviso_PowerUp.transform.GetChild(0).gameObject.SetActive(false);
                GetComponent<Player>().m_Avviso_Treasure.transform.GetChild(0).gameObject.SetActive(false);
                LocalGameManager.Instance.m_CanvasHUD.GetComponent<InGameCanvasController>().CountDownRespawn.SetActive(false);
                LocalGameManager.Instance.m_CanvasHUD.GetComponent<InGameCanvasController>().CountDownRespawn.SetActive(true);
                GameObject.FindGameObjectWithTag("YohohoTag").transform.GetChild(0).gameObject.SetActive(false);
                GameObject.FindGameObjectWithTag("Yohoho").transform.GetChild(0).gameObject.SetActive(false);

            }

        }
    }


    [Command]
    public void CmdResetMyLife()
    {
        GetComponent<FlagshipStatus>().DeathStatus();
        for (int i=0; i < GetComponent<FlagshipStatus>().debuffList.Count; i++)
        {
            GetComponent<FlagshipStatus>().debuffList[i] = false;
        }

        for (int i = 0; i < GetComponent<FlagshipStatus>().buffList.Count; i++)
        {
            GetComponent<FlagshipStatus>().buffList[i] = false;
        }

        GetComponent<FlagshipStatus>().m_isDead = false;
        GetComponent<Player>().m_InsideArena = true;
        GetComponent<FlagshipStatus>().m_yohoho = 0;
        GetComponent<FlagshipStatus>().m_Health = GetComponent<FlagshipStatus>().m_MaxHealth;
        GetComponent<FlagshipStatus>().m_maxSpeed = GetComponent<FlagshipStatus>().m_maksuSpeedo;

        GetComponent<FlagshipStatus>().m_DoT = 0;

    }


    [Command]
    public void CmdAttivaYohoho()
    {
        GetComponent<FlagshipStatus>().Yohoho();

    }

    [ClientRpc]
    public void RpcYohohoParticle()
    {
        StartCoroutine(activateYohohoParticle());
    }

    IEnumerator activateYohohoParticle()
    {
        YohohoParticle.SetActive(true);
        yield return new WaitForSeconds((int) BuffTiming.YOHOHO_DURATION);
        LocalGameManager.Instance.yohoho_icon = false;
        YohohoParticle.SetActive(false);
    }

    [Command]
    public void CmdDamageThis(NetworkInstanceId who, int amount)
    {
        Debug.Log("Passato " + who);
        GameObject g = NetworkServer.FindLocalObject(who);
        if (g.GetComponent<PowerUp>())
        {
            Debug.Log("PowerUp colpito " + who + " danno preso " + amount);
            g.GetComponent<PowerUp>().m_health -= amount;

            if(g.GetComponent<PowerUp>().m_health <= 0)
            {
                Debug.Log("Toccato un powerUp da " + netId);
                gameObject.GetComponentInParent<Player>().CatchAPowerUp(g.GetComponent<PowerUp>().type, gameObject.GetComponentInParent<Player>().playerId);
                gameObject.GetComponentInParent<Player>().RpcAvvisoPowerUp(netId, g.GetComponent<PowerUp>().type == PowerUP.REGEN ? "Regen" : "SpeedUp");
                g.GetComponent<PowerUp>().killMe();
            }
        }
    }

    private IEnumerator GlobalCoolDown(GameObject UI)
    {
        UI.GetComponent<CoolDownIndicator>().OnCoolDown(1.5f);
        yield return new WaitForSeconds(1.5f);
        fire = false;
    }

    private IEnumerator MainAttack(int playerId, string tag)
    {
        RpcSetActiveTrigger(playerId, "MAP");
        yield return new WaitForSeconds(Symbols.mainAttackDelay);
        RpcSetUnactiveTrigger(playerId, "MAP");
        if(GetComponent<FlagshipStatus>().shipClass == FlagshipStatus.ShipClass.egyptians)
        {
            StartCoroutine(continuousAttack(playerId, tag));
        }else
        {
            RpcSetActiveTrigger(playerId, tag);
            yield return new WaitForSeconds(0.2f);
            RpcSetUnactiveTrigger(playerId, tag);
        }
        yield return new WaitForSeconds(GetComponent<FlagshipStatus>().m_mainCD - Symbols.mainAttackDelay - 0.2f);
        RpcEndMainCoolDown();
    }

    IEnumerator continuousAttack(int playerId, string tag)
    {
        yield return new WaitForSeconds(0.4f);
        for (int i = 0; i < 5; i++)
        {
            RpcSetActiveTrigger(playerId, tag);
            yield return new WaitForSeconds(0.2f);
            RpcSetUnactiveTrigger(playerId, tag);
            //yield return new WaitForSeconds(0.1f);
        }
    }

    [ClientRpc]
    public void RpcEndMainCoolDown()
    {
        mainCoolDown = false;
    }

    [Command]
    public void CmdMainAttack(int playerId, string tag)
    {
        StartCoroutine(MainAttack(playerId, tag));
    }

    private IEnumerator SpecialAttack(int playerId, string tag)
    {
        RpcSetActiveTrigger(playerId, "SAP");
        yield return new WaitForSeconds(Symbols.specAttackDelay);
        RpcSetUnactiveTrigger(playerId, "SAP");
        RpcSetActiveTrigger(playerId, tag);
        yield return new WaitForSeconds(0.2f);
        RpcSetUnactiveTrigger(playerId, tag);
        yield return new WaitForSeconds(GetComponent<FlagshipStatus>().m_specialCD - Symbols.specAttackDelay - 0.2f);
        RpcEndSpecCoolDown();
    }

    [ClientRpc]
    public void RpcEndSpecCoolDown()
    {
        specCoolDown = false;
    }

    [Command]
    public void CmdSpecialAttack(int playerId, string tag)
    {
        StartCoroutine(SpecialAttack(playerId, tag));
    }

    [ClientRpc]
    public void RpcSetActiveTrigger(int playerId, string tag)
    {
        //Debug.Log("trigger playerId: "+playerId + "go: " + Utility.FindChildWithTag(LocalGameManager.Instance.GetPlayer(playerId), tag).name);
        if(tag.Equals("MAP") || tag.Equals("SAP"))
        {
            if (tag.Equals("MAP"))
            {
                GetComponent<Animator>().SetTrigger("MainAttack");
            }
            else
            {

                GetComponent<Animator>().SetTrigger("SpecAttack");
            }


        }

        Utility.FindChildWithTag(LocalGameManager.Instance.GetPlayer(playerId), tag).SetActive(true);

    }

    public void DeactivateSecondaryWeapon()
    {
        Debug.Log("Deattiva");
        SecondWeapon.SetActive(false);
    }

    public void ActivateSecondaryWeapon()
    {
        Debug.Log("Attiva");
        SecondWeapon.SetActive(true);
    }

    [ClientRpc]
    public void RpcSetUnactiveTrigger(int playerId, string tag)
    {
        //Debug.Log("trigger playerId: "+playerId + "go: " + Utility.FindChildWithTag(LocalGameManager.Instance.GetPlayer(playerId), tag).name);
        Utility.FindChildWithTag(LocalGameManager.Instance.GetPlayer(playerId), tag).SetActive(false);
        if (tag.Equals("MAP") || tag.Equals("SAP"))
        {
            StartCoroutine(CreaScia(tag.Equals("MAP") ? true : false));
        }
        /*
        if (tag.Equals("MA"))
            waterLocation = transform.position + transform.forward * GetComponent<FlagshipStatus>().m_mainDistance;
        else if (tag.Equals("SA"))
        {
            waterLocation = transform.position + transform.forward * GetComponent<FlagshipStatus>().m_specialDistance;
        }*/
    }


    IEnumerator CreaScia(bool which)
    {
        if (which)
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
        }
        else
        {
            switch (GetComponent<FlagshipStatus>().shipClass)
            {
                case FlagshipStatus.ShipClass.pirates:
                    GameObject g = GameObject.Instantiate(SpecialParticles[1], transform);
                    g.SetActive(true);
                    StartCoroutine(shutdownParticle(SpecialParticles[1].GetComponent<ParticleSystem>().main.duration, g));
                    yield return new WaitForSeconds(0.3f);
                    g.transform.SetParent(null);
                    break;
                case FlagshipStatus.ShipClass.egyptians:
                    break;
                case FlagshipStatus.ShipClass.orientals:
                    break;
                case FlagshipStatus.ShipClass.vikings:
                    break;

            }
        }
        yield return null;
    }


    public void SparoMain()
    {
        Debug.Log("SparoMain");
        StartCoroutine(startParticle(true));
    }

    IEnumerator startParticle(bool main)
    {
        if (main)
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
        }
        else
        {
            switch (GetComponent<FlagshipStatus>().shipClass)
            {
                case FlagshipStatus.ShipClass.pirates:

                    SpecialParticles[0].SetActive(false);
                    SpecialParticles[0].SetActive(true);
                    StartCoroutine(shutdownParticle(SpecialParticles[0].GetComponent<ParticleSystem>().main.duration, SpecialParticles[0]));
                    break;
                case FlagshipStatus.ShipClass.egyptians:
                    SpecialParticles[0].SetActive(false);
                    SpecialParticles[0].SetActive(true);
                    //StartCoroutine(shutdownParticle(SpecialParticles[0].GetComponent<ParticleSystem>().main.duration, SpecialParticles[0]));
                    break;
                case FlagshipStatus.ShipClass.orientals:
                    SpecialParticles[0].SetActive(false);
                    SpecialParticles[1].SetActive(false);
                    SpecialParticles[0].SetActive(true);
                    StartCoroutine(shutdownParticle(SpecialParticles[0].GetComponent<ParticleSystem>().main.duration, SpecialParticles[0]));
                    yield return new WaitForSeconds(SpecialParticles[0].GetComponent<ParticleSystem>().main.duration );
                    SpecialParticles[1].SetActive(true);
                    break;
                case FlagshipStatus.ShipClass.vikings:
                    SpecialParticles[0].SetActive(false);
                    SpecialParticles[0].SetActive(true);
                    StartCoroutine(shutdownParticle(SpecialParticles[0].GetComponent<ParticleSystem>().main.duration, SpecialParticles[0]));
                    break;

            }
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

    public void SparoSpecial()
    {
        StartCoroutine(startParticle(false));
    }


    bool lockForDoubleAttack = false;
    int lastAttacker = -1;

    void OnTriggerEnter(Collider other)
    {
        if (isLocalPlayer && !GetComponent<FlagshipStatus>().m_isDead)
        {
            if (other.gameObject.Equals(gameObject))
                return;
            int dmg;
            switch (other.tag)
            {
                case "mainAttack":
                    danno.GetComponent<Animation>().Play("FadeDanno");
                    dmg = other.GetComponentInParent<FlagshipStatus>().m_main;
                    if(other.gameObject.GetComponentInParent<FlagshipStatus>().shipClass != FlagshipStatus.ShipClass.egyptians)
                    {
                        if (!lockForDoubleAttack && lastAttacker != other.gameObject.GetComponentInParent<Player>().playerId)
                        {
                            lockForDoubleAttack = true;
                            lastAttacker = other.gameObject.GetComponentInParent<Player>().playerId;
                            GetComponent<FlagshipStatus>().CmdTakeDamage(Mathf.RoundToInt(dmg * (1f - GetComponent<FlagshipStatus>().m_defense)), GetComponent<Player>().playerName, other.transform.parent.parent.gameObject.GetComponent<Player>().playerId);
                            StartCoroutine(doppioCollider());
                        }
                    }
                    else
                    {
                        GetComponent<FlagshipStatus>().CmdTakeDamage(Mathf.RoundToInt(dmg * (1f - GetComponent<FlagshipStatus>().m_defense)), GetComponent<Player>().playerName, other.transform.parent.parent.gameObject.GetComponent<Player>().playerId);
                    }

                    break;
                case "specialAttack":
                    danno.GetComponent<Animation>().Play("FadeDanno");
                    dmg = other.GetComponentInParent<FlagshipStatus>().m_special;
                    if (!lockForDoubleAttack && lastAttacker != other.gameObject.GetComponentInParent<Player>().playerId)
                    {
                        lockForDoubleAttack = true;
                        lastAttacker = other.gameObject.GetComponentInParent<Player>().playerId;
                        GetComponent<FlagshipStatus>().CmdTakeDamage(Mathf.RoundToInt(dmg * (1f - GetComponent<FlagshipStatus>().m_defense)), GetComponent<Player>().playerName, other.transform.parent.parent.gameObject.GetComponent<Player>().playerId);
                        StartCoroutine(doppioCollider());
                    }
                    break;
                case "Miasma":
                    bool check = GetComponent<FlagshipStatus>().debuffList[(int)DebuffStatus.poison];
                    GetComponent<FlagshipStatus>().CmdMiasma(check);

                    break;
                case "RasEye":
                    bool check1 = GetComponent<FlagshipStatus>().debuffList[(int)DebuffStatus.blind];
                    GetComponent<FlagshipStatus>().CmdBlind(GetComponent<NetworkIdentity>(), check1);
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
}
