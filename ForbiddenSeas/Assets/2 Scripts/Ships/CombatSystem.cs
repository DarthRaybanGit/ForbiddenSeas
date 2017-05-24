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
            return;
        
        if (Input.GetMouseButtonDown(0))
        {
            GlobalMUI.GetComponent<CoolDownIndicator>().OnGlobalPressed();
            if (!mainCoolDown && !fire)
            {
                Debug.Log("Sparo main");
                fire = true;
                mainCoolDown = true;
                StartCoroutine(GlobalCoolDown(SpecUI));
                CmdMainAttack(LocalGameManager.Instance.GetPlayerId(gameObject), "MA");
                Debug.Log("main cd: "+ GetComponent<FlagshipStatus>().m_mainCD);
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
                CmdSpecialAttack(LocalGameManager.Instance.GetPlayerId(gameObject), "SA");
                SpecUI.GetComponent<CoolDownIndicator>().OnCoolDown(GetComponent<FlagshipStatus>().m_specialCD);
            }
            else
            {
                SpecUI.transform.parent.GetComponent<CoolDownIndicator>().NotAvailable();
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
        Utility.FindChildWithTag(LocalGameManager.Instance.GetPlayer(playerId), tag).SetActive(true);
    }

    [ClientRpc]
    public void RpcSetUnactiveTrigger(int playerId, string tag)
    {
        //Debug.Log("trigger playerId: "+playerId + "go: " + Utility.FindChildWithTag(LocalGameManager.Instance.GetPlayer(playerId), tag).name);
        Utility.FindChildWithTag(LocalGameManager.Instance.GetPlayer(playerId), tag).SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isLocalPlayer)
        {
            if (other.gameObject.Equals(gameObject))
                return;
            int dmg;
            switch (other.tag)
            {
                case "mainAttack":
                    dmg = other.GetComponentInParent<FlagshipStatus>().m_main;
                    GetComponent<FlagshipStatus>().CmdTakeDamage(dmg, LocalGameManager.Instance.GetPlayerId(gameObject).ToString(), LocalGameManager.Instance.GetPlayerId(other.transform.parent.parent.gameObject).ToString());
                    break;
                case "specialAttack":
                    dmg = other.GetComponentInParent<FlagshipStatus>().m_special;
                    GetComponent<FlagshipStatus>().CmdTakeDamage(dmg, LocalGameManager.Instance.GetPlayerId(gameObject).ToString(), LocalGameManager.Instance.GetPlayerId(other.transform.parent.parent.gameObject).ToString());
                    break;
                case "Miasma":
                    GetComponent<FlagshipStatus>().CmdMiasma();
                    StartCoroutine(statusHUD.ActivateDebuff((int)DebuffStatus.poison,(float)DebuffTiming.POISON_DURATION));
                    break;
                case "RasEye":
                    StartCoroutine(statusHUD.ActivateDebuff((int)DebuffStatus.blind,(float)DebuffTiming.BLIND_DURATION));
                    GetComponent<FlagshipStatus>().CmdBlind(GetComponent<NetworkIdentity>());
                    break;
                default:
                    return;   
            }
        }
    }
}
