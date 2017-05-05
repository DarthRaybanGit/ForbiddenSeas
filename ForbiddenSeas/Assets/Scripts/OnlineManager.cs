using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OnlineManager : NetworkLobbyManager {


    public static OnlineManager s_Singleton;

    public GameObject[] m_playerPlacement;
    public GameObject[] m_AdmiralList = new GameObject[4];
    public Dictionary<int, int[]> currentPlayers;

    public GameObject m_GamePlayer;

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
        return true;
    }

    public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
    {
        if (!currentPlayers.ContainsKey(conn.connectionId))
            currentPlayers.Add(conn.connectionId, new int[10]);

        return base.OnLobbyServerCreateLobbyPlayer(conn, playerControllerId);
    }

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

    public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
    {

        GameObject pl = GameObject.Instantiate(m_AdmiralList[conn.playerControllers.ToArray()[0].gameObject.GetComponent<PlayerManager>().m_LocalClass]);

        GameObject g = GameObject.Instantiate(gamePlayerPrefab);

        //Setting startup PlayerInfo

        SetPlayerInfoLoadedFlag(conn, false);

        pl.transform.SetParent(g.transform);
        NetworkTransformChild ntc = g.GetComponent<NetworkTransformChild>();
        ntc.target = pl.transform;
        ntc.enabled = true;

        NetworkServer.ReplacePlayerForConnection(conn, pl, playerControllerId);

        SetPlayerInfoNetID(conn, pl.GetComponent<Player>().netId.Value);
        return pl;
    }


    public override void OnLobbyClientSceneChanged(NetworkConnection conn)
    {
        base.OnLobbyClientSceneChanged(conn);
    }
}
