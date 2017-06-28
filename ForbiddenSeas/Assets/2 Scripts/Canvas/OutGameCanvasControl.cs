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
    public GameObject m_ReadyText;

    public GameObject m_StartServer;
    public GameObject m_NumPlayer;
    public GameObject m_Minuti;


    public GameObject[] m_stats;
    public Animation logo;
    public InputField indirizzoIP;
    public InputField n_players;
    public InputField minutes;
    private int currentSelectedClass = 0;
    public bool vai = false;

    public GameObject LocalGameManagerPrefab;

    public string m_PlayerName = "Player";

    public void Start()
    {
        /*
        Debug.Log("Lobby Start");
        if (LocalGameManager.Instance.m_GameIsStarted)
        {

            if (LocalGameManager.Instance.isClient)
            {

                for (int i = 0; i < OnlineManager.s_Singleton.lobbySlots.Length; i++)
                {
                    if (OnlineManager.s_Singleton.lobbySlots[i].isLocalPlayer)
                    {
                        LocalGameManager.Instance.m_LocalPlayer = OnlineManager.s_Singleton.lobbySlots[i].gameObject;
                    }
                }
                SelectionStart();
            }
        }*/

    }

    public void UnHideServerSettings()
    {
        foreach(Transform t in m_ConnectButtons.transform)
        {
            t.gameObject.SetActive(false);
        }

        m_NumPlayer.SetActive(true);
        m_StartServer.SetActive(true);
        m_Minuti.SetActive(true);
    }

    public void StartConnectionAsServer()
    {
        if (OnlineManager.s_Singleton.StartServer()) Debug.Log("Server initialized correctly."); else Debug.Log("Server initialization fault.");
        m_ConnectButtons.SetActive(false);
    }

    public void StartConnectionAsClient()
    {
        string ip = indirizzoIP.text;

        if (ip.Length < 2)
            ip = "169.254.239.89";

        NetworkManager.singleton.networkAddress = ip;
        m_ConnectButtons.SetActive(false);
        m_InputName.SetActive(true);


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
        {
            foreach(Transform t in m_LobbyButtons.transform)
            {
                if (t.gameObject.GetComponent<Button>())
                {
                    t.gameObject.GetComponent<Button>().interactable = true;
                    vai = true;
                }
            }
        }

        Debug.Log("Settaggio");
        LocalGameManager.Instance.m_LocalPlayer.GetComponent<PlayerManager>().CmdsetLocalName(m_PlayerName);
        LocalGameManager.Instance.m_LocalPlayer.GetComponent<PlayerManager>().setLocalClass(n);
    }

    public void ReadyToPlay()
    {
        LocalGameManager.Instance.m_LocalPlayer.GetComponent<NetworkLobbyPlayer>().readyToBegin = true;
        LocalGameManager.Instance.m_LocalPlayer.GetComponent<PlayerManager>().CmdImReadyToBegin();
        m_LobbyButtons.SetActive(false);
        m_ReadyText.SetActive(true);
        LocalGameManager.Instance.m_LocalPlayer.GetComponent<NetworkLobbyPlayer>().SendReadyToBeginMessage();
    }

    public void UnHideButtons()
    {
        m_ConnectButtons.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
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
        m_stats[currentSelectedClass].SetActive(false);
        if (currentSelectedClass == 3)
            currentSelectedClass = -1;
        m_stats[++currentSelectedClass].SetActive(true);

        Camera.main.GetComponent<CameraController>().moveCamera(currentSelectedClass);
        SelectClass(currentSelectedClass);
    }

    public void prevClassSelect()
    {
        m_stats[currentSelectedClass].SetActive(false);
        if (currentSelectedClass == 0)
            currentSelectedClass = 4;
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

    public void SetPlayerNumbers()
    {
        OnlineManager.s_Singleton.minPlayers = Int32.Parse(n_players.text);
    }

    public void SetMinutes()
    {
        OnlineManager.s_Singleton.m_matchDuration = Int32.Parse(minutes.text) * 60f;
    }
}
