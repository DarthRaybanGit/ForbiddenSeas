using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowInGameStats : MonoBehaviour
{
    public Text [] names;
    public Text [] arrhs;
    public Text [] kills;
    public Text [] deaths;
    public Image [] classes;
    public Sprite [] sprites;

    void Update()
    {
        if (Input.GetKey(KeyCode.Tab))
        {
            ShowStats();
        }
        transform.GetChild(0).gameObject.SetActive(Input.GetKey(KeyCode.Tab));

    }

    void ShowStats()
    {
        for (int i = 0; i < LocalGameManager.Instance.m_Players.Length; i++)
        {
            names[i].text = LocalGameManager.Instance.GetPlayer(i+1).GetComponent<Player>().playerName;
            kills[i].text = LocalGameManager.Instance.m_playerKills[i].ToString();
            arrhs[i].text = LocalGameManager.Instance.m_playerArrh[i].ToString();
            deaths[i].text = LocalGameManager.Instance.m_playerDeaths[i].ToString();
            classes[i].sprite = sprites[(int)LocalGameManager.Instance.GetPlayer(i+1).GetComponent<FlagshipStatus>().shipClass];
        }
    }
}
