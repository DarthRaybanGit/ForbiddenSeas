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
    public GameObject m_InputName;
    public GameObject m_ReadyButton;
    public GameObject[] m_stats;
    public Animation logo;
    private int currentSelectedClass = 0;

    public string m_PlayerName = "Player";


	public void StartConnectionAsServer()
    {
        if (OnlineManager.s_Singleton.StartServer()) Debug.Log("Server initialized correctly."); else Debug.Log("Server initialization fault.");
        m_ConnectButtons.SetActive(false);
    }

    public void StartConnectionAsClient()
    {
        m_InputName.SetActive(true);
        m_ConnectButtons.SetActive(false);

    }

    public void StartConnectionAsHost()
    {
        OnlineManager.s_Singleton.StartHost();
        m_ConnectButtons.SetActive(false);
        m_LobbyButtons.SetActive(true);
    }

    public void SelectClass(int n, bool first = false)
    {
        Debug.Log("Selected class " + n);
        StartCoroutine(waitPlayer(n, first));
    }

    IEnumerator waitPlayer(int n, bool first)
    {
        yield return new WaitForSeconds(first ? 3f : 0.2f);
        if (first)
            m_LobbyButtons.transform.GetChild(0).gameObject.GetComponent<Button>().interactable = true;
        Debug.Log("Settaggio");
        LocalGameManager.Instance.m_LocalPlayer.GetComponent<PlayerManager>().CmdsetLocalName(m_PlayerName);
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
        SelectClass(0, true);
    }

    public void nextClassSelect()
    {
        if (currentSelectedClass == 3)
            return;
        m_stats[currentSelectedClass].SetActive(false);
        m_stats[++currentSelectedClass].SetActive(true);

        Camera.main.GetComponent<CameraController>().moveCamera(currentSelectedClass);
        SelectClass(currentSelectedClass);
    }

    public void prevClassSelect()
    {
        if (currentSelectedClass == 0)
            return;

        m_stats[currentSelectedClass].SetActive(false);
        m_stats[--currentSelectedClass].SetActive(true);

        Camera.main.GetComponent<CameraController>().moveCamera(currentSelectedClass);
        SelectClass(currentSelectedClass);
    }



    public void SetName(InputField t)
    {

        m_PlayerName = t.text;
        OnlineManager.s_Singleton.StartClient();
        m_LobbyButtons.SetActive(true);
        SelectionStart();
        m_InputName.SetActive(false);
    }
}
