using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFloatingName : MonoBehaviour
{
    [Range(1,4)]
    public int id;
    private Transform target;
    public Vector2 offset;

    void Start()
    {
        StartCoroutine(WaitForReady());
    }

    void Update ()
    {
        if(target)
        {
            Vector2 targetPos = Camera.main.WorldToScreenPoint(target.position);
            transform.position = targetPos + offset;
            GetComponent<Text>().text = "Player " + id + " " + target.gameObject.GetComponent<FlagshipStatus>().m_Health;
        }

    }

    IEnumerator WaitForReady()
    {
        yield return new WaitUntil(() => LocalGameManager.Instance.m_PlayerRegistered);

        target = LocalGameManager.Instance.GetPlayer(id).transform;
    }
}
