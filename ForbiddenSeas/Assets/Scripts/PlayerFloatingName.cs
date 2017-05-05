using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFloatingName : MonoBehaviour
{
    [Range(1,4)]
    public int id;
    public Transform target;
    public Vector2 offset;

    public bool trovato = false;

    void Start()
    {
        StartCoroutine(WaitForReady());
    }

    void Update ()
    {
        if(trovato && target != null)
        {
            Vector2 targetPos = Camera.main.WorldToScreenPoint(target.position);
            transform.position = targetPos + offset;
            GetComponent<Text>().text = "Player " + id + " " + target.gameObject.GetComponent<FlagshipStatus>().m_Health;
        }

    }

    IEnumerator WaitForReady()
    {
        yield return new WaitUntil(() => LocalGameManager.Instance.m_PlayerRegistered && LocalGameManager.Instance.m_GameIsStarted && LocalGameManager.Instance.m_serverTimeSended);
        if (id > LocalGameManager.Instance.m_Players.Length)
        {
            gameObject.SetActive(false);
        }
        else
        {
            target = LocalGameManager.Instance.GetPlayer(id - 1).transform;
            Debug.Log("Ho trovato il player " + target.name + " " + target.GetComponent<Player>().netId);
            trovato = true;
        }

    }
}
