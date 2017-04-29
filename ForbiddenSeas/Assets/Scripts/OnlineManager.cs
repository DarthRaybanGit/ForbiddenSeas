using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OnlineManager : NetworkLobbyManager {


    public static OnlineManager s_Singleton;

    public GameObject[] m_playerPlacement;
    public GameObject[] m_AdmiralList = new GameObject[4];
    Dictionary<int, int> currentPlayers;

    public GameObject m_GamePlayer;

    void Start()
    {
        s_Singleton = this;
        m_playerPlacement = new GameObject[4];
        currentPlayers = new Dictionary<int, int>();
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
            currentPlayers.Add(conn.connectionId, 0);

        return base.OnLobbyServerCreateLobbyPlayer(conn, playerControllerId);
    }

    public void SetPlayerTypeLobby(NetworkConnection conn, int _type)
    {
        if (currentPlayers.ContainsKey(conn.connectionId))
            currentPlayers[conn.connectionId] = _type;
    }

    public override GameObject OnLobbyServerCreateGamePlayer(NetworkConnection conn, short playerControllerId)
    {

        SetPlayerTypeLobby(conn, conn.playerControllers.ToArray()[0].gameObject.GetComponent<PlayerManager>().m_LocalClass);

        GameObject pl = GameObject.Instantiate(m_AdmiralList[conn.playerControllers.ToArray()[0].gameObject.GetComponent<PlayerManager>().m_LocalClass]);

        GameObject g = GameObject.Instantiate(gamePlayerPrefab);


        pl.transform.SetParent(g.transform);
        NetworkTransformChild ntc = g.GetComponent<NetworkTransformChild>();
        ntc.target = pl.transform;
        ntc.enabled = true;

        NetworkServer.ReplacePlayerForConnection(conn, pl, playerControllerId);

        return pl;
    }


    public override void OnLobbyClientSceneChanged(NetworkConnection conn)
    {
        base.OnLobbyClientSceneChanged(conn);

    }
}
