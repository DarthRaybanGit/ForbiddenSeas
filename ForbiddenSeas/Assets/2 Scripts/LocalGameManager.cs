using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LocalGameManager : NetworkBehaviour {

    public static LocalGameManager Instance = null;

    public GameObject m_LocalPlayer;

    public Dictionary<int,int> m_PlayersID;
    public GameObject[] m_Players;
    public bool m_PlayerRegistered = false;


    public GameObject[] m_LocalClassViewer;

    public bool m_GameIsStarted = false;
    public bool m_GameGeneralLoopIsStarted = false;

    //Variabili per la sincronizzazione del tempo di gioco
    public bool m_serverTimeSended = false;
    public bool m_timeIsSynced = false;
    public float m_ServerOffsetTime;

    public static float m_MatchEndTime;

    public GameObject m_Treasure;
    public bool m_TreasureIsInGame = false;
    public GameObject[] m_Ports;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        DontDestroyOnLoad(transform.gameObject);
        m_PlayersID = new Dictionary<int, int>();
    }

    public override void OnStartServer()
    {
        m_Ports = new GameObject[4];
    }



    //Funzione per la restituzione dell'array contenente i connection ID dei player
    private bool b_playersRegistered = false;

    [ClientRpc]
    public void RpcNotifyPlayersInGame(int[] players, int[] ids)
    {
        Debug.Log("Sto registrando i player " + players.Length + " con " + ids.Length + " IDS");

        if (players.Length != ids.Length)
        {
            return;
        }

        for(int i = 0; i < players.Length; i++)
        {
            if (!m_PlayersID.ContainsKey(players[i]))
                m_PlayersID.Add(players[i], ids[i]);
        }

        Debug.Log("Ho aggiunto " + m_PlayersID.Keys.Count + " chiavi" + " Ho aggiunto " + m_PlayersID.Values.Count + " valori");
        b_playersRegistered = true;
    }

    private bool idRegistrati()
    {
        return b_playersRegistered;
    }

    [ClientRpc]
    public void RpcSearchGameObjectForPlayers()
    {
        StartCoroutine(delayedGameObjectRegistration());
    }

    IEnumerator delayedGameObjectRegistration()
    {
        yield return new WaitUntil(() => idRegistrati());

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        LocalGameManager.Instance.m_Players = new GameObject[players.Length];

        //Debug.Log("Ho trovato " + players.Length + " giocatori. " + m_Players.Length + " e IDs " + m_PlayersID.Keys.Count);

        int count = 0;
        foreach (int i in m_PlayersID.Values)
        {
            int j;
            for (j = 0; j < players.Length; j++)
            {
                if (i == (int)players[j].GetComponent<Player>().netId.Value)
                    break;
            }
            LocalGameManager.Instance.m_Players[count] = players[j];
            count++;
        }

        m_PlayerRegistered = true;
    }


    //Funzioni public per la restituzione del gameobject dato l'ID

    public GameObject GetPlayer(int playerId)
    {
        return (playerId) > m_Players.Length || playerId <= 0 ? null : m_Players[playerId - 1];
    }

    public GameObject GetPlayerFromNetID(int netID)
    {
        int count = 0;
        foreach(int i in m_PlayersID.Keys)
        {
            if (m_PlayersID[i] == netID)
                return m_Players[count];
            count++;
        }
        return null;
    }

    public int GetPlayerId(GameObject player_go)
    {
        if (!player_go || !player_go.CompareTag("Player"))
            return Symbols.PLAYER_NOT_SET;

        int i;

        for(i = 0; i < m_Players.Length; i++)
        {
            if (m_Players[i].GetComponent<Player>().netId == player_go.GetComponent<Player>().netId)
                return i + 1;
        }
        return Symbols.PLAYER_NOT_SET;
    }

    //Funzioni per la sincronizzazione del timestamp del server sui clients.

    [ClientRpc]
    public void RpcNotifyServerTime(float time)
    {
        Debug.Log("Sto notificando il time");
        m_ServerOffsetTime = time - Time.timeSinceLevelLoad;
        m_timeIsSynced = true;
        m_serverTimeSended = true;

        //ShowCountdown
    }

    public float syncedTime()
    {
        return isServer ? Time.timeSinceLevelLoad  : Time.timeSinceLevelLoad + m_ServerOffsetTime;
    }


    /*
     *
     *  Co-routines per la gestione del Loop di Gioco
     *
     */

    [Server]
    public IEnumerator c_WaitForTreasure()
    {
        yield return new WaitForSeconds((int)FixedDelayInGame.TREASURE_FIRST_SPAWN);
        //Risincronizza il time per sicurezza
        LocalGameManager.Instance.RpcNotifyServerTime(Time.timeSinceLevelLoad);

        m_Treasure = GameObject.Instantiate(OnlineManager.s_Singleton.spawnPrefabs.ToArray()[(int)SpawnIndex.TREASURE]);

        Debug.Log("Tesoro Spawn!!!");
        NetworkServer.Spawn(m_Treasure);
        //Aprire le porte dei canali.

        //Spawn dei Porti
        int count = 0;
        for(; count < m_Ports.Length; count++)
        {
            m_Ports[count] = GameObject.Instantiate(OnlineManager.s_Singleton.spawnPrefabs.ToArray()[(int)SpawnIndex.PORTO], OnlineManager.s_Singleton.m_PortSpawnPosition[count].position, Quaternion.identity);
        }


        foreach(GameObject g in m_Ports)
        {
            NetworkServer.Spawn(g, g.GetComponent<NetworkIdentity>().assetId);
        }

    }

    [Server]
    public IEnumerator c_LoopPowerUp()
    {
        while (LocalGameManager.Instance.m_GameIsStarted)
        {
            yield return new WaitForSeconds((int)FixedDelayInGame.POWERUP_SPAWN);
            //Risincronizza il time per sicurezza
            LocalGameManager.Instance.RpcNotifyServerTime(Time.timeSinceLevelLoad);
            Debug.Log("PowerUp SPAWN!!!");

            //Controllare se un power up è già presente oppure no in quel caso non spawnare nulla.
        }
    }

    [Server]
    public IEnumerator c_RespawnTreasure()
    {
        yield return new WaitForSeconds((int)FixedDelayInGame.TREASURE_RESPAWN);
        Destroy(m_Treasure);
        m_Treasure = GameObject.Instantiate(OnlineManager.s_Singleton.spawnPrefabs.ToArray()[(int)SpawnIndex.TREASURE]);
        NetworkServer.Spawn(m_Treasure);
    }

    [Server]
    public IEnumerator c_WaitUntilEveryPlayersOnline(int[] msg1, int[] msg2)
    {
        float timestamp = Time.time;
        yield return new WaitUntil(() => OnlineManager.s_Singleton.EveryoneIsOnline() || Time.time > (timestamp + (int)FixedDelayInGame.PLAYERS_DELAY));


        if (!OnlineManager.s_Singleton.EveryoneIsOnline())
        {
            //Se ci sono tre giocatori si inizia ugualmente
            //se no fa tornare tutti i player alla lobby
        }
        else
        {
            //Start Game!
            Debug.Log("START GAME!!!");
            LocalGameManager.Instance.RpcNotifyPlayersInGame(msg1,msg2);
            StartCoroutine(LocalGameManager.Instance.c_WaitForTreasure());
            StartCoroutine(LocalGameManager.Instance.c_LoopPowerUp());
            LocalGameManager.Instance.m_serverTimeSended = true;
            LocalGameManager.Instance.RpcNotifyServerTime(Time.timeSinceLevelLoad);
            LocalGameManager.Instance.RpcSearchGameObjectForPlayers();
        }
    }


    public bool IsEveryPlayerRegistered()
    {
        //Debug.Log("Looking for Player: " + m_PlayerRegistered);
        if (m_PlayerRegistered)
        {
            foreach (GameObject g in m_Players)
            {
                if(!g || !g.activeSelf)
                    return false;
            }
        }
        else
        {
            return false;
        }
        return m_PlayerRegistered;
    }

    public int WhoAmI(GameObject me)
    {
        if (IsEveryPlayerRegistered())
        {
            return GetPlayerId(me);
        }
        else
            return Symbols.PLAYER_NOT_SET;
    }

    [Client]
    public GameObject WhoAsTheTreasure()
    {
        if (IsEveryPlayerRegistered())
        {
            foreach(GameObject g in m_Players)
            {
                if (g.GetComponent<Player>().m_HasTreasure)
                    return g;
            }
        }
        return null;
    }

    [ClientRpc]
    public void RpcNotifyNewTreasureOwner(int playerId, NetworkInstanceId treasure)
    {
        if (IsEveryPlayerRegistered())
        {
            GameObject g = GetPlayerFromNetID(playerId);
            Player pl = g ? g.GetComponent<Player>() : null;
            if (pl)
            {
                pl.m_LocalTreasure.SetActive(true);
                pl.m_HasTreasure = true;

                GameObject tr = ClientScene.FindLocalObject(treasure);
                if (tr)
                {
                    Destroy(tr);
                }
            }
            else
            {
                Debug.Log("No player " + playerId + " trovato!");
            }
        }
    }



}
