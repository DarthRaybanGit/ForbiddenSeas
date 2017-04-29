using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;


public class CanvasControl : MonoBehaviour {

    public GameObject m_ConnectButtons;
    public GameObject m_LobbyButtons;
    public GameObject m_Clock;


	public void StartConnectionAsServer()
    {
        if (OnlineManager.s_Singleton.StartServer()) Debug.Log("Server initialized correctly."); else Debug.Log("Server initialization fault.");
        m_ConnectButtons.SetActive(false);
        m_Clock.transform.SetParent(transform);
        m_Clock.SetActive(true);
    }

    public void StartConnectionAsClient()
    {
        OnlineManager.s_Singleton.StartClient();
        m_ConnectButtons.SetActive(false);
        m_LobbyButtons.SetActive(true);

    }

    public void StartConnectionAsHost()
    {
        OnlineManager.s_Singleton.StartHost();
        m_ConnectButtons.SetActive(false);
        m_LobbyButtons.SetActive(true);
    }

    public void SelectClass(int n)
    {
        Debug.Log("Selected class " + n);
        LocalGameManager.Instance.m_LocalPlayer.GetComponent<PlayerManager>().setLocalClass(n);
    }

    public void ReadyToPlay()
    {
        LocalGameManager.Instance.m_LocalPlayer.GetComponent<NetworkLobbyPlayer>().readyToBegin = true;
        m_LobbyButtons.SetActive(false);
        LocalGameManager.Instance.m_LocalPlayer.GetComponent<NetworkLobbyPlayer>().SendReadyToBeginMessage();
    }


    public void Update()
    {
        if(LocalGameManager.Instance.m_timeIsSynced || LocalGameManager.Instance.m_serverTimeSended)
        {
            float time = LocalGameManager.Instance.syncedTime();
            int minutes = Mathf.FloorToInt(time/60);
            m_Clock.GetComponent<Text>().text = String.Format("{0,2:D2} {1,2:D2}", minutes, Mathf.FloorToInt(time - minutes * 60));
        }
    }


}
