﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour
{
    [SyncVar]
    public int playerId;
    [SyncVar]
    public string playerName;

    public GameObject[] m_AdmiralList = new GameObject[4];
    public GameObject m_LocalCamera;


    public bool m_HasTreasure = false;
    public GameObject m_LocalTreasure;

    public Vector3 m_SpawnPoint;
    public Text m_reputationTextUI;
    public Text m_scoreTextUI;
    public GameObject m_Avviso;

    public GameObject myTag;


    [SyncVar]
    public bool m_InsideArena = true;

    [SyncVar]
    public int m_score = 0;
    [SyncVar]
    public int m_Class = 0;

    public void Start()
    {
        if (!isLocalPlayer)
        {
            Destroy(m_LocalCamera.GetComponent<AudioListener>());
            return;
        }
        if (isLocalPlayer || isServer)
        {

        }

    }

    public override void OnStartLocalPlayer()
    {
        if (!isServer)
        {
            if (isLocalPlayer)
            {
                //Camera.main.gameObject.SetActive(true);
                /*m_LocalCamera.tag = "MainCamera";
                m_LocalCamera.SetActive(true);
                m_LocalCamera.GetComponent<Camera>().enabled = true;
                Debug.Log("Ho finito di settare la camera.");*/
                LocalGameManager.Instance.m_LocalPlayer = gameObject;
                CmdStartGeneralLoop((int)this.netId.Value);
                m_SpawnPoint = transform.position;
                LocalGameManager.Instance.m_GameIsStarted = true;
                m_reputationTextUI = GameObject.FindGameObjectWithTag("ReputationUI").GetComponent<Text>();
                m_scoreTextUI = GameObject.FindGameObjectWithTag("ScoreUI").GetComponent<Text>();
                m_Avviso = GameObject.FindGameObjectWithTag("Avviso");
            }

        }

    }



    public override void OnStartServer()
    {
        m_SpawnPoint = transform.position;
        LocalGameManager.Instance.m_GameIsStarted = true;
    }

    public void SetClass(int playerClass)
    {
        m_Class = playerClass;
    }


    //Funzione per richiedere al server di far partire il Loop di gioco.
    //Il primo player che spawna all'interno dell'arena farà partire il gioco.

    //Problemi --> Se uno lagga e la scena viene caricata dopo un certo tot? Parte con il gioco già startato e sarà quindi svantaggiato?
    //Soluzione: Inserire un countdown in cui il server aspetta il check per tutti i player e in quel caso far partire subito il gioco?
    //Se entro quel countdown tutti i player non saranno entrati in partita riporterebbe tutti alla Lobby.

    [Command]
    public void CmdStartGeneralLoop(int connectionID)
    {

        ImOnline(connectionID);
        if (LocalGameManager.Instance.m_GameIsStarted && !LocalGameManager.Instance.m_GameGeneralLoopIsStarted)
        {
            Debug.Log("Sono il primo! Aspettiamo gli altri prima di iniziare il game!");
            LocalGameManager.Instance.m_GameGeneralLoopIsStarted = true;

            foreach(int i in OnlineManager.s_Singleton.currentPlayers.Keys)
            {
                Debug.Log("è presente il Player " + i);
            }



            StartCoroutine(LocalGameManager.Instance.c_WaitUntilEveryPlayersOnline());
        }

    }

    [Command]
    public void CmdRequestTimetoServer()
    {
        if (!LocalGameManager.Instance.m_serverTimeSended)
        {
            LocalGameManager.Instance.m_serverTimeSended = true;
            Debug.Log("Start Game LOOP!!!");
            LocalGameManager.Instance.m_ServerOffsetTime = Time.timeSinceLevelLoad;
            LocalGameManager.Instance.m_GameStartTime = LocalGameManager.Instance.m_ServerOffsetTime;

            LocalGameManager.Instance.RpcNotifyServerTime(LocalGameManager.Instance.m_ServerOffsetTime, true);
            StartCoroutine(LocalGameManager.Instance.c_WaitForTreasure());
            for(int i = 0; i <= (int)PowerUP.DAMAGE_UP; i++)
                StartCoroutine(LocalGameManager.Instance.c_LoopPowerUp(0, SpawnIndex.REGEN + i));
            StartCoroutine(LocalGameManager.Instance.c_LoopCoins());

            for(int i = 0; i < OnlineManager.s_Singleton.m_MinesSpawnPosition.Length; i++)
            {
                StartCoroutine(LocalGameManager.Instance.c_LoopMines(2, i));
            }

        }

    }

    public void ImOnline(int connectionID)
    {
        Debug.Log("Sta notificando il player " + connectionID);

        foreach(int i in OnlineManager.s_Singleton.currentPlayers.Keys)
        {
            if(OnlineManager.s_Singleton.currentPlayers[i][(int) PlayerInfo.ID] == connectionID)
                OnlineManager.s_Singleton.currentPlayers[i][(int)PlayerInfo.IS_LOADED] = 1;
        }
    }


    //Funzione per richiedere al server il timestamp con il quale sincronizzarsi.
    [Command]
    public void CmdAskForCurrentTime()
    {
        LocalGameManager.Instance.m_serverTimeSended = true;
        LocalGameManager.Instance.RpcNotifyServerTime(Time.timeSinceLevelLoad, false);
    }

    [Server]
    public void CatchAPowerUp(PowerUP p)
    {
        switch (p)
        {
            case PowerUP.REGEN:
                GetComponent<FlagshipStatus>().Regen();
                break;
            case PowerUP.DAMAGE_UP:
                GetComponent<FlagshipStatus>().DamageUp();
                break;
            case PowerUP.SPEED_UP:
                GetComponent<FlagshipStatus>().SpeedUp();
                break;
        }
    }


    [Server]
    public void CatchTheTreasure()
    {
        if (!LocalGameManager.Instance.m_Treasure.activeSelf)
            return;

        LocalGameManager.Instance.m_Treasure.SetActive(false);
        Debug.Log("Il player " + playerId + " ha preso il tesoro!");

        //Debug.Log("Gli sto facendo prendere il tesoro!");
        GetComponent<FlagshipStatus>().m_reputation += ReputationValues.TREASURE;
        TargetRpcUpdateReputationUI(GetComponent<NetworkIdentity>().connectionToClient);

        m_HasTreasure = true;
        LocalGameManager.Instance.RpcNotifyNewTreasureOwner(netId, LocalGameManager.Instance.m_Treasure.GetComponent<NetworkIdentity>().netId);
        StartCoroutine(yohohoBarGrow(this));


    }

    [Server]
    public IEnumerator yohohoBarGrow(Player pl)
    {

        while (pl.m_HasTreasure && pl.gameObject.GetComponent<FlagshipStatus>().m_yohoho < 100)
        {

            yield return new WaitForSeconds((int)FixedDelayInGame.YOHOHO_UPDATE_INTERVAL);

            pl.gameObject.GetComponent<FlagshipStatus>().m_yohoho += 100 / (float)FixedDelayInGame.YOHOHO_FULLFY_SPAN;
            if (pl.gameObject.GetComponent<FlagshipStatus>().m_yohoho > 100)
                pl.gameObject.GetComponent<FlagshipStatus>().m_yohoho = 100;

        }
    }

    [Server]
    public IEnumerator LostTheTreasure()
    {
        Vector3 futureSpawn = gameObject.transform.position + Vector3.forward;
        RpcHideTreasure();
        Destroy(LocalGameManager.Instance.m_Treasure);
        yield return new WaitForSeconds(0.8f);

        LocalGameManager.Instance.m_Treasure = GameObject.Instantiate(OnlineManager.s_Singleton.spawnPrefabs.ToArray()[(int)SpawnIndex.TREASURE]);
        LocalGameManager.Instance.m_Treasure.transform.position = futureSpawn;

        NetworkServer.Spawn(LocalGameManager.Instance.m_Treasure);

    }

    [Server]
    public void ScoreAnARRH()
    {

        LocalGameManager.Instance.m_playerArrh[playerId]++;
        m_score++;
        GetComponent<FlagshipStatus>().m_reputation += ReputationValues.ARRH;
        TargetRpcUpdateReputationUI(GetComponent<NetworkIdentity>().connectionToClient);
        TargetRpcUpdateScoreUI(GetComponent<NetworkIdentity>().connectionToClient);
        RpcHideTreasure();
        StartCoroutine(LocalGameManager.Instance.c_RespawnTreasure());

        RpcArrhScoredBy(netId);
    }

    [Command]
    public void CmdImIntheArenaNow()
    {
        m_InsideArena = true;
    }

    IEnumerator RespawnPlayerOutsideArena(Player p)
    {
        p.StartCoroutine(p.shutdownAvviso());
        p.gameObject.GetComponent<MoveSimple>().ActualSpeed = 0;
        p.gameObject.GetComponent<MoveSimple>().DontPush = true;

        yield return new WaitForSeconds(Symbols.arrhCelebrationTimeLength);
        p.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        p.gameObject.transform.position = p.m_SpawnPoint;
        CmdImIntheArenaNow();

        yield return new WaitForSeconds(1f);
        p.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        p.gameObject.GetComponent<MoveSimple>().DontPush = false;
    }


    public IEnumerator shutdownAvviso()
    {
        yield return new WaitForSeconds(Symbols.avvisoTimeLength);
        Utility.recursivePlayAnimation(m_Avviso.transform, "FadeOut");
        yield return new WaitUntil(() => !m_Avviso.GetComponent<Animation>().isPlaying);
        m_Avviso.transform.GetChild(0).gameObject.SetActive(false);
    }

    [ClientRpc]
    public void RpcArrhScoredBy(NetworkInstanceId p)
    {
        Player pl = LocalGameManager.Instance.m_LocalPlayer.GetComponent<Player>();
        Utility.recursiveSetAlphaChannel(pl.m_Avviso.transform);
        pl.m_Avviso.transform.GetChild(0).gameObject.SetActive(true);

        if(p == pl.netId)
        {
            pl.m_Avviso.transform.GetChild(0).gameObject.GetComponentInChildren<Text>().text = "You have scored an ARRH!...To the Arena!";
            StartCoroutine(RespawnPlayerOutsideArena(pl));
        }
        else
        {
            pl.m_Avviso.transform.GetChild(0).gameObject.GetComponentInChildren<Text>().text = ClientScene.FindLocalObject(p).GetComponent<Player>().playerName + " has scored an ARRH!";

            if (!pl.m_InsideArena)
            {
                pl.m_Avviso.transform.GetChild(0).gameObject.GetComponentInChildren<Text>().text += "...To the Arena!";
                StartCoroutine(RespawnPlayerOutsideArena(pl));
            }

        }

        Utility.recursivePlayAnimation(pl.m_Avviso.transform, "FadeIn");

    }

    [TargetRpc]
    public void TargetRpcUpdateReputationUI(NetworkConnection u)
    {
        StartCoroutine(UpdateReputationUI());
    }

    IEnumerator UpdateReputationUI()
    {
        yield return new WaitForSeconds(0.1f);
        m_reputationTextUI.text = "Reputation " + GetComponent<FlagshipStatus>().m_reputation.ToString();
    }

    [TargetRpc]
    public void TargetRpcUpdateScoreUI(NetworkConnection u)
    {
        StartCoroutine(UpdateScoreUI());
    }

    IEnumerator UpdateScoreUI()
    {
        yield return new WaitForSeconds(0.1f);
        m_scoreTextUI.text = "Score " + m_score.ToString();
    }

    [ClientRpc]
    public void RpcHideTreasure()
    {
        if (m_HasTreasure)
        {
            GameObject.FindGameObjectWithTag("TreasureUI").GetComponent<Image>().enabled = false;
            m_HasTreasure = false;
            m_LocalTreasure.SetActive(false);
            LocalGameManager.Instance.m_TreasureIsInGame = false;
        }
    }

    public int GetPlayerId()
    {
        return playerId;
    }

    public bool isThisPlayerLocal()
    {
        return isLocalPlayer;
    }


    private float startTime, journeyLength;
    public bool barca_isMoving = false;

    public void SpostaBarca(GameObject barca, Vector3 end, float speed)
    {
        startTime = Time.time;
        journeyLength = Vector3.Distance(barca.transform.position, end);
        StartCoroutine(SpostaBarcaLoop(barca, end, speed));
    }

    private IEnumerator SpostaBarcaLoop(GameObject barca, Vector3 end, float speed)
    {
        if (barca.GetComponent<Player>())
            barca.GetComponent<Player>().barca_isMoving = true;

        while (Vector3.Distance(barca.transform.position, end) > 0.01f)
        {
            float distCovered = (Time.time - startTime) * speed;
            float fracJourney = distCovered / journeyLength;
            barca.transform.position = Vector3.Lerp(barca.transform.position, end, fracJourney);
            yield return null;
        }
        barca.transform.position = end;

        if (barca.GetComponent<Player>())
            barca.GetComponent<Player>().barca_isMoving = false;
    }
}
