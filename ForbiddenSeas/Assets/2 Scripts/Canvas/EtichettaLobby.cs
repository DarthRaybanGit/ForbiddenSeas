using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EtichettaLobby : MonoBehaviour {

    [Range(1,4)]
    public int m_player;

    public Text m_PlayerName;
    public Image m_Class;
    public Sprite[] sprites;
    public Text m_WaitForReady;


	// Update is called once per frame
	void Update () {
        if(OnlineManager.s_Singleton.lobbySlots.Length >= m_player && OnlineManager.s_Singleton.lobbySlots[m_player - 1])
        {
            //OnlineManager.s_Singleton.currentPlayers[m_player - 1];
            m_PlayerName.text = OnlineManager.s_Singleton.lobbySlots[m_player - 1].gameObject.GetComponent<PlayerManager>().m_PlayerName;
            m_Class.sprite = sprites[OnlineManager.s_Singleton.lobbySlots[m_player - 1].gameObject.GetComponent<PlayerManager>().m_LocalClass];
            m_WaitForReady.text = OnlineManager.s_Singleton.lobbySlots[m_player - 1].gameObject.GetComponent<PlayerManager>().ready ? "Ready!" : "Waiting...";
        }

	}
}
