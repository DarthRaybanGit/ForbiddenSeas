using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LocalGameManager : NetworkBehaviour {

    public static LocalGameManager Instance = null;

    public GameObject m_LocalPlayer;

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

    }



    //Funzioni per la sincronizzazione del timestamp del server sui clients.

    [ClientRpc]
    public void RpcNotifyServerTime(float time)
    {
        Debug.Log("Sto notificando il time");
        m_ServerOffsetTime = time - Time.timeSinceLevelLoad;
        m_timeIsSynced = true;
        m_serverTimeSended = true;
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

    public IEnumerator c_WaitForTreasure()
    {
        yield return new WaitForSeconds(180f);
        //Risincronizza il time per sicurezza
        LocalGameManager.Instance.RpcNotifyServerTime(Time.timeSinceLevelLoad);
        Debug.Log("Tesoro Spawn!!!");

        //Spawnare il tesoro e aprire le porte dei canali.
    }

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


}
