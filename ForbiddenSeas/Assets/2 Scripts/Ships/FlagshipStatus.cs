using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


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

    public float m_maksuSpeedo;

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
    public GameObject Ombra;

    public bool sonoMortissimo = false;

	public bool wait = false;

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
        m_maksuSpeedo = m_maxSpeed;
    }

    [Command]
    public void CmdTakeDamage(int dmg, string a_name, int da_playerId)
    {


        m_Health = Mathf.Clamp(m_Health - dmg, -50, m_MaxHealth);
        //RpcTakenDamage(a_name, da_playerId);


        if (m_Health <= 0 && !m_isDead)
        {

            m_Health = 0;
            if (da_playerId != -1)
            {
                LocalGameManager.Instance.m_playerKills[da_playerId - 1]++;
                foreach(GameObject g in GameObject.FindGameObjectsWithTag("Player"))
                {
                    if(g && g.GetComponent<FlagshipStatus>())
                    {
                        if (g.GetComponent<Player>().playerId == da_playerId)
                        {
                            g.GetComponent<FlagshipStatus>().m_reputation += ReputationValues.KILL;
                            g.GetComponent<Player>().TargetRpcUpdateReputationUI(g.GetComponent<NetworkIdentity>().connectionToClient);
                        }
                    }
                }
                NetworkInstanceId nid = GetComponent<Player>().netId;
                NetworkInstanceId nid2 = LocalGameManager.Instance.GetPlayerServer(da_playerId).GetComponent<Player>().netId;
                GetComponent<Player>().RpcAvvisoKill(nid, nid2);

            }
            OnDeath();

        }
    }

    public void PrendiDannoDaEnemy(int dmg)
    {
       


		if (m_Health <= 0 && !m_isDead)
        {
            m_Health = 0;
            OnDeath();
        }
		if (m_Health>0 && !m_isDead)
			m_Health -= dmg;
    }

    public void OnDeath()
    {
        if (m_Me.m_LocalTreasure && m_Me.m_HasTreasure)
        {
            m_Me.m_HasTreasure = false;

            m_maxSpeed = m_maksuSpeedo;
            //if(m_Me.m_InsideArena)
                StartCoroutine(m_Me.LostTheTreasure());
            /*
            else
            {
                m_Me.RpcHideTreasure();
                StartCoroutine(LocalGameManager.Instance.c_RespawnTreasure());
            }
            */

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
            LocalGameManager.Instance.m_playerDeaths[GetComponent<Player>().playerId - 1]++;
            m_reputation += ReputationValues.KILLED;
            m_reputation = (m_reputation < 0) ? 0 : m_reputation;
        }
        m_isDead = true;
        GetComponent<Player>().TargetRpcUpdateReputationUI(GetComponent<NetworkIdentity>().connectionToClient);

        RpcRespawn();
		NetworkServer.FindLocalObject (m_Me.netId).GetComponent<FlagshipStatus> ().DeathStatus();

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
        GameObject.FindGameObjectWithTag("Aboard").transform.GetChild(0).gameObject.SetActive(false);
        GetComponent<Animator>().SetTrigger("isDead");
    }

    public IEnumerator Respawn()
    {

        //yield return new WaitUntil(() => sonoMortissimo);
        //GetComponent<Player>().SpostaBarca(gameObject, transform.position + Vector3.down, 5f);
        sonoMortissimo = false;
        //yield return new WaitForSeconds(2f);
        GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        if (Ombra)
            Ombra.SetActive(false);
        Debug.Log("Si Blocca qui " + Time.time);
        GetComponent<Animator>().SetTrigger("Respawn");
        yield return new WaitForSeconds(1f);
        Debug.Log("Riprende qui " + Time.time);
        transform.position = GetComponent<Player>().m_SpawnPoint;

        if (isLocalPlayer)
        {
            GetComponent<Animator>().SetFloat("Speed", 0);
            GetComponent<MoveSimple>().ActualSpeed = 0f;
            GetComponent<MoveSimple>().State = 0;
            Debug.Log("Voglio rivivere. NOPE");
            CmdIwantoToLive();
            transform.position = GetComponent<Player>().m_SpawnPoint;
            transform.LookAt(new Vector3(0, 0, 0));
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 180 + transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
            GetComponent<MoveSimple>().TransmitPosition();
        }
    }

    [Command]
    public void CmdIwantoToLive()
    {
        //Start CountDown
        GetComponent<Player>().m_InsideArena = true;
        TargetRpcCountDownRespawn(GetComponent<NetworkIdentity>().connectionToClient);
        StartCoroutine(CountDownRespawn());
    }

    IEnumerator CountDownRespawn()
    {
        yield return new WaitForSeconds((int)FixedDelayInGame.PLAYERS_RESPAWN);
        m_isDead = false;
        m_Health = m_MaxHealth;
        RpcImBack();
		if (LocalGameManager.Instance.m_TreasureOwned)
			StartCoroutine (GetComponent<Player> ().yohohoBarGrow (GetComponent<Player> (), 0f));
    }

    [ClientRpc]
    public void RpcImBack()
    {

        StartCoroutine(ResetLocationInfo());
    }

    IEnumerator ResetLocationInfo()
    {
        GetComponent<MoveSimple>().canSync = false;

        yield return new WaitForFixedUpdate();

        if (GetComponent<Player>().myTag)
            GetComponent<Player>().myTag.SetActive(true);

        transform.position = GetComponent<Player>().m_SpawnPoint;

        GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        if (Ombra)
            Ombra.SetActive(true);
        yield return new WaitForFixedUpdate();
        GetComponent<MoveSimple>().canSync = true;


    }

    [TargetRpc]
    public void TargetRpcCountDownRespawn(NetworkConnection nc)
    {
		GameObject.FindGameObjectWithTag("YohohoTag").transform.GetChild(0).gameObject.SetActive(false);
        LocalGameManager.Instance.m_CanvasHUD.GetComponent<InGameCanvasController>().CountDownRespawn.transform.GetChild(1).gameObject.GetComponent<Text>().text = ((int) FixedDelayInGame.PLAYERS_RESPAWN).ToString() ;
        LocalGameManager.Instance.m_CanvasHUD.GetComponent<InGameCanvasController>().CountDownRespawn.GetComponent<Animation>().Play("DeathGUI");
        StartCoroutine(StartCountDownLocal());

    }

    IEnumerator StartCountDownLocal()
    {
        GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        for (int i = (int)FixedDelayInGame.PLAYERS_RESPAWN; i > 0; i--)
        {
            yield return new WaitForSeconds(1f);
            LocalGameManager.Instance.m_CanvasHUD.GetComponent<InGameCanvasController>().CountDownRespawn.transform.GetChild(1).gameObject.GetComponent<Text>().text = i.ToString();
        }
        LocalGameManager.Instance.m_CanvasHUD.GetComponent<InGameCanvasController>().CountDownRespawn.transform.GetChild(1).gameObject.GetComponent<Text>().text = "GO!";
        yield return new WaitForSeconds(1f);
        LocalGameManager.Instance.m_CanvasHUD.GetComponent<InGameCanvasController>().CountDownRespawn.GetComponent<Animation>().Play("DeathGUI_Respawn");
    }

    public void Morte()
    {
        Debug.Log("#######Sto Morendo " + m_Me.playerName);
        sonoMortissimo = true;
        if(GetComponent<Player>().myTag)
            GetComponent<Player>().myTag.SetActive(false);
        StartCoroutine(Respawn());
    }

    //status alterati - power-ups

    [Command]
    public void CmdMiasma(bool check)
    {
        debuffList[(int)DebuffStatus.poison] = true;
        m_DoT += Orientals.specAttackDmg;
        StartCoroutine(resetDoT(Orientals.specAttackDmg, (float)DebuffTiming.POISON_DURATION));
        TargetRpcMiasma(GetComponent<NetworkIdentity>().connectionToClient, check);
    }

    [TargetRpc]
    private void TargetRpcMiasma(NetworkConnection nc, bool check1)
    {
        StartCoroutine(statusHUD.ActivateDebuff((int)DebuffStatus.poison, (float)DebuffTiming.POISON_DURATION, check1));
    }

    [Command]
    public void CmdBlind(NetworkIdentity u, bool check)
    {
        debuffList[(int)DebuffStatus.blind] = true;
        TargetRpcBlind(u.connectionToClient, check);
        StartCoroutine(resetBlindStatus());

    }

    [Server]
    IEnumerator resetBlindStatus()
    {
        yield return new WaitForSeconds((float)DebuffTiming.BLIND_DURATION);
        debuffList[(int)DebuffStatus.blind] = false;
    }

    [TargetRpc]
    private void TargetRpcBlind(NetworkConnection nc, bool check1)
    {
        Blind bl = GameObject.FindWithTag("Blind").GetComponent<Blind>();
        bl.SetBlind(true);
        StartCoroutine(statusHUD.ActivateDebuff((int)DebuffStatus.blind, (float)DebuffTiming.BLIND_DURATION, check1));
        StartCoroutine(resetBlind(bl, (float)DebuffTiming.BLIND_DURATION));
    }

    [Server]
    public void DamageUp(int player)
    {
        bool check = buffList[(int)BuffStatus.dmgUp];
        RpcDmgUpParticle(player);
        StartCoroutine(DamageUpBuff());
        TargetRpcDmgUp(GetComponent<NetworkIdentity>().connectionToClient, check);
    }

    IEnumerator DamageUpBuff()
    {
        Debug.Log("dmgUP now");
        buffList[(int)BuffStatus.dmgUp] = true;
        int currentMain = m_main;
        int currentSpec = m_special;

		m_main += (int)(m_main /100f * (int)BuffValue.DmgUpValue);
		m_special += (int)(m_special /100f * (int)BuffValue.DmgUpValue);

        float currTime = Time.time;
        yield return new WaitUntil(() => (Time.time - currTime >= (float)BuffTiming.DAMAGE_UP_DURATION || m_isDead));
        //yield return new WaitForSeconds((float)BuffTiming.DAMAGE_UP_DURATION);

		if (m_isDead)
		{
			m_main = getMainAttackStats(shipClass);
			m_special = getSpecAttackStats(shipClass);
			buffList [(int)BuffStatus.dmgUp] = false;
		}
        else
        {
            m_main = currentMain;
            m_special = currentSpec;
            buffList[(int)BuffStatus.dmgUp] = false;
        }
    }

    private int getMainAttackStats(ShipClass sc)
    {
        switch (sc)
        {
            case ShipClass.pirates:
                return Pirates.mainAttackDmg;
            case ShipClass.vikings:
                return Vikings.mainAttackDmg;
            case ShipClass.egyptians:
                return Egyptians.mainAttackDmg;
            case ShipClass.orientals:
                return Orientals.mainAttackDmg;
        }
        return 0;
    }

    private int getSpecAttackStats(ShipClass sc)
    {
        switch (sc)
        {
            case ShipClass.pirates:
                return Pirates.specAttackDmg;
            case ShipClass.vikings:
                return Vikings.specAttackDmg;
            case ShipClass.egyptians:
                return Egyptians.specAttackDmg;
            case ShipClass.orientals:
                return Orientals.specAttackDmg;
        }
        return 0;
    }

    [ClientRpc]
    public void RpcDmgUpParticle(int player)
    {
        Utility.FindChildWithTag(LocalGameManager.Instance.GetPlayer(player), "dmgUP_Particle").SetActive(true);
        LocalGameManager.Instance.GetPlayer(player).transform.GetChild(0).GetChild(1).GetComponent<Animation>().Play("DamageUp");
		StartCoroutine(EndDmgUpParticle(player,(float)BuffTiming.DAMAGE_UP_DURATION));
    }

	IEnumerator EndDmgUpParticle(int player, float time)
    {
        float currTime = Time.time;
        yield return new WaitUntil(() => (Time.time - currTime >= time || m_isDead));
        //yield return new WaitForSeconds(time);
        Utility.FindChildWithTag(LocalGameManager.Instance.GetPlayer(player), "dmgUP_Particle").SetActive(false);
        LocalGameManager.Instance.GetPlayer(player).transform.GetChild(0).GetChild(1).GetComponent<Animation>().Stop();
        LocalGameManager.Instance.GetPlayer(player).transform.GetChild(0).GetChild(1).GetComponent<Animation>().Play("ResetColor");
    }

    [TargetRpc]
    public void TargetRpcDmgUp(NetworkConnection c, bool check)
    {
        StartCoroutine(DmgUpIcon(check));
    }

    IEnumerator DmgUpIcon(bool check)
    {
        //bool check = buffList[(int)BuffStatus.dmgUp];

        Debug.Log("dmgUP now");
        /*
        buffList[(int)BuffStatus.dmgUp] = true;

        int currentMain = m_main;
        int currentSpec = m_special;

        m_main += (m_main / (int)BuffValue.DmgUpValue);
        m_special += (m_special / (int)BuffValue.DmgUpValue);
        */
        StartCoroutine(statusHUD.ActivateBuff((int)BuffStatus.dmgUp, (float)BuffTiming.DAMAGE_UP_DURATION, check));
        /*
        yield return new WaitForSeconds((float)BuffTiming.DAMAGE_UP_DURATION);

        m_main = currentMain;
        m_special = currentSpec;
        buffList[(int)BuffStatus.dmgUp] = false;
        */
        Debug.Log("end dmgUP");
        yield return null;
    }

    [Server]
    public void SpeedUp(int player)
    {
        RpcSpeedUpParticle(player);
        bool check = buffList[(int)BuffStatus.speedUp];
        StartCoroutine(SpeedUpBuff());
        TargetRpcSpeedUp(GetComponent<NetworkIdentity>().connectionToClient, check);
    }

	IEnumerator SpeedUpBuff()
    {

        buffList[(int)BuffStatus.speedUp] = true;
        float currentSpeed = m_maxSpeed;
        m_maxSpeed += (m_maxSpeed * ((float)BuffValue.SpeedUpValue / 100f));

        float currTime = Time.time;
        yield return new WaitUntil(() => (Time.time - currTime >= (float)BuffTiming.SPEED_UP_DURATION || m_isDead));

        //yield return new WaitForSeconds((float)BuffTiming.SPEED_UP_DURATION);

		if (m_isDead)
		{
			m_maxSpeed = m_maksuSpeedo;
			buffList [(int)BuffStatus.speedUp] = false;
		}
        else
        {
            m_maxSpeed = currentSpeed;
            buffList[(int)BuffStatus.speedUp] = false;
        }
    }

    [ClientRpc]
    public void RpcSpeedUpParticle(int player)
    {
        LocalGameManager.Instance.GetPlayer(player).transform.GetChild(0).GetChild(1).GetComponent<Animation>().Play("SpeedUp");
        Utility.FindChildWithTag(LocalGameManager.Instance.GetPlayer(player), "spdUP_Particle").SetActive(true);
		StartCoroutine(EndSpeedUpParticle(player,(float)BuffTiming.SPEED_UP_DURATION));
    }

	IEnumerator EndSpeedUpParticle(int player, float time)
    {
        float currTime = Time.time;
        yield return new WaitUntil(() => (Time.time - currTime >= time || m_isDead));
        //yield return new WaitForSeconds(time);
        Utility.FindChildWithTag(LocalGameManager.Instance.GetPlayer(player), "spdUP_Particle").SetActive(false);
        LocalGameManager.Instance.GetPlayer(player).transform.GetChild(0).GetChild(1).GetComponent<Animation>().Stop();
        LocalGameManager.Instance.GetPlayer(player).transform.GetChild(0).GetChild(1).GetComponent<Animation>().Play("ResetColor");
    }

    [TargetRpc]
    public void TargetRpcSpeedUp(NetworkConnection c, bool check)
    {
        Debug.Log("Sto eseguendo lo speedup, sono " + GetComponent<Player>().playerName);
        StartCoroutine(IESpeedUpIcon(check));
    }

    IEnumerator IESpeedUpIcon(bool check)
    {
        Camera.main.transform.GetChild(0).gameObject.SetActive(true);
        /*
        bool check = buffList[(int)BuffStatus.speedUp];
        buffList[(int)BuffStatus.speedUp] = true;
        float currentSpeed = m_maxSpeed;
        m_maxSpeed += (m_maxSpeed / (float)BuffValue.SpeedUpValue);
        */
        StartCoroutine(statusHUD.ActivateBuff((int)BuffStatus.speedUp, (float)BuffTiming.SPEED_UP_DURATION, check));
        yield return new WaitForSeconds((float)BuffTiming.SPEED_UP_DURATION);
        /*
        m_maxSpeed = currentSpeed;
        buffList[(int)BuffStatus.speedUp] = false;
        */
        Camera.main.transform.GetChild(0).gameObject.SetActive(false);
    }

    [Server]
    public void Yohoho()
    {
        GetComponent<FlagshipStatus>().m_yohoho = 0;
        GetComponent<CombatSystem>().RpcYohohoParticle();
		StartCoroutine(GetComponent<Player>().yohohoBarGrow(GetComponent<Player>(), (float)BuffTiming.YOHOHO_DURATION));
        RpcYohohoParticle(GetComponent<Player>().playerId);
        bool check = buffList[(int)BuffStatus.yohoho];
        StartCoroutine(IEYohohoBuff());
        TargetRpcYohohoIcon(GetComponent<NetworkIdentity>().connectionToClient, check);
    }

    [TargetRpc]
    public void TargetRpcYohohoIcon(NetworkConnection c, bool check)
    {
        StartCoroutine(statusHUD.ActivateBuff((int)BuffStatus.yohoho, (float)BuffTiming.YOHOHO_DURATION, check));
        StartCoroutine(YOHOHO_message());

    }

    IEnumerator YOHOHO_message()
    {
        GameObject.FindGameObjectWithTag("Yohoho").transform.GetChild(0).gameObject.SetActive(true);
        GameObject.FindGameObjectWithTag("Yohoho").GetComponent<Animation>().Play();
        yield return new WaitForSeconds(5f);
        GameObject.FindGameObjectWithTag("Yohoho").transform.GetChild(0).gameObject.SetActive(false);
    }

    IEnumerator IEYohohoBuff()
    {

        buffList[(int)BuffStatus.yohoho] = true;
        float currentSpeed = m_maxSpeed;
        m_maxSpeed += (m_maxSpeed * ((float)BuffValue.YohohoSpeed / 100f));
        m_DoT += (int)Symbols.YOHOHO_REGEN_AMOUNT;

        float currTime = Time.time;

        yield return new WaitUntil(() => (Time.time - currTime >= (float)BuffTiming.YOHOHO_DURATION || m_isDead));

        if (!m_isDead)
        {
            m_maxSpeed = currentSpeed;
            m_DoT -= (int)Symbols.YOHOHO_REGEN_AMOUNT;
            buffList[(int)BuffStatus.yohoho] = false;
        }
        else
        {
            Debug.Log("Sono morto perciò ci pensa la morte a disattivare tutto.");
            m_maxSpeed = m_maksuSpeedo;
        }

    }

    [ClientRpc]
    public void RpcYohohoParticle(int player)
    {
        LocalGameManager.Instance.GetPlayer(player).GetComponent<CombatSystem>().YohohoParticle.SetActive(true);
		StartCoroutine(EndYohohoParticle(player,(float)BuffTiming.YOHOHO_DURATION));
    }

	IEnumerator EndYohohoParticle(int player, float time)
    {
        float currTime = Time.time;
        yield return new WaitUntil(() => (Time.time - currTime >= time || m_isDead));
        //yield return new WaitForSeconds(time);
        LocalGameManager.Instance.GetPlayer(player).GetComponent<CombatSystem>().YohohoParticle.SetActive(false);

    }

    [Server]
    public void Regen(int player)
    {

        m_DoT += Symbols.REGEN_AMOUNT;
        RpcRegenParticle(player);
        bool check = buffList[(int)BuffStatus.regen];
        StartCoroutine(RegenRemoteIcon());
        StartCoroutine(resetDoT(Symbols.REGEN_AMOUNT, (float)BuffTiming.REGEN_DURATION));
        TargetRpcRegenIcon(GetComponent<NetworkIdentity>().connectionToClient, check);
    }

    IEnumerator RegenRemoteIcon()
    {
        buffList[(int)BuffStatus.regen] = true;

        yield return new WaitForSeconds((float)BuffTiming.REGEN_DURATION);

        buffList[(int)BuffStatus.regen] = false;
    }

    [TargetRpc]
    public void TargetRpcRegenIcon(NetworkConnection c, bool check)
    {
        StartCoroutine(statusHUD.ActivateBuff((int)BuffStatus.regen, (float)BuffTiming.REGEN_DURATION, check));
    }

    [ClientRpc]
    public void RpcRegenParticle(int player)
    {
        Utility.FindChildWithTag(LocalGameManager.Instance.GetPlayer(player), "regen_Particle").SetActive(true);
		StartCoroutine(EndRegenParticle(player,(float)BuffTiming.DAMAGE_UP_DURATION));
    }

	IEnumerator EndRegenParticle(int player, float time)
    {
        float currTime = Time.time;
        yield return new WaitUntil(() => (Time.time - currTime >= time || m_isDead));
        //yield return new WaitForSeconds(time);
        Utility.FindChildWithTag(LocalGameManager.Instance.GetPlayer(player), "regen_Particle").SetActive(false);
    }

    private IEnumerator resetDoT(int dmg, float duration)
    {
        float currtime = Time.time;
        yield return new WaitUntil(() => (Time.time - currtime >= duration || m_isDead));
        //yield return new WaitForSeconds(duration);
		if (!m_isDead)
        {
            Debug.Log("Sto resettando il DoT da vivo " + dmg + " danno aggiunto a Dot che ora è " + m_DoT);
            m_DoT -= dmg;
            debuffList[(int)DebuffStatus.poison] = false;
        }
        else
        {
            Debug.Log("Sono morto perciò il DoT viene resettato in altro modo");
        }

        //debuffList[(int)DebuffStatus.poison] = false;
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

	[Server]
	public void DeathStatus()
	{
        m_DoT = 0;
        if (buffList [(int)BuffStatus.dmgUp])
		{
			m_main =(int)( (float)m_main / (1f + (float)BuffValue.DmgUpValue / 100f));		//FIXME discrepanza sul calcolo dell'attacco

			m_special =(int)( (float)m_special / (1f + (float)BuffValue.DmgUpValue / 100f));
			buffList [(int)BuffStatus.dmgUp] = false;
			TargetRpcDamageUpParticleStop (GetComponent<NetworkIdentity> ().connectionToClient,buffList[(int)BuffStatus.dmgUp]);
		}
		if (buffList [(int)BuffStatus.speedUp])
		{
			m_maxSpeed = m_maxSpeed / (1 + (float)BuffValue.SpeedUpValue / 100f);
			buffList [(int)BuffStatus.speedUp] = false;
			TargetRpcSpeedUpParticleStop (GetComponent<NetworkIdentity> ().connectionToClient,buffList[(int)BuffStatus.speedUp]);
		}
		if (buffList [(int)BuffStatus.regen])
		{
			buffList [(int)BuffStatus.regen] = false;
			TargetRpcRegenParticleStop (GetComponent<NetworkIdentity> ().connectionToClient,buffList[(int)BuffStatus.regen]);
		}
		if (buffList [(int)BuffStatus.yohoho])
		{
			buffList [(int)BuffStatus.yohoho] = false;
			TargetRpcYohohoParticleStop (GetComponent<NetworkIdentity> ().connectionToClient,buffList[(int)BuffStatus.yohoho]);
		}
		if (debuffList [(int)DebuffStatus.blind])
		{
			debuffList [(int)DebuffStatus.blind] = false;
			TargetRpcBlindStop (GetComponent<NetworkIdentity> ().connectionToClient,buffList[(int)BuffStatus.yohoho]);
		}
		if (debuffList [(int)DebuffStatus.poison])
		{
			debuffList [(int)DebuffStatus.poison] = false;
			TargetRpcPoisonStop (GetComponent<NetworkIdentity> ().connectionToClient,buffList[(int)BuffStatus.yohoho]);
		}
	}

	[TargetRpc]
	public void TargetRpcSpeedUpParticleStop(NetworkConnection c, bool check)
	{
		StartCoroutine(EndSpeedUpParticle(m_Me.playerId,0f));
		Camera.main.transform.GetChild(0).gameObject.SetActive(false);
	}

	[TargetRpc]
	public void TargetRpcDamageUpParticleStop(NetworkConnection c, bool check)
	{
		StartCoroutine(EndDmgUpParticle(m_Me.playerId,0f));
	}

	[TargetRpc]
	public void TargetRpcYohohoParticleStop(NetworkConnection c, bool check)
	{
		StartCoroutine(EndYohohoParticle(m_Me.playerId,0f));
	}

	[TargetRpc]
	public void TargetRpcRegenParticleStop(NetworkConnection c, bool check)
	{
		StartCoroutine(EndRegenParticle(m_Me.playerId,0f));

	}

	[TargetRpc]
	public void TargetRpcBlindStop(NetworkConnection c, bool check)
	{
		Blind bl = GameObject.FindWithTag("Blind").GetComponent<Blind>();
		StartCoroutine(resetBlind(bl,0f));
	}

	[TargetRpc]
	public void TargetRpcPoisonStop(NetworkConnection c, bool check)
	{
		//spegnere i particle qua
		m_DoT = 0;
	}
}