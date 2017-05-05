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
    [SyncVar]
    public int m_Health;
    public static float m_Maneuvrability;
    public static float m_maxSpeed;
    public Attack m_main, m_special;

    void Start()
    {
        if (!isLocalPlayer)
            return;

        switch ((int)shipClass)
        {
            case 0:
                shipName = Pirates.shipName;
                m_MaxHealth = Pirates.maxHealth;
                m_Maneuvrability = Pirates.maneuverability;
                m_maxSpeed = Pirates.maxSpeed;
                m_main = Pirates.mainAttack;
                m_special = Pirates.specialAttack;
                break;

            case 1:
                shipName = Vikings.shipName;
                m_MaxHealth = Vikings.maxHealth;
                m_Maneuvrability = Vikings.maneuverability;
                m_maxSpeed = Vikings.maxSpeed;
                m_main = Vikings.mainAttack;
                m_special = Vikings.specialAttack;
                break;

            case 2:
                shipName = Venetians.shipName;
                m_MaxHealth = Venetians.maxHealth;
                m_Maneuvrability = Venetians.maneuverability;
                m_maxSpeed = Venetians.maxSpeed;
                m_main = Venetians.mainAttack;
                m_special = Venetians.specialAttack;
                break;

            case 3:
                shipName = Orientals.shipName;
                m_MaxHealth = Orientals.maxHealth;
                m_Maneuvrability = Orientals.maneuverability;
                m_maxSpeed = Orientals.maxSpeed;
                m_main = Orientals.mainAttack;
                m_special = Orientals.specialAttack;
                break;

            default:
                return;

        }
        m_Health = m_MaxHealth;
    }

    [Command]
    public void CmdTakeDamage(int dmg, string name)
    {
        m_Health -= dmg;
        RpcTakenDamage(name);
    
        if (m_Health <= 0)
            OnDeath();
    }
        
    public void OnDeath()
    {
        transform.GetChild(0).GetComponentInChildren<Material>().color = Color.red;
    }

    [ClientRpc]
    void RpcTakenDamage(string o)
    {
        Debug.Log("io sono: " + OnlineManager.s_Singleton.client.connection.connectionId.ToString() + " Colpito " + o);
    }

    //metodo DoT && HoT


}


