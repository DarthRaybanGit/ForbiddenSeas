using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour {

    public GameObject[] m_AdmiralList = new GameObject[4];
    public GameObject m_LocalCamera;
    public int playerId;

    public bool m_HasTreasure = false;
    public GameObject m_LocalTreasure;


    [SyncVar]
    public int m_Class = 0;

    public void Start()
    {
        if (!isLocalPlayer)
        {
            Destroy(m_LocalCamera.GetComponent<AudioListener>());
            return;
        }
        else
        {

        }
    }

    public override void OnStartLocalPlayer()
    {
        if (!isServer)
        {
            if (isLocalPlayer)
            {
                Camera.main.gameObject.SetActive(false);
                m_LocalCamera.tag = "MainCamera";
                m_LocalCamera.SetActive(true);
                m_LocalCamera.GetComponent<Camera>().enabled = true;
                Debug.Log("Ho finito di settare la camera.");
                CmdStartGeneralLoop((int)this.netId.Value);
                LocalGameManager.Instance.m_GameIsStarted = true;
            }

        }
    }



    public override void OnStartServer()
    {
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

            int[] to_Send = new int[OnlineManager.s_Singleton.currentPlayers.Keys.Count];
            int[] to_SendIds = new int[OnlineManager.s_Singleton.currentPlayers.Keys.Count];

            int count = 0;

            foreach (int i in OnlineManager.s_Singleton.currentPlayers.Keys)
            {
                to_Send[count] = i;
                to_SendIds[count] = OnlineManager.s_Singleton.currentPlayers[i][(int)PlayerInfo.ID];
                count++;
            }

            LocalGameManager.Instance.RpcNotifyPlayersInGame(to_Send, to_SendIds);


            StartCoroutine(LocalGameManager.Instance.c_WaitUntilEveryPlayersOnline());
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
        LocalGameManager.Instance.RpcNotifyServerTime(Time.timeSinceLevelLoad);
    }

    [Command]
    public void CmdCatchTheTreasure(int playerId)
    {
        LocalGameManager.Instance.m_Treasure.SetActive(false);
        foreach(GameObject g in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (g.GetComponent<Player>())
            {
                if(g.GetComponent<Player>().netId.Value == playerId)
                {
                    g.GetComponent<Player>().m_HasTreasure = true;
                    LocalGameManager.Instance.RpcNotifyNewTreasureOwner(playerId);
                }
            }
        }
    }

    public int GetPlayerId()
    {
        return playerId;
    }
}
