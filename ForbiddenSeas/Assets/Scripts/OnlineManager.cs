using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class OnlineManager : NetworkManager {

    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);

    }

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        base.OnServerAddPlayer(conn, playerControllerId);
        foreach(PlayerController pc in conn.playerControllers)
        {
            Debug.Log("Player name: " + pc.gameObject.name + " ID: " + pc.gameObject.GetInstanceID());
        }
        Debug.Log("ID: " + playerControllerId);
        //GameObject pg = conn.playerControllers.ToArray()[0].gameObject;
    }



    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);
        Debug.Log(ClientScene.localPlayers.Count);
    }
}
