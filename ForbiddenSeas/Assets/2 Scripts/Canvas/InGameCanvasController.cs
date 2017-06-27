using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class InGameCanvasController : MonoBehaviour
{

    public GameObject m_Clock;
    public GameObject CountDownRespawn;
    public GameObject CountDownStart;


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
                    int max = 0;
                    for(int i = 0; i < LocalGameManager.Instance.m_playerArrh.Count; i++)
                    {
                        if(LocalGameManager.Instance.m_playerArrh[i] >= max)
                        {
                            max = LocalGameManager.Instance.m_playerArrh[i];
                        }
                    }

                    int giocatoriArrh = 0;

                    for (int i = 0; i < LocalGameManager.Instance.m_playerArrh.Count; i++)
                    {
                        if (LocalGameManager.Instance.m_playerArrh[i] == max)
                        {
                            player = i + 1;
                            giocatoriArrh++;
                        }
                    }

                    if (giocatoriArrh > 1)
                    {
                        //Situazione di paraggio --> Controlla la reputazione
                        GameObject[] g = GameObject.FindGameObjectsWithTag("Player");
                        if(g.Length > 0)
                        {
                            GameObject winner = gameObject;
                            int max_Rep = 0;

                            foreach(GameObject gg in g)
                            {
                                if (gg.GetComponent<FlagshipStatus>().m_reputation >= max_Rep)
                                {
                                    max_Rep = gg.GetComponent<FlagshipStatus>().m_reputation;
                                }

                            }

                            int giocatori = 0;

                            foreach (GameObject gg in g)
                            {
                                if (gg.GetComponent<FlagshipStatus>().m_reputation == max_Rep)
                                {
                                    giocatori++;
                                    winner = gg;
                                }

                            }

                            if (winner.GetComponent<Player>())
                            {
                                if(giocatori == 1)
                                {
                                    LocalGameManager.Instance.RpcPartitaConclusa(winner.GetComponent<Player>().playerId);
                                    Debug.Log("Il vincitore è il player " + winner.GetComponent<Player>().playerName);
                                }
                                else
                                {
                                    //pareggio
                                    Debug.Log("I giocatori hanno la stessa reputazione è un pareggio");
                                }
                            }
                        }

                    }
                    else
                    {
                        Debug.Log("Il vincitore è il player " + player);
                        LocalGameManager.Instance.RpcPartitaConclusa(player);
                    }
                }
            }

            m_Clock.GetComponent<Text>().text = String.Format("{0,2:D2}:{1,2:D2}", minutes, Mathf.FloorToInt(time - minutes * 60));

        }
    }


}
