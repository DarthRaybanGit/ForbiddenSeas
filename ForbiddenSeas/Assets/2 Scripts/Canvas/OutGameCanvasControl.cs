using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;


public class OutGameCanvasControl : MonoBehaviour
{

    public GameObject m_ConnectButtons;
    public GameObject m_LobbyButtons;
    public GameObject m_OtherPlayers;
    public Animation logo;
    private int currentSelectedClass = 0;


	public void StartConnectionAsServer()
    {
        if (OnlineManager.s_Singleton.StartServer()) Debug.Log("Server initialized correctly."); else Debug.Log("Server initialization fault.");
        m_ConnectButtons.SetActive(false);
    }

    public void StartConnectionAsClient()
    {
        OnlineManager.s_Singleton.StartClient();
        m_ConnectButtons.SetActive(false);
        m_LobbyButtons.SetActive(true);
        SelectionStart();
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

    public void UnHideButtons()
    {
        m_ConnectButtons.SetActive(true);
    }

    private void SelectionStart()
    {
        Camera.main.GetComponent<CameraController>().moveCamera(0);
        m_OtherPlayers.SetActive(true);
        logo.Play("LogoBack");
        SelectClass(0);
    }

    public void nextClassSelect()
    {
        if (currentSelectedClass == 3)
            return;
        
        currentSelectedClass++;
        Camera.main.GetComponent<CameraController>().moveCamera(currentSelectedClass);
        SelectClass(currentSelectedClass);
    }

    public void prevClassSelect()
    {
        if (currentSelectedClass == 0)
            return;
        
        currentSelectedClass--;
        Camera.main.GetComponent<CameraController>().moveCamera(currentSelectedClass);
        SelectClass(currentSelectedClass);
    }

}
