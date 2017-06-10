using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class InGameCanvasController : MonoBehaviour
{

    public GameObject m_Clock;
    public bool partitaFinita = false;

    public float gapTime;

    // Use this for initialization
    void Start()
    {
        m_Clock.transform.SetParent(transform);
        m_Clock.SetActive(true);
    }

    public void FixedUpdate()
    {
        if (LocalGameManager.Instance.m_GameIsStarted && (LocalGameManager.Instance.m_timeIsSynced || LocalGameManager.Instance.m_serverTimeSended))
        {


            float time = LocalGameManager.Instance.syncedTime() <= 0 ? 0 : LocalGameManager.Instance.syncedTime();
            int minutes = Mathf.FloorToInt(time / 60);

            if (LocalGameManager.Instance.isServer)
            {
                if(time >= (int)FixedDelayInGame.END_GAME && !partitaFinita)
                {
                    partitaFinita = true;
                    int player = -1;
                    //Fine partita.
                    int max = LocalGameManager.Instance.m_playerArrh[0];
                    for(int i = 0; i < LocalGameManager.Instance.m_playerArrh.Count; i++)
                    {
                        if(LocalGameManager.Instance.m_playerArrh[i] >= max)
                        {
                            player = i + 1;
                            max = LocalGameManager.Instance.m_playerArrh[i];
                        }
                    }

                    if(max == 0 || player == -1)
                    {
                        //Situazione di paraggio --> Controlla la reputazione
                        GameObject[] g = GameObject.FindGameObjectsWithTag("Player");
                        if(g.Length > 0)
                        {
                            GameObject winner = null;
                            int max_Rep = g[0].GetComponent<FlagshipStatus>().m_reputation;

                            foreach(GameObject gg in g)
                            {
                                if (gg.GetComponent<FlagshipStatus>().m_reputation > max_Rep)
                                    winner = gg;
                            }

                            if(winner == null)
                            {
                                //è un fottuto pareggio
                            }else
                            {
                                LocalGameManager.Instance.RpcPartitaConclusa(winner.GetComponent<Player>().playerId);
                            }
                        }

                    }
                    else
                    {
                        LocalGameManager.Instance.RpcPartitaConclusa(player);
                    }
                }
            }

            m_Clock.GetComponent<Text>().text = String.Format("{0,2:D2}:{1,2:D2}", minutes, Mathf.FloorToInt(time - minutes * 60));

        }
    }


}
