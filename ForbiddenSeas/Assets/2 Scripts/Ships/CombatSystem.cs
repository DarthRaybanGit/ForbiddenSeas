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
    }

    void Update()
    {
        if (!isLocalPlayer)
        {

            return;
        }

        if (!GetComponent<FlagshipStatus>().m_isDead && LocalGameManager.Instance.GameCanStart())
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
                    CmdAttivaYohoho();
                }
            }


        }
    }

    [Command]
    public void CmdAttivaYohoho()
    {
        GetComponent<FlagshipStatus>().Yohoho();
        RpcYohohoParticle();
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
        RpcSetActiveTrigger(playerId, tag);
        yield return new WaitForSeconds(0.2f);
        RpcSetUnactiveTrigger(playerId, tag);
        yield return new WaitForSeconds(GetComponent<FlagshipStatus>().m_mainCD - Symbols.mainAttackDelay - 0.2f);
        RpcEndMainCoolDown();
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
                    dmg = other.GetComponentInParent<FlagshipStatus>().m_main;
                    GetComponent<FlagshipStatus>().CmdTakeDamage(Mathf.RoundToInt(dmg * (1f - GetComponent<FlagshipStatus>().m_defense)), GetComponent<Player>().playerName, other.transform.parent.parent.gameObject.GetComponent<Player>().playerId);
                    break;
                case "specialAttack":
                    dmg = other.GetComponentInParent<FlagshipStatus>().m_special;
                    GetComponent<FlagshipStatus>().CmdTakeDamage(Mathf.RoundToInt(dmg * (1f - GetComponent<FlagshipStatus>().m_defense)), GetComponent<Player>().playerName, other.transform.parent.parent.gameObject.GetComponent<Player>().playerId);
                    break;
                case "Miasma":
                    bool check = GetComponent<FlagshipStatus>().debuffList[(int)DebuffStatus.poison];
                    GetComponent<FlagshipStatus>().CmdMiasma();
                    StartCoroutine(statusHUD.ActivateDebuff((int)DebuffStatus.poison,(float)DebuffTiming.POISON_DURATION, check));
                    break;
                case "RasEye":
                    bool check1 = GetComponent<FlagshipStatus>().debuffList[(int)DebuffStatus.blind];
                    StartCoroutine(statusHUD.ActivateDebuff((int)DebuffStatus.blind,(float)DebuffTiming.BLIND_DURATION, check1));
                    GetComponent<FlagshipStatus>().CmdBlind(GetComponent<NetworkIdentity>());
                    break;
                default:
                    return;
            }
        }

    }
}
