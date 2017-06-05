using System.Collections;
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
            LocalGameManager.Instance.RpcNotifyServerTime(LocalGameManager.Instance.m_ServerOffsetTime, true);
            StartCoroutine(LocalGameManager.Instance.c_WaitForTreasure());
            StartCoroutine(LocalGameManager.Instance.c_LoopPowerUp());
            StartCoroutine(LocalGameManager.Instance.c_LoopCoins());

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
        LocalGameManager.Instance.m_PowerUp[(int)p] = false;
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
        Debug.Log("Player " + (int)netId.Value + " ha segnato un ARRH!");

        m_score++;
        GetComponent<FlagshipStatus>().m_reputation += ReputationValues.ARRH;
        TargetRpcUpdateReputationUI(GetComponent<NetworkIdentity>().connectionToClient);
        TargetRpcUpdateScoreUI(GetComponent<NetworkIdentity>().connectionToClient);
        RpcHideTreasure();
        StartCoroutine(LocalGameManager.Instance.c_RespawnTreasure());

        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (!g.GetComponent<Player>().m_InsideArena)
            {
                RpcResetPlayerPosition(g.GetComponent<Player>().netId, netId);
                g.GetComponent<Player>().m_InsideArena = true;
            }
        }
    }

    IEnumerator RespawnPlayerOutsideArena(Player p)
    {
        yield return new WaitForSeconds(3f);
        p.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        p.gameObject.transform.position = p.m_SpawnPoint;
        if (p.netId == LocalGameManager.Instance.m_LocalPlayer.GetComponent<NetworkIdentity>().netId)
        {
            Utility.recursivePlayAnimation(m_Avviso.transform, "FadeOut");
        }
        yield return new WaitForSeconds(1f);
        p.gameObject.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        yield return new WaitUntil(() => !m_Avviso.GetComponent<Animation>().isPlaying);
        m_Avviso.GetComponent<Text>().enabled = false;

    }

    [ClientRpc]
    public void RpcResetPlayerPosition(NetworkInstanceId p, NetworkInstanceId who)
    {
        if(p == LocalGameManager.Instance.m_LocalPlayer.GetComponent<NetworkIdentity>().netId)
        {
            m_Avviso.GetComponent<Text>().enabled = true;
            m_Avviso.GetComponent<Text>().text = (p == who) ? "You have scored an ARRH!...To Arena!"  : ClientScene.FindLocalObject(who).GetComponent<Player>().playerId + " has scored an ARRH!...To Arena!";
            Utility.recursivePlayAnimation(m_Avviso.transform, "FadeIn");
        }

        StartCoroutine(RespawnPlayerOutsideArena(ClientScene.FindLocalObject(p).GetComponent<Player>()));
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
