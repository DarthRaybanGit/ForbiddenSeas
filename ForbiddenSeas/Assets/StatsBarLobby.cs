using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsBarLobby : MonoBehaviour
{
    public enum ShipClass {pirates, egyptians, vikings, orientals};
    public ShipClass shipClass;
    public Image[] bars;

	void Start ()
    {
        float maxAttack = (float)Pirates.mainAttackDmg;
        float maxDefence = (float)Orientals.defense;
        float maxHP = (float)Orientals.maxHealth;
        float maxSpeed = Egyptians.maxSpeed;
        float maxManov = ManeuverabilityLevel.HIGH;

        switch (shipClass)
        {
            case ShipClass.pirates:
                
                bars[0].fillAmount = Pirates.mainAttackDmg / maxAttack;
                bars[1].fillAmount = Pirates.defense / maxDefence;
                bars[2].fillAmount = Pirates.maxHealth / maxHP;
                bars[3].fillAmount = Pirates.maxSpeed / maxSpeed;
                bars[4].fillAmount = Pirates.maneuverability / maxManov;
                break;

            case ShipClass.egyptians:

                bars[0].fillAmount = Egyptians.mainAttackDmg / maxAttack;
                bars[1].fillAmount = Egyptians.defense / maxDefence;
                bars[2].fillAmount = Egyptians.maxHealth / maxHP;
                bars[3].fillAmount = Egyptians.maxSpeed / maxSpeed;
                bars[4].fillAmount = Egyptians.maneuverability / maxManov;
                break;

            case ShipClass.vikings:

                bars[0].fillAmount = Vikings.mainAttackDmg / maxAttack;
                bars[1].fillAmount = Vikings.defense / maxDefence;
                bars[2].fillAmount = Vikings.maxHealth / maxHP;
                bars[3].fillAmount = Vikings.maxSpeed / maxSpeed;
                bars[4].fillAmount = Vikings.maneuverability / maxManov;
                break;

            case ShipClass.orientals:

                bars[0].fillAmount = Orientals.mainAttackDmg / maxAttack;
                bars[1].fillAmount = Orientals.defense / maxDefence;
                bars[2].fillAmount = Orientals.maxHealth / maxHP;
                bars[3].fillAmount = Orientals.maxSpeed / maxSpeed;
                bars[4].fillAmount = Orientals.maneuverability / maxManov;
                break;
        }

	}
	
}
