﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class FlagshipStatus : NetworkBehaviour
{
    public enum ShipClass {pirates, vikings, egyptians, orientals};

    public ShipClass shipClass;
    public static string shipName;
    public int m_MaxHealth;
	[SyncVar]
    public float m_Maneuvrability;
	[SyncVar]
    public float m_maxSpeed;

    [SyncVar]
    public int m_Health;
    [SyncVar]
    public int m_reputation = 0;
    [SyncVar]
    public float m_yohoho = 0;
    [SyncVar]
    public bool m_isDead = false;
    [SyncVar]
    public int m_DoT = 0;

    [SyncVar]
    public int m_main, m_special;
    [SyncVar]
    public float m_mainCD, m_specialCD;
    [SyncVar]
    public SyncListBool debuffList = new SyncListBool();
    [SyncVar]
    public SyncListBool buffList = new SyncListBool();
    [SyncVar]
    public float m_mainDistance;
    [SyncVar]
    public float m_specialDistance;
    [SyncVar]
    public float m_defense;

    public StatusHUD statusHUD;
    public Player m_Me;

    public bool sonoMortissimo = false;

    void Start()
    {
        m_Me = gameObject.GetComponent<Player>();
        statusHUD = GameObject.FindGameObjectWithTag("StatusHUD").GetComponent<StatusHUD>();


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
                m_mainCD = Pirates.mainAttackCD;
                m_specialCD = Pirates.specAttackCD;
                m_mainDistance = Pirates.mainDistance;
                m_specialDistance = Pirates.specialDistance;
                m_defense = Pirates.defense;
                break;

            case 1:
                shipName = Vikings.shipName;
                m_MaxHealth = Vikings.maxHealth;
                m_Maneuvrability = Vikings.maneuverability;
                m_maxSpeed = Vikings.maxSpeed;
                m_main = Vikings.mainAttackDmg;
                m_special = Vikings.specAttackDmg;
                m_mainCD = Vikings.mainAttackCD;
                m_specialCD = Vikings.specAttackCD;
                m_mainDistance = Vikings.mainDistance;
                m_specialDistance = Vikings.specialDistance;
                m_defense = Vikings.defense;
                break;

            case 2:
                shipName = Egyptians.shipName;
                m_MaxHealth = Egyptians.maxHealth;
                m_Maneuvrability = Egyptians.maneuverability;
                m_maxSpeed = Egyptians.maxSpeed;
                m_main = Egyptians.mainAttackDmg;
                m_special = Egyptians.specAttackDmg;
                m_mainCD = Egyptians.mainAttackCD;
                m_specialCD = Egyptians.specAttackCD;
                m_mainDistance = Egyptians.mainDistance;
                m_specialDistance = Egyptians.specialDistance;
                m_defense = Egyptians.defense;

                break;

            case 3:
                shipName = Orientals.shipName;
                m_MaxHealth = Orientals.maxHealth;
                m_Maneuvrability = Orientals.maneuverability;
                m_maxSpeed = Orientals.maxSpeed;
                m_main = Orientals.mainAttackDmg;
                m_special = Orientals.specAttackDmg;
                m_mainCD = Orientals.mainAttackCD;
                m_specialCD = Orientals.specAttackCD;
                m_mainDistance = Orientals.mainDistance;
                m_specialDistance = Orientals.specialDistance;
                m_defense = Orientals.defense;

                break;

            default:
                return;
        }


        for(int i =0; i<2;i++)
            debuffList.Add(false);
        for(int i =0; i<4;i++)
            buffList.Add(false);

        m_Health = m_MaxHealth;
    }

    [Command]
    public void CmdTakeDamage(int dmg, string a_name, int da_playerId)
    {


        m_Health = Mathf.Clamp(m_Health - dmg, -50, m_MaxHealth);
        //RpcTakenDamage(a_name, da_playerId);


        if (m_Health <= 0)
        {
            OnDeath();
            if(da_playerId != -1)
                LocalGameManager.Instance.m_playerKills[da_playerId - 1]++;
        }
    }

    public void PrendiDannoDaEnemy(int dmg)
    {
        m_Health -= dmg;


        if (m_Health <= 0)
        {
            OnDeath();
        }
    }

    public void OnDeath()
    {
        if (m_Me.m_LocalTreasure && m_Me.m_HasTreasure)
        {
            m_Me.m_HasTreasure = false;
            if(m_Me.m_InsideArena)
                StartCoroutine(m_Me.LostTheTreasure());
            else
            {
                m_Me.RpcHideTreasure();
                StartCoroutine(LocalGameManager.Instance.c_RespawnTreasure());
            }

        }
        if (!m_isDead)
        {
            //Set all the penalties here.
            m_yohoho -= 15f;
            if (m_yohoho < 0)
                m_yohoho = 0;
            //Decrease Reputation
            //Increase Death Count
            //Increase Opponent Kill Count
        }
        m_isDead = true;
        LocalGameManager.Instance.m_playerDeaths[GetComponent<Player>().playerId - 1]++;
        m_reputation += ReputationValues.KILLED;
        m_reputation = (m_reputation < 0) ? 0 : m_reputation;
        GetComponent<Player>().TargetRpcUpdateReputationUI(GetComponent<NetworkIdentity>().connectionToClient);
        m_Health = m_MaxHealth;
        RpcRespawn();

    }

    /*
    [ClientRpc]
    void RpcTakenDamage(string a, int da)
    {
        Debug.Log("io sono: player " + LocalGameManager.Instance.GetPlayerId(gameObject).ToString() + " Colpito " + a + " da " + da);
    }*/

    //metodo DoT && HoT
    private IEnumerator DmgOverTime()
    {
        Debug.Log("dentro coroutine");
        while (true)
        {
            yield return new WaitForSeconds(3f);
            CmdTakeDamage(m_DoT, "Player " + gameObject.GetComponent<Player>().playerName, -1);
        }
    }

    [ClientRpc]
    public void RpcRespawn()
    {
        GetComponent<Animator>().SetTrigger("isDead");
        StartCoroutine(Respawn());
    }

    public IEnumerator Respawn()
    {

        yield return new WaitUntil(() => sonoMortissimo);
        GetComponent<Player>().SpostaBarca(gameObject, transform.position + Vector3.down, 5f);
        sonoMortissimo = false;
        yield return new WaitWhile(() => GetComponent<Player>().barca_isMoving);
        GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        Debug.Log("Si Blocca qui " + Time.time);
        GetComponent<Animator>().SetTrigger("Respawn");
        yield return new WaitForSeconds(2f);
        Debug.Log("Riprende qui " + Time.time);
        GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        if (isLocalPlayer)
        {
            Debug.Log("Voglio rivivere.");
            CmdIwantoToLive();
            transform.position = GetComponent<Player>().m_SpawnPoint;
        }
    }

    [Command]
    public void CmdIwantoToLive()
    {
        m_isDead = false;
    }


    public void Morte()
    {
        Debug.Log("#######CAZZZOOOOO " + m_Me.playerName);
        sonoMortissimo = true;
    }

    //status alterati - power-ups

    [Command]
    public void CmdMiasma()
    {
        debuffList[(int)DebuffStatus.poison] = true;
        m_DoT += Orientals.specAttackDmg;
        StartCoroutine(resetDoT(Orientals.specAttackDmg, (float)DebuffTiming.POISON_DURATION));
    }

    [Command]
    public void CmdBlind(NetworkIdentity u)
    {
        debuffList[(int)DebuffStatus.blind] = true;
        TargetRpcBlind(u.connectionToClient);
    }

    [TargetRpc]
    private void TargetRpcBlind(NetworkConnection nc)
    {
        Blind bl = GameObject.FindWithTag("Blind").GetComponent<Blind>();
        bl.SetBlind(true);
        StartCoroutine(resetBlind(bl, (float)DebuffTiming.BLIND_DURATION));
    }

    [Server]
    public void DamageUp()
    {
        TargetRpcDmgUp(GetComponent<NetworkIdentity>().connectionToClient);
    }

    [TargetRpc]
    public void TargetRpcDmgUp(NetworkConnection c)
    {
        StartCoroutine(DmgUp());
    }

    IEnumerator DmgUp()
    {
        bool check = buffList[(int)BuffStatus.dmgUp];

        Debug.Log("dmgUP now");
        buffList[(int)BuffStatus.dmgUp] = true;
        int currentMain = m_main;
        int currentSpec = m_special;

        m_main += (m_main / (int)BuffValue.DmgUpValue);
        m_special += (m_special / (int)BuffValue.DmgUpValue);
        statusHUD.ActivateBuff((int)BuffStatus.dmgUp, (float)BuffTiming.DAMAGE_UP_DURATION, check);
        yield return new WaitForSeconds((float)BuffTiming.DAMAGE_UP_DURATION);
        m_main = currentMain;
        m_special = currentSpec;
        buffList[(int)BuffStatus.dmgUp] = false;
        Debug.Log("end dmgUP");
    }

    [Server]
    public void SpeedUp()
    {
        TargetRpcSpeedUp(GetComponent<NetworkIdentity>().connectionToClient);
    }

    [TargetRpc]
    public void TargetRpcSpeedUp(NetworkConnection c)
    {
        StartCoroutine(IESpeedUp());
    }

    IEnumerator IESpeedUp()
    {
        bool check = buffList[(int)BuffStatus.speedUp];
        buffList[(int)BuffStatus.speedUp] = true;
        float currentSpeed = m_maxSpeed;
        m_maxSpeed += (m_maxSpeed / (float)BuffValue.SpeedUpValue);
        statusHUD.ActivateBuff((int)BuffStatus.speedUp, (float)BuffTiming.SPEED_UP_DURATION, check);
        yield return new WaitForSeconds((float)BuffTiming.SPEED_UP_DURATION);
        m_maxSpeed = currentSpeed;
        buffList[(int)BuffStatus.speedUp] = false;
    }

    [Server]
    public void Yohoho()
    {
        StartCoroutine(IEYohoho());
    }

    IEnumerator IEYohoho()
    {
        buffList[(int)BuffStatus.yohoho] = true;
        float currentSpeed = m_maxSpeed;
        m_maxSpeed += (m_maxSpeed / (float)BuffValue.YohohoSpeed);
        m_DoT += (int)BuffValue.YohohoRegen;
        yield return new WaitForSeconds((float)BuffTiming.YOHOHO_DURATION);
        m_maxSpeed = currentSpeed;
        m_DoT -= (int)BuffValue.YohohoRegen;
        buffList[(int)BuffStatus.yohoho] = false;
    }

    public void Regen()
    {
        m_DoT += Symbols.REGEN_AMOUNT;
        StartCoroutine(resetDoT(Symbols.REGEN_AMOUNT, (float)BuffTiming.REGEN_DURATION));
    }

    private IEnumerator resetDoT(int dmg, float duration)
    {
        yield return new WaitForSeconds(duration);
        m_DoT -= dmg;
        debuffList[(int)DebuffStatus.poison] = false;
    }

    private IEnumerator resetBlind(Blind bl, float duration)
    {
        yield return new WaitForSeconds(duration);
        bl.SetBlind(false);
        debuffList[(int)DebuffStatus.blind] = false;
    }

    public static int maxNumberStatus(SyncListBool b)
    {
        int c = 0;
        for(int i = 0; i< b.Count; i++)
        {
            if(b[i])
                c++;
        }
        return c;
    }

}