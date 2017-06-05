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
        for (int i = 0; i < 4; i++)
        {
            names[i].text = LocalGameManager.Instance.GetPlayer(i).name;
            kills[i].text = LocalGameManager.Instance.m_playerKills[i].ToString();
            arrhs[i].text = LocalGameManager.Instance.m_playerArrh[i].ToString();
            deaths[i].text = LocalGameManager.Instance.m_playerDeaths[i].ToString();
        }
    }
}
