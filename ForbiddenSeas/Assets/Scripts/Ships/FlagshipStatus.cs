using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FlagshipStatus : NetworkBehaviour
{
    public enum ShipClass {pirates, vikings, venetians, orientals};

    public ShipClass shipClass;
    public static string shipName;
    public static int m_MaxHealth;
    public static float m_Maneuvrability;
    public static float m_maxSpeed;

    [SyncVar]
    public int m_Health;
    public int m_reputation = 0;
    public int m_yohoho = 0;
    public int m_DoT = 10; // da azzerare

    [SyncVar]
    public int m_main, m_special;

    void Start()
    {
        if(isLocalPlayer)
            StartCoroutine(DmgOverTime());
    }

    public void InitializeFlagshipStatus()
    {
        switch ((int)shipClass)
        {
            case 0:
                shipName = Pirates.shipName;
                m_MaxHealth = Pirates.maxHealth;
                m_Maneuvrability = Pirates.maneuverability;
                m_maxSpeed = Pirates.maxSpeed;
                m_main = Pirates.mainAttackDmg;
                m_special = Pirates.specAttackDmg;
                break;

            case 1:
                shipName = Vikings.shipName;
                m_MaxHealth = Vikings.maxHealth;
                m_Maneuvrability = Vikings.maneuverability;
                m_maxSpeed = Vikings.maxSpeed;
                m_main = Vikings.mainAttackDmg;
                m_special = Vikings.specAttackDmg;
                break;

            case 2:
                shipName = Venetians.shipName;
                m_MaxHealth = Venetians.maxHealth;
                m_Maneuvrability = Venetians.maneuverability;
                m_maxSpeed = Venetians.maxSpeed;
                m_main = Venetians.mainAttackDmg;
                m_special = Venetians.specAttackDmg;
                break;

            case 3:
                shipName = Orientals.shipName;
                m_MaxHealth = Orientals.maxHealth;
                m_Maneuvrability = Orientals.maneuverability;
                m_maxSpeed = Orientals.maxSpeed;
                m_main = Orientals.mainAttackDmg;
                m_special = Orientals.specAttackDmg;
                break;

            default:
                return;
        }

        m_Health = m_MaxHealth;
    }

    [Command]
    public void CmdTakeDamage(int dmg, string a_name, string da_name)
    {
        m_Health -= dmg;
        RpcTakenDamage(a_name, da_name);

        if (m_Health <= 0)
            OnDeath();
    }

    public void OnDeath()
    {
        transform.GetChild(0).GetComponentInChildren<Material>().color = Color.red;
    }

    [ClientRpc]
    void RpcTakenDamage(string a, string da)
    {
        Debug.Log("io sono: player " + LocalGameManager.Instance.GetPlayerId(gameObject).ToString() + " Colpito " + a + " da " + da);
    }

    //metodo DoT && HoT
    private IEnumerator DmgOverTime()
    {
        Debug.Log("dentro coroutine");
        while (true)
        {
            yield return new WaitForSeconds(3f);
            CmdTakeDamage(m_DoT, "Player " + LocalGameManager.Instance.GetPlayerId(gameObject).ToString(), "DoT");
        }
    }


}


