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

    void Start()
    {
        if (!isLocalPlayer)
            return;
        
        switch (shipClass)
        {
            case ShipClass.pirates:
                shipName = Pirates.shipName;
                m_MaxHealth = Pirates.maxHealth;
                m_Maneuvrability = Pirates.maneuverability;
                m_maxSpeed = Pirates.maxSpeed;
                break;

            case ShipClass.vikings:
                shipName = Vikings.shipName;
                m_MaxHealth = Vikings.maxHealth;
                m_Maneuvrability = Vikings.maneuverability;
                m_maxSpeed = Vikings.maxSpeed;
                break;

            case ShipClass.venetians:
                shipName = Venetians.shipName;
                m_MaxHealth = Venetians.maxHealth;
                m_Maneuvrability = Venetians.maneuverability;
                m_maxSpeed = Venetians.maxSpeed;
                break;

            case ShipClass.orientals:
                shipName = Orientals.shipName;
                m_MaxHealth = Orientals.maxHealth;
                m_Maneuvrability = Orientals.maneuverability;
                m_maxSpeed = Orientals.maxSpeed;
                break;

            default:
                return;

        }
        m_Health = m_MaxHealth;
    }

    public void takeDamage(int dmg)
    {
        m_Health -= dmg;

        if (m_Health <= 0)
            onDeath();
    }

    public void onDeath()
    {
        GetComponent<Material>().color = Color.red;
    }
}


