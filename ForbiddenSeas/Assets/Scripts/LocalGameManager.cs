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


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        DontDestroyOnLoad(transform.gameObject);
        m_PlayersID = new Dictionary<int, int>();
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


    [ClientRpc]
    public void RpcNotifyPlayersID(int[] ids)
    {
        Debug.Log("CAZZO");
        StartCoroutine(delayedIDRegistration(ids));
    }

    IEnumerator delayedIDRegistration(int[] ids)
    {
        yield return new WaitUntil(() => b_playersRegistered);
        Debug.Log("Sto registrando gli NetID dei player " + ids.Length);
        for (int i = 0; i < ids.Length; i++)
        {
            Debug.Log("Ho ricevuto il NetID " + ids[i]);
            m_PlayersID[i] = ids[i];
        }
        Debug.Log("Ho aggiunto " + m_PlayersID.Values.Count + " valori");
    }

    [ClientRpc]
    public void RpcSearchGameObjectForPlayers()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        LocalGameManager.Instance.m_Players = new GameObject[players.Length];

        Debug.Log("Ho trovato " + players.Length + " giocatori. " + m_Players.Length + " e IDs " + m_PlayersID.Keys.Count);

        int count = 0;
        foreach(int i in m_PlayersID.Values)
        {
            int j;
            for(j = 0; j < players.Length; j++)
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
        return (playerId) > m_Players.Length ? null : m_Players[playerId];
    }

    public int GetPlayerId(GameObject player_go)
    {
        int i;
        for(i = 0; i < m_Players.Length; i++)
        {
            if (m_Players[i].GetInstanceID() == player_go.GetInstanceID())
                return i + 1;

        }
        return -1;
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
        yield return new WaitForSeconds(180f);
        //Risincronizza il time per sicurezza
        LocalGameManager.Instance.RpcNotifyServerTime(Time.timeSinceLevelLoad);
        Debug.Log("Tesoro Spawn!!!");

        //Spawnare il tesoro e aprire le porte dei canali.
    }

    [Server]
    public IEnumerator c_LoopPowerUp()
    {
        while (LocalGameManager.Instance.m_GameIsStarted)
        {
            yield return new WaitForSeconds(60f);
            //Risincronizza il time per sicurezza
            LocalGameManager.Instance.RpcNotifyServerTime(Time.timeSinceLevelLoad);
            Debug.Log("PowerUp SPAWN!!!");

            //Controllare se un power up è già presente oppure no in quel caso non spawnare nulla.
        }
    }

    [Server]
    public IEnumerator c_WaitUntilEveryPlayersOnline()
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
            StartCoroutine(LocalGameManager.Instance.c_WaitForTreasure());
            StartCoroutine(LocalGameManager.Instance.c_LoopPowerUp());
            LocalGameManager.Instance.m_serverTimeSended = true;
            LocalGameManager.Instance.RpcNotifyServerTime(Time.timeSinceLevelLoad);
            LocalGameManager.Instance.RpcSearchGameObjectForPlayers();
        }
    }




}
