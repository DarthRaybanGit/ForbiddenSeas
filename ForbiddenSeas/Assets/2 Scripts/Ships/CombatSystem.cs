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

    void Awake()
    {
        MainUI = GameObject.FindGameObjectWithTag("mainCD_UI");
        SpecUI = GameObject.FindGameObjectWithTag("specCD_UI");
        GlobalMUI = GameObject.FindGameObjectWithTag("GlobalM_UI");
        GlobalSUI = GameObject.FindGameObjectWithTag("GlobalS_UI");
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
                StartCoroutine(MainAttack());
                Debug.Log("main cd: "+ GetComponent<FlagshipStatus>().m_mainCD);
                MainUI.GetComponent<CoolDownIndicator>().OnCoolDown(GetComponent<FlagshipStatus>().m_mainCD);
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
                StartCoroutine(SpecialAttack());
                SpecUI.GetComponent<CoolDownIndicator>().OnCoolDown(GetComponent<FlagshipStatus>().m_specialCD);
            }
        }
    }

    private IEnumerator GlobalCoolDown(GameObject UI)
    {
        UI.GetComponent<CoolDownIndicator>().OnCoolDown(1.5f);
        yield return new WaitForSeconds(1.5f);
        fire = false;
    }
        
    private IEnumerator MainAttack()
    {
        yield return new WaitForSeconds(Symbols.mainAttackDelay);
        CmdActivateTrigger(LocalGameManager.Instance.GetPlayerId(gameObject), "MainAttackSystem", true);
        Debug.Log("inizio" + Time.time);
        yield return new WaitForSeconds(0.5f);
        CmdActivateTrigger(LocalGameManager.Instance.GetPlayerId(gameObject), "MainAttackSystem", false);
        Debug.Log("end" + Time.time);
        yield return new WaitForSeconds(GetComponent<FlagshipStatus>().m_mainCD - Symbols.mainAttackDelay - 0.1f);
        mainCoolDown = false;
    }

    private IEnumerator SpecialAttack()
    {
        yield return new WaitForSeconds(Symbols.mainAttackDelay);
        Utility.FindChildWithTag(gameObject, "SpecAttackSystem").SetActive(true);
        yield return new WaitForSeconds(0.8f);
        Utility.FindChildWithTag(gameObject, "SpecAttackSystem").SetActive(false);
        yield return new WaitForSeconds(GetComponent<FlagshipStatus>().m_specialCD - Symbols.mainAttackDelay - 0.1f);
        specCoolDown = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isLocalPlayer)
        {
            if (other.gameObject.Equals(gameObject))
                return;

            if (other.tag.Equals("mainAttack") || other.tag.Equals("specialAttack"))
            {
                Debug.Log(gameObject.name + "Preso danno da "+ other.name + other.GetComponentInParent<FlagshipStatus>().m_main);
                int dmg = 0;
                if (other.tag.Equals("mainAttack"))
                    dmg = other.GetComponentInParent<FlagshipStatus>().m_main;
                else
                    dmg = other.GetComponentInParent<FlagshipStatus>().m_special;

                GetComponent<FlagshipStatus>().CmdTakeDamage(dmg, LocalGameManager.Instance.GetPlayerId(gameObject).ToString(), LocalGameManager.Instance.GetPlayerId(other.transform.parent.gameObject).ToString());
            }
        }
    }

    [Command]
    public void CmdActivateTrigger(int playerId, string tag, bool b)
    {
        RpcSetActiveTrigger(playerId, tag, b);
    }

    [ClientRpc]
    public void RpcSetActiveTrigger(int playerId, string tag, bool b)
    {
        Debug.Log("trigger playerId: "+playerId + "go: "+Utility.FindChildWithTag(LocalGameManager.Instance.GetPlayer(playerId), tag).name);
        Utility.FindChildWithTag(LocalGameManager.Instance.GetPlayer(playerId), tag).SetActive(b);
    }
}
