using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class OnlineManager : NetworkLobbyManager {


    public static OnlineManager s_Singleton;

    public GameObject[] m_playerPlacement;

    public Transform[] m_SpawnPosition;
    public Transform[] m_PortSpawnPosition;
    public Transform[] m_PowerUpSpawnPosition;
    public Transform[] m_MinesSpawnPosition;

    public GameObject[] m_AdmiralList = new GameObject[4];

    public Dictionary<int, int[]> currentPlayers;

    public GameObject m_GamePlayer;


    public bool gameInPlay = false;

    void Start()
    {
        s_Singleton = this;
        m_playerPlacement = new GameObject[4];
        currentPlayers = new Dictionary<int, int[]>();
    }


    public override void OnLobbyServerPlayersReady()
    {
        base.OnLobbyServerPlayersReady();
        Debug.Log("All Ready!");
    }

    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {

        int cc = lobbyPlayer.GetComponent<PlayerManager>().m_LocalClass;
        gamePlayer.GetComponent<Player>().SetClass(cc);
        gamePlayer.GetComponent<Player>().playerName = lobbyPlayer.GetComponent<PlayerManager>().m_PlayerName;
        for(int i = 0; i < lobbySlots.Length; i++)
        {
            if(lobbySlots[i] && lobbySlots[i].GetComponent<PlayerManager>().netId == lobbyPlayer.GetComponent<PlayerManager>().netId)
            {
                gamePlayer.GetComponent<Player>().playerId = i + 1;
            }
        }

        return true;
    }

    public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
    {
        if (!currentPlayers.ContainsKey(conn.connectionId))
        {
            currentPlayers.Add(conn.connectionId, new int[10]);

        }
        GameObject g = base.OnLobbyServerCreateLobbyPlayer(conn, playerControllerId);
        int count = 0;
        for(int i = 0; i < lobbySlots.Length; i++)
        {
            if (lobbySlots[i])
                count++;
        }
        Debug.Log("Si è connesso " + conn.connectionId + " il numero di player attuali è " + count);

        StartCoroutine(waitForLobbyFill(conn, count + 1));

        return g;
    }

    IEnumerator waitForLobbyFill(NetworkConnection conn, int n)
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            int count = 0;
            for (int i = 0; i < lobbySlots.Length; i++)
            {
                if (lobbySlots[i])
                    count++;
            }
            if (count >= n)
                break;
        }
        LocalGameManager.Instance.TargetRpcNotifyClientConnection(conn);

    }

    //Modify Player Info

    public void SetPlayerInfoNetID(NetworkConnection conn, uint id)
    {
        if (currentPlayers.ContainsKey(conn.connectionId))
            currentPlayers[conn.connectionId][(int)PlayerInfo.ID] = (int)id;
    }

    public void SetPlayerInfoLoadedFlag(NetworkConnection conn, bool loaded)
    {
        if (currentPlayers.ContainsKey(conn.connectionId))
            currentPlayers[conn.connectionId][(int)PlayerInfo.IS_LOADED] = loaded ? 1 : 0;
    }

    public void SetPlayerInfoRespawnLocation(NetworkConnection conn, int index)
    {
        if (currentPlayers.ContainsKey(conn.connectionId))
            currentPlayers[conn.connectionId][(int)PlayerInfo.SPAWN_POSITION] = index;
    }

    public bool EveryoneIsOnline()
    {
        int checksum = 0;
        foreach(int[] i in currentPlayers.Values)
        {
            checksum += i[(int)PlayerInfo.IS_LOADED];
        }
        Debug.Log("Ci sono Online: " + checksum + " su " + currentPlayers.Keys.Count);
        return checksum == currentPlayers.Keys.Count;
    }

    private int count = 0;

    public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
    {
        SetPlayerInfoRespawnLocation(conn, count);
        count++;
        GameObject pl = GameObject.Instantiate(m_AdmiralList[conn.playerControllers.ToArray()[0].gameObject.GetComponent<PlayerManager>().m_LocalClass], m_SpawnPosition[currentPlayers[conn.connectionId][(int)PlayerInfo.SPAWN_POSITION]].position, Quaternion.identity);

        GameObject g = GameObject.Instantiate(gamePlayerPrefab);

        //Setting startup PlayerInfo

        SetPlayerInfoLoadedFlag(conn, false);

        pl.transform.SetParent(g.transform);
        /*
        NetworkTransformChild ntc = g.GetComponent<NetworkTransformChild>();
        ntc.target = pl.transform;
        ntc.enabled = true;
        */

        NetworkServer.ReplacePlayerForConnection(conn, pl, playerControllerId);

        SetPlayerInfoNetID(conn, pl.GetComponent<Player>().netId.Value);

        pl.GetComponent<FlagshipStatus>().InitializeFlagshipStatus();
        return pl;
    }


    public override void OnLobbyClientSceneChanged(NetworkConnection conn)
    {
        base.OnLobbyClientSceneChanged(conn);
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (LocalGameManager.Instance.m_GameIsStarted)
        {
            Debug.Log("Qualcuno si è disconnesso " + conn.connectionId + 1);
            LocalGameManager.Instance.RpcToTheLobby();

            StartCoroutine(toLobby());
        }
        base.OnServerDisconnect(conn);
    }


    IEnumerator toLobby()
    {
        yield return new WaitForSeconds(3f);
        ServerReturnToLobby();
        Destroy(LocalGameManager.Instance);
        yield return new WaitForSeconds(1f);
        StopServer();
        StartServer();
    }

}
