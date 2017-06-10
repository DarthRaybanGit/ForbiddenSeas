using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndGameStats : MonoBehaviour
{
    public Text [] names;
    public Text [] arrhs;
    public Text [] kills;
    public Text [] deaths;
    public Text [] reputations;
    public Image [] classes;
    public Sprite [] sprites;

    void OnEnable()
    {
        ShowStats();
    }

    void ShowStats()
    {
        for (int i = 0; i < LocalGameManager.Instance.m_Players.Length; i++)
        {
            names[i].text = LocalGameManager.Instance.GetPlayer(i+1).GetComponent<Player>().playerName;
            kills[i].text = LocalGameManager.Instance.m_playerKills[i].ToString();
            arrhs[i].text = LocalGameManager.Instance.m_playerArrh[i].ToString();
            deaths[i].text = LocalGameManager.Instance.m_playerDeaths[i].ToString();
            reputations[i].text = LocalGameManager.Instance.GetPlayer(i + 1).GetComponent<FlagshipStatus>().m_reputation.ToString();
            classes[i].sprite = sprites[(int)LocalGameManager.Instance.GetPlayer(i+1).GetComponent<FlagshipStatus>().shipClass];
        }
    }

    public void returnToLobby()
    {
        //nfewjin nj doicarjtu pa, no dearsweoy myojirtu arrh!
    }
}
