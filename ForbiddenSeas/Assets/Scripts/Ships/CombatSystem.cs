using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CombatSystem : NetworkBehaviour
{
    bool mainCoolDown = false;
    bool specCoolDown = false;
    bool fire = false;

    void Update()
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (!mainCoolDown && !fire)
            {
                Debug.Log("Sparo main");
                fire = true;
                mainCoolDown = true;
                StartCoroutine(MainAttack());
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (!specCoolDown && !fire)
            {
                Debug.Log("Sparo special");
                fire = true;
                specCoolDown = true;
                StartCoroutine(SpecialAttack());
            }
        }
    }

    private IEnumerator MainAttack()
    {
        yield return new WaitForSeconds(Symbols.mainAttackDelay);
        Utility.FindChildWithTag(gameObject, "mainAttack").SetActive(true);
        yield return new WaitForSeconds(0.1f);
        Utility.FindChildWithTag(gameObject, "mainAttack").SetActive(false);
        fire = false;
        yield return new WaitForSeconds(GetComponent<FlagshipStatus>().m_mainCD - Symbols.mainAttackDelay - 0.1f);
        mainCoolDown = false;
    }

    private IEnumerator SpecialAttack()
    {
        yield return new WaitForSeconds(Symbols.mainAttackDelay);
        Utility.FindChildWithTag(gameObject, "specialAttack").SetActive(true);
        yield return new WaitForSeconds(0.1f);
        Utility.FindChildWithTag(gameObject, "specialAttack").SetActive(false);
        fire = false;
        yield return new WaitForSeconds(GetComponent<FlagshipStatus>().m_specialCD - Symbols.mainAttackDelay - 0.1f);
        specCoolDown = false;
    }

    void OnTriggerStay(Collider other)
    {
        if (isLocalPlayer && other.CompareTag("Player"))
        {
            if (other.gameObject.Equals(gameObject))
                return;

            Debug.Log(gameObject.name + "Preso danno da "+ other.name + other.GetComponentInParent<FlagshipStatus>().m_main);

            if (other.tag.Equals("mainAttack") || other.tag.Equals("specialAttack"))
            {
                int dmg = 0;
                if (other.tag.Equals("mainAttack"))
                    dmg = other.GetComponentInParent<FlagshipStatus>().m_main;
                else
                    dmg = other.GetComponentInParent<FlagshipStatus>().m_special;

                GetComponent<FlagshipStatus>().CmdTakeDamage(dmg, LocalGameManager.Instance.GetPlayerId(gameObject).ToString(), LocalGameManager.Instance.GetPlayerId(other.transform.parent.gameObject).ToString());
            }
        }
    }
}
